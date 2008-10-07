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
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;
using Microsoft.Practices.EnterpriseLibrary.Validation;
using Microsoft.Practices.RecipeFramework.Library;

namespace Microsoft.Practices.ServiceFactory.Library.Validation
{
	/// <summary/>
	public class ValidationEventArgs : EventArgs
	{
		private IValidationResultReport report;
		private ValidationResults validationResults;

		public ValidationEventArgs(IValidationResultReport report)
		{
			this.report = report;
		}

		/// <summary>
		/// Gets the results of the validation for the control.
		/// </summary>
		public ValidationResults ValidationResults
		{
			get { return validationResults; }
			set
			{
				Guard.ArgumentNotNull(value, "value");
				validationResults = value;
				if (validationResults.IsValid)
				{
					report.Clear();
				}
				else
				{
					report.SetError(GetMessage());
				}
			}
		}

		private string GetMessage()
		{
			if (validationResults != null)
			{
				string message = string.Empty;
				foreach (ValidationResult result in validationResults)
				{
					message = (message.Length == 0 ? message : message + Environment.NewLine) + result.Message;
				}
				return message;
			}
			return String.Empty;
		}
	}
}
