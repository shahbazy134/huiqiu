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
using System.Reflection;
using System.IO;

namespace Microsoft.Practices.ServiceFactory.RecipeFramework.Extensions.Tests
{
	[TestClass]
	public class AssemblyResolverFixture
	{
		[TestMethod]
		public void ResolveNullAssemblyNameShouldReturnNull()
		{
			AssemblyResolver target = new AssemblyResolver();

			Assembly actual = target.Resolve(null);

			Assert.IsNull(actual, "Type should not be resolved");
		}

		[TestMethod]
		public void ResolveEmptyStringAssemblyNameShouldReturnNull()
		{
			AssemblyResolver target = new AssemblyResolver();

			Assembly actual = target.Resolve(String.Empty);

			Assert.IsNull(actual, "Type should not be resolved");
		}

		[TestMethod]
		public void ResolveBadAssemblyNameShouldReturnNull()
		{
			AssemblyResolver target = new AssemblyResolver();

			Assembly actual = target.Resolve("foo");

			Assert.IsNull(actual, "Type should not be resolved");
		}

		[TestMethod]
		public void ResolveAlreadyLoadedAssemblySuccessfully()
		{
			AssemblyResolver target = new AssemblyResolver();

			Assembly actual = target.Resolve("System.Xml, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");

			Assert.IsNotNull(actual, "Loaded type could not be resolved");		
		}

		[TestMethod]
		public void ResolveAlreadyLoadedAssemblyWithDifferentPublicKeyFails()
		{
			AssemblyResolver target = new AssemblyResolver();

			Assembly actual = target.Resolve("System.Xml, Version=2.0.0.0, Culture=neutral, PublicKeyToken=aaaaaaaaaaaaaaaa");

			Assert.IsNull(actual, "Type should not be resolved");
		}

		[TestMethod]
		public void ResolveAlreadyLoadedAssemblyWithNullPublicKeyFails()
		{
			AssemblyResolver target = new AssemblyResolver();

			Assembly actual = target.Resolve("System.Xml, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null");

			Assert.IsNull(actual, "Type should not be resolved");
		}

		[TestMethod]
		public void ResolveAlreadyLoadedAssemblyWithDifferentVersionFails()
		{
			AssemblyResolver target = new AssemblyResolver();

			Assembly actual = target.Resolve("System.Xml, Version=2.0.0.1, Culture=neutral, PublicKeyToken=b77a5c561934e089");

			Assert.IsNull(actual, "Type should not be resolved");
		}

		[TestMethod]
		public void ResolveAlreadyLoadedAssemblyWithDifferentLocaleFails()
		{
			AssemblyResolver target = new AssemblyResolver();

			Assembly actual = target.Resolve("System.Xml, Version=2.0.0.0, Culture=en-us, PublicKeyToken=b77a5c561934e089");

			Assert.IsNull(actual, "Type should not be resolved");
		}

		[TestMethod]
		public void ResolveAlreadyLoadedAssemblyWithNoPublicKeySucceeds()
		{
			AssemblyResolver target = new AssemblyResolver();

			Assembly actual = target.Resolve("System.Xml, Version=2.0.0.0, Culture=neutral");

			Assert.IsNotNull(actual, "Loaded type could not be resolved");
		}

		[TestMethod]
		public void ResolveAlreadyLoadedAssemblyWithNoVersionSucceeds()
		{
			AssemblyResolver target = new AssemblyResolver();

			Assembly actual = target.Resolve("System.Xml, Culture=neutral, PublicKeyToken=b77a5c561934e089");

			Assert.IsNotNull(actual, "Loaded type could not be resolved");
		}

		[TestMethod]
		public void ResolveAlreadyLoadedAssemblyWithNoCultureSucceeds()
		{
			AssemblyResolver target = new AssemblyResolver();

			Assembly actual = target.Resolve("System.Xml, Version=2.0.0.0, PublicKeyToken=b77a5c561934e089");

			Assert.IsNotNull(actual, "Loaded type could not be resolved");
		}

		[TestMethod]
		[DeploymentItem("System.Security.dll")]
		public void ResolveAssemblyThatsLoadedInTheLoadFromContext()
		{
			string assemblyName = "System.Security, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a";

			AssemblyResolver target = new AssemblyResolver();
			Assert.IsNull(target.Resolve(assemblyName), "The test assembly should not be loaded before the test starts");

			Assembly.LoadFrom(Path.Combine(Directory.GetCurrentDirectory().ToString(), "System.Security.dll"));

			Assembly actual = target.Resolve(assemblyName);
			
			Assert.IsNotNull(actual, "Loaded type could not be resolved");		
		}
	}
}
