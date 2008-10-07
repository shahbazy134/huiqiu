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
using System.Runtime.Serialization;
using Microsoft.Practices.Modeling.Dsl.Integration.References;
using Microsoft.Practices.ServiceFactory.HostDesigner;
using Microsoft.Practices.Modeling.ExtensionProvider.Extension;
using Microsoft.Practices.Common;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Practices.RecipeFramework.Library;
using System.Security.Permissions;
using Microsoft.Practices.ServiceFactory.Extenders.HostDesigner.Wcf;

namespace Microsoft.Practices.ServiceFactory.References
{
	[Serializable]
	public class ClientProxyShapeReference : ShapeReference, IAttributesConfigurable
	{
		private const string ImplementationTechnologyNameAttribute = "ImplementationTechnology";
		private const string wcf = "WCF";
		private string implementationTechnology;

		public ClientProxyShapeReference(string recipe)
			: base(recipe)
		{
		}

		public override string AppliesTo
		{
			get { return Properties.Resources.ClientProxyShapeReference; }
		}

		public override bool IsEnabledFor(object target)
		{
			if(base.IsEnabledFor(target))
			{
				Proxy proxy = this.SelectedShape.ModelElement as Proxy;

				if(proxy != null)
				{
					if(VerifyExtenderTechnology(proxy.ClientApplication.ImplementationTechnology) && !string.IsNullOrEmpty(proxy.ClientApplication.ImplementationProject))
					{
						if(this.implementationTechnology.Equals(wcf, StringComparison.OrdinalIgnoreCase))
						{
							if(proxy.Endpoint != null && proxy.Endpoint.ServiceDescription != null)
							{
								ServiceDescription service = proxy.Endpoint.ServiceDescription;

								if(service.ObjectExtender != null && service.ObjectExtender is WcfServiceDescription)
								{
									WcfServiceDescription wcfService = service.ObjectExtender as WcfServiceDescription;

									return wcfService.EnableMetadataPublishing;
								}
							}
						}
						else
						{
							return (proxy.Endpoint != null);
						}
					}
				}
			}
			return false;
		}

		public new void Configure(System.Collections.Specialized.StringDictionary attributes)
		{
			Guard.ArgumentNotNull(attributes, "attributes");

			base.Configure(attributes);

			if(attributes.ContainsKey(ImplementationTechnologyNameAttribute))
			{
				implementationTechnology = attributes[ImplementationTechnologyNameAttribute];
			}
		}

		#region ISerializable Members

		protected ClientProxyShapeReference(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			Guard.ArgumentNotNull(info, "info");
			implementationTechnology = info.GetString(ImplementationTechnologyNameAttribute);
		}

		[SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
		protected override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue(ImplementationTechnologyNameAttribute, implementationTechnology);
		}
		#endregion ISerializable Members

		private bool VerifyExtenderTechnology(IExtensionProvider provider)
		{
			if(provider != null)
			{
				foreach(Type extender in provider.ObjectExtenders)
				{
					if(extender.Name.StartsWith(this.implementationTechnology, StringComparison.OrdinalIgnoreCase))
					{
						return true;
					}
				}
			}
			return false;
		}
	}
}
