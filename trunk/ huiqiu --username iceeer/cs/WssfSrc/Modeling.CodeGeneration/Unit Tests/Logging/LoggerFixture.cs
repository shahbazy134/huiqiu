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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Practices.Modeling.CodeGeneration.Logging;
using System.Diagnostics;

namespace Microsoft.Practices.Modeling.CodeGeneration.Tests.Logging
{
	[TestClass]
	public class LoggerFixture
	{
		[TestInitialize]
		public void SetUp()
		{
			Logger.Reset();
			MockTraceListener.Reset();
		}

		[TestCleanup]
		public void TearDown()
		{
			Logger.Reset();
			MockTraceListener.Reset();
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void TestWriteWithNullParameter1()
		{
			Logger.Write(null);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void TestWriteWithNullParameter2()
		{
			Logger.Write(null, string.Empty);
		}
	}
}