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

using Microsoft.Practices.EnterpriseLibrary.Validation;
using Microsoft.Practices.Modeling.Presentation.Models;
using Microsoft.Practices.RecipeFramework.Library;
using Microsoft.Practices.ServiceFactory.Library.Validation;
using Microsoft.Practices.ServiceFactory.Validation;
using System;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;

namespace Microsoft.Practices.ServiceFactory.Recipes.CreateTranslator.Presentation
{
	public class CreateTranslatorPropertiesPresenter
	{
		private ICreateTranslatorPropertiesView view;
		private CreateTranslatorPropertiesModel model;
		private IProjectModel project;

		private AndCompositeValidator mappingClassNameValidator;
		private Validator<string> mappingClassNamespaceValidator;
        private PredicateValidator<Type> selectedTypesValidator;
        private ClassHasDefaultConstructorValidator defaultConstructorValidator;


		public CreateTranslatorPropertiesPresenter(
			IProjectModel projectModel,
			ICreateTranslatorPropertiesView view,
			CreateTranslatorPropertiesModel model)
		{
			Guard.ArgumentNotNull(projectModel, "projectModel");
			Guard.ArgumentNotNull(view, "view");
			Guard.ArgumentNotNull(model, "model");

			this.project = projectModel;
			this.view = view;
			this.model = model;

            mappingClassNameValidator = new AndCompositeValidator(
               new IdentifierValidator(IdentifierValidator.CSharpLanguage, Properties.Resources.ProjectItemInvalidName),
               new ProjectItemIsUniqueValidator(this.project, "cs", Properties.Resources.ProjectItemNameAlreadyExists)
           );

			mappingClassNamespaceValidator = 
				new NamespaceValidator(Properties.Resources.InvalidNamespaceError);

            selectedTypesValidator = new PredicateValidator<Type>(
                delegate(Type t) { return (view.FirstType != view.SecondType); },
                Properties.Resources.TranslatedTypesMustBeDifferent);

            defaultConstructorValidator = new ClassHasDefaultConstructorValidator();

			view.MappingClassNameChanged += OnMappingClassNameChanged;
			view.MappingClassNamespaceChanged += OnMappingClassNamespaceChanged;
			view.FirstTypeChanged += OnFirstTypeChanged;
			view.SecondTypeChanged += OnSecondTypeChanged;
			view.RequestIsDataValid += OnRequestIsDataValid;
		}

		void OnMappingClassNameChanged(object sender, ValidationEventArgs e)
		{
			e.ValidationResults = mappingClassNameValidator.Validate(view.MappingClassName);

			if (e.ValidationResults.IsValid)
			{
				model.MappingClassName = view.MappingClassName;
			}
			else
			{
				model.MappingClassName = null;
			}
		}

		void OnMappingClassNamespaceChanged(object sender, ValidationEventArgs e)
		{
			e.ValidationResults = mappingClassNamespaceValidator.Validate(view.MappingClassNamespace);

			if (e.ValidationResults.IsValid)
			{
				model.MappingNamespace = view.MappingClassNamespace;
			}
			else
			{
				model.MappingNamespace = null;
			}
		}

		void OnFirstTypeChanged(object sender, ValidationEventArgs e)
		{
			e.ValidationResults = selectedTypesValidator.Validate(view.FirstType);

			if(e.ValidationResults.IsValid)
			{
                // Warn the user if the class doesn't have a default ctor.
                e.ValidationResults = defaultConstructorValidator.Validate(view.FirstType);
				model.FirstType = view.FirstType;
			}
			else
			{
				model.FirstType = null;
			}
		}

		void OnSecondTypeChanged(object sender, ValidationEventArgs e)
		{
			e.ValidationResults = selectedTypesValidator.Validate(view.SecondType);

			if (e.ValidationResults.IsValid)
			{
                // Warn the user if the class doesn't have a default ctor.
                e.ValidationResults = defaultConstructorValidator.Validate(view.SecondType);
				model.SecondType = view.SecondType;
			}
			else
			{
				model.SecondType = null;
			}
		}

		void OnRequestIsDataValid(object sender, RequestDataEventArgs<bool> e)
		{
			e.Value = model.IsValid;
		}
	}
}