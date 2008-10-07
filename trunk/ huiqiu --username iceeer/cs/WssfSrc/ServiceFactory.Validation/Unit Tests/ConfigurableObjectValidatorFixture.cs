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
using System.Collections.Specialized;
using System.Text;
using System.Collections.Generic;
using Microsoft.Practices.EnterpriseLibrary.Validation.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Practices.EnterpriseLibrary.Validation;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using System.IO;

namespace Microsoft.Practices.ServiceFactory.Validation.Tests
{
	[TestClass]
	public class ConfigurableObjectValidatorFixture
	{
		[TestMethod]
		[DeploymentItem("Microsoft.Practices.EnterpriseLibrary.Validation.dll")]
		[DeploymentItem("Microsoft.Practices.EnterpriseLibrary.Common.dll")]
		[DeploymentItem("Microsoft.Practices.ObjectBuilder.dll")]
		[DeploymentItem("Microsoft.Practices.RecipeFramework.Library.dll")]
        [DeploymentItem("ConfigurableObjectValidatorFixture.config")]
		public void SingleObjectPassesValidation()
		{
            string configurationFile = Path.Combine(Directory.GetCurrentDirectory(), @"ConfigurableObjectValidatorFixture.config");

			TheClass a = new TheClass();
			a.Property = "valid";

			Validator validator = ValidationFactory.CreateValidatorFromConfiguration(typeof(TheClass), 
				"Rule Set",
                new FileConfigurationSource(configurationFile));
			ValidationResults result = validator.Validate(a);

			Assert.IsTrue(result.IsValid);
		}

		[TestMethod]
		[DeploymentItem("ConfigurableObjectValidatorFixture.config")]
		public void SingleObjectFailsValidation()
		{
            string configurationFile = Path.Combine(Directory.GetCurrentDirectory(), @"ConfigurableObjectValidatorFixture.config");

			TheClass a = new TheClass();
			a.Property = "bad";

			Validator validator = ValidationFactory.CreateValidatorFromConfiguration(typeof(TheClass), 
				"Rule Set",
                new FileConfigurationSource(configurationFile));
			ValidationResults result = validator.Validate(a);
			List<ValidationResult> results = new List<ValidationResult>(result);

			Assert.IsFalse(result.IsValid);
			Assert.AreEqual(1, results.Count);
			Assert.AreEqual("Invalid property", results[0].Message);
		}

		[TestMethod]
		[DeploymentItem("ConfigurableObjectValidatorFixture.config")]
		public void TraversesObjectGraphToInvalidObject()
		{
            string configurationFile = Path.Combine(Directory.GetCurrentDirectory(), @"ConfigurableObjectValidatorFixture.config");

			TheClass a = new TheClass();
			a.Child = new TheClass();
			a.Property = "valid";
			a.Child.Property = "bad";

			Validator validator = ValidationFactory.CreateValidatorFromConfiguration(typeof(TheClass), 
				"Rule Set",
                new FileConfigurationSource(configurationFile));
			ValidationResults result = validator.Validate(a);
			List<ValidationResult> results = new List<ValidationResult>(result);
			
			Assert.IsFalse(result.IsValid);
			Assert.AreEqual(1, results.Count);
			Assert.AreEqual("Invalid property", results[0].Message);
		}

		[TestMethod]
		[DeploymentItem("ConfigurableObjectValidatorFixture.config")]
		public void TraversesObjectGraphToValidObject()
		{
            string configurationFile = Path.Combine(Directory.GetCurrentDirectory(), @"ConfigurableObjectValidatorFixture.config");

			TheClass a = new TheClass();
			a.Child = new TheClass();
			a.Property = "valid";
			a.Child.Property = "valid";

			Validator validator = ValidationFactory.CreateValidatorFromConfiguration(typeof(TheClass), 
				"Rule Set",
                new FileConfigurationSource(configurationFile));
			ValidationResults result = validator.Validate(a);

			Assert.IsTrue(result.IsValid);
		}
	}

	[ConfigurationElementType(typeof(CustomValidatorData))]
	public class TestClassObjectValidator : ConfigurableObjectValidator<TheClass>
	{
		public TestClassObjectValidator(NameValueCollection configuration)
			: base(configuration)
        {
            TargetConfigurationFile = Path.Combine(
                            Directory.GetCurrentDirectory(),
                            configuration["fileConfigurationSource"]
                            );
        }	
	}

	public class TheClass
	{
		private TheClass _child;

		public TheClass Child
		{
			get { return _child; }
			set { _child = value; }
		}

		private string _property;

		public string Property
		{
			get { return _property; }
			set { _property = value; }
		}

	}
}