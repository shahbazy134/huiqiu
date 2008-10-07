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
using System.Configuration;
using System.IO;
using System.ServiceModel.Configuration;
using EnvDTE;
using Microsoft.Practices.ServiceFactory.Description;
using Microsoft.Practices.ServiceFactory.Extenders.HostDesigner.Wcf.CodeGeneration;
using Microsoft.Practices.ServiceFactory.Extenders.HostDesigner.Wcf.Strategies;
using Microsoft.Practices.ServiceFactory.HostDesigner;
using Microsoft.Practices.UnitTestLibrary;
using Microsoft.Practices.VisualStudio.Helper;
using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Modeling.Diagrams;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Practices.ServiceFactory.ServiceContracts;
using Microsoft.Practices.ServiceFactory.RecipeFramework.Extensions.Tests.Mocks;
using Microsoft.Practices.Modeling.Dsl.Service;
using Microsoft.Practices.ServiceFactory.Extenders.ServiceContract.Wcf;
using Microsoft.Practices.Modeling.CodeGeneration.Artifacts;
using Microsoft.Practices.UnitTestLibrary.Utilities;

namespace Microsoft.Practices.ServiceFactory.Extenders.HostDesigner.Wcf.Tests
{
	[TestClass]
	public class ServiceModelConfigurationStrategyFixture
	{
		#region Init Tests

		const string serviceName = "MyService";
		const string serviceElementName = "Namespace1.MyService";
		const string endpointElementName = "Namespace1.IMyServiceContract";
		const string behaviorElementName = "Namespace1.MyService_Behavior";
		const string serviceContractName = "MyServiceContract";
		const string serviceModelProjectName = "Project1.model";
		const string serviceModelFileName = "sc.servicecontract";
		const string serviceMelReferenceName = "ServiceMelReference1";
		const string projectMappingTableName = "WCF";

		private Store scStore;
		private Store hdStore;
		private DomainModel scDomainModel;
		private DomainModel hdDomainModel;
		private Transaction scTransaction;
		private Transaction hdTransaction;
		private ServiceContractModel scModel;
		private HostDesignerModel hdModel;
		private MockServiceProvider serviceProvider;
		private ServiceReference reference;
		ServiceModelConfigurationManager manager;
		ServiceModelConfigurationStrategy strategy;
		ProjectNode project;
		WcfServiceConfigurationArtifactLink link;
		string tempFile;
		string serviceMoniker;

		[TestInitialize]		
		public void TestInitialize()
		{
			Microsoft.Practices.RecipeFramework.Library.Guard.ArgumentNotNullOrEmptyString("not empty", "dummy");

			serviceProvider = new MockMappingServiceProvider();
			scStore = new Store(serviceProvider, typeof(CoreDesignSurfaceDomainModel), typeof(ServiceContractDslDomainModel));
			hdStore = new Store(serviceProvider, typeof(CoreDesignSurfaceDomainModel), typeof(HostDesignerDomainModel));

			serviceProvider.AddService(typeof(IDslIntegrationService), new MockDIS(scStore));

			Service service = PopulateServiceContractModel();
			PopulateHostDesignerModel(service);

			strategy = new ServiceModelConfigurationStrategy();
			project = GetTestProjectNode(EnvDTE.CodeModelLanguageConstants.vsCMLanguageVB);
			link = GetArtifactLink(project, serviceProvider, "Web.Config");
			tempFile = Path.GetTempFileName();
		}

		[TestCleanup]
		public void TestCleanup()
		{
			scTransaction.Rollback();
			hdTransaction.Rollback();

			File.Delete(tempFile);
		}

		#endregion

		[TestMethod]
		[DeploymentItem("ProjectMapping.HostDesigner.Tests.xml")]
		[DeploymentItem("Microsoft.Practices.RecipeFramework.Library.dll")]
		public void CanGetGeneratedService()
		{
			ProjectMappingManagerSetup.InitializeManager("ProjectMapping.HostDesigner.Tests.xml");

			HostDesignerModel model = GetModelElement<HostDesignerModel>(hdStore, HostDesignerModel.DomainClassId);
			HostApplication host = GetModelElement<HostApplication>(hdStore, HostApplication.DomainClassId);
			ServiceReference serviceRef = GetServiceReference(hdStore, serviceMoniker);
			Endpoint endpoint = GetEndpoint(hdStore, "FooEndpoint", "http://Foo");

			serviceRef.Endpoints.Add(endpoint);
			host.ServiceDescriptions.Add(serviceRef);
			model.HostApplications.Add(host);

			link.ModelElement = serviceRef;

			string content = strategy.Generate(link)[@".\Web.config"];

			manager = new ServiceModelConfigurationManager(GetConfiguration(tempFile, content));

			ServiceEndpointElement endpointElement =
				manager.GetEndpoint(
					serviceElementName,
					ServiceModelConfigurationManager.BuildEndpointName(
						endpoint.Name, new Uri(endpoint.Address), BindingType.basicHttpBinding.ToString(), endpointElementName));

			Assert.AreEqual(BindingType.basicHttpBinding.ToString(), endpointElement.Binding);
		}

		[TestMethod]
		[DeploymentItem("ProjectMapping.HostDesigner.Tests.xml")]
		public void CanGetGeneratedEndpoint()
		{
			ProjectMappingManagerSetup.InitializeManager("ProjectMapping.HostDesigner.Tests.xml");

			HostDesignerModel model = GetModelElement<HostDesignerModel>(hdStore, HostDesignerModel.DomainClassId);
			HostApplication host = GetModelElement<HostApplication>(hdStore, HostApplication.DomainClassId);
			ServiceReference serviceRef = GetServiceReference(hdStore, serviceMoniker);
			Endpoint endpoint = GetEndpoint(hdStore, "FooEndpoint", "http://Foo");

			serviceRef.Endpoints.Add(endpoint);
			host.ServiceDescriptions.Add(serviceRef);
			model.HostApplications.Add(host);

			link.ModelElement = serviceRef;

			string content = strategy.Generate(link)[@".\Web.config"];

			manager = new ServiceModelConfigurationManager(GetConfiguration(tempFile, content));

			ServiceEndpointElement endpointElement =
				manager.GetEndpoint(
					serviceElementName,
					ServiceModelConfigurationManager.BuildEndpointName(
						endpoint.Name, new Uri(endpoint.Address), BindingType.basicHttpBinding.ToString(), endpointElementName));

			Assert.IsNotNull(endpoint);
			Assert.AreEqual(BindingType.basicHttpBinding.ToString(), endpointElement.Binding);
			Assert.AreEqual(endpointElementName, endpointElement.Contract);
		}

		[TestMethod]
		[DeploymentItem("ProjectMapping.HostDesigner.Tests.xml")]
		public void CanGetGeneratedBehavior()
		{
			ProjectMappingManagerSetup.InitializeManager("ProjectMapping.HostDesigner.Tests.xml");

			HostDesignerModel model = GetModelElement<HostDesignerModel>(hdStore, HostDesignerModel.DomainClassId);
			HostApplication host = GetModelElement<HostApplication>(hdStore, HostApplication.DomainClassId);
			ServiceReference serviceRef = GetServiceReference(hdStore, serviceMoniker);
			Endpoint endpoint = GetEndpoint(hdStore, "FooEndpoint", "http://Foo");

			serviceRef.Endpoints.Add(endpoint);
			host.ServiceDescriptions.Add(serviceRef);
			model.HostApplications.Add(host);

			link.ModelElement = serviceRef;

			string content = strategy.Generate(link)[@".\Web.config"];

			manager = new ServiceModelConfigurationManager(GetConfiguration(tempFile, content));

			NamedServiceModelExtensionCollectionElement<BehaviorExtensionElement> behavior =
				manager.GetBehavior(behaviorElementName);

			Assert.IsNotNull(behavior);
			Assert.AreEqual(2, behavior.Count);
		}

		[TestMethod]
		[DeploymentItem("ProjectMapping.HostDesigner.Tests.xml")]
		public void CanSetWsHttpBinding()
		{
			ProjectMappingManagerSetup.InitializeManager("ProjectMapping.HostDesigner.Tests.xml");

			HostDesignerModel model = GetModelElement<HostDesignerModel>(hdStore, HostDesignerModel.DomainClassId);
			HostApplication host = GetModelElement<HostApplication>(hdStore, HostApplication.DomainClassId);
			ServiceReference serviceRef = GetServiceReference(hdStore, serviceMoniker);
			Endpoint endpoint = GetEndpoint(hdStore, "FooEndpoint", "http://Foo", BindingType.wsHttpBinding);

			serviceRef.Endpoints.Add(endpoint);
			host.ServiceDescriptions.Add(serviceRef);
			model.HostApplications.Add(host);

			link.ModelElement = serviceRef;

			string content = strategy.Generate(link)[@".\Web.config"];

			manager = new ServiceModelConfigurationManager(GetConfiguration(tempFile, content));

			ServiceEndpointElement endpointElement =
				manager.GetEndpoint(
					serviceElementName,
					ServiceModelConfigurationManager.BuildEndpointName(
						endpoint.Name, new Uri(endpoint.Address), BindingType.wsHttpBinding.ToString(), endpointElementName));

			Assert.AreEqual(BindingType.wsHttpBinding.ToString(), endpointElement.Binding);
		}

		[TestMethod]
		[DeploymentItem("ProjectMapping.HostDesigner.Tests.xml")]
		public void AddServiceMetadataElementWithTrue()
		{
			ProjectMappingManagerSetup.InitializeManager("ProjectMapping.HostDesigner.Tests.xml");

			HostDesignerModel model = GetModelElement<HostDesignerModel>(hdStore, HostDesignerModel.DomainClassId);
			HostApplication host = GetModelElement<HostApplication>(hdStore, HostApplication.DomainClassId);
			ServiceReference serviceRef = GetServiceReference(hdStore, serviceMoniker);
			Endpoint endpoint = GetEndpoint(hdStore, "FooEndpoint", "http://Foo", BindingType.wsHttpBinding);

			serviceRef.Endpoints.Add(endpoint);
			host.ServiceDescriptions.Add(serviceRef);
			model.HostApplications.Add(host);

			link.ModelElement = serviceRef;

			string content = strategy.Generate(link)[@".\Web.config"];

			manager = new ServiceModelConfigurationManager(GetConfiguration(tempFile, content));

			ServiceMetadataPublishingElement svcMetadata =
				ServiceModelConfigurationManager.GetBehaviorExtensionElement<ServiceMetadataPublishingElement>(manager.GetBehavior(behaviorElementName));
			Assert.IsNotNull(svcMetadata);
			Assert.IsTrue(svcMetadata.HttpGetEnabled);
		}

		[TestMethod]
		[DeploymentItem("ProjectMapping.HostDesigner.Tests.xml")]
		public void AddServiceMetadataElementWithFalse()
		{
			ProjectMappingManagerSetup.InitializeManager("ProjectMapping.HostDesigner.Tests.xml");

			HostDesignerModel model = GetModelElement<HostDesignerModel>(hdStore, HostDesignerModel.DomainClassId);
			HostApplication host = GetModelElement<HostApplication>(hdStore, HostApplication.DomainClassId);
			ServiceReference serviceRef = GetServiceReference(hdStore, serviceMoniker, false);
			Endpoint endpoint = GetEndpoint(hdStore, "FooEndpoint", "http://Foo", BindingType.wsHttpBinding);

			serviceRef.Endpoints.Add(endpoint);
			host.ServiceDescriptions.Add(serviceRef);
			model.HostApplications.Add(host);

			link.ModelElement = serviceRef;

			string content = strategy.Generate(link)[@".\Web.config"];

			manager = new ServiceModelConfigurationManager(GetConfiguration(tempFile, content));

			ServiceMetadataPublishingElement svcMetadata =
				ServiceModelConfigurationManager.GetBehaviorExtensionElement<ServiceMetadataPublishingElement>(manager.GetBehavior(behaviorElementName));
			Assert.IsNotNull(svcMetadata);
			Assert.IsFalse(svcMetadata.HttpGetEnabled);
		}

		[TestMethod]
		[DeploymentItem("ProjectMapping.HostDesigner.Tests.xml")]
		public void EnableMetadataExchangeEndpoint()
		{
			ProjectMappingManagerSetup.InitializeManager("ProjectMapping.HostDesigner.Tests.xml");

			HostDesignerModel model = GetModelElement<HostDesignerModel>(hdStore, HostDesignerModel.DomainClassId);
			HostApplication host = GetModelElement<HostApplication>(hdStore, HostApplication.DomainClassId);
			ServiceReference serviceRef = GetServiceReference(hdStore, serviceMoniker);
			Endpoint endpoint = GetEndpoint(hdStore, "FooEndpoint", "http://Foo", BindingType.wsHttpBinding);

			serviceRef.Endpoints.Add(endpoint);
			host.ServiceDescriptions.Add(serviceRef);
			model.HostApplications.Add(host);

			link.ModelElement = serviceRef;

			string content = strategy.Generate(link)[@".\Web.config"];

			manager = new ServiceModelConfigurationManager(GetConfiguration(tempFile, content));

			ServiceElement service = manager.GetService(serviceElementName);
			ServiceEndpointElement mexEndpoint = ServiceModelConfigurationManager.GetMetadataExchangeEndpoint();
			foreach (ServiceEndpointElement endpointElement in service.Endpoints)
			{
				if (endpointElement.Binding == mexEndpoint.Binding)
				{
					Assert.AreEqual(mexEndpoint.Address, endpointElement.Address);
					Assert.AreEqual(mexEndpoint.Contract, endpointElement.Contract);
					return;
				}
			}
			Assert.Fail("MEX endpoint was not found");
		}

		[TestMethod]
		[DeploymentItem("Microsoft.Practices.RecipeFramework.Library.dll")]
		[DeploymentItem("ProjectMapping.HostDesigner.Tests.xml")]
		[DeploymentItem("InvalidSection.config")]
		public void ShouldNotUpdateAnInvalidFile()
		{
			ProjectMappingManagerSetup.InitializeManager("ProjectMapping.HostDesigner.Tests.xml");

			HostDesignerModel model = GetModelElement<HostDesignerModel>(hdStore, HostDesignerModel.DomainClassId);
			HostApplication host = GetModelElement<HostApplication>(hdStore, HostApplication.DomainClassId);
			ServiceReference serviceRef = GetServiceReference(hdStore, serviceMoniker);
			Endpoint endpoint = GetEndpoint(hdStore, "FooEndpoint", "http://Foo");

			serviceRef.Endpoints.Add(endpoint);
			host.ServiceDescriptions.Add(serviceRef);
			model.HostApplications.Add(host);

			link = GetArtifactLink(project, serviceProvider, "InvalidSection.config");
			link.ModelElement = serviceRef;
                
			string originalConfig = File.ReadAllText(ConfigurationLoader.GetConfigurationFilePath("InvalidSection.config"));
			string content = null;
			try
			{
				content = strategy.Generate(link)[@".\InvalidSection.config"];
				Assert.Fail();
			}
			catch (ConfigurationErrorsException)
			{
				Assert.IsTrue(string.IsNullOrEmpty(content));
				Assert.AreEqual<string>(originalConfig,
					File.ReadAllText(ConfigurationLoader.GetConfigurationFilePath("InvalidSection.config")));
			}
		}

		#region Private members

		private static ServiceReference GetServiceReference(Store hdStore, string implementationType)
		{
			return GetServiceReference(hdStore, implementationType, true);
		}

		private static ServiceReference GetServiceReference(Store hdStore, string implementationType, bool exposeMetadata)
		{
			ServiceReference serviceRef = GetModelElement<ServiceReference>(hdStore, ServiceReference.DomainClassId);
			WcfServiceDescription wcfServiceDescription = new WcfServiceDescription();
			wcfServiceDescription.EnableMetadataPublishing = exposeMetadata;

			serviceRef.ServiceImplementationType = implementationType;
			serviceRef.ObjectExtender = wcfServiceDescription;

			return serviceRef;
		}

		private static Endpoint GetEndpoint(Store hdStore, string name, string address)
		{
			return GetEndpoint(hdStore, name, address, BindingType.basicHttpBinding);
		}

		private static Endpoint GetEndpoint(Store hdStore, string name, string address, BindingType bindingType)
		{
			Endpoint endpoint = GetModelElement<Endpoint>(hdStore, Endpoint.DomainClassId);
			WcfEndpoint wcfEndpoint = new WcfEndpoint();
			wcfEndpoint.BindingType = bindingType;

			endpoint.Name = name;
			endpoint.Address = address;
			endpoint.ObjectExtender = wcfEndpoint;

			return endpoint;
		}

		private static TModelElement GetModelElement<TModelElement>(Store hdStore, Guid domainClassId) where TModelElement : ModelElement
		{
			return hdStore.ElementFactory.CreateElement(domainClassId) as TModelElement;
		}

		private static WcfServiceConfigurationArtifactLink GetArtifactLink(ProjectNode project, IServiceProvider serviceProvider, string itemName)
		{
			WcfServiceConfigurationArtifactLink link = new WcfServiceConfigurationArtifactLink();

			link.ItemName = itemName;
			link.Path = @"\";
			link.Container = project.ProjectGuid;
			link.Project = (Project)project.ExtObject;

			link.Data.Add(typeof(IServiceProvider).FullName, serviceProvider);
			link.Data.Add(typeof(ProjectNode).FullName, project);

			return link;
		}

		private static System.Configuration.Configuration GetConfiguration(string configurationFile, string configurationContent)
		{
			File.WriteAllText(configurationFile, configurationContent);
			return OpenExeConfiguration(configurationFile);
		}

		private static System.Configuration.Configuration OpenExeConfiguration(string configurationFile)
		{
			FileInfo fileInfo = new FileInfo(configurationFile);
			ExeConfigurationFileMap exeFileMap = new ExeConfigurationFileMap();
			exeFileMap.ExeConfigFilename = fileInfo.FullName;

			return System.Configuration.ConfigurationManager.OpenMappedExeConfiguration(
				exeFileMap, ConfigurationUserLevel.None);
		}

		private static ProjectNode GetTestProjectNode()
		{
			return GetTestProjectNode(EnvDTE.CodeModelLanguageConstants.vsCMLanguageCSharp);
		}

		private static ProjectNode GetTestProjectNode(string language)
		{
			MockVSHierarchy root = new MockVSHierarchy();
			MockVsSolution vsSolution = new MockVsSolution(root);
			MockVSHierarchy project = new MockVSHierarchy("Project.project");

			MockEnvDTEProject mockEnvDteProject = project.ExtObject as MockEnvDTEProject;
			if (mockEnvDteProject != null)
			{
				mockEnvDteProject.SetCodeModel(new MockCodeModel(language));
			}
			root.AddProject(project);

			ProjectNode projectNode = new ProjectNode(vsSolution, project.GUID);

			return projectNode;
		}

		private Service PopulateServiceContractModel()
		{
			scDomainModel = scStore.GetDomainModel<ServiceContractDslDomainModel>();
			scTransaction = scStore.TransactionManager.BeginTransaction();

			scModel = (ServiceContractModel)scDomainModel.CreateElement(new Partition(scStore), typeof(ServiceContractModel), null);
			scModel.ImplementationTechnology = new ServiceContractWCFExtensionProvider();
			scModel.ProjectMappingTable = projectMappingTableName;

			Service service = scStore.ElementFactory.CreateElement(Service.DomainClassId) as Service;
			service.Name = serviceName;
			WCFService wcfService = new WCFService();
			wcfService.ModelElement = service;
			service.ObjectExtender = wcfService;

			Microsoft.Practices.ServiceFactory.ServiceContracts.ServiceContract contract =
				scStore.ElementFactory.CreateElement(Microsoft.Practices.ServiceFactory.ServiceContracts.ServiceContract.DomainClassId) as Microsoft.Practices.ServiceFactory.ServiceContracts.ServiceContract;

			contract.Name = serviceContractName;

			WCFServiceContract wcfServiceContract = new WCFServiceContract();
			wcfServiceContract.ModelElement = contract;
			contract.ObjectExtender = wcfServiceContract;
			scModel.ServiceContracts.Add(contract);

			service.ServiceContract = contract;

			scModel.Services.Add(service);
			return service;
		}

		private void PopulateHostDesignerModel(Service service)
		{
			hdDomainModel = hdStore.GetDomainModel<HostDesignerDomainModel>();
			hdTransaction = hdStore.TransactionManager.BeginTransaction();
			hdModel = (HostDesignerModel)hdDomainModel.CreateElement(new Partition(hdStore), typeof(HostDesignerModel), null);

			HostApplication app = (HostApplication)hdStore.ElementFactory.CreateElement(HostApplication.DomainClassId);

			app.ImplementationTechnology = new HostDesignerWcfExtensionProvider();

			reference = (ServiceReference)hdStore.ElementFactory.CreateElement(ServiceReference.DomainClassId);

			//mel://[DSLNAMESPACE]\[MODELELEMENTTYPE]\[MODELELEMENT.GUID]@[PROJECT]\[MODELFILE]
			serviceMoniker = string.Format(@"mel://{0}\{1}\{2}@{3}\{4}",
				service.GetType().Namespace,
				serviceName,
				service.Id.ToString(),
				serviceModelProjectName, serviceModelFileName);

			reference.Name = serviceMelReferenceName;
			reference.ServiceImplementationType = serviceMoniker;
			WcfServiceDescription wcfServiceDescription = new WcfServiceDescription();
			wcfServiceDescription.ModelElement = reference;
			reference.ObjectExtender = wcfServiceDescription;

			app.ServiceDescriptions.Add(reference);
		}

		#endregion
	}
}