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
using Microsoft.Practices.ServiceFactory.Actions;
using Microsoft.Practices.RecipeFramework;
using Microsoft.Practices.RecipeFramework.Extensions.Actions.VisualStudio;
using EnvDTE;
using Microsoft.Practices.RecipeFramework.Extensions.Actions.Templates;
using System.IO;
using Microsoft.Practices.Common.Services;
using System.ComponentModel.Design;
using System.Diagnostics;
using Microsoft.VisualStudio.Shell.Interop;

namespace Microsoft.Practices.ServiceFactory.Recipes.CreateModel
{
	public class CreateModelAction : ActionBase
	{
		private const string NamespaceParameter = "$Namespace$";

		private ModelType modelType;

		[Input(Required = true)]
		public ModelType ModelType
		{
			get { return modelType; }
			set { modelType = value; }
		}

		private string modelName;

		[Input(Required = true)]
		public string ModelName
		{
			get { return modelName; }
			set { modelName = value; }
		}

		private Project modelProject;

		[Input(Required = true)]
		public Project ModelProject
		{
			get { return modelProject; }
			set { modelProject = value; }
		}

		private string namespacePrefix;

		[Input(Required = false)]
		public string NamespacePrefix
		{
			get { return namespacePrefix; }
			set { namespacePrefix = value; }
		}

		public override void Execute()
		{
			string templateRelativePath = string.Empty;
			string modelExtension = string.Empty;
			string itemPath = string.Empty;

			switch(this.modelType)
			{
				case ModelType.ServiceModel:
					templateRelativePath = this.GetTemplatePath(@"Items\ServiceModel.vstemplate");
					modelExtension = Constants.ServiceModelExtension;
					break;
				case ModelType.DataContractModel:
					templateRelativePath = this.GetTemplatePath(@"Items\DataContractModel.vstemplate");
					modelExtension = Constants.DataContractModelExtension;
					break;
				case ModelType.HostDesignerModel:
					templateRelativePath = this.GetTemplatePath(@"Items\HostDesignerModel.vstemplate");
					modelExtension = Constants.HostDesignerModelExtension;
					break;

				default:
					break;
			}

			using(UnfoldItemTemplateAction action = new UnfoldItemTemplateAction())
			{
				action.Site = this.Site;
				action.Template = templateRelativePath;
				action.ItemName = string.Concat(this.modelName, modelExtension);
				action.Project = this.modelProject;
				action.Execute();

				itemPath = action.NewItem.Document.FullName;
				
				action.NewItem.Document.Close(vsSaveChanges.vsSaveChangesYes);
				ReplaceItemContent(itemPath);
				Window wnd = action.NewItem.Open(EnvDTE.Constants.vsViewKindPrimary);
				wnd.Visible = true;
				wnd.Activate();
			}
		}

		public override void Undo()
		{
			//Do Nothing
		}

		private void ReplaceItemContent(string itemPath)
		{
			string templateContent = File.ReadAllText(itemPath);
			string replacedContent = templateContent.Replace(NamespaceParameter, this.namespacePrefix);
			File.WriteAllText(itemPath, replacedContent);
		}
	}
}