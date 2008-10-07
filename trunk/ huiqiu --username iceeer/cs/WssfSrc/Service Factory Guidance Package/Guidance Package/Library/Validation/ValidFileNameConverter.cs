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
using System.Text.RegularExpressions;
using System.ComponentModel;
using Microsoft.Practices.Common;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using Microsoft.Practices.EnterpriseLibrary.Validation;
using Microsoft.Practices.RecipeFramework.Library;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Practices.ServiceFactory.Validation;

namespace Microsoft.Practices.ServiceFactory.Library.Validation
{
	public class ValidFileNameConverter : StringConverter, IAttributesConfigurable
	{
		public const string ExtensionKey = "Extension";
		private string extension;

		/// <summary>
		/// Returns whether the given value object is valid for this type and for the specified context.
		/// </summary>
		/// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"></see> that provides a format context.</param>
		/// <param name="value">The <see cref="T:System.Object"></see> to test for validity.</param>
		/// <returns>
		/// true if the specified value is valid for this object; otherwise, false.
		/// </returns>
		public override bool IsValid(ITypeDescriptorContext context, object value)
		{
			AndCompositeValidator validator = new AndCompositeValidator
				(
					new FileNameValidator(),
					new FileExtensionValidator(this.extension)
				);

			ValidationResults result = validator.Validate(value);
			return result.IsValid;
		}

		#region IAttributesConfigurable Members

		/// <summary>
		/// Configures the component using the dictionary of attributes specified
		/// in the configuration file.
		/// </summary>
		/// <param name="attributes">The attributes in the configuration element.</param>
		public void Configure(StringDictionary attributes)
		{
			Guard.ArgumentNotNull(attributes, "attributes");

			if (attributes.ContainsKey(ExtensionKey))
			{
				this.extension = attributes[ExtensionKey];
			}

			Guard.ArgumentNotNullOrEmptyString(this.extension, ExtensionKey);
		}

		#endregion
	}
}
