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
using System.IO;
using Microsoft.Practices.EnterpriseLibrary.Validation;
using Microsoft.Practices.ServiceFactory.Validation;

namespace Microsoft.Practices.ServiceFactory.Validation
{
	/// <summary>
    /// Validator for checking existing files
    /// </summary>
	public class FileExistsValidator : Validator<string>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="FileExistsValidator"/> class.
		/// </summary>
		public FileExistsValidator()
			: base(Resources.FileExistsValidatorMessage, null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="FileExistsValidator"/> class.
		/// </summary>
		/// <param name="errorMessage">The error message.</param>
		public FileExistsValidator(string errorMessage)
			: base(errorMessage, null)
		{
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
			if (!File.Exists(objectToValidate))
			{
				this.LogValidationResult(validationResults, this.MessageTemplate, currentTarget, key);
			}
		}

		/// <summary>
		/// Gets the default message template.
		/// </summary>
		/// <value>The default message template.</value>
		protected override string DefaultMessageTemplate
		{
			get { return Resources.FileExistsValidatorMessage; }
		}
	}
}
