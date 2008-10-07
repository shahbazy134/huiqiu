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
using Microsoft.VisualStudio.Package.Automation;
using Microsoft.VisualStudio.Package;
using System.Runtime.InteropServices;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Practices.ServiceFactory.VsPkg.ModelProject
{
    [ComVisible(true)]
    public class OAModelProject : OAProject
    {
        #region Constructors        
		public OAModelProject(ModelProjectNode project)
            : base(project)
        {
        }
        #endregion

		// catch any COMException in ActiveConfiguration so we fail silently returning a null ConfigurationManager
		// fixes the Setup project wizard error.
		public override EnvDTE.ConfigurationManager ConfigurationManager
		{
			get
			{
				EnvDTE.ConfigurationManager configuration = base.ConfigurationManager;

				try
				{
					return configuration.ActiveConfiguration != null ? configuration : null;
				}
				catch (COMException)
				{
					return null;
				}
			}
		}
    }

    [ComVisible(true)]
    [Guid("92E10853-D626-4A1B-BEAD-0A274D1054AF")]
    public class OAModelProjectFileNode : OAFileItem
    {
        #region Constructors
        public OAModelProjectFileNode(OAProject project, FileNode node)
            : base(project, node)
        {
        }
        #endregion
    }
}
