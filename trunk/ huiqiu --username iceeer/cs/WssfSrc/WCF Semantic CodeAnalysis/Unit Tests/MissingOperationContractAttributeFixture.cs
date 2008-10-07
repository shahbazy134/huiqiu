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
using Microsoft.Cci;
using System.ServiceModel;

namespace Microsoft.Practices.FxCop.Rules.WcfSemantic.Tests
{
    /// <summary>
    /// Summary description for MissingOperationContractAttributeFixture
    /// </summary>
    [TestClass]
    public class MissingOperationContractAttributeFixture
    {
        [TestMethod]
        public void ShouldGetOneProblemWithOneMissingAttribute()
        {
            MissingOperationContractAttribute rule = new MissingOperationContractAttribute();
            rule.Check(TypeNode.GetTypeNode(typeof(IMockMissingOperationAttribute)));
            
            Assert.AreEqual(1, rule.Problems.Count);
            Assert.AreEqual("BadOperation", rule.Problems[0].Resolution.Items[0]);
        }
    }

    [ServiceContract(Name = "IMockMissingOperationAttribute")]
    interface IMockMissingOperationAttribute
    {
        [OperationContract()]
        string GoodOperation(string message);
        string BadOperation(string message);
    }
}