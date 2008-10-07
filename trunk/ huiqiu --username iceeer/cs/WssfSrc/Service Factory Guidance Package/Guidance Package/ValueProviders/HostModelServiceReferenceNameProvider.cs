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
using Microsoft.Practices.Modeling.Dsl.Integration.ValueProviders;
using Microsoft.Practices.ServiceFactory.HostDesigner;

namespace Microsoft.Practices.ServiceFactory.ValueProviders
{
	public class HostModelServiceReferenceNameProvider : DesignerSelectedElementProvider
	{
		public override bool OnBeginRecipe(object currentValue, out object newValue)
		{
			object modelElement;
			newValue = string.Empty;

			if (base.OnBeginRecipe(currentValue, out modelElement))
			{
				Proxy proxy = modelElement as Proxy;
				if (proxy != null)
				{
					if(!string.IsNullOrEmpty(proxy.Name))
					{
						newValue = proxy.Name;
					}
					else if (proxy.Endpoint != null)
					{
						newValue = 	proxy.Endpoint.ServiceDescription.Name;
					}
					return true;
				}
			}
			
			return false;
		}
	}
}
