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
using Microsoft.Practices.Modeling.ExtensionProvider.Serialization;

namespace Microsoft.Practices.ServiceFactory.Extenders.HostDesigner.Wcf.Tests
{
	/// <summary>
	/// Summary description for ExtendersSerializationFixture
	/// </summary>
	[TestClass]
	public class ExtendersSerializationFixture
	{
		[TestMethod]
		public void CanSerializeWcfEndpoint()
		{
			WcfEndpoint typeInstance = new WcfEndpoint();
			string typeRepresentation = GenericSerializer.Serialize<WcfEndpoint>(typeInstance);

			Assert.IsTrue(!String.IsNullOrEmpty(typeRepresentation), "Serialization failed");
			Assert.IsFalse(Uri.IsHexDigit(typeRepresentation[0]), "the specified type is not XML serializable");
		}

		[TestMethod]
		public void CanSerializeWcfServiceDescription()
		{
			WcfServiceDescription typeInstance = new WcfServiceDescription();
			string typeRepresentation = GenericSerializer.Serialize<WcfServiceDescription>(typeInstance);

			Assert.IsTrue(!String.IsNullOrEmpty(typeRepresentation), "Serialization failed");
			Assert.IsFalse(Uri.IsHexDigit(typeRepresentation[0]), "the specified type is not XML serializable");
		}

		[TestMethod]
		public void CanSerializeWCFDataContractMessagePart()
		{
			Microsoft.Practices.ServiceFactory.Extenders.ServiceContract.Wcf.WCFDataContractMessagePart typeInstance = new Microsoft.Practices.ServiceFactory.Extenders.ServiceContract.Wcf.WCFDataContractMessagePart();
			string typeRepresentation = GenericSerializer.Serialize<Microsoft.Practices.ServiceFactory.Extenders.ServiceContract.Wcf.WCFDataContractMessagePart>(typeInstance);

			Assert.IsTrue(!String.IsNullOrEmpty(typeRepresentation), "Serialization failed");
			Assert.IsFalse(Uri.IsHexDigit(typeRepresentation[0]), "the specified type is not XML serializable");
		}

		[TestMethod]
		public void CanSerializeWCFMessageContract()
		{
			Microsoft.Practices.ServiceFactory.Extenders.ServiceContract.Wcf.WCFMessageContract typeInstance = new Microsoft.Practices.ServiceFactory.Extenders.ServiceContract.Wcf.WCFMessageContract();
			string typeRepresentation = GenericSerializer.Serialize<Microsoft.Practices.ServiceFactory.Extenders.ServiceContract.Wcf.WCFMessageContract>(typeInstance);

			Assert.IsTrue(!String.IsNullOrEmpty(typeRepresentation), "Serialization failed");
			Assert.IsFalse(Uri.IsHexDigit(typeRepresentation[0]), "the specified type is not XML serializable");
		}

		[TestMethod]
		public void CanSerializeWCFOperationContract()
		{
			Microsoft.Practices.ServiceFactory.Extenders.ServiceContract.Wcf.WCFOperationContract typeInstance = new Microsoft.Practices.ServiceFactory.Extenders.ServiceContract.Wcf.WCFOperationContract();
			string typeRepresentation = GenericSerializer.Serialize<Microsoft.Practices.ServiceFactory.Extenders.ServiceContract.Wcf.WCFOperationContract>(typeInstance);

			Assert.IsTrue(!String.IsNullOrEmpty(typeRepresentation), "Serialization failed");
			Assert.IsFalse(Uri.IsHexDigit(typeRepresentation[0]), "the specified type is not XML serializable");
		}

		[TestMethod]
		public void CanSerializeWCFPrimitiveMessagePart()
		{
			Microsoft.Practices.ServiceFactory.Extenders.ServiceContract.Wcf.WCFPrimitiveMessagePart typeInstance = new Microsoft.Practices.ServiceFactory.Extenders.ServiceContract.Wcf.WCFPrimitiveMessagePart();
			string typeRepresentation = GenericSerializer.Serialize<Microsoft.Practices.ServiceFactory.Extenders.ServiceContract.Wcf.WCFPrimitiveMessagePart>(typeInstance);

			Assert.IsTrue(!String.IsNullOrEmpty(typeRepresentation), "Serialization failed");
			Assert.IsFalse(Uri.IsHexDigit(typeRepresentation[0]), "the specified type is not XML serializable");
		}

		[TestMethod]
		public void CanSerializeWCFService()
		{
			Microsoft.Practices.ServiceFactory.Extenders.ServiceContract.Wcf.WCFService typeInstance = new Microsoft.Practices.ServiceFactory.Extenders.ServiceContract.Wcf.WCFService();
			string typeRepresentation = GenericSerializer.Serialize<Microsoft.Practices.ServiceFactory.Extenders.ServiceContract.Wcf.WCFService>(typeInstance);

			Assert.IsTrue(!String.IsNullOrEmpty(typeRepresentation), "Serialization failed");
			Assert.IsFalse(Uri.IsHexDigit(typeRepresentation[0]), "the specified type is not XML serializable");
		}

		[TestMethod]
		public void CanSerializeWCFServiceContract()
		{
			Microsoft.Practices.ServiceFactory.Extenders.ServiceContract.Wcf.WCFServiceContract typeInstance = new Microsoft.Practices.ServiceFactory.Extenders.ServiceContract.Wcf.WCFServiceContract();
			string typeRepresentation = GenericSerializer.Serialize<Microsoft.Practices.ServiceFactory.Extenders.ServiceContract.Wcf.WCFServiceContract>(typeInstance);

			Assert.IsTrue(!String.IsNullOrEmpty(typeRepresentation), "Serialization failed");
			Assert.IsFalse(Uri.IsHexDigit(typeRepresentation[0]), "the specified type is not XML serializable");
		}

		[TestMethod]
		public void CanSerializeWCFXsdElementFault()
		{
			Microsoft.Practices.ServiceFactory.Extenders.ServiceContract.Wcf.WCFXsdElementFault typeInstance = new Microsoft.Practices.ServiceFactory.Extenders.ServiceContract.Wcf.WCFXsdElementFault();
			string typeRepresentation = GenericSerializer.Serialize<Microsoft.Practices.ServiceFactory.Extenders.ServiceContract.Wcf.WCFXsdElementFault>(typeInstance);

			Assert.IsTrue(!String.IsNullOrEmpty(typeRepresentation), "Serialization failed");
			Assert.IsFalse(Uri.IsHexDigit(typeRepresentation[0]), "the specified type is not XML serializable");
		}

		[TestMethod]
		public void CanSerializeWCFXsdMessageContract()
		{
			Microsoft.Practices.ServiceFactory.Extenders.ServiceContract.Wcf.WCFXsdMessageContract typeInstance = new Microsoft.Practices.ServiceFactory.Extenders.ServiceContract.Wcf.WCFXsdMessageContract();
			string typeRepresentation = GenericSerializer.Serialize<Microsoft.Practices.ServiceFactory.Extenders.ServiceContract.Wcf.WCFXsdMessageContract>(typeInstance);

			Assert.IsTrue(!String.IsNullOrEmpty(typeRepresentation), "Serialization failed");
			Assert.IsFalse(Uri.IsHexDigit(typeRepresentation[0]), "the specified type is not XML serializable");
		}

		#region Other extenders tests

		// NOTE:
		// These tests were added here but they are testing serialization for other extenders projects.
		// They only check that all the included extenders in WSSF are XML serializable
		// In order to work, references to other extenders proyects should be added.

		//[TestMethod]
		//public void CanSerializeAsmxServiceDescription()
		//{
		//    Microsoft.Practices.ServiceFactory.Extenders.HostDesigner.Asmx.AsmxServiceDescription typeInstance = new Microsoft.Practices.ServiceFactory.Extenders.HostDesigner.Asmx.AsmxServiceDescription();
		//    string typeRepresentation = GenericSerializer.Serialize<Microsoft.Practices.ServiceFactory.Extenders.HostDesigner.Asmx.AsmxServiceDescription>(typeInstance);

		//    Assert.IsTrue(!String.IsNullOrEmpty(typeRepresentation), "Serialization failed");
		//    Assert.IsFalse(Uri.IsHexDigit(typeRepresentation[0]), "the specified type is not XML serializable");
		//}

		//[TestMethod]
		//public void CanSerializeAsmxDataContract()
		//{
		//    Microsoft.Practices.ServiceFactory.Extenders.DataContract.Asmx.AsmxDataContract typeInstance = new Microsoft.Practices.ServiceFactory.Extenders.DataContract.Asmx.AsmxDataContract();
		//    string typeRepresentation = GenericSerializer.Serialize<Microsoft.Practices.ServiceFactory.Extenders.DataContract.Asmx.AsmxDataContract>(typeInstance);

		//    Assert.IsTrue(!String.IsNullOrEmpty(typeRepresentation), "Serialization failed");
		//    Assert.IsFalse(Uri.IsHexDigit(typeRepresentation[0]), "the specified type is not XML serializable");
		//}

		//[TestMethod]
		//public void CanSerializeAsmxDataContractCollection()
		//{
		//    Microsoft.Practices.ServiceFactory.Extenders.DataContract.Asmx.AsmxDataContractCollection typeInstance = new Microsoft.Practices.ServiceFactory.Extenders.DataContract.Asmx.AsmxDataContractCollection();
		//    typeInstance.CollectionType = typeof(System.Collections.Hashtable);
		//    string typeRepresentation = GenericSerializer.Serialize<Microsoft.Practices.ServiceFactory.Extenders.DataContract.Asmx.AsmxDataContractCollection>(typeInstance);

		//    Assert.IsTrue(!String.IsNullOrEmpty(typeRepresentation), "Serialization failed");
		//    Assert.IsFalse(Uri.IsHexDigit(typeRepresentation[0]), "the specified type is not XML serializable");
		//}

		//[TestMethod]
		//public void CanDeserializeAsmxDataContractCollection()
		//{
		//    Microsoft.Practices.ServiceFactory.Extenders.DataContract.Asmx.AsmxDataContractCollection typeInstance = new Microsoft.Practices.ServiceFactory.Extenders.DataContract.Asmx.AsmxDataContractCollection();
		//    typeInstance.CollectionType = typeof(System.Collections.Hashtable);
		//    string typeRepresentation = GenericSerializer.Serialize<Microsoft.Practices.ServiceFactory.Extenders.DataContract.Asmx.AsmxDataContractCollection>(typeInstance);

		//    Microsoft.Practices.ServiceFactory.Extenders.DataContract.Asmx.AsmxDataContractCollection newTypeInstance = GenericSerializer.Deserialize<Microsoft.Practices.ServiceFactory.Extenders.DataContract.Asmx.AsmxDataContractCollection>(typeRepresentation);

		//    Assert.AreEqual<Type>(typeInstance.CollectionType, newTypeInstance.CollectionType);
		//}

		//[TestMethod]
		//public void CanSerializeAsmxDataContractEnum()
		//{
		//    Microsoft.Practices.ServiceFactory.Extenders.DataContract.Asmx.AsmxDataContractEnum typeInstance = new Microsoft.Practices.ServiceFactory.Extenders.DataContract.Asmx.AsmxDataContractEnum();
		//    string typeRepresentation = GenericSerializer.Serialize<Microsoft.Practices.ServiceFactory.Extenders.DataContract.Asmx.AsmxDataContractEnum>(typeInstance);

		//    Assert.IsTrue(!String.IsNullOrEmpty(typeRepresentation), "Serialization failed");
		//    Assert.IsFalse(Uri.IsHexDigit(typeRepresentation[0]), "the specified type is not XML serializable");
		//}

		//[TestMethod]
		//public void CanSerializeAsmxDataElement()
		//{
		//    Microsoft.Practices.ServiceFactory.Extenders.DataContract.Asmx.AsmxDataElement typeInstance = new Microsoft.Practices.ServiceFactory.Extenders.DataContract.Asmx.AsmxDataElement();
		//    string typeRepresentation = GenericSerializer.Serialize<Microsoft.Practices.ServiceFactory.Extenders.DataContract.Asmx.AsmxDataElement>(typeInstance);

		//    Assert.IsTrue(!String.IsNullOrEmpty(typeRepresentation), "Serialization failed");
		//    Assert.IsFalse(Uri.IsHexDigit(typeRepresentation[0]), "the specified type is not XML serializable");
		//}

		//[TestMethod]
		//public void CanSerializeAsmxFaultContract()
		//{
		//    Microsoft.Practices.ServiceFactory.Extenders.DataContract.Asmx.AsmxFaultContract typeInstance = new Microsoft.Practices.ServiceFactory.Extenders.DataContract.Asmx.AsmxFaultContract();
		//    string typeRepresentation = GenericSerializer.Serialize<Microsoft.Practices.ServiceFactory.Extenders.DataContract.Asmx.AsmxFaultContract>(typeInstance);

		//    Assert.IsTrue(!String.IsNullOrEmpty(typeRepresentation), "Serialization failed");
		//    Assert.IsFalse(Uri.IsHexDigit(typeRepresentation[0]), "the specified type is not XML serializable");
		//}

		//[TestMethod]
		//public void CanSerializeAsmxPrimitiveDataTypeCollection()
		//{
		//    Microsoft.Practices.ServiceFactory.Extenders.DataContract.Asmx.AsmxPrimitiveDataTypeCollection typeInstance = new Microsoft.Practices.ServiceFactory.Extenders.DataContract.Asmx.AsmxPrimitiveDataTypeCollection();
		//    string typeRepresentation = GenericSerializer.Serialize<Microsoft.Practices.ServiceFactory.Extenders.DataContract.Asmx.AsmxPrimitiveDataTypeCollection>(typeInstance);

		//    Assert.IsTrue(!String.IsNullOrEmpty(typeRepresentation), "Serialization failed");
		//    Assert.IsFalse(Uri.IsHexDigit(typeRepresentation[0]), "the specified type is not XML serializable");
		//}

		//[TestMethod]
		//public void CanSerializeDataContractAsmxExtensionProvider()
		//{
		//    Microsoft.Practices.ServiceFactory.Extenders.DataContract.Asmx.DataContractAsmxExtensionProvider typeInstance = new Microsoft.Practices.ServiceFactory.Extenders.DataContract.Asmx.DataContractAsmxExtensionProvider();
		//    string typeRepresentation = GenericSerializer.Serialize<Microsoft.Practices.ServiceFactory.Extenders.DataContract.Asmx.DataContractAsmxExtensionProvider>(typeInstance);

		//    Assert.IsTrue(!String.IsNullOrEmpty(typeRepresentation), "Serialization failed");
		//    Assert.IsFalse(Uri.IsHexDigit(typeRepresentation[0]), "the specified type is not XML serializable");
		//}

		//[TestMethod]
		//public void CanSerializeWCFDataContract()
		//{
		//    Microsoft.Practices.ServiceFactory.Extenders.DataContract.Wcf.WCFDataContract typeInstance = new Microsoft.Practices.ServiceFactory.Extenders.DataContract.Wcf.WCFDataContract();
		//    string typeRepresentation = GenericSerializer.Serialize<Microsoft.Practices.ServiceFactory.Extenders.DataContract.Wcf.WCFDataContract>(typeInstance);

		//    Assert.IsTrue(!String.IsNullOrEmpty(typeRepresentation), "Serialization failed");
		//    Assert.IsFalse(Uri.IsHexDigit(typeRepresentation[0]), "the specified type is not XML serializable");
		//}

		//[TestMethod]
		//public void CanSerializeWCFDataContractCollection()
		//{
		//    Microsoft.Practices.ServiceFactory.Extenders.DataContract.Wcf.WCFDataContractCollection typeInstance = new Microsoft.Practices.ServiceFactory.Extenders.DataContract.Wcf.WCFDataContractCollection();
		//    typeInstance.CollectionType = typeof(System.Collections.Hashtable);
		//    string typeRepresentation = GenericSerializer.Serialize<Microsoft.Practices.ServiceFactory.Extenders.DataContract.Wcf.WCFDataContractCollection>(typeInstance);

		//    Assert.IsTrue(!String.IsNullOrEmpty(typeRepresentation), "Serialization failed");
		//    Assert.IsFalse(Uri.IsHexDigit(typeRepresentation[0]), "the specified type is not XML serializable");
		//}

		//[TestMethod]
		//public void CanDeserializeWCFDataContractCollection()
		//{
		//    Microsoft.Practices.ServiceFactory.Extenders.DataContract.Wcf.WCFDataContractCollection typeInstance = new Microsoft.Practices.ServiceFactory.Extenders.DataContract.Wcf.WCFDataContractCollection();
		//    typeInstance.CollectionType = typeof(System.Collections.Hashtable);
		//    string typeRepresentation = GenericSerializer.Serialize<Microsoft.Practices.ServiceFactory.Extenders.DataContract.Wcf.WCFDataContractCollection>(typeInstance);

		//    Microsoft.Practices.ServiceFactory.Extenders.DataContract.Wcf.WCFDataContractCollection newTypeInstance = GenericSerializer.Deserialize<Microsoft.Practices.ServiceFactory.Extenders.DataContract.Wcf.WCFDataContractCollection>(typeRepresentation);

		//    Assert.AreEqual<Type>(typeInstance.CollectionType, newTypeInstance.CollectionType);
		//}

		//[TestMethod]
		//public void CanSerializeWCFDataContractEnum()
		//{
		//    Microsoft.Practices.ServiceFactory.Extenders.DataContract.Wcf.WCFDataContractEnum typeInstance = new Microsoft.Practices.ServiceFactory.Extenders.DataContract.Wcf.WCFDataContractEnum();
		//    string typeRepresentation = GenericSerializer.Serialize<Microsoft.Practices.ServiceFactory.Extenders.DataContract.Wcf.WCFDataContractEnum>(typeInstance);

		//    Assert.IsTrue(!String.IsNullOrEmpty(typeRepresentation), "Serialization failed");
		//    Assert.IsFalse(Uri.IsHexDigit(typeRepresentation[0]), "the specified type is not XML serializable");
		//}

		//[TestMethod]
		//public void CanSerializeWCFDataElement()
		//{
		//    Microsoft.Practices.ServiceFactory.Extenders.DataContract.Wcf.WCFDataElement typeInstance = new Microsoft.Practices.ServiceFactory.Extenders.DataContract.Wcf.WCFDataElement();
		//    string typeRepresentation = GenericSerializer.Serialize<Microsoft.Practices.ServiceFactory.Extenders.DataContract.Wcf.WCFDataElement>(typeInstance);

		//    Assert.IsTrue(!String.IsNullOrEmpty(typeRepresentation), "Serialization failed");
		//    Assert.IsFalse(Uri.IsHexDigit(typeRepresentation[0]), "the specified type is not XML serializable");
		//}

		//[TestMethod]
		//public void CanSerializeWCFFaultContract()
		//{
		//    Microsoft.Practices.ServiceFactory.Extenders.DataContract.Wcf.WCFFaultContract typeInstance = new Microsoft.Practices.ServiceFactory.Extenders.DataContract.Wcf.WCFFaultContract();
		//    string typeRepresentation = GenericSerializer.Serialize<Microsoft.Practices.ServiceFactory.Extenders.DataContract.Wcf.WCFFaultContract>(typeInstance);

		//    Assert.IsTrue(!String.IsNullOrEmpty(typeRepresentation), "Serialization failed");
		//    Assert.IsFalse(Uri.IsHexDigit(typeRepresentation[0]), "the specified type is not XML serializable");
		//}

		//[TestMethod]
		//public void CanSerializeAsmxMessageContract()
		//{
		//    Microsoft.Practices.ServiceFactory.Extenders.ServiceContract.Asmx.AsmxMessageContract typeInstance = new Microsoft.Practices.ServiceFactory.Extenders.ServiceContract.Asmx.AsmxMessageContract();
		//    string typeRepresentation = GenericSerializer.Serialize<Microsoft.Practices.ServiceFactory.Extenders.ServiceContract.Asmx.AsmxMessageContract>(typeInstance);

		//    Assert.IsTrue(!String.IsNullOrEmpty(typeRepresentation), "Serialization failed");
		//    Assert.IsFalse(Uri.IsHexDigit(typeRepresentation[0]), "the specified type is not XML serializable");
		//}

		//[TestMethod]
		//public void CanSerializeAsmxOperationContract()
		//{
		//    Microsoft.Practices.ServiceFactory.Extenders.ServiceContract.Asmx.AsmxOperationContract typeInstance = new Microsoft.Practices.ServiceFactory.Extenders.ServiceContract.Asmx.AsmxOperationContract();
		//    string typeRepresentation = GenericSerializer.Serialize<Microsoft.Practices.ServiceFactory.Extenders.ServiceContract.Asmx.AsmxOperationContract>(typeInstance);

		//    Assert.IsTrue(!String.IsNullOrEmpty(typeRepresentation), "Serialization failed");
		//    Assert.IsFalse(Uri.IsHexDigit(typeRepresentation[0]), "the specified type is not XML serializable");
		//}

		//[TestMethod]
		//public void CanSerializeAsmxService()
		//{
		//    Microsoft.Practices.ServiceFactory.Extenders.ServiceContract.Asmx.AsmxService typeInstance = new Microsoft.Practices.ServiceFactory.Extenders.ServiceContract.Asmx.AsmxService();
		//    string typeRepresentation = GenericSerializer.Serialize<Microsoft.Practices.ServiceFactory.Extenders.ServiceContract.Asmx.AsmxService>(typeInstance);

		//    Assert.IsTrue(!String.IsNullOrEmpty(typeRepresentation), "Serialization failed");
		//    Assert.IsFalse(Uri.IsHexDigit(typeRepresentation[0]), "the specified type is not XML serializable");
		//}

		//[TestMethod]
		//public void CanSerializeAsmxServiceContract()
		//{
		//    Microsoft.Practices.ServiceFactory.Extenders.ServiceContract.Asmx.AsmxServiceContract typeInstance = new Microsoft.Practices.ServiceFactory.Extenders.ServiceContract.Asmx.AsmxServiceContract();
		//    string typeRepresentation = GenericSerializer.Serialize<Microsoft.Practices.ServiceFactory.Extenders.ServiceContract.Asmx.AsmxServiceContract>(typeInstance);

		//    Assert.IsTrue(!String.IsNullOrEmpty(typeRepresentation), "Serialization failed");
		//    Assert.IsFalse(Uri.IsHexDigit(typeRepresentation[0]), "the specified type is not XML serializable");
		//}

		//[TestMethod]
		//public void CanSerializeAsmxXsdMessageContract()
		//{
		//    Microsoft.Practices.ServiceFactory.Extenders.ServiceContract.Asmx.AsmxXsdMessageContract typeInstance = new Microsoft.Practices.ServiceFactory.Extenders.ServiceContract.Asmx.AsmxXsdMessageContract();
		//    string typeRepresentation = GenericSerializer.Serialize<Microsoft.Practices.ServiceFactory.Extenders.ServiceContract.Asmx.AsmxXsdMessageContract>(typeInstance);

		//    Assert.IsTrue(!String.IsNullOrEmpty(typeRepresentation), "Serialization failed");
		//    Assert.IsFalse(Uri.IsHexDigit(typeRepresentation[0]), "the specified type is not XML serializable");
		//}

		//[TestMethod]
		//public void CanSerializeServiceContractAsmxExtensionProvider()
		//{
		//    Microsoft.Practices.ServiceFactory.Extenders.ServiceContract.Asmx.ServiceContractAsmxExtensionProvider typeInstance = new Microsoft.Practices.ServiceFactory.Extenders.ServiceContract.Asmx.ServiceContractAsmxExtensionProvider();
		//    string typeRepresentation = GenericSerializer.Serialize<Microsoft.Practices.ServiceFactory.Extenders.ServiceContract.Asmx.ServiceContractAsmxExtensionProvider>(typeInstance);

		//    Assert.IsTrue(!String.IsNullOrEmpty(typeRepresentation), "Serialization failed");
		//    Assert.IsFalse(Uri.IsHexDigit(typeRepresentation[0]), "the specified type is not XML serializable");
		//}

		#endregion
	}
}
