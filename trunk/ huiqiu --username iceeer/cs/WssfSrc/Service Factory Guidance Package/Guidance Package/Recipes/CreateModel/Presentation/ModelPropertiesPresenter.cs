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
using Microsoft.Practices.RecipeFramework.Library;
using Microsoft.Practices.EnterpriseLibrary.Validation;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using Microsoft.Practices.ServiceFactory.Library.Validation;
using Microsoft.Practices.ServiceFactory.Validation;

namespace Microsoft.Practices.ServiceFactory.Recipes.CreateModel.Presentation
{
	public class ModelPropertiesPresenter
	{
		private IModelPropertiesView view;
		private IModelPropertiesModel model;
		private AndCompositeValidator modelNameValidator;
        private UriValidator namespacePrefixValidator;

		public ModelPropertiesPresenter(IModelPropertiesView view, IModelPropertiesModel model)
		{
			Guard.ArgumentNotNull(view, "view");
			Guard.ArgumentNotNull(model, "model");

			this.view = view;
			this.model = model;

			view.ModelNameChanged += OnModelNameChanged;
			view.ModelTypeChanged += OnModelTypeChanged;
			view.NamespacePrefixChanged += OnNamespacePrefixChanged;

            namespacePrefixValidator = new UriValidator();
		}

		private void OnModelTypeChanged(object sender, EventArgs e)
		{
			model.ModelType = view.CurrentModelType;
			model.ModelName = null;
		}

		private void OnModelNameChanged(object sender, ValidationEventArgs e)
		{			
			modelNameValidator = new AndCompositeValidator
				(						
					new FileNameValidator(),
					new ProjectItemIsUniqueValidator(delegate { return model.ModelProject; }, GetModelExtension())
				);

			e.ValidationResults = modelNameValidator.Validate(view.CurrentModelName);

			if(!e.ValidationResults.IsValid)
			{
				model.ModelName = null;
				return;
			}
			else
			{
				model.ModelName = view.CurrentModelName;
			}
		}

		private void OnNamespacePrefixChanged(object sender, ValidationEventArgs e)
		{
			e.ValidationResults = namespacePrefixValidator.Validate(view.CurrentNamespacePrefix);

			if(!e.ValidationResults.IsValid)
			{
				model.NamespacePrefix = null;
			}
			else
			{
				model.NamespacePrefix = view.CurrentNamespacePrefix;
			}
		}

		private string GetModelExtension()
		{
			string modelExtension = string.Empty;

			switch(model.ModelType)
			{
				case ModelType.ServiceModel:
					modelExtension = Constants.ServiceModelExtension.Substring(1);
					break;
				case ModelType.DataContractModel:
					modelExtension = Constants.DataContractModelExtension.Substring(1);
					break;
				case ModelType.HostDesignerModel:
					modelExtension = Constants.HostDesignerModelExtension.Substring(1);
					break;
				default:
					break;
			}

			return modelExtension;
		}
	}
}