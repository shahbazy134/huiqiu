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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Practices.ServiceFactory.RecipeFramework.Extensions.Editors.TypeBrowser;

namespace Microsoft.Practices.ServiceFactory.RecipeFramework.Extensions.Tests
{
	/// <summary>
	/// Summary description for PublicPrimitiveTypeFilterFixture
	/// </summary>
	[TestClass]
	public class PublicPrimitiveTypeFilterFixture
	{
		private PublicPrimitiveTypeFilter filter;

		[TestInitialize]
		public void TestInitialize()
		{
			filter = new PublicPrimitiveTypeFilter();
		}

		[TestMethod]
		public void ShouldReturnFalseOnEnum()
		{
			Assert.IsFalse( filter.CanFilterType(typeof(EnvironmentVariableTarget), false));
		}

		[TestMethod]
		public void ShouldReturnFalseOnObject()
		{
			Assert.IsFalse( filter.CanFilterType(typeof(object), false));
		}

		[TestMethod]
		public void ShouldReturnTrueOnGuid()
		{
			Assert.IsTrue( filter.CanFilterType(typeof(Guid), false));
		}

		[TestMethod]
		public void ShouldReturnTrueOnString()
		{
			Assert.IsTrue(filter.CanFilterType(typeof(string), false));
		}

		[TestMethod]
		public void ShouldReturnTrueOnPrimitive()
		{
			Assert.IsTrue( filter.CanFilterType(typeof(int), false));
		}
	}
}
