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
using Microsoft.Practices.Common;
using Microsoft.Practices.Modeling.CodeGeneration.Logging;
using Microsoft.Practices.RecipeFramework;
using Microsoft.Practices.ServiceFactory.RecipeFramework.Extensions.ProjectMapping;
using Microsoft.Practices.ServiceFactory.RecipeFramework.Extensions.ProjectMapping.Configuration;
using Microsoft.Practices.VisualStudio.Helper;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using System.ComponentModel.Design;
using System.Security.Permissions;

namespace Microsoft.Practices.ServiceFactory.References
{
	[Serializable]
	public class ProjectIsInRoleReference : UnboundRecipeReference, IAttributesConfigurable
	{
		private const string RoleAttribute = "Role";
		private string targetRole;

		public ProjectIsInRoleReference(string recipe)
			: base(recipe)
		{
		}

		public override string AppliesTo
		{
			get { return Properties.Resources.ProjectIsInRoleReference; }
		}

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
		public override bool IsEnabledFor(object target)
		{
			try
			{
				Project project = target as Project;

				if(project != null)
				{
					ReadOnlyCollection<string> mappingTableNames =
						ProjectMappingManager.Instance.GetMappingTableNames();

					foreach(string tableName in mappingTableNames)
					{
						try
						{
							ReadOnlyCollection<Role> roles =
								ProjectMappingManager.Instance.GetProjectRoles(
									tableName,
									GetProjectGuid(this.Site, project));

							foreach(Role role in roles)
							{
								if(role.Name.Equals(targetRole, StringComparison.OrdinalIgnoreCase))
								{
									return true;
								}
							}
						}
						catch(ProjectMappingNotFoundException)
						{
							//Do Nothing, go to the mapping table
						}
					}
				}
			}
			catch(Exception e)
			{
				Logger.Write(e.ToString());
			}

			return false;
		}

		#region ISerializable Members

		protected ProjectIsInRoleReference(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			targetRole = info.GetString(RoleAttribute);
		}

		[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
		protected override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue(RoleAttribute, targetRole);
		}
		#endregion ISerializable Members

		#region IAttributesConfigurable Members
		[SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly")]
		public void Configure(System.Collections.Specialized.StringDictionary attributes)
		{
			if(attributes == null)
				throw new ArgumentNullException("attributes");

			if(attributes.ContainsKey(RoleAttribute))
			{
				targetRole = attributes[RoleAttribute];
			}
			else
				throw new ArgumentNullException(RoleAttribute);
		}

		#endregion

		private static Guid GetProjectGuid(IServiceProvider serviceProvider, Project project)
		{
			IVsSolution sol = (IVsSolution)serviceProvider.GetService(typeof(SVsSolution));
			using (HierarchyNode node = new HierarchyNode(sol, project.UniqueName))
			{
				return node.ProjectGuid;
			}
		}
	}
}