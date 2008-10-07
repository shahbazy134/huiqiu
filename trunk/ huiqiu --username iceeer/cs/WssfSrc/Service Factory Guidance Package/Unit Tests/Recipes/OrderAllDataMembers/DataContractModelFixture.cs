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
using System.CodeDom.Compiler;
using Microsoft.VisualStudio.Modeling.Diagrams;
using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.TextTemplating;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Practices.ServiceFactory.DataContracts;
using Microsoft.Practices.UnitTestLibrary;
using Microsoft.Practices.UnitTestLibrary.Utilities;

namespace GuidancePackage.Tests.Recipes.OrderAllDataMembers
{
	public class DataContractModelFixture: ModelFixture
	{
		protected const string ElementName = "MyElementName";
		protected const string ElementNamespace = "http://mynamespace";
		protected const string PrimitiveDataElementName1 = "Element1";
		protected const string PrimitiveDataElementType1 = "System.Int32";
		protected const string PrimitiveDataElementName2 = "Element2";
		protected const string PrimitiveDataElementType2 = "System.String";

		private Store store = null;
		private DataContractDslDomainModel dm = null;

		protected DataContract CreateDefaultDataContract()
		{
			DataContract rootElement = new DataContract(Store);
			rootElement.DataContractModel = new DataContractModel(Store);
			rootElement.Name = ElementName;
			rootElement.Namespace = ElementNamespace;
			rootElement.DataMembers.Add(LoadPrimitiveDataElement(PrimitiveDataElementName1, PrimitiveDataElementType1));
			rootElement.DataMembers.Add(LoadPrimitiveDataElement(PrimitiveDataElementName2, PrimitiveDataElementType2));
			rootElement.ObjectExtender = new ContractExtender();
			return rootElement;
		}

		protected FaultContract CreateDefaultFaultContract()
		{
			FaultContract rootElement = new FaultContract(Store);
			rootElement.DataContractModel = new DataContractModel(Store);
			rootElement.Name = ElementName;
			rootElement.Namespace = ElementNamespace;
			rootElement.DataMembers.Add(LoadPrimitiveDataElement(PrimitiveDataElementName1, PrimitiveDataElementType1));
			rootElement.DataMembers.Add(LoadPrimitiveDataElement(PrimitiveDataElementName2, PrimitiveDataElementType2));
			rootElement.ObjectExtender = new ContractExtender();
			return rootElement;
		}

		private PrimitiveDataType LoadPrimitiveDataElement(string name, string type)
		{
			PrimitiveDataType element = new PrimitiveDataType(Store);
			element.ObjectExtender = new DataMemberExtender();
			element.Name = name;
			element.Type = type;
			return element;
		}

		protected override Store Store
		{
			get
			{
				if ( store==null )
				{
					store = new Store(ServiceProvider, typeof(CoreDesignSurfaceDomainModel), typeof(DataContractDslDomainModel));
				}
				return store;
			}
		}

		protected override DomainModel DomainModel
		{
			get
			{
				if ( dm==null )
				{
					dm = Store.GetDomainModel<DataContractDslDomainModel>();
				}
				return dm;
			}
		}

		protected override Type ContractType
		{
			get { throw new NotImplementedException(); }
		}

		protected override string Template
		{
			get { throw new NotImplementedException(); }
		}
	}

	#region Extenders

	class DataMemberExtender
	{
		private int order;

		public int Order
		{
			get { return order; }
			set { order = value; }
		}
	}

	class ContractExtender
	{
		private bool orderParts;

		public bool OrderParts
		{
			get { return orderParts; }
			set { orderParts = value; }
		}
	}

	#endregion
}
