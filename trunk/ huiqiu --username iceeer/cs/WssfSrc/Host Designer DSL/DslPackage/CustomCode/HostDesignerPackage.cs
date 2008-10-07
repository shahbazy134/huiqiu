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
using System.ComponentModel.Design;

namespace Microsoft.Practices.ServiceFactory.HostDesigner
{
	internal sealed partial class HostDesignerPackage : HostDesignerPackageBase
	{
		protected override void Initialize()
		{
			base.Initialize();

			IServiceContainer serviceContainer = this.GetService(typeof(IServiceContainer)) as IServiceContainer;
			if (serviceContainer != null)
			{
				serviceContainer.AddService(typeof(ExplorerSynchronization), new ExplorerSynchronization());
			}
		}
	}
}
