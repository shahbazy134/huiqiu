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
using EnvDTE;
using Microsoft.Practices.RecipeFramework.Library;
using Microsoft.Practices.RecipeFramework.Extensions.CommonHelpers;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.Shell.Design;
using Microsoft.VisualStudio.Shell.Interop;
using System.Diagnostics;
using Microsoft.VisualStudio;
using Microsoft.Practices.Modeling.Presentation.Models;
using Microsoft.Practices.VisualStudio.Helper;

namespace Microsoft.Practices.ServiceFactory.Library.Presentation.Models
{
	/// <summary>
	/// Implementation of the IProjectModel interface that talks to the
	/// visual studio DTE object model.
	/// </summary>
	public class DteProjectModel : IProjectModel
	{
		private Project project;
		private IServiceProvider serviceProvider;
		private ITypeResolutionService typeResolutionService;

		public object Project
		{
			get { return this.project; }
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:DteProjectModel"/> class.
		/// </summary>
		/// <param name="currentProject">The current project.</param>
		public DteProjectModel(Project project, IServiceProvider serviceProvider)
		{
			this.project = project;
			this.serviceProvider = serviceProvider;
		}

		/// <summary>
		/// Returns true if the file is in the current project.
		/// </summary>
		/// <param name="filename">The filename.</param>
		/// <returns></returns>
		public bool ProjectContainsFile(string filename)
		{
			IVsSolution solution = serviceProvider.GetService(typeof(IVsSolution)) as IVsSolution;
			using (HierarchyNode node = new HierarchyNode(solution, project.Object as IVsHierarchy))
			{
				return (node.RecursiveFindByName(filename) != null);
			}
		}

		/// <summary>
		/// Returns an instance of <see cref="ITypeResolutionService"/> for the project scope
		/// </summary>
		public ITypeResolutionService TypeResolutionService
		{
			get
			{
				if (typeResolutionService == null)
				{
					DynamicTypeService typeService = (DynamicTypeService)serviceProvider.GetService(typeof(DynamicTypeService));
					Debug.Assert(typeService != null, "No dynamic type service registered.");

					IVsHierarchy hier = DteHelper.GetCurrentSelection(serviceProvider);
					Debug.Assert(hier != null, "No active hierarchy is selected.");

					typeResolutionService = typeService.GetTypeResolutionService(hier);
					Debug.Assert(typeResolutionService != null, "No type resolution service is returned");
				}

				return typeResolutionService;
			}
		}

        /// <summary>
        /// Returns a list with the types defined in the project
        /// </summary>
        /// <value></value>
		public IList<ITypeModel> Types
		{
			get
			{
				List<ITypeModel> types = new List<ITypeModel>();
				foreach (CodeElement codeElement in EnumerateCodeTypes(project.CodeModel.CodeElements))
				{
					if (codeElement.IsCodeType && 
						codeElement.InfoLocation == vsCMInfoLocation.vsCMInfoLocationProject)
					{
						types.Add(new DteTypeModel((CodeType)codeElement));
					}
				}
				return types;
			}
		}

        /// <summary>
        /// Gets a value indicating whether this instance is web project.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is web project; otherwise, <c>false</c>.
        /// </value>
		public bool IsWebProject
		{
			get
			{
				return DteHelper.IsWebProject(project);
			}
		}

        /// <summary>
        /// Gets the project items.
        /// </summary>
        /// <value>The project items.</value>
        public IList<IProjectItemModel> ProjectItems
        {
            get
            {
                List<IProjectItemModel> projectItems = new List<IProjectItemModel>();
                foreach (ProjectItem projectItem in new DteHelperEx.ProjectItemIterator(this.project))
                {
                    projectItems.Add(new DteProjectItemModel(projectItem));
                }
                return projectItems;
            }
        }

		private IEnumerable<CodeType> EnumerateCodeTypes(CodeElements elements)
		{
			foreach (CodeElement codeElement in elements)
			{
				if (codeElement.IsCodeType)
				{
					yield return (CodeType)codeElement;
				}
				else if (codeElement.Kind == vsCMElement.vsCMElementNamespace)
				{
					foreach (CodeType codeType in EnumerateCodeTypes(((CodeNamespace)codeElement).Members))
					{
						yield return codeType;
					}
				}
			}
		}

        /// <summary>
        /// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </returns>
		public override string ToString()
		{
			return project.Name;
		}
	}
}
