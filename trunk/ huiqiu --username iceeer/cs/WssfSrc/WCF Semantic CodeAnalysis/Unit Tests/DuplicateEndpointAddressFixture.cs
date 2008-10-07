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
using System.Configuration;
using Microsoft.Practices.FxCop.Rules.WcfSemantic.Tests.Utilities;
using Microsoft.Cci;

namespace Microsoft.Practices.FxCop.Rules.WcfSemantic.Tests
{
    /// <summary>
    /// Summary description for DuplicateEndpointAddressFixture
    /// </summary>
    [TestClass]
    public class DuplicateEndpointAddressFixture
    {
        [TestMethod]
        [DeploymentItem(@"TestConfigs\DuplicateEndpointAddress.config")]
        public void ShouldGetOneProblemWithConfigFile()
        {
            Configuration configuration = ConfigurationLoader.LoadConfiguration("DuplicateEndpointAddress.config");
            DuplicateEndpointAddress rule = new DuplicateEndpointAddress();
            rule.Check(configuration);

            Assert.AreEqual(1, rule.Problems.Count);
            Assert.AreEqual("http://localhost:5665/host", rule.Problems[0].Resolution.Items[0]);
        }
    }
}
