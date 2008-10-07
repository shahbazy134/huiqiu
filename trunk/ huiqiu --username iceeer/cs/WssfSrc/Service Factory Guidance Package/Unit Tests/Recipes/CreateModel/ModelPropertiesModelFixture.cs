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
using Microsoft.Practices.ServiceFactory.Library.Presentation.Models;
using ServicesGuidancePackage.Tests.Common;
using Microsoft.Practices.ServiceFactory.Recipes.CreateModel;
using Microsoft.Practices.Modeling.Presentation.Models;

namespace ServicesGuidancePackage.Tests.Library.Presentation.CreateModel
{
	[TestClass]
	public class ModelPropertiesModelFixture
	{
		IDictionaryService dictionary;
		IModelPropertiesModel model;

		[TestInitialize]
		public void TestInitialize()
		{
			dictionary = CreateDictionaryService();
			IProjectModel projectModel = new MockProjectModel();
			model = CreateModel(dictionary, projectModel);
		}

		[TestMethod]
		[DeploymentItem("Microsoft.Practices.RecipeFramework.Library.dll")]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ThrowOnNullDictionary()
		{
			CreateModel(null, null);
		}

		[TestMethod]
		[DeploymentItem("Microsoft.Practices.RecipeFramework.Library.dll")]
		public void ModelIsInvalidAtStartup()
		{
			Assert.IsFalse(model.IsValid);
		}

		[TestMethod]
		[DeploymentItem("Microsoft.Practices.RecipeFramework.Library.dll")]
		public void ModelIsValidWithAllFieldsSet()
		{
			dictionary.SetValue("ModelType", ModelType.ServiceModel);
			dictionary.SetValue("NamespacePrefix", "Foo");
			dictionary.SetValue("ModelName", "ModelName1");

			Assert.IsTrue(model.IsValid);
		}

		[TestMethod]
		[DeploymentItem("Microsoft.Practices.RecipeFramework.Library.dll")]
		public void CanSetValues()
		{
			model.ModelType = ModelType.ServiceModel;
			model.ModelName = "ModelName1";

			Assert.AreEqual(model.ModelType, dictionary.GetValue("ModelType"));
			Assert.AreEqual(model.ModelName, dictionary.GetValue("ModelName"));
		}

		[TestMethod]
		[DeploymentItem("Microsoft.Practices.RecipeFramework.Library.dll")]
		public void CanGetValues()
		{
			dictionary.SetValue("ModelType", ModelType.ServiceModel);
			dictionary.SetValue("ModelName", "ModelName1");

			Assert.AreEqual(ModelType.ServiceModel, model.ModelType);
			Assert.AreEqual("ModelName1", model.ModelName);
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
