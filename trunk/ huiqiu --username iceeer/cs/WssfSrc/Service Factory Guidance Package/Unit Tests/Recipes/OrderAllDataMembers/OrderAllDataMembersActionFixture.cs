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
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Practices.ServiceFactory.Recipes.OrderAllDataMembers;
using Microsoft.VisualStudio.Modeling;
using Microsoft.Practices.ServiceFactory.DataContracts;

namespace GuidancePackage.Tests.Recipes.OrderAllDataMembers
{
	/// <summary>
	/// Summary description for OrderAllDataMembersActionFixture
	/// </summary>
	[TestClass]
	public class OrderAllDataMembersActionFixture : DataContractModelFixture
	{
		[TestMethod]
		[DeploymentItem("Microsoft.Practices.Modeling.Common.dll")]
		public void ShouldOrderDataContractMembers()
		{
			MockOrderAllDataMembersAction mock = new MockOrderAllDataMembersAction();
			mock.SelectedElement = base.CreateDefaultDataContract();
			mock.Order();

			DataContract result = (DataContract)mock.SelectedElement;
			Assert.AreEqual<int>(2, mock.OrderedDataMembers[result.Name]);
			Assert.IsTrue((bool)result.ObjectExtender.GetType().GetProperty("OrderParts").GetValue(result.ObjectExtender, null));
			Assert.AreEqual<int>(0, (int)result.DataMembers[0].ObjectExtender.GetType().GetProperty("Order").GetValue(result.DataMembers[0].ObjectExtender, null));
			Assert.AreEqual<int>(1, (int)result.DataMembers[1].ObjectExtender.GetType().GetProperty("Order").GetValue(result.DataMembers[1].ObjectExtender, null));
		}

		[TestMethod]
		[DeploymentItem("Microsoft.Practices.Modeling.Common.dll")]
		public void ShouldOrderFaultContractMembers()
		{
			MockOrderAllDataMembersAction mock = new MockOrderAllDataMembersAction();
			mock.SelectedElement = base.CreateDefaultFaultContract();
			mock.Order();

			FaultContract result = (FaultContract)mock.SelectedElement;
			Assert.AreEqual<int>(2, mock.OrderedDataMembers[result.Name]);
			Assert.IsTrue((bool)result.ObjectExtender.GetType().GetProperty("OrderParts").GetValue(result.ObjectExtender, null));
			Assert.AreEqual<int>(0, (int)result.DataMembers[0].ObjectExtender.GetType().GetProperty("Order").GetValue(result.DataMembers[0].ObjectExtender, null));
			Assert.AreEqual<int>(1, (int)result.DataMembers[1].ObjectExtender.GetType().GetProperty("Order").GetValue(result.DataMembers[1].ObjectExtender, null));
		}

		#region MockOrderAllDataMembersAction class

		class MockOrderAllDataMembersAction : OrderAllDataMembersAction
		{
			private Dictionary<string, int> orderedDataMembers = new Dictionary<string, int>();

			public Dictionary<string, int> OrderedDataMembers
			{
				get { return orderedDataMembers; }
			}

			public void Order()
			{
				this.OrderDataMembers();
			}

			protected override void OrderedDataMember(string name, int index)
			{
				orderedDataMembers.Add(name, index);
			}
		}

		#endregion
	}
}
