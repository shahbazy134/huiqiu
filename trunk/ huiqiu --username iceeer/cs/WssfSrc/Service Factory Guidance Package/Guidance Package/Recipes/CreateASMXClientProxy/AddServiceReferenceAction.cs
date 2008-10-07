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

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Practices.RecipeFramework;
using Microsoft.VisualStudio.Modeling.Shell;
using Microsoft.Practices.Modeling.Dsl.Integration.Helpers;
using Microsoft.Practices.Modeling.CodeGeneration;
using EnvDTE;
using EnvDTE80;
using Microsoft.Practices.ServiceFactory.Actions;
using System.Runtime.InteropServices;

namespace Microsoft.Practices.ServiceFactory.Recipes.CreateASMXClientProxy
{
	public class AddServiceReferenceAction : ActionBase
	{
		private Project clientProject;

		[Input(Required = true)]
		public Project ClientProject
		{
			get { return clientProject; }
			set { clientProject = value; }
		}

		private string serviceAddress;

		[Input(Required = true)]
		public string ServiceAddress
		{
			get { return serviceAddress; }
			set { serviceAddress = value; }
		}

		protected override void OnExecute()
		{
			if (IsValidModel())
			{
				AddServiceReference();
			}
		}

		private void AddServiceReference()
		{
			SelectProject(this.ClientProject.DTE.Solution, this.ClientProject.Name);
			try
			{
				//try with this command that will require a Targetframework of 3.5
				this.ClientProject.DTE.ExecuteCommand("Project.AddServiceReference", "");
			}
			catch (COMException)
			{
				// retry with Project.AddWebReference that is compatible with v2.0
				this.ClientProject.DTE.ExecuteCommand("Project.AddWebReference", "");
			}
		}

		public static void SelectProject(Solution solution, string projectName)
		{
			UIHierarchyItem projectH = FindItemByName<Project>(
				((DTE2)solution.DTE).ToolWindows.SolutionExplorer.UIHierarchyItems, projectName);

			solution.DTE.Windows.Item(Constants.vsWindowKindSolutionExplorer).Activate();

			if(projectH != null)
			{
				projectH.Select(vsUISelectionType.vsUISelectionTypeSelect);
			}
		}

		private static UIHierarchyItem FindItemByName<TKind>(UIHierarchyItems items, string itemName)
		{
			foreach(UIHierarchyItem item in items)
			{
				if(item.Name == itemName)
				{
					return item;
				}
				else
				{
					UIHierarchyItem item2 = FindItemByName<TKind>(item.UIHierarchyItems, itemName);

					if(item2 != null)
					{
						return item2;
					}
				}
			}

			return null;
		}
	}
}