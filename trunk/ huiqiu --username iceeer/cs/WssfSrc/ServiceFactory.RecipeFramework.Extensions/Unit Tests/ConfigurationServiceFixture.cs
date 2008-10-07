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
using Microsoft.Practices.UnitTestLibrary;
using Microsoft.Win32;
using System.IO;
using System.Reflection;
using Microsoft.Practices.RecipeFramework.Services;
using Microsoft.VisualStudio.Shell.Interop;

namespace Microsoft.Practices.ServiceFactory.RecipeFramework.Extensions.Tests
{
	[TestClass]
	public class TestableConfigurationServiceFixture
	{
		[TestMethod]
		public void ValidPackageInstallReturnsPackagePath()
		{
			string packagePath = Environment.CurrentDirectory;
			string registryRoot = @"SOFTWARE\Microsoft\VisualStudio\8.0\";
			Guid packageGuid = new Guid();

			MockRegistry registry = CreateMockRegistry(registryRoot, packagePath, packageGuid);

			MockServiceProvider serviceProvider = new MockServiceProvider();
			serviceProvider.AddService(typeof(SLocalRegistry), new MockLocalRegistry(registryRoot));

			TestableConfigurationService target = new TestableConfigurationService(packageGuid, registry, serviceProvider);

			Assert.AreEqual(packagePath, target.BasePath); 
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void NoPackageInstallRegistryEntries()
		{
			string packagePath = Environment.CurrentDirectory;
			string registryRoot = @"SOFTWARE\Microsoft\VisualStudio\8.0\";

			MockServiceProvider serviceProvider = new MockServiceProvider();
			serviceProvider.AddService(typeof(SLocalRegistry), new MockLocalRegistry(registryRoot));
			TestableConfigurationService target = new TestableConfigurationService(new Guid(), new MockRegistry(), serviceProvider);

			Assert.AreEqual(packagePath, target.BasePath);
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void PackageInstallRegistryKeyMissingEntries()
		{
			string packagePath = Environment.CurrentDirectory;
			string registryRoot = @"SOFTWARE\Microsoft\VisualStudio\8.0\";
			Guid packageGuid = Guid.NewGuid();

			MockRegistry registry = CreateMockRegistry(registryRoot, packagePath, Guid.NewGuid());
			registry.MockSetValue(Registry.LocalMachine, Path.Combine(registryRoot, "Packages"), "different package name");

			MockServiceProvider serviceProvider = new MockServiceProvider();
			serviceProvider.AddService(typeof(SLocalRegistry), new MockLocalRegistry(registryRoot));

			TestableConfigurationService target = new TestableConfigurationService(packageGuid, registry, serviceProvider);

			Assert.AreEqual(packagePath, target.BasePath);
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void PackageInstallRegistryKeyRefersToInvalidPath()
		{
			string packagePath = @"\no such folder";
			string registryRoot = @"SOFTWARE\Microsoft\VisualStudio\8.0\";
			Guid packageGuid = new Guid();

			MockRegistry registry = CreateMockRegistry(registryRoot, packagePath, packageGuid);

			MockServiceProvider serviceProvider = new MockServiceProvider();
			serviceProvider.AddService(typeof(SLocalRegistry), new MockLocalRegistry(registryRoot));

			TestableConfigurationService target = new TestableConfigurationService(packageGuid, registry, serviceProvider);

			Assert.AreEqual(packagePath, target.BasePath);
		}

		[TestMethod]
		public void UseLocalRegistryToFindPackagePath()
		{
			string packagePath = Environment.CurrentDirectory;
			string registryRoot = @"SOFTWARE\Microsoft\VisualStudio\8.0\";
			Guid packageGuid = new Guid();

			MockRegistry registry = CreateMockRegistry(registryRoot, packagePath, packageGuid);

			MockServiceProvider serviceProvider = new MockServiceProvider();
			serviceProvider.AddService(typeof(SLocalRegistry), new MockLocalRegistry(registryRoot));

			TestableConfigurationService target = new TestableConfigurationService(packageGuid, registry, serviceProvider);

			Assert.AreEqual(packagePath, target.BasePath);
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void FailIfLocalRegistryServiceNotAvailable()
		{
			MockRegistry registry = new MockRegistry();

			MockServiceProvider serviceProvider = new MockServiceProvider();
			TestableConfigurationService target = new TestableConfigurationService(new Guid(), registry, serviceProvider);

			string value = target.BasePath;
		}

		//	Configure a mock registry as though a package was installed.
		//
		private MockRegistry CreateMockRegistry(string registryRoot, string packagePath, Guid packageGuid)
		{
			const string packageName = "package name";

			string packageKey = Path.Combine(registryRoot, "Packages");
			packageKey = Path.Combine(packageKey, packageGuid.ToString("B"));
			string packageMenuKey = Path.Combine(Path.Combine(registryRoot, "Menus"), packageName);

			MockRegistry registry = new MockRegistry();
			registry.MockSetValue(Registry.LocalMachine, packageKey, packageName);
			registry.MockSetValue(Registry.LocalMachine, packageMenuKey, Path.Combine(packagePath, "package.dll"));

			return registry;
		}
	}

	public class TestableConfigurationService: ConfigurationService
	{
		private IAccessRegistry registry;

		public TestableConfigurationService(Guid packageGuid, IAccessRegistry registry)
			: base(packageGuid)
		{
			this.registry = registry;
		}

		public TestableConfigurationService(Guid packageGuid, IAccessRegistry registry, IServiceProvider serviceProvider)
			: base(packageGuid, serviceProvider)
		{
			this.registry = registry;
		}

		protected override IAccessRegistry GetRegistryInstance()
		{
			return registry;
		}
	}
}
