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
using System.Collections.Specialized;
using System.Globalization;
using System.Text;
using System.Threading;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Validation;
using Microsoft.Practices.EnterpriseLibrary.Validation.Configuration;
using Microsoft.Practices.Modeling.CodeGeneration;
using Microsoft.Practices.Modeling.Dsl.Service;
using Microsoft.VisualStudio.Modeling;
using System.Diagnostics;
using Microsoft.Practices.Modeling.Dsl.Integration.Helpers;

namespace Microsoft.Practices.ServiceFactory.Validation
{
	/// <summary>
	/// Validate that the model reference points to a MEL that exists on another model.
	/// </summary>
	[ConfigurationElementType(typeof(CustomValidatorData))]
	public class CrossModelReferenceValidator : Validator<string>
	{
		private string elementNameProperty;
		private bool validateReferencedElement;

		/// <summary>
		/// Initializes a new instance of the <see cref="CrossModelReferenceValidator"/> class.
		/// </summary>
		/// <param name="attributes">The attributes.</param>
		public CrossModelReferenceValidator(NameValueCollection attributes)
			: base(null, null)
        {
			if (attributes == null)
			{
				return;
			}

			elementNameProperty = attributes.Get("elementNameProperty") ?? "Name";
			Boolean.TryParse(attributes.Get("validateReferencedElement") ?? "false", out validateReferencedElement);
		}

		/// <summary>
		/// Does the validate.
		/// </summary>
		/// <param name="objectToValidate">The object to validate.</param>
		/// <param name="currentTarget">The current target.</param>
		/// <param name="key">The key.</param>
		/// <param name="validationResults">The validation results.</param>
		protected override void DoValidate(string objectToValidate, object currentTarget, string key, ValidationResults validationResults)
		{
			ModelElement currentElement = currentTarget as ModelElement;
			Debug.Assert(currentElement != null);
			string elementNamePropertyValue = ValidationEngine.GetUniquePropertyValue(currentTarget, elementNameProperty);

			if (string.IsNullOrEmpty(objectToValidate))
			{
				validationResults.AddResult(
					new ValidationResult(String.Format(CultureInfo.CurrentUICulture, Resources.ModelReferenceValidatorMessage, key, currentTarget.GetType().Name, elementNamePropertyValue), currentTarget, key, String.Empty, this));
				return;
			}

			if (!DomainModelHelper.IsModelReferenceValid(objectToValidate))
			{
				validationResults.AddResult(
					new ValidationResult(String.Format(CultureInfo.CurrentUICulture, this.MessageTemplate, key, currentTarget.GetType().Name, elementNamePropertyValue), currentTarget, key, String.Empty, this));
				return;
			}

			ModelElement referencedElement = ResolveReference(currentElement, objectToValidate);

			if (referencedElement == null)
			{
				validationResults.AddResult(
					new ValidationResult(String.Format(CultureInfo.CurrentUICulture, this.DefaultMessageTemplate, key, currentTarget.GetType().Name, elementNamePropertyValue), currentTarget, key, String.Empty, this));
				return;
			}

			if (validateReferencedElement)
			{
				IValidatableElement validatableReferencedElement = referencedElement as IValidatableElement;
 				if (validatableReferencedElement != null)
 				{
					Debug.Assert(ValidationEngine.CurrentValidationContext != null);
					validatableReferencedElement.ContinueValidation(ValidationEngine.CurrentValidationContext);
 				}
			}
		}

		/// <summary>
		/// Resolves the reference.
		/// </summary>
		/// <param name="currentElement">The current element.</param>
		/// <param name="reference">The reference.</param>
		/// <returns></returns>
		protected virtual ModelElement ResolveReference(ModelElement currentElement, string reference)
		{
			Debug.Assert(currentElement != null);
			if (currentElement == null)
			{
				throw new ArgumentNullException("currentElement");
			}

			IDslIntegrationService dslIntegrationService = (IDslIntegrationService)((IServiceProvider)currentElement.Store).GetService(typeof(IDslIntegrationService));
			if (dslIntegrationService == null)
			{
				throw new InvalidOperationException(Resources.ModelReferenceValidator_NoDis);
			}

			return DomainModelHelper.ResolveModelReference(dslIntegrationService, reference, true);
		}

		/// <summary>
		/// Gets the default message template.
		/// </summary>
		/// <value>The default message template.</value>
		protected override string DefaultMessageTemplate
		{
			get { return Resources.ModelReferenceValidatorMessage; }
		}
	}
}
