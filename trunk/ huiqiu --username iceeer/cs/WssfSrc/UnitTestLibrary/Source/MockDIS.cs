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
using Microsoft.Practices.Modeling.Dsl.Service;
using Microsoft.VisualStudio.Modeling;

namespace Microsoft.Practices.UnitTestLibrary
{
	public class MockDIS : IDslIntegrationService
	{
		ModelElement referencedElement;
		string dslNamespace;
		string moniker;
		Store store;

		public MockDIS(Store store)
		{
			this.store = store;
		}

		public MockDIS(ModelElement referencedElement, string dslNamespace, string moniker)
		{
			this.referencedElement = referencedElement;
			this.dslNamespace = dslNamespace;
			this.moniker = moniker;
		}

		#region IDslIntegrationService Members

		public IDomainModelBrowser GetDomainModelBrowser(string domainModelNamespace)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public string ResolveDocumentPath(string domainNamespace, string instanceNamespace)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public ModelElement ResolveExportedInstanceInDocument(string domainNamespace, string instanceNamespace, bool openDocumentIfNeeded)
		{
			if(this.store != null)
			{
				//Moniker format:
				//mel://[DSLNAMESPACE]\[MODELELEMENTTYPE]\[MODELELEMENT.GUID]@[PROJECT]\[MODELFILE]
				string[] data = instanceNamespace.Split('@');

				if(data.Length != 2)
				{
					return null;
				}

				string[] modelData = data[0].Split('\\');
				
				if(modelData.Length != 3)
				{
					return null;
				}

				Guid id = new Guid(modelData[2]);

				if(store.ElementDirectory.ContainsElement(id))
				{
					return store.ElementDirectory.GetElement(id);
				}
			}
			else if(domainNamespace == dslNamespace && instanceNamespace == moniker)
			{
				return referencedElement;
			}

			return null;
		}

		public ModelElement ResolveExportedInstanceInStore(string domainNamespace, string instanceNamespace, Microsoft.VisualStudio.Modeling.Store store)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		#endregion
	}
}