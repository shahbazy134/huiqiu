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
using Microsoft.Practices.WizardFramework;
using System.ComponentModel.Design;
using System.Windows.Forms;
using Microsoft.Practices.ServiceFactory.Library.Validation;
using Microsoft.Practices.EnterpriseLibrary.Validation;

namespace Microsoft.Practices.ServiceFactory.Library.Presentation.Base
{
	public class ViewBase : CustomWizardPage, IView
	{
		private bool lastValidation;

		public ViewBase()
		{
		}

		public ViewBase(WizardForm wizard)
			: base(wizard)
		{
		}

		#region IView Members

		public IDictionaryService DictionaryService
		{
			get { return (IDictionaryService)GetService(typeof(IDictionaryService)); }
		}

		#endregion

		protected override void OnLoad(EventArgs e)
		{
			if (!this.DesignMode)
			{
				this.Logo = Properties.Resources.SF;
				this.Wizard.Title = Properties.Resources.ServiceSoftwareFactory;
			}
			base.OnLoad(e);
		}

		public override bool IsDataValid
		{
			get
			{
				return base.IsDataValid && this.LastValidation;
			}
		}

		protected bool LastValidation
		{
			get { return lastValidation; }
			set { lastValidation = value; }
		}

		protected ValidationEventArgs CreateValidationEventArgs(Control validatedControl, ErrorProvider errorProvider)
		{
			return new ValidationEventArgs(new ErrorProviderValidationResultReport(validatedControl, errorProvider));
		}

		protected virtual void InitializeControls()
		{
		}
	}
}