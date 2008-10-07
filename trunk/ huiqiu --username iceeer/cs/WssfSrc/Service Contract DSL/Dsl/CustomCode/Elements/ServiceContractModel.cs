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
using Microsoft.Practices.Modeling.CodeGeneration;
using Microsoft.VisualStudio.Modeling.Validation;
using Microsoft.Practices.ServiceFactory.Validation;
using Microsoft.Practices.Modeling.ExtensionProvider.Extension;

namespace Microsoft.Practices.ServiceFactory.ServiceContracts
{
	[ValidationState(ValidationState.Enabled)]
	public partial class ServiceContractModel : IValidatableElement, ITechnologyProvider
	{
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

		#region ITechnologyProvider Members

		IExtensionProvider ITechnologyProvider.ImplementationTechnology
		{
			get { return this.ImplementationTechnology; }
		}

		#endregion
	}
}
