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
using Microsoft.VisualStudio.Modeling.Validation;
using Microsoft.Practices.Modeling.CodeGeneration;
using Microsoft.Practices.ServiceFactory.Validation;

namespace Microsoft.Practices.ServiceFactory.DataContracts
{
    [ValidationState(ValidationState.Enabled)]
	public partial class ModelElementReference
	{
		/// <summary>
		/// Sets the linked element. Used for unit testing.
		/// </summary>
		/// <param name="linkedElement">The linked element.</param>
		[global::System.ComponentModel.Browsable(false)]
		public void SetLinkedElement(Guid linkedElement)
		{
			this.ModelElementGuid = linkedElement;
		}

        #region Validation support

        [ValidationMethod(ValidationCategories.Menu | ValidationCategories.Open | ValidationCategories.Save)]
        private void OnValidate(ValidationContext context)
        {
            ValidationEngine.Validate(ValidationElementState.FirstElement, context, this, context.Categories.ToString());
        }

        public void ContinueValidation(ValidationContext context)
        {
            ValidationEngine.Validate(ValidationElementState.LinkedElement, context, this, context.Categories.ToString());
        }

        #endregion
	}
}