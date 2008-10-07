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
using EnvDTE;
using Microsoft.Practices.Modeling.Presentation.Models;

namespace Microsoft.Practices.ServiceFactory.Library.Presentation.Models
{
    public class DteProjectItemModel : IProjectItemModel
    {
        private ProjectItem projectItem;

        public DteProjectItemModel(ProjectItem projectItem)
        {
            this.projectItem = projectItem;
        }

        #region IProjectItemModel Members

        public string Name
        {
            get { return projectItem.Name; }
        }

        public string FullPath
        {
            get { return projectItem.Properties.Item("FullPath").Value as string; }
        }

        #endregion
    }
}
