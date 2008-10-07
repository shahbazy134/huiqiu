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
using System.Threading;
using System.ServiceModel;
using System.ServiceModel.Configuration;
using System.ServiceModel.Security;
using System.ComponentModel.Design;
using System.ServiceModel.Description;
using System.IO;
using System.Windows.Forms;
using System.Configuration;
using Microsoft.Practices.ServiceFactory.Recipes.CreateWCFClientProxy.Presentation;
using Microsoft.Practices.ServiceFactory.Library.Validation;
using Microsoft.Practices.UnitTestLibrary;
using Microsoft.Practices.UnitTestLibrary.Utilities;
using System.CodeDom.Compiler;
using System.Reflection;

namespace GuidancePackage.Tests.Recipes.CreateWCFClientProxy.Presentation
{
	/// <summary>
	/// Summary description for SecureClientConfigPresenterFixture
	/// </summary>
	[TestClass]
	public class SecureClientConfigPresenterFixture
	{
		private MockSecureClientConfigPage view;
		private SecureClientConfigPresenter presenter;
		private SecureClientConfigModel model;
		private IDictionaryService dict;

		[TestInitialize]
		public void TestInitialize()
		{
			dict = new MockDictionaryService();
			dict.SetValue("ClientProjectNamespace", "TestNamespace");
            string assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            view = new MockSecureClientConfigPage(Path.Combine(assemblyPath, @"SampleData\DescriptionModel\MockService.wsdl"));
			model = new SecureClientConfigModel(dict);
			presenter = new SecureClientConfigPresenter(view, model);
		}

		[TestMethod]
		[DeploymentItem(@"SampleData\DescriptionModel\MockService.wsdl", @"SampleData\DescriptionModel\")]
		[DeploymentItem(@"SampleData\DescriptionModel\MockService.wsdl0", @"SampleData\DescriptionModel\")]
		[DeploymentItem(@"SampleData\DescriptionModel\MockService.xsd0", @"SampleData\DescriptionModel\")]
		[DeploymentItem(@"SampleData\DescriptionModel\MockService.xsdl", @"SampleData\DescriptionModel\")]
		[DeploymentItem("Microsoft.Practices.RecipeFramework.Extensions.dll")]
		[TimeoutAttribute(60000)]
		public void ShouldPopulateEndpointsOnViewLoaded()
		{
			AutoResetEvent signal = new AutoResetEvent(false);
			int endpointAddressesCount = 0;
			bool progress = false;
			string errorMessage = null;

			presenter.ViewLoaded += delegate(object sender, EventArgs e)
			{
				endpointAddressesCount = view.EndpointAddresses.Count;
				progress = view.Progress;
				errorMessage = view.ErrorMessage;
				signal.Set();
			};
			Assert.AreEqual(0, view.EndpointAddresses.Count);
			view.FireViewLoading();
			signal.WaitOne(60000, true);
			Assert.AreEqual(1, endpointAddressesCount);
			Assert.IsFalse(progress);
			Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
			Assert.IsTrue(view.FireRequestIsDataValid());
		}

		[TestMethod]
		[DeploymentItem(@"SampleData\DescriptionModel\Importers.config", @"SampleData\DescriptionModel\")]
		[DeploymentItem(@"SampleData\DescriptionModel\MockService.wsdl", @"SampleData\DescriptionModel\")]
		[DeploymentItem(@"SampleData\DescriptionModel\MockImporters.cs", @"SampleData\DescriptionModel\")]
		[DeploymentItem(@"SampleData\DescriptionModel\MockService.wsdl0", @"SampleData\DescriptionModel\")]
		[DeploymentItem(@"SampleData\DescriptionModel\MockService.xsd0", @"SampleData\DescriptionModel\")]
		[DeploymentItem(@"SampleData\DescriptionModel\MockService.xsdl", @"SampleData\DescriptionModel\")]
		[DeploymentItem("System.ServiceModel.dll")]
		[DeploymentItem("Microsoft.Practices.RecipeFramework.Extensions.dll")]
		[TimeoutAttribute(60000)]
		public void ShouldGetImporterOnImporterNotFound()
		{
			dict.SetValue("Configuration", ConfigurationLoader.LoadConfiguration("Importers.config", @"SampleData\DescriptionModel\"));
			view.ImporterFile = CreateMockImportersAssembly("MockImporters.dll");
			AutoResetEvent signal = new AutoResetEvent(false);
			int endpointAddressesCount = 0;
			bool progress = false;
			string errorMessage = null;

			presenter.ViewLoaded += delegate(object sender, EventArgs e)
			{
				endpointAddressesCount = view.EndpointAddresses.Count;
				progress = view.Progress;
				errorMessage = view.ErrorMessage;
				signal.Set();
			};
			Assert.AreEqual(0, view.EndpointAddresses.Count);
			view.FireViewLoading();
			signal.WaitOne(60000, true);
			Assert.AreEqual(1, endpointAddressesCount);
			Assert.IsFalse(progress);
			Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
			Assert.IsTrue(view.FireRequestIsDataValid());
		}

		private string CreateMockImportersAssembly(string assemblyName)
		{
			CompilerResults results = DynamicCompilation.CompileAssemblyFromSource(
				File.ReadAllText(ConfigurationLoader.GetConfigurationFilePath(@"SampleData\DescriptionModel\MockImporters.cs")),
				assemblyName,
				new Microsoft.CSharp.CSharpCodeProvider(),
				new string[] { "System.ServiceModel.dll", "System.Web.Services.dll", "System.Xml.dll" });

			if (results.PathToAssembly == null)
			{
				return null;
			}

			return ConfigurationLoader.GetConfigurationFilePath(results.PathToAssembly);
		}

	}

	#region MockSecureClientConfigPage class

	class MockSecureClientConfigPage : ISecureClientConfigView
	{
		private IDictionaryService dictionaryService;
		public bool Progress;
		public string ErrorMessage = null;
		public List<string> EndpointAddresses;
		public string ImporterFile = null;

		public MockSecureClientConfigPage(string serviceAddress)
		{
			this.serviceAddress = serviceAddress;
			this.dictionaryService = new MockDictionaryService();
			this.EndpointAddresses = new List<string>();
		}

		public void FireViewLoading()
		{
			if (ViewLoading != null)
			{
				ViewLoading(this, new EventArgs());
			}
		}

		public void FireViewClosing()
		{
			if (ViewClosing != null)
			{
				ViewClosing(this, new System.Windows.Forms.FormClosingEventArgs(CloseReason.UserClosing, false));
			}
		}

		public bool FireRequestIsDataValid()
		{
			bool result = false;
			if (RequestIsDataValid != null)
			{
				RequestDataEventArgs<bool> request = new RequestDataEventArgs<bool>();
				RequestIsDataValid(this, request);
				result = request.Value;
			}
			return result;
		}

		private ValidationEventArgs CreateValidationEventArgs()
		{
			return new ValidationEventArgs(CreateFailureReport());
		}


		private IValidationResultReport CreateFailureReport()
		{
			return new DefaultValidationResultReport();
		}

		#region ISecureClientConfigView Members

		string ISecureClientConfigView.ServiceAddress
		{
			get { return serviceAddress; }
		} private string serviceAddress;

		string ISecureClientConfigView.SelectedEndpoint
		{
			get { return selectedEndpointAddress; }
			set
			{
				selectedEndpointAddress = value;
				if (SelectedEndpointChanging != null)
				{
					SelectedEndpointChanging(this, CreateValidationEventArgs());
				}
			}
		} private string selectedEndpointAddress;

		void ISecureClientConfigView.AddEndpoint(string endpoint)
		{
			EndpointAddresses.Add(endpoint);
		}

		void ISecureClientConfigView.ClearEndpoints()
		{
			EndpointAddresses.Clear();
		}

		void ISecureClientConfigView.SetProgress(bool value)
		{
			Progress = value;
		}

		void ISecureClientConfigView.ShowMessage(string message, MessageBoxIcon icon)
		{
			ErrorMessage = message;
		}

		string ISecureClientConfigView.GetImporterFile(string message, string initialDirectory)
		{
			return ImporterFile;
		}

		public event EventHandler ViewLoading;
		public event EventHandler<FormClosingEventArgs> ViewClosing;
		public event EventHandler<ValidationEventArgs> SelectedEndpointChanging;
		public event EventHandler<RequestDataEventArgs<bool>> RequestIsDataValid;

		#endregion
	}

	#endregion
}
