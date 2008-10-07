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

using Microsoft.Practices.Modeling.CodeGeneration;
using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Modeling.Validation;
using Microsoft.Practices.Modeling.Dsl.Integration.Helpers;

namespace Microsoft.Practices.ServiceFactory.ServiceContracts
{
	internal partial class ServiceContractDslDocData : IValidationControllerAccesor
	{
		ModelingDocDataObserver observer;

		public override void Initialize(Store sharedStore)
		{
			base.Initialize(sharedStore);
			observer = new ModelingDocDataObserver(this);
		}

		protected override void Dispose(bool disposing)
		{
			try
			{
				if(disposing && observer != null)
				{
					observer.Dispose();
					observer = null;
				}
			}
			finally
			{
				base.Dispose(disposing);
			}
		}

		protected override bool CanSave(bool allowUserInterface)
		{
			return observer.CanSave<ServiceContractModel>(
				this.ValidationController, 
				allowUserInterface,
				ServiceContractDslDomainModel.SingletonResourceManager.GetString("UnloadableSaveValidationFailed"),
				ServiceContractDslDomainModel.SingletonResourceManager.GetString("SaveValidationFailed"));
		}

		protected override void Save(string fileName)
		{
			DesignerHelper.EnsureValidPath(fileName, this);
			base.Save(fileName);
		}

		#region IValidationControllerAccesor Members

		public ValidationController Controller
		{
			get
			{
				return this.ValidationController;
			}
		}

		#endregion
	}
}