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
using System.Collections.Specialized;
using Microsoft.Practices.EnterpriseLibrary.Validation.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using System.Collections;
using Microsoft.VisualStudio.Modeling;
using Microsoft.Practices.Modeling.CodeGeneration;
using System.Diagnostics;
using System.Globalization;

namespace Microsoft.Practices.ServiceFactory.Validation
{
	/// <summary>
	/// Performs validation on a collection of objects by applying the validation rules specified for a supplied type.
	/// </summary>
	[ConfigurationElementType(typeof(CustomValidatorData))]
	public class ElementObjectCollectionValidator : ConfigurableObjectCollectionValidator<ModelElement>
	{
		public ElementObjectCollectionValidator(NameValueCollection configuration)
            : base(configuration)
        {
			base.TargetConfigurationFile = ValidationEngine.GetConfigurationRulePath();
        }

		protected override bool IsValidated(ModelElement objectToValidate)
		{
			Debug.WriteLine(String.Format(CultureInfo.CurrentUICulture, "Validating '{0}'...", ValidationEngine.ModelElementToString(objectToValidate)));
			Debug.WriteLineIf(ValidationEngine.IsValidated(objectToValidate.Id), "Skipping");

			return ValidationEngine.IsValidated(objectToValidate.Id);
		}
    }
}
