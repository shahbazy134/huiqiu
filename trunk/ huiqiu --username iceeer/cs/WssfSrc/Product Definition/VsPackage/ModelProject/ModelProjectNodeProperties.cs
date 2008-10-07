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
using Microsoft.VisualStudio.Package;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Practices.ServiceFactory.VsPkg.ModelProject
{
    [ComVisible(true)]
    [SuppressMessage("Microsoft.Interoperability","CA1408:DoNotUseAutoDualClassInterfaceType")]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [Guid("F2DDD451-D6DB-4085-89A2-53277739D914")]
    public class ModelProjectNodeProperties : ProjectNodeProperties
    {
        public ModelProjectNodeProperties(ProjectNode node)
            : base(node)
        {
        }

		[Browsable(false)]
		public string TargetFramework
		{
			get; set; 
		}
    }
}
