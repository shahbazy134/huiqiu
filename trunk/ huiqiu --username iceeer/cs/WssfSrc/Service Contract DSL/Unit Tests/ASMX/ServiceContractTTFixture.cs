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
using System.IO;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Practices.ServiceFactory.ServiceContracts;
using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.TextTemplating;
using Microsoft.VisualStudio.Modeling.Diagrams;
using Microsoft.Practices.ServiceFactory.DataContracts;
using Microsoft.Practices.ServiceFactory.RecipeFramework.Extensions.ProjectMapping;
using Microsoft.Practices.UnitTestLibrary;
using Microsoft.Practices.UnitTestLibrary.Utilities;
using System.CodeDom.Compiler;
using System.Reflection;
using Microsoft.Practices.RecipeFramework.Library;
using Microsoft.Practices.ServiceFactory.Extenders.ServiceContract.Asmx.CodeGeneration;
using Microsoft.Practices.ServiceFactory.Extenders.ServiceContract.Asmx;
using Microsoft.Practices.Modeling.CodeGeneration.Strategies;
using System.Web.Services.Protocols;

namespace ServiceContractDsl.Tests.ASMX
{
	[TestClass]
	public class ServiceContractTTFixture : ServiceContractModelFixture
	{
		const string ServiceContractElementName = "Name1";
		const string ServiceContractElementNamespace = "http://mynamespace";

		[TestMethod]
		[DeploymentItem("Microsoft.Practices.ServiceFactory.ServiceContracts.Dsl.dll")]
		[DeploymentItem("Microsoft.VisualStudio.Modeling.Sdk.dll")]
		[DeploymentItem("Microsoft.Practices.RecipeFramework.Library.dll")]
		[DeploymentItem("Microsoft.Practices.RecipeFramework.Extensions.dll")]
		[DeploymentItem("Microsoft.Practices.Common.dll")]
		[DeploymentItem("Microsoft.Practices.ServiceFactory.Description.dll")]
		[DeploymentItem("Microsoft.Practices.ComponentModel.dll")]
		[DeploymentItem("Microsoft.Practices.EnterpriseLibrary.Common.dll")]
		[DeploymentItem("Microsoft.Practices.EnterpriseLibrary.Validation.dll")]
		[DeploymentItem(@"TextTemplates\ASMX\CS\CommonTextTransformation.tt", @"TextTemplates\ASMX\CS")]
		[DeploymentItem(@"TextTemplates\ASMX\CS\ServiceContract.tt", @"TextTemplates\ASMX\CS")]
		[DeploymentItem("ProjectMapping.ServiceContractDsl.Tests.xml")]
		public void TestHeaderGeneration()
		{
			string content;

			ProjectMappingManagerSetup.InitializeManager(ServiceProvider, "ProjectMapping.ServiceContractDsl.Tests.xml");

			ServiceContract rootElement = CreateRoot(ServiceContractElementName, ServiceContractElementNamespace);
			content = RunTemplate(rootElement);
			Assert.IsTrue(content.Contains("public partial interface IName1"));

			Type generatedType = CompileAndGetType(content);
			Assert.AreEqual<string>("I" + ServiceContractElementName, generatedType.Name);
		}

		[TestMethod]
		[DeploymentItem("Microsoft.Practices.ServiceFactory.ServiceContracts.Dsl.dll")]
		[DeploymentItem("Microsoft.VisualStudio.Modeling.Sdk.dll")]
		[DeploymentItem("Microsoft.Practices.RecipeFramework.Library.dll")]
		[DeploymentItem("Microsoft.Practices.RecipeFramework.Extensions.dll")]
		[DeploymentItem("Microsoft.Practices.Common.dll")]
		[DeploymentItem("Microsoft.Practices.ComponentModel.dll")]
		[DeploymentItem("Microsoft.Practices.EnterpriseLibrary.Common.dll")]
		[DeploymentItem("Microsoft.Practices.EnterpriseLibrary.Validation.dll")]
		[DeploymentItem(@"TextTemplates\ASMX\CS\CommonTextTransformation.tt", @"TextTemplates\ASMX\CS")]
		[DeploymentItem(@"TextTemplates\ASMX\CS\ServiceContract.tt", @"TextTemplates\ASMX\CS")]
		[DeploymentItem("ProjectMapping.ServiceContractDsl.Tests.xml")]
		public void TestXsdMessageUnwrappedRequestGeneration()
		{
			ProjectMappingManagerSetup.InitializeManager(ServiceProvider, "ProjectMapping.ServiceContractDsl.Tests.xml");

			ServiceContract rootElement = CreateRoot(ServiceContractElementName, ServiceContractElementNamespace);
			rootElement.ServiceContractModel.ImplementationTechnology = new ServiceContractAsmxExtensionProvider();
			rootElement.ServiceContractModel.SerializerType = SerializerType.XmlSerializer;

			Operation op1 = new Operation(Store);
			op1.ObjectExtender = new AsmxOperationContract();
			op1.Name = "op1";
			op1.Action = "op1";
			op1.ServiceContract = rootElement;
			XsdMessage request = new XsdMessage(Store);
			request.Name = "Request1";
			request.Element = @"xsd://schemas\file.xsd?MyType";
			request.IsWrapped = false;
			request.ServiceContractModel = rootElement.ServiceContractModel;
			AsmxXsdMessageContract wcfXsdMc = new AsmxXsdMessageContract();
			wcfXsdMc.ModelElement = request;
			request.ObjectExtender = wcfXsdMc;

			op1.Request = request;
			string content = RunTemplate(rootElement);

			EnsureType(ref content, "MyType");
			Type generatedType = CompileAndGetType(content);
			MethodInfo method = TypeAsserter.AssertMethod(op1.Name, generatedType);
			Assert.AreEqual<int>(1, method.GetParameters().Length);
			Assert.AreEqual<string>("MyType", ((ParameterInfo)method.GetParameters().GetValue(0)).ParameterType.Name);
			Assert.AreEqual<string>("Void", method.ReturnType.Name);
		}

		[TestMethod]
		[DeploymentItem("Microsoft.Practices.ServiceFactory.ServiceContracts.Dsl.dll")]
		[DeploymentItem("Microsoft.VisualStudio.Modeling.Sdk.dll")]
		[DeploymentItem("Microsoft.Practices.RecipeFramework.Library.dll")]
		[DeploymentItem("Microsoft.Practices.RecipeFramework.Extensions.dll")]
		[DeploymentItem("Microsoft.Practices.Common.dll")]
		[DeploymentItem("Microsoft.Practices.ComponentModel.dll")]
		[DeploymentItem("Microsoft.Practices.EnterpriseLibrary.Common.dll")]
		[DeploymentItem("Microsoft.Practices.EnterpriseLibrary.Validation.dll")]
		[DeploymentItem(@"TextTemplates\ASMX\CS\CommonTextTransformation.tt", @"TextTemplates\ASMX\CS")]
		[DeploymentItem(@"TextTemplates\ASMX\CS\ServiceContract.tt", @"TextTemplates\ASMX\CS")]
		[DeploymentItem("ProjectMapping.ServiceContractDsl.Tests.xml")]
		public void TestXsdMessageWrappedRequestGeneration()
		{
			ProjectMappingManagerSetup.InitializeManager(ServiceProvider, "ProjectMapping.ServiceContractDsl.Tests.xml");

			ServiceContract rootElement = CreateRoot(ServiceContractElementName, ServiceContractElementNamespace);
			rootElement.ServiceContractModel.ImplementationTechnology = new ServiceContractAsmxExtensionProvider();
			rootElement.ServiceContractModel.SerializerType = SerializerType.XmlSerializer;

			Operation op1 = new Operation(Store);
			op1.ObjectExtender = new AsmxOperationContract();
			op1.Name = "op1";
			op1.Action = "op1";
			op1.ServiceContract = rootElement;
			XsdMessage request = new XsdMessage(Store);
			request.Name = "Request1";
			request.Element = @"xsd://schemas\file.xsd?MyType";
			request.IsWrapped = true;
			request.ServiceContractModel = rootElement.ServiceContractModel;
			AsmxXsdMessageContract wcfXsdMc = new AsmxXsdMessageContract();
			wcfXsdMc.ModelElement = request;
			request.ObjectExtender = wcfXsdMc;

			op1.Request = request;
			string content = RunTemplate(rootElement);

			EnsureType(ref content, "Request1");
			Type generatedType = CompileAndGetType(content);
			MethodInfo method = TypeAsserter.AssertMethod(op1.Name, generatedType);
			Assert.AreEqual<int>(1, method.GetParameters().Length);
			Assert.AreEqual<string>("Request1", ((ParameterInfo)method.GetParameters().GetValue(0)).ParameterType.Name);
			Assert.AreEqual<string>("Void", method.ReturnType.Name);
		}

		[TestMethod]
		[DeploymentItem("Microsoft.Practices.ServiceFactory.ServiceContracts.Dsl.dll")]
		[DeploymentItem("Microsoft.VisualStudio.Modeling.Sdk.dll")]
		[DeploymentItem("Microsoft.Practices.RecipeFramework.Library.dll")]
		[DeploymentItem("Microsoft.Practices.RecipeFramework.Extensions.dll")]
		[DeploymentItem("Microsoft.Practices.Common.dll")]
		[DeploymentItem("Microsoft.Practices.ComponentModel.dll")]
		[DeploymentItem("Microsoft.Practices.EnterpriseLibrary.Common.dll")]
		[DeploymentItem("Microsoft.Practices.EnterpriseLibrary.Validation.dll")]
		[DeploymentItem(@"TextTemplates\ASMX\CS\CommonTextTransformation.tt", @"TextTemplates\ASMX\CS")]
		[DeploymentItem(@"TextTemplates\ASMX\CS\ServiceContract.tt", @"TextTemplates\ASMX\CS")]
		[DeploymentItem("ProjectMapping.ServiceContractDsl.Tests.xml")]
		public void TestSoapDocumentMethodAttributeParanmeters()
		{
			ProjectMappingManagerSetup.InitializeManager(ServiceProvider, "ProjectMapping.ServiceContractDsl.Tests.xml");

			ServiceContract rootElement = CreateRoot(ServiceContractElementName, ServiceContractElementNamespace);
			rootElement.ServiceContractModel.ImplementationTechnology = new ServiceContractAsmxExtensionProvider();
			rootElement.ServiceContractModel.SerializerType = SerializerType.XmlSerializer;

			Operation op1 = new Operation(Store);
			op1.ObjectExtender = new AsmxOperationContract();
			op1.Name = "op1";
			op1.Action = "op1";
			op1.ServiceContract = rootElement;
			string content = RunTemplate(rootElement);

			EnsureType(ref content, "MyType");
			Type generatedType = CompileAndGetType(content);
			MethodInfo method = TypeAsserter.AssertMethod(op1.Name, generatedType);
			SoapDocumentMethodAttribute soapAttr = TypeAsserter.AssertAttribute<SoapDocumentMethodAttribute>(method);
			Assert.AreEqual(soapAttr.ParameterStyle, SoapParameterStyle.Wrapped);
			Assert.AreEqual<string>(soapAttr.Action, rootElement.Namespace + "/" + op1.Action);
		}

		#region Private Implementation

		private Type CompileAndGetType(string content)
		{
			EnsureNamespace(ref content);
			string typeName = DefaultNamespace + ".I" + ServiceContractElementName;
			CompilerResults results = DynamicCompilation.CompileAssemblyFromSource(
				content,
				new string[] { "System.dll",
					"System.EnterpriseServices.dll",
					"System.Xml.dll", 
					"System.Web.Services.dll", 
					"Microsoft.Practices.ServiceFactory.ServiceContracts.Dsl.dll",
					"Microsoft.Practices.ServiceFactory.Description.dll",
					"Microsoft.VisualStudio.Modeling.Sdk.dll" });
			Type generatedType = results.CompiledAssembly.GetType(typeName, false);
			Assert.IsNotNull(generatedType, "Invalid type: " + typeName);
			return generatedType;
		}

		private ServiceContract CreateRoot(string name, string ns)
		{
			Guard.ArgumentNotNull(name,"name");
			Guard.ArgumentNotNull(ns, "ns");

			ServiceContract rootElement = new ServiceContract(Store);
			rootElement.ServiceContractModel = new ServiceContractModel(Store);
			rootElement.Name = name;
			rootElement.Namespace = ns;
			rootElement.ServiceContractModel.ProjectMappingTable = "ASMX";
			dataContractStore = new Store(ServiceProvider, typeof(CoreDesignSurfaceDomainModel), typeof(DataContractDslDomainModel));
			ResolveModelElement("foo");
			return rootElement;
		}

		private Store dataContractStore = null;
		private Store DataContractStore
		{
			get
			{
				if (dataContractStore == null)
				{
					dataContractStore = new Store(ServiceProvider, typeof(CoreDesignSurfaceDomainModel), typeof(DataContractDslDomainModel));
				}
				return dataContractStore;
			}
		}
		
		protected override string Template
		{
			get
			{
				ASMXServiceContractLink link = new ASMXServiceContractLink();
				return GetWrappedLink(link).GetTemplate(TextTemplateTargetLanguage.CSharp);
			}
		}

		protected override Type ContractType
		{
			get { return typeof(AsmxServiceContract); }
		}

		#endregion
	}
}
