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
using Microsoft.VisualStudio.Package;
using Microsoft.VisualStudio.Shell;
using System.Diagnostics;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Microsoft.Practices.ServiceFactory.VsPkg.ModelProject
{
    [ComVisible(true)]
    [Guid("9896202D-ADB9-41DD-8B0F-C415AF8C7D4D")]
    public class ModelNodeProperties :  FileNodeProperties
    {
        public ModelNodeProperties(FileNode node)
            : base(node)
        {
        }
    }
}
