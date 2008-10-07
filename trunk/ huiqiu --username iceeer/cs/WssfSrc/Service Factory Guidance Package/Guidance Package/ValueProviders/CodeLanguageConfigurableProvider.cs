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
using EnvDTE;
using Microsoft.Practices.RecipeFramework.Extensions.CommonHelpers;
using Microsoft.Practices.ComponentModel;
using Microsoft.Practices.RecipeFramework.Library;

namespace Microsoft.Practices.ServiceFactory.ValueProviders
{
	public class CodeLanguageConfigurableProvider : SelectedProjectProviderBase
	{
		public override bool OnBeginRecipe(object currentValue, out object newValue)
		{
			return this.SetValue(currentValue, out newValue);
		}

		private bool SetValue(object currentValue, out object newValue)
		{
			if (currentValue == null)
			{
				newValue = null;
				if (this.SelectedProject == null)
				{
					return false;
				}
				newValue = DteHelperEx.GetCodeDomProvider(this.SelectedProject);
				return true;
			}
			newValue = null;
			return false;
		}
	}
}
