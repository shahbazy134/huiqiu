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
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.ServiceModel.Description;
using System.ServiceModel.Channels;
using System.Threading;
using Microsoft.Practices.RecipeFramework;
using Microsoft.VisualStudio.Modeling;
using Microsoft.Practices.RecipeFramework.Library;
using Microsoft.Practices.ServiceFactory.Actions;
using Microsoft.Practices.RecipeFramework.Extensions.CommonHelpers;
using ExtFwk = Microsoft.Practices.Modeling.ExtensionProvider.Extension;
using Microsoft.Practices.Modeling.CodeGeneration.Artifacts;
using Microsoft.Practices.Modeling.ExtensionProvider.Helpers;
using Microsoft.VisualStudio.Shell.Interop;
using System.Globalization;
using Microsoft.Practices.Modeling.ExtensionProvider.Extension;
using Microsoft.Practices.Modeling.CodeGeneration;
using Microsoft.Practices.VisualStudio.Helper;
using Microsoft.VisualStudio.Modeling.Diagrams;
using Microsoft.Practices.Modeling.Dsl.Integration.Helpers;
using Microsoft.VisualStudio.Modeling.Shell;
using Microsoft.VisualStudio.Modeling.Validation;

namespace Microsoft.Practices.ServiceFactory.Actions
{
	public class GenerateArtifactAction : ActionBase
	{
		private ModelElement selectedElement;
		private int generatedObjects;

		[Input(Required = true)]
		public ModelElement SelectedElement
		{
			get { return selectedElement; }
			set { selectedElement = value; }
		}

		[Output]
		public int GeneratedObjects
		{
			get { return generatedObjects; }
		}

		protected override void OnExecute()
		{
			ModelingDocView docView = DesignerHelper.GetModelingDocView(this.Site);

			IValidationControllerAccesor accesor = docView.DocData as IValidationControllerAccesor;

			if(accesor != null)
			{
				if(ModelValidator.ValidateModelElement(selectedElement, accesor.Controller))
				{
					GenerateArtifacts();
				}
			}
			else
			{
				GenerateArtifacts();
			}
		}

		private void GenerateArtifacts()
		{
			IArtifactLinkContainer links = ModelCollector.GetArtifacts(this.Site);
			if (links != null)
			{
				this.Execute(links);
				base.Trace(Properties.Resources.GeneratedObjects, this.GeneratedObjects);
			}
		}

		protected void Execute(IArtifactLinkContainer links)
		{
			if (links != null && links.ArtifactLinks != null)
			{
				ICodeGenerationService codeGenerationService = GetService<ICodeGenerationService>();
				generatedObjects = codeGenerationService.GenerateArtifacts(links.ArtifactLinks);
			}
		}
	}
}
