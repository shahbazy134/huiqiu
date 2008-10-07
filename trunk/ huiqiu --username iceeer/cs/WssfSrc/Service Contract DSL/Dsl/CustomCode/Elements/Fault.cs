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
using System.Collections.ObjectModel;
using System.ComponentModel;
using Microsoft.Practices.Modeling.ExtensionProvider.TypeDescription;
using Microsoft.Practices.Modeling.ExtensionProvider.Extension;
using Microsoft.VisualStudio.Modeling;

namespace Microsoft.Practices.ServiceFactory.ServiceContracts
{
	[TypeDescriptionProvider(typeof(ExtendedTypeDescriptionProvider))]
	[ValidationState(ValidationState.Enabled)]
	public partial class Fault : IExtensibleObject, IValidatableElement
	{
		#region IExtensibleObject Members

		private object objectExtender;

		[Browsable(false)]
		public object ObjectExtender
		{
			get { return objectExtender; }
			set { objectExtender = value; }
		}

		[Browsable(false)]
		public ModelElement ModelElement
		{
			get { return this; }
		}

		[Browsable(false)]
		public IExtensionProvider ExtensionProvider
		{
			get
			{
				if (this.Operation != null &&
					this.Operation.ServiceContractModel != null)
				{
					return this.Operation.ServiceContractModel.ImplementationTechnology;
				}
				return null;
			}
		}

		#endregion

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
