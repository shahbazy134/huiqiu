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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel.Design;
using Microsoft.Practices.UnitTestLibrary;
using Microsoft.Practices.ServiceFactory.Recipes.CreateModel.Presentation;
using Microsoft.Practices.ServiceFactory.Recipes.CreateModel;
using Microsoft.Practices.ServiceFactory.Library.Presentation.Models;
using ServicesGuidancePackage.Tests.Common;
using Microsoft.Practices.EnterpriseLibrary.Validation;
using Microsoft.Practices.Modeling.Presentation.Models;

namespace ServicesGuidancePackage.Tests.Library.Presentation.CreateModel
{
	[TestClass()]
    public class ModelPropertiesPresenterFixture
	{
		IDictionaryService dictionary;
		IModelPropertiesModel model;
		MockModelPropertiesView view;
		ModelPropertiesPresenter presenter;
		ValidationResults results;

		[TestInitialize]
		public void TestInitialize()
		{
			dictionary = CreateDictionaryService();
			results = new ValidationResults();
			IProjectModel projectModel = new MockProjectModel("Foo.servicecontract");
			model = CreateModel(dictionary, projectModel);
			view = new MockModelPropertiesView();
			presenter = new ModelPropertiesPresenter(view, model);
		}

		[TestMethod]
		[DeploymentItem("Microsoft.Practices.RecipeFramework.Library.dll")]
		public void ShouldBeValidWithValidArguments()
		{
			ValidationResults results = view.FireModelNameChanged("ModelName1");
			Assert.IsNotNull(model.ModelName);
			Assert.AreEqual(model.ModelName, "ModelName1");
			Assert.IsTrue(results.IsValid);

			view.FireModelTypeChanged(ModelType.ServiceModel);
			Assert.IsNotNull(model.ModelType);
			Assert.AreEqual(model.ModelType, ModelType.ServiceModel);
		}

		[TestMethod]
		[DeploymentItem("Microsoft.Practices.RecipeFramework.Library.dll")]
		public void ShouldBeInValidWithValidArguments()
		{
			//Empty file name
			ValidationResults results = view.FireModelNameChanged(string.Empty);
			Assert.IsNull(model.ModelName);
			Assert.IsFalse(results.IsValid);

			//Invalid file name
			results = view.FireModelNameChanged("PRN");
			Assert.IsNull(model.ModelName);
			Assert.IsFalse(results.IsValid);

			//Existing project item
			results = view.FireModelNameChanged("Foo");
			Assert.IsNull(model.ModelName);
			Assert.IsFalse(results.IsValid);
		}

		private IModelPropertiesModel CreateModel(IDictionaryService dictionary, IProjectModel projectModel)
		{
			return new ModelPropertiesModel(dictionary, projectModel);
		}

		private IDictionaryService CreateDictionaryService()
		{
			return new MockDictionaryService();
		}
	}
}