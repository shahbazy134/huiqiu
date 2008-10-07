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
using Microsoft.Practices.RecipeFramework;
using Microsoft.Practices.Common;
using System.ComponentModel.Design;
using Microsoft.Practices.ComponentModel;
using Microsoft.Practices.RecipeFramework.Library;

namespace Microsoft.Practices.ServiceFactory.ValueProviders
{
	public class ProjectNamespaceProvider : SelectedProjectProviderBase
	{
		public override bool OnArgumentChanged(string changedArgumentName, object changedArgumentValue, object currentValue, out object newValue)
		{
			newValue = GetNamespace(changedArgumentValue as string);
			return true;
		}

		public override bool OnBeforeActions(object currentValue, out object newValue)
		{
			newValue = currentValue ?? GetNamespace(string.Empty);
			return true;
		}

		private string GetNamespace(string suffix)
		{
			if (this.SelectedProject == null ||
				DteHelper.IsWebProject(this.SelectedProject))
			{
				return string.Empty;
			}

			return this.SelectedProject.Properties.Item("DefaultNamespace").Value.ToString() +
					(string.IsNullOrEmpty(suffix) ? string.Empty : "." + suffix);
		}
	}
}
