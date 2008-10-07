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
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Package;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

namespace Microsoft.Practices.ServiceFactory.VsPkg.ModelProject
{
	[Guid("1DF4D691-79FD-4e3e-B7A6-A1A5DC772BBD")]
	public class ModelProjectFactory : ProjectFactory
	{
		public ModelProjectFactory(VsPkg.ModelProjectPackage package)
			: base(package)
		{
		}

        protected override ProjectNode CreateProject()
        {
            ModelProjectNode projectNode = new ModelProjectNode();
            projectNode.SetSite((IOleServiceProvider)((IServiceProvider)this.Package).GetService(typeof(IOleServiceProvider)));
            return projectNode;
        }
    }
}
