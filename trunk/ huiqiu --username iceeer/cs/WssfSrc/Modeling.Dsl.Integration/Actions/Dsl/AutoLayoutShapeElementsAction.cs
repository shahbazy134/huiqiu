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
using Microsoft.Practices.ComponentModel;
using Microsoft.Practices.RecipeFramework;
using Microsoft.Practices.Modeling.Dsl.Integration.Helpers;
using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Modeling.Shell;
using Microsoft.VisualStudio.Modeling.Diagrams;
using Microsoft.VisualStudio.Modeling.Diagrams.GraphObject;
using Microsoft.Practices.Modeling.Dsl.Service;
using System.IO;
using EnvDTE;
using Microsoft.Practices.RecipeFramework.Library;

namespace Microsoft.Practices.Modeling.Dsl.Integration.Actions.Dsl
{
	[ServiceDependency(typeof(DTE))]
	[ServiceDependency(typeof(IServiceProvider))]
	public class AutoLayoutShapeElementsAction : ConfigurableAction
	{
		#region Input Properties

		private Diagram diagram;
		private string modelingDocDataFileName;
		private Project modelProject;

		/// <summary>
		/// The Diagram where to apply the auto layout.
		/// The default Diagram will be the current selected Diagram.
		/// </summary>
		[Input(Required = false)]
		public Diagram Diagram
		{
			get { return diagram; }
			set { diagram = value; }
		}

		/// <summary>
		/// In case there is not <see cref="Diagram"/> value specified,
		/// set the project where the <see cref="ModelingDocDataFileName"/> file is located.
		/// </summary>
		[Input(Required = false)]
		public Project ModelProject
		{
			get { return modelProject; }
			set { modelProject = value; }
		}

		/// <summary>
		/// The modeling file to get the Diagram to apply the auto layout.
		/// Set this value in case the <see cref="Diagram"/> argument is not specified.
		/// </summary>
		[Input(Required = false)]
		public string ModelingDocDataFileName
		{
			get { return modelingDocDataFileName; }
			set { modelingDocDataFileName = value; }
		}

		#endregion

		public override void Execute()
		{
			if (!string.IsNullOrEmpty(modelingDocDataFileName) &&
				modelProject != null)
			{
				diagram = GetDiagramFromFile(modelingDocDataFileName, modelProject);
			}

			if (diagram == null)
			{				

				DiagramDocView docView = DesignerHelper.GetDiagramDocView(this.GetService<IServiceProvider>());
                if (docView == null) return;
				diagram = docView.CurrentDiagram;
			}

			using (Transaction transaction = diagram.Store.TransactionManager.BeginTransaction())
			{
				diagram.AutoLayoutShapeElements(
					diagram.NestedChildShapes, 
					VGRoutingStyle.VGRouteOrgChartNS, 
					PlacementValueStyle.VGPlaceSN, 
					true);

				transaction.Commit();
			}			
		}

		public override void Undo()
		{
			// not implemented
		}

		private Diagram GetDiagramFromFile(string file, Project modelProject)
		{
			Diagram diagram = null;
			string fullPath = GetFullPath(file, modelProject);
			
			if (!string.IsNullOrEmpty(fullPath))
			{
				DslIntegrationService dslIntegration = new DslIntegrationService(this.Site);
				ModelingDocData docData = dslIntegration.GetModelingDocDataFromFile(fullPath);
				if (docData.DocViews.Count > 0)
				{
                    SingleDiagramDocView diagramDocView = docData.DocViews[0] as SingleDiagramDocView;
                    if (diagramDocView != null)
                    {
                        diagram = diagramDocView.Diagram;
                    }
				}
			}

			return diagram;
		}

		private string GetFullPath(string fileName, Project modelProject)
		{
			if (!Path.IsPathRooted(fileName))
			{
				ProjectItem item = DteHelper.FindItemByName(modelProject.ProjectItems, fileName, true);
				if (item != null)
				{
					return item.Document.FullName; //.Properties.Item("FullPath").Value.ToString()
				}
				return null;
			}
			return fileName;
		}
	}
}
