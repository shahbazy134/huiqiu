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
using Microsoft.VisualStudio.Modeling;
using Microsoft.Practices.ServiceFactory.HostDesigner;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.Practices.ComponentModel;
using Microsoft.Practices.VisualStudio.Helper;
using System.IO;
using Microsoft.Practices.Common;
using Microsoft.Practices.RecipeFramework.Library;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Practices.ServiceFactory.ValueProviders
{
	[ServiceDependency(typeof(SVsSolution))]
	public class HostModelEndpointAddressProvider : DesignerSelectedElementProvider, IAttributesConfigurable
	{
		private const string ServiceURLEndMaskNameAttribute = "ServiceURLEndMask";
		private string serviceURLEndMask;

		public override bool OnBeginRecipe(object currentValue, out object newValue)
		{
			object modelElement = null; 
			if (base.OnBeginRecipe(currentValue, out modelElement))
			{
				Proxy proxy = modelElement as Proxy;
				if (proxy != null && 
					proxy.Endpoint != null &&
					!string.IsNullOrEmpty(proxy.Endpoint.ServiceDescription.HostApplication.ImplementationProject))
				{
					newValue = GetEndpointAddress(proxy);
					if (this.Argument.Converter.IsValid(newValue))
					{
						return true;
					}
				}
			}
			newValue = null;
			return false;
		}

		#region IAttributesConfigurable Members

		public void Configure(System.Collections.Specialized.StringDictionary attributes)
		{
			Guard.ArgumentNotNull(attributes, "attributes");

			if(attributes.ContainsKey(ServiceURLEndMaskNameAttribute))
			{
				serviceURLEndMask = attributes[ServiceURLEndMaskNameAttribute];
			}
		}

		#endregion

		private string GetEndpointAddress(Proxy proxy)
		{
			IVsSolution solution = GetService<IVsSolution, SVsSolution>();
			using (HierarchyNode projectNode = new HierarchyNode(solution,
				proxy.Endpoint.ServiceDescription.HostApplication.ImplementationProject))
			{
				if (projectNode != null)
				{
					using (ProjectNode node = new ProjectNode(solution, projectNode.ProjectGuid))
					{
						Uri address = BuildAddress(node, proxy);
						if (address != null)
						{
							return address.AbsoluteUri;
						}
					}
				}
			}
			return string.Empty;
		}

		private Uri BuildAddress(ProjectNode node, Proxy proxy)
		{
			string url = Uri.EscapeUriString(
				GetBrowseURL(node) +
				Path.AltDirectorySeparatorChar + 
				proxy.Endpoint.ServiceDescription.Name + 
				this.serviceURLEndMask); 

			if (Uri.IsWellFormedUriString(url, UriKind.Absolute))
			{
				return new Uri(url);
			}
			return null;
		}


        /// <summary>
        /// Retrieves the BrowseURL by first trying the BrowseURL property for
        /// the project, then the WebApplication.BrowseURL
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private string GetBrowseURL(ProjectNode node)
        {
            string browseURL = string.Empty;

            browseURL = node.GetEvaluatedProperty("BrowseURL");

            if (browseURL.Length == 0)
            {
				// throws if not found
                browseURL = node.GetEvaluatedProperty("WebApplication.BrowseURL", true);
            }

            return browseURL;
        }
	}
}