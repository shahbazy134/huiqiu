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
using System.Diagnostics;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Validation.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Validation;
using System.Collections.Specialized;
using Microsoft.Practices.Modeling.CodeGeneration;
using Microsoft.VisualStudio.Modeling;
using System.Globalization;

namespace Microsoft.Practices.ServiceFactory.Validation
{
	/// <summary>
	/// Performs validation on an ModelElement by applying the validation rules specified for a supplied type.
	/// </summary>
    [ConfigurationElementType(typeof(CustomValidatorData))]
    public class ElementObjectValidator : ConfigurableObjectValidator<ModelElement>
    {
		public ElementObjectValidator(NameValueCollection configuration)
			: base(configuration)
        {
			base.TargetConfigurationFile = ValidationEngine.GetConfigurationRulePath();
        }

		protected override bool IsValidated(ModelElement objectToValidate)
		{
			Debug.WriteLine(String.Format(CultureInfo.CurrentUICulture, "Validating '{0}'...", ValidationEngine.ModelElementToString(objectToValidate)));
			Debug.WriteLineIf(ValidationEngine.IsValidated(objectToValidate.Id), String.Format(CultureInfo.CurrentUICulture, "{0} Skipping", ValidationEngine.ModelElementToString(objectToValidate)));

			return ValidationEngine.IsValidated(objectToValidate.Id);
		}

		protected override void DoValidate(ModelElement objectToValidate, object currentTarget, string key, ValidationResults validationResults)
		{
			base.DoValidate(objectToValidate, currentTarget, key, validationResults);
		}
    }
}
