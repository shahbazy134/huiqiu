//===============================================================================
// Microsoft patterns & practices
// Web Service Software Factory
//===============================================================================
// Copyright � Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================
// The example companies, organizations, products, domain names,
// e-mail addresses, logos, people, places, and events depicted
// herein are fictitious.  No association with any real company,
// organization, product, domain name, email address, logo, person,
// places, or events is intended or should be inferred.
//===============================================================================

using EnvDTE;
using Microsoft.Practices.RecipeFramework;
using Microsoft.Practices.RecipeFramework.Library;
using Microsoft.Practices.ServiceFactory.RecipeFramework.Extensions.Enums;
using Microsoft.Practices.ServiceFactory.RecipeFramework.Extensions.ProjectMapping;
using Microsoft.Practices.ServiceFactory.RecipeFramework.Extensions.ProjectMapping.Configuration;
using Microsoft.Practices.VisualStudio.Helper;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Practices.ServiceFactory.Actions
{
	public class PopulateProjectMappingTableAction : ActionBase
	{
		#region Constants

		private readonly Guid SolutionFolderGuid = new Guid(EnvDTE.Constants.vsProjectItemKindVirtualFolder);
		private const string RolesEntry = "Roles";
		private readonly char[] RolesDelimiter = { '|' };

		#endregion

		#region Fields

		private string solutionFolderName;
		private string mappingTableName;

		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets the name of the solution folder.
		/// </summary>
		/// <value>The name of the solution folder.</value>
		[Input(Required = true)]
		public string SolutionFolderName
		{
			get { return solutionFolderName; }
			set { solutionFolderName = value; }
		}

		/// <summary>
		/// Gets or sets the name of the mapping table.
		/// </summary>
		/// <value>The name of the mapping table.</value>
		[Input(Required = false)]
		public string MappingTableName
		{
			get { return mappingTableName; }
			set { mappingTableName = value; }
		}

		private string projectPath;

		/// <summary>
		/// Gets or sets the project path.
		/// </summary>
		/// <value>The project path.</value>
		[Input(Required = false)]
		public string ProjectPath
		{
			get { return projectPath; }
			set { projectPath = value; }
		}

		#endregion

		protected override void OnExecute()
		{
			EnsureArguments();
			EnsureMappingTableExists();
			IVsSolution solution = GetService<IVsSolution, SVsSolution>();
			using (HierarchyNode folder = new HierarchyNode(solution, solutionFolderName))
			{
				TraverseHierarchyNode(folder);
			}
		}

		#region Private Implementation

		private void EnsureArguments()
		{
			if (string.IsNullOrEmpty(mappingTableName))
			{
				mappingTableName = solutionFolderName;
				Project solutionFolder = DteHelper.FindProjectByName(Dte, mappingTableName);
				solutionFolderName = solutionFolder.UniqueName;
			}

			// Ensure ProjectMapping.xml exists
			ProjectMappingManager.Instance.CreateMappingFile().Dispose();
		}

		private void EnsureMappingTableExists()
		{
			if (ProjectMappingManager.Instance.GetMappingTable(mappingTableName) != null)
			{
				mappingTableName = CreateNewMappingTableName();
			}
			// Create new mapping table
			ProjectMappingTable projectMappingTable = new ProjectMappingTable(mappingTableName);
			ProjectMappingManager.Instance.AddProjectMappingTableEntry(projectMappingTable);
		}

		private string CreateNewMappingTableName()
		{
			string newName; 
			int suffix = 1;
			while (ProjectMappingManager.Instance.GetMappingTable(
				newName = mappingTableName + suffix.ToString(CultureInfo.InvariantCulture)) != null)
			{
				suffix++;
			}
			return newName;
		}

		private void TraverseHierarchyNode(HierarchyNode node)
		{
			node.RecursiveForEach(delegate(HierarchyNode child)
			{
				// recurse if this node is a Solution Folder
				if (child.TypeGuid != SolutionFolderGuid)
				{
					// If this is a project, add the mapping
					if (child.ExtObject is EnvDTE.Project)
					{
						Collection<Role> roles = GetRoles(child.ExtObject as Project);
						if (child.ProjectGuid != Guid.Empty)
						{
							ProjectMappingEntry mapping = new ProjectMappingEntry(child.ProjectGuid, this.ProjectPath, roles, child.Name);
							ProjectMappingManager.Instance.AddProjectMappingEntry(mappingTableName, mapping);
						}
					}
				}
			});
		}

		private Collection<Role> GetRoles(Project project)
		{
			if (project.Globals != null && 
				project.Globals.get_VariableExists(RolesEntry))
			{
				string projectRolesEntry = project.Globals[RolesEntry].ToString();
				string[] projectRoles = projectRolesEntry.Split(RolesDelimiter);
				return BuildRolesCollection(projectRoles);
			}

			return InferRolesFromProjectName(project.Name);
		}

		private Collection<Role> BuildRolesCollection(string[] projectRoles)
		{
			List<Role> roles = new List<Role>();

			foreach (string role in projectRoles)
			{
				try
				{
					if (!String.IsNullOrEmpty(role))
					{
						ServiceFactoryRoleType roleType =
							(ServiceFactoryRoleType)Enum.Parse(typeof(ServiceFactoryRoleType), role, true);

						Role serviceFactoryRole = new Role(roleType.ToString());

						if (roles.Find(delegate(Role match)
						{
							if (match.Name == roleType.ToString())
							{
								return true;
							}

							return false;
						}
							) == null)
						{
							roles.Add(serviceFactoryRole);
						}
					}
				}
				catch (ArgumentException)
				{
					//Do Nothing Is not a valid role value
				}
			}

			return (new Collection<Role>(roles));
		}

        [SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase")]
		private Collection<Role> InferRolesFromProjectName(string projectName)
		{
			Collection<Role> roles = new Collection<Role>();
			projectName = projectName.ToLowerInvariant();

			foreach (string roleName in Enum.GetNames(typeof(ServiceFactoryRoleType)))
			{
				if (projectName.Contains(FormatRoleSuffix(roleName)))
				{
					roles.Add(new Role(roleName));
					// add second role only for MessageContracts
					if (roleName == ServiceFactoryRoleType.ServiceContractRole.ToString())
					{
						roles.Add(new Role(ServiceFactoryRoleType.MessageContractRole.ToString()));
					}
					break;
				}
			}

			return roles;
		}

        [SuppressMessage("Microsoft.Globalization","CA1308:NormalizeStringsToUppercase")]
		private string FormatRoleSuffix(string roleName)
		{
			// strip the "Role" suffix
			// case insensitive comparison (ToLowerInvariant) 
			string suffix = roleName.Replace("Role", string.Empty);

			// adjust suffix to v2 naming convention
			if (suffix.Equals("service", StringComparison.OrdinalIgnoreCase))
			{
				return "serviceimplementation";
			}

			return suffix.ToLowerInvariant();
		}

		#endregion
	}
}