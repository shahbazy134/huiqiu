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
using Microsoft.Practices.ServiceFactory.RecipeFramework.Extensions.ProjectMapping;
using Microsoft.Practices.UnitTestLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Practices.ServiceFactory.DataContracts;
using Microsoft.Practices.Modeling.CodeGeneration.TextTemplating;
using Microsoft.Practices.UnitTestLibrary.Utilities;
using System.CodeDom.Compiler;
using System.Runtime.Serialization;
using System.Reflection;

namespace DataContractDsl.Tests
{
	/// <summary>
	/// Summary description for DataContractTTFixture
	/// </summary>
	[TestClass]
	public class DataContractTTFixture : DataContractModelFixture
	{
		const string DataContractElementName = "MyDataContractElement";
		const string DataContractElementNamespace = "http://mynamespace";
		const string PrimitiveDataElementName1 = "Element1";
		const string PrimitiveDataElementType1 = "System.Int32";
		const string PrimitiveDataElementName2 = "Element2";
		const string PrimitiveDataElementType2 = "System.String";
		const string DataMemberAttribute = "[WcfSerialization::DataMember(";
		const string DataContractLinkedElementName = "MyLinkedDataContractElement";
		const string DataContractLinkedElementType = "DataContractLinkedElementType";

		[TestMethod]
		[DeploymentItem("System.ServiceModel.dll")]
		[DeploymentItem("System.Xml.dll")]
		[DeploymentItem("System.Runtime.Serialization.dll")]
		[DeploymentItem("Microsoft.Practices.ServiceFactory.DataContracts.Dsl.dll")]
		[DeploymentItem("Microsoft.Practices.RecipeFramework.Library.dll")]
		[DeploymentItem("Microsoft.Practices.RecipeFramework.Extensions.dll")]
		[DeploymentItem("Microsoft.Practices.Common.dll")]
		[DeploymentItem("Microsoft.Practices.ComponentModel.dll")]
		[DeploymentItem("Microsoft.VisualStudio.Modeling.Sdk.dll")]
		[DeploymentItem("ProjectMapping.DataContractDsl.Tests.xml")]
		[DeploymentItem("Microsoft.Practices.Modeling.Dsl.Integration.dll")]
		[DeploymentItem("Microsoft.Practices.EnterpriseLibrary.Common.dll")]
		[DeploymentItem("Microsoft.Practices.EnterpriseLibrary.Validation.dll")]
		public void TestHeaderGeneration()
		{
			ProjectMappingManagerSetup.InitializeManager(ServiceProvider, "ProjectMapping.DataContractDsl.Tests.xml");

			DataContractElement rootElement = CreateDefaultDataContractElement(); 
			string content = RunTemplate(rootElement);

			Assert.IsTrue(content.Contains("public partial class " + DataContractElementName));
			Assert.IsTrue(content.Contains("Namespace = \"" + DataContractElementNamespace + "\""));
			Assert.IsFalse(content.Contains(DataMemberAttribute));

			Type generatedType = CompileAndGetType(content);
			DataContractAttribute dataContract = TypeAsserter.AssertAttribute<DataContractAttribute>(generatedType);
			Assert.AreEqual<string>(DataContractElementNamespace, dataContract.Namespace);
		}

		[TestMethod]
		public void TestMembersWithPrimitiveTypesGeneration()
		{
			ProjectMappingManagerSetup.InitializeManager(ServiceProvider, "ProjectMapping.DataContractDsl.Tests.xml");

			DataContractElement rootElement = CreateDefaultDataContractElement();
			rootElement.DataElements.AddRange(LoadPrimitiveDataElements(false, true));
			string content = RunTemplate(rootElement);

			Assert.IsTrue(content.Contains(DataMemberAttribute));
			Assert.IsTrue(content.Contains("private " + Utility.GetCSharpTypeOutput(PrimitiveDataElementType1) + " " + Utility.ToCamelCase(PrimitiveDataElementName1)));
			Assert.IsTrue(content.Contains("private " + Utility.GetCSharpTypeOutput(PrimitiveDataElementType2) + " " + Utility.ToCamelCase(PrimitiveDataElementName2)));

			Type generatedType = CompileAndGetType(content);
			TypeAsserter.AssertField(PrimitiveDataElementType1, PrimitiveDataElementName1, generatedType);
			TypeAsserter.AssertField(PrimitiveDataElementType2, PrimitiveDataElementName2, generatedType);
		}

		[TestMethod]
		public void TestMembersWithPrimitiveArrayTypesGeneration()
		{
			ProjectMappingManagerSetup.InitializeManager(ServiceProvider, "ProjectMapping.DataContractDsl.Tests.xml");
			
			DataContractElement rootElement = CreateDefaultDataContractElement();
			rootElement.DataElements.AddRange(LoadPrimitiveDataElements(true, true));
			string content = RunTemplate(rootElement);

			Assert.IsTrue(content.Contains("private IList<" + Utility.GetCSharpTypeOutput(PrimitiveDataElementType1) + "> " + Utility.ToCamelCase(PrimitiveDataElementName1)));
			Assert.IsTrue(content.Contains("private IList<" + Utility.GetCSharpTypeOutput(PrimitiveDataElementType2) + "> " + Utility.ToCamelCase(PrimitiveDataElementName2)));

			Type generatedType = CompileAndGetType(content);
			TypeAsserter.AssertField(typeof(IList<int>), PrimitiveDataElementName1, generatedType);
			TypeAsserter.AssertField(typeof(IList<string>), PrimitiveDataElementName2, generatedType);
		}

		[TestMethod]
		[DeploymentItem("ProjectMapping.DataContractDsl.Tests.xml")]
		public void TestMembersWithPrimitiveNoDataMembersGeneration()
		{
			ProjectMappingManagerSetup.InitializeManager(ServiceProvider, "ProjectMapping.DataContractDsl.Tests.xml");

			DataContractElement rootElement = CreateDefaultDataContractElement();
			rootElement.DataElements.AddRange(LoadPrimitiveDataElements(false, false));
			string content = RunTemplate(rootElement);

			Assert.IsTrue(content.Contains("private " + Utility.GetCSharpTypeOutput(PrimitiveDataElementType1) + " " + Utility.ToCamelCase(PrimitiveDataElementName1)));
			Assert.IsTrue(content.Contains("private " + Utility.GetCSharpTypeOutput(PrimitiveDataElementType2) + " " + Utility.ToCamelCase(PrimitiveDataElementName2)));

			Type generatedType = CompileAndGetType(content);
			TypeAsserter.AssertField(PrimitiveDataElementType1, PrimitiveDataElementName1, generatedType);
			TypeAsserter.AssertField(PrimitiveDataElementType2, PrimitiveDataElementName2, generatedType);
		}

		[TestMethod]
		[DeploymentItem("ProjectMapping.DataContractDsl.Tests.xml")]
		public void TestLinkedDataContractElementsWithMultiplicityOneGeneration()
		{
			ProjectMappingManagerSetup.InitializeManager(ServiceProvider, "ProjectMapping.DataContractDsl.Tests.xml");

			DataContractElement rootElement = CreateDefaultDataContractElement();
			rootElement.DataElements.AddRange(LoadDataContractDataElements(rootElement, Multiplicity.Single));
			string content = RunTemplate(rootElement);

			Assert.IsTrue(content.Contains(DataMemberAttribute));
			Assert.IsTrue(content.Contains("public " + DataContractLinkedElementType + " " + DataContractLinkedElementName));

			EnsureType(ref content, DataContractLinkedElementType);
			Type generatedType = CompileAndGetType(content);
			TypeAsserter.AssertProperty(generatedType.Assembly.GetType(DataContractLinkedElementType), DataContractLinkedElementName, generatedType);
		}

		[TestMethod]
		[DeploymentItem("ProjectMapping.DataContractDsl.Tests.xml")]
		public void TestLinkedDataContractElementsWithMultiplicityOneManyGeneration()
		{
			ProjectMappingManagerSetup.InitializeManager(ServiceProvider, "ProjectMapping.DataContractDsl.Tests.xml");

			DataContractElement rootElement = CreateDefaultDataContractElement();
			rootElement.DataElements.AddRange(LoadDataContractDataElements(rootElement, Multiplicity.Multiple));
			string content = RunTemplate(rootElement);

			Assert.IsTrue(content.Contains(DataMemberAttribute));
			Assert.IsTrue(content.Contains("public IList<" + DataContractLinkedElementType + "> " + DataContractLinkedElementName));

			EnsureType(ref content, DataContractLinkedElementType);
			Type generatedType = CompileAndGetType(content);
			TypeAsserter.AssertProperty(
				typeof(IList<>).GetGenericTypeDefinition().MakeGenericType(generatedType.Assembly.GetType(DataContractLinkedElementType)), 
				DataContractLinkedElementName, generatedType);
		}

		[TestMethod]
		[DeploymentItem("ProjectMapping.DataContractDsl.Tests.xml")]
		public void TestWCFDataElementPropertiesGeneration()
		{
			ProjectMappingManagerSetup.InitializeManager(ServiceProvider, "ProjectMapping.DataContractDsl.Tests.xml");

			DataContractElement rootElement = CreateDefaultDataContractElement();
			WCFDataElement dataElement = new WCFDataElement();
			dataElement.IsRequired = true;
			dataElement.Order = 1;
			dataElement.ModelElement = rootElement;
			PrimitiveDataElement element = LoadDefaultPrimitiveDataElement();
			element.ObjectExtender = dataElement;
			rootElement.DataElements.Add(element);
			string content = RunTemplate(rootElement);

			Assert.IsTrue(content.Contains("IsRequired = true"));
			Assert.IsTrue(content.Contains("Order = 1"));

			Type generatedType = CompileAndGetType(content);
			DataMemberAttribute dataMember = TypeAsserter.AssertAttribute<DataMemberAttribute>(generatedType.GetProperty(element.Name));
			Assert.IsTrue(dataMember.IsRequired);
			Assert.AreEqual<int>(1, dataMember.Order);
		}

		[TestMethod]
		public void TestMembersWithPrimitiveNullableTypeGeneration()
		{
			ProjectMappingManagerSetup.InitializeManager(ServiceProvider, "ProjectMapping.DataContractDsl.Tests.xml");

			DataContractElement rootElement = CreateDefaultDataContractElement();
			rootElement.DataElements.Add(LoadPrimitiveDataElement(PrimitiveDataElementName1, PrimitiveDataElementType1, false, true, true));
			string content = RunTemplate(rootElement);

			Assert.IsTrue(content.Contains("private System.Nullable<int>"));
		}

		protected override string Template
		{
			get
			{
				DataContractElementLink link = new DataContractElementLink();
				return link.Template;
			}
		}

		protected override Type ContractType
		{
			get { return typeof(WCFDataContract); }
		}

		#region Private methods

		private Type CompileAndGetType(string content)
		{
			EnsureNamespace(ref content);
			string typeName = DefaultNamespace + "." + DataContractElementName;
			CompilerResults results = DynamicCompilation.CompileAssemblyFromSource(
				content,
				new string[] { "System.Xml.dll", 
							   "System.Runtime.Serialization.dll", 
							   "Microsoft.Practices.ServiceFactory.DataContracts.Dsl.dll",
							   "Microsoft.VisualStudio.Modeling.Sdk.dll" });
			Type generatedType = results.CompiledAssembly.GetType(typeName, false);
			Assert.IsNotNull(generatedType, "Invalid type: " + typeName);
			return generatedType;
		}

		private DataContractElement CreateDefaultDataContractElement()
		{
			DataContractElement rootElement = new DataContractElement(Store);
			rootElement.DataContractModel = new DataContractModel(Store);
			rootElement.DataContractModel.ProjectMappingTable = "WCF";
			rootElement.Name = DataContractElementName;
			rootElement.Namespace = DataContractElementNamespace;
			return rootElement;
		}

		private List<DataElement> LoadPrimitiveDataElements(bool isArray, bool isDataMember)
		{
			List<DataElement> dataElements = new List<DataElement>();
			dataElements.Add(
				LoadPrimitiveDataElement(PrimitiveDataElementName1, PrimitiveDataElementType1, isArray, isDataMember, false));

			dataElements.Add(
				LoadPrimitiveDataElement(PrimitiveDataElementName2, PrimitiveDataElementType2, isArray, isDataMember, false));

			return dataElements;
		}

		private List<DataElement> LoadDataContractDataElements(DataContractElement sourceElement, Multiplicity multiplicity)
		{
			DataContractElement targetElement = new DataContractElement(Store);
			targetElement.Name = DataContractLinkedElementType;
			DataContractElementBaseCanBeContainedOnContracts link = new DataContractElementBaseCanBeContainedOnContracts(sourceElement, targetElement);
			link.TargetMultiplicity = multiplicity;

			List<DataElement> dataElements = new List<DataElement>();
			ModelElementReferenceDataElement element1 = new ModelElementReferenceDataElement(Store);
			element1.Name = DataContractLinkedElementName;
			element1.Type = DataContractLinkedElementType;
			element1.SetLinkedElement(link.Id);

			dataElements.Add(element1);
			return dataElements;
		}

		private PrimitiveDataElement LoadDefaultPrimitiveDataElement()
		{
			return LoadPrimitiveDataElement(
				PrimitiveDataElementName1, PrimitiveDataElementType1, false, true, false);
		}

		private PrimitiveDataElement LoadPrimitiveDataElement(
			string name, string type, bool isArray, bool isDataMember, bool isNullable)
		{
			//TODO: FIXXXXXXXXXX
			PrimitiveDataElement element = new PrimitiveDataElement(Store);
			element.Name = name;
			element.Type = type;
			element.CollectionType = (isArray ? typeof(List<>) : null);
			element.IsNullable = isNullable;
			element.IsDataMember = isDataMember;
			return element;
		}

		#endregion
	}
}
