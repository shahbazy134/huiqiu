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
using Microsoft.Practices.RecipeFramework.Extensions.Actions.VisualStudio;
using Microsoft.Practices.ServiceFactory.Actions;
using Microsoft.Practices.Modeling.CodeGeneration.Logging;
using System.Globalization;

namespace Microsoft.Practices.ServiceFactory.Recipes.CreateWCFClientProxy
{
	public class AddClientProxyAction : AddItemFromStringToProjectItemByNameAction
	{
		public override void Execute()
		{
			base.Execute();
			Logger.Write(
				string.Format(CultureInfo.CurrentCulture, Properties.Resources.UpdatedFile, this.TargetFileName),
				String.Empty, System.Diagnostics.TraceEventType.Information, 1000);
		}
	}
}
