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
using System.Windows.Forms;

namespace Microsoft.Practices.ServiceFactory.Library.Validation
{
	public class ErrorProviderValidationResultReport : IValidationResultReport
	{
		private Control control;
		private ErrorProvider provider;

		public ErrorProviderValidationResultReport(Control control, ErrorProvider provider)
		{
			this.control = control;
			this.provider = provider;
		}

		#region IValidationResultReport Members

		public void Clear()
		{
			provider.Clear();
		}

		public void SetError(string message)
		{
			provider.SetError(control, message);
		}

		#endregion
	}
}
