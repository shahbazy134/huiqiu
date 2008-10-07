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
using System.ComponentModel;
using System.IO;
using Microsoft.Practices.ServiceFactory.Library.Validation;
using Microsoft.Practices.Common;
using Microsoft.Practices.RecipeFramework.Library;
using Microsoft.Practices.EnterpriseLibrary.Validation;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Practices.ServiceFactory.Validation;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;

namespace Microsoft.Practices.ServiceFactory.Library.Validation
{
	/// <summary>
	/// Validates if the specified value is a valid folder name.
	/// </summary>
	public class ValidFolderNameConverter : StringConverter
	{
		/// <summary>
		/// Returns whether the given value object is valid for this type and for the specified context.
		/// </summary>
		/// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"></see> that provides a format context.</param>
		/// <param name="value">The <see cref="T:System.Object"></see> to test for validity.</param>
		/// <returns>
		/// true if the specified value is valid for this object; otherwise, false.
		/// </returns>
        /// 
		public override bool IsValid(ITypeDescriptorContext context, object value)
		{
            AndCompositeValidator validator = new AndCompositeValidator
                (
                    new FileNameValidator(),
                    new NamespaceValidator()
                );

            // Replace the empty space
            string folderName = value.ToString().Replace(" ", "_");

            ValidationResults result = validator.Validate(folderName);
            return result.IsValid;
		}
	}
}
