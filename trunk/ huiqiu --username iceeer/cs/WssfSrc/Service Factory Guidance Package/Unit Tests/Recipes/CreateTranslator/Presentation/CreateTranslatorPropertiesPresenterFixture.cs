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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.ComponentModel.Design;
using Microsoft.Practices.ServiceFactory.Recipes.CreateTranslator.Presentation;
using Microsoft.Practices.UnitTestLibrary;
using Microsoft.Practices.ServiceFactory.Library.Validation;
using ServicesGuidancePackage.Tests.Common;

namespace ServicesGuidancePackage.Tests.CreateTranslator.Presentation
{
    [TestClass]
	public class CreateTranslatorPropertiesPresenterFixture
    {
        [TestMethod]
        public void ShouldBeInvalidWithSameTypes()
        {
            IDictionaryService dictionary;
			DefaultValidationResultReport context;
			MockCreateTranslatorPropertiesView view;
			CreateTranslatorPropertiesModel model;
            Setup(out dictionary, out context, out view, out model);

            view.MappingClassName = "Foo";
            view.MappingClassNamespace = "MyNamespace";
            view.FirstType = typeof(object);
            view.SecondType = typeof(object);

            Assert.IsFalse(model.IsValid);
        }

        [TestMethod]
        public void ShouldBeValidWithValidArguments()
        {
            IDictionaryService dictionary;
			DefaultValidationResultReport context;
			MockCreateTranslatorPropertiesView view;
			CreateTranslatorPropertiesModel model;
            Setup(out dictionary, out context, out view, out model);

            view.MappingClassName = "Foo";
            view.MappingClassNamespace = "MyNamespace";
            view.FirstType = typeof(object);
            view.SecondType = typeof(int);

            Assert.IsTrue(model.IsValid);
        }

        [TestMethod]
        public void ShouldBeInvalidWithInvalidNamespace()
        {
            IDictionaryService dictionary;
			DefaultValidationResultReport context;
			MockCreateTranslatorPropertiesView view;
			CreateTranslatorPropertiesModel model;
            Setup(out dictionary, out context, out view, out model);

            view.MappingClassName = "Foo.cs";
            view.MappingClassNamespace = null;

            Assert.IsFalse(model.IsValid);
        }

        [TestMethod]
        public void ShouldBeInvalidWithInvalidClassName()
        {
			IDictionaryService dictionary;
			DefaultValidationResultReport context;
			MockCreateTranslatorPropertiesView view;
			CreateTranslatorPropertiesModel model;
            Setup(out dictionary, out context, out view, out model);

            view.MappingClassName = null;
            view.MappingClassNamespace = "MyNamespace";

            Assert.IsFalse(model.IsValid);
        }

        [TestMethod]
        public void ValidateNamespace()
        {
			IDictionaryService dictionary;
			DefaultValidationResultReport context;
			MockCreateTranslatorPropertiesView view;
			CreateTranslatorPropertiesModel model;
            Setup(out dictionary, out context, out view, out model);

            view.MappingClassNamespace = "This is an invalid namespace";

			Assert.AreEqual("Invalid namespace", context.Message);

        }

		private static void Setup(out IDictionaryService dictionary, out DefaultValidationResultReport context, out MockCreateTranslatorPropertiesView view, out CreateTranslatorPropertiesModel model)
        {
            dictionary = new MockDictionaryService();
			context = new DefaultValidationResultReport();
			view = new MockCreateTranslatorPropertiesView(context);
			model = new CreateTranslatorPropertiesModel(dictionary);
            MockProjectModel project = new MockProjectModel();

			CreateTranslatorPropertiesPresenter presenter =
					new CreateTranslatorPropertiesPresenter(project, view, model);
        }
    }

	public class MockCreateTranslatorPropertiesView : ICreateTranslatorPropertiesView
	{
		#region ICreateTranslatorPropertiesView Members

		private string mappingClassNamespace;

        public string MappingClassNamespace
        {
            get
            {
                return mappingClassNamespace;
            }
            set
            {
                mappingClassNamespace = value;
                MappingClassNamespaceChanged("MappingClassNamespace", CreateValidationEventArgs());
            }
        }

        private Type firstType;

        public Type FirstType
        {
            get
            {
                return firstType;
            }
            set
            {
                firstType = value;
                FirstTypeChanged(this, CreateValidationEventArgs());
            }
        }

        private Type secondType;

        public Type SecondType
        {
            get
            {
                return secondType;
            }
            set
            {
                secondType = value;
                SecondTypeChanged(this, CreateValidationEventArgs());
            }
        }

        string mappginClassName;

        public string MappingClassName
        {
            get
            {
                return mappginClassName;
            }
            set
            {
                mappginClassName = value;
                MappingClassNameChanged(this, CreateValidationEventArgs());
            }
        }

        private ValidationEventArgs CreateValidationEventArgs()
        {
            return new ValidationEventArgs(context);
        }

        public event EventHandler<ValidationEventArgs> MappingClassNameChanged;

        public event EventHandler<ValidationEventArgs> MappingClassNamespaceChanged;

        public event EventHandler<ValidationEventArgs> FirstTypeChanged;

        public event EventHandler<ValidationEventArgs> SecondTypeChanged;

        #endregion

		private IValidationResultReport context;

		public MockCreateTranslatorPropertiesView(IValidationResultReport context)
        {
            this.context = context;
        }

		public event EventHandler<RequestDataEventArgs<bool>> RequestIsDataValid;

		protected virtual void OnRequestIsDataValid(object sender, RequestDataEventArgs<bool> e)
		{
		    if (RequestIsDataValid != null)
		    {
		        RequestIsDataValid(sender, e);
		    }
		}
    }
}