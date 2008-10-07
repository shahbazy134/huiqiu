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
using Microsoft.Practices.UnitTestLibrary.Utilities;
using System.CodeDom.Compiler;
using System.Security.AccessControl;
using System.Threading;

namespace Microsoft.Practices.ServiceFactory.RecipeFramework.Extensions.Tests
{
	[TestClass]
	public class AssemblyLoaderFixture
	{
		[TestMethod]
		public void AssemblyLoaderLoadsAnAssemblyInLibraryFolder()
		{
			ResetLibrary();
			AssemblyLoader.ResetCache();

			string assemblyName = "AssemblyLoaderLoadsAnAssemblyInLibraryFolder.TestCode";

			GenerateTestAssembly(assemblyName);
			Assert.IsFalse(IsAssemblyLoaded(assemblyName));

			AssemblyLoader.LoadAll(Environment.CurrentDirectory);

			Assert.IsTrue(IsAssemblyLoaded(assemblyName));
		}

		[TestMethod]
		public void AssemblyLoaderLoadsMultipleAssembliesInLibraryFolder()
		{
			ResetLibrary();
			AssemblyLoader.ResetCache();

			string assemblyName1 = "AssemblyLoaderLoadsMultipleAssembliesInLibraryFolder.TestCode";
			GenerateTestAssembly(assemblyName1);
			Assert.IsFalse(IsAssemblyLoaded(assemblyName1));

			string assemblyName2 = "AssemblyLoaderLoadsMultipleAssembliesInLibraryFolder.TestCode";
			GenerateTestAssembly(assemblyName2);
			Assert.IsFalse(IsAssemblyLoaded(assemblyName2));

			AssemblyLoader.LoadAll(Environment.CurrentDirectory);

			Assert.IsTrue(IsAssemblyLoaded(assemblyName1));
			Assert.IsTrue(IsAssemblyLoaded(assemblyName2));
		}

		[TestMethod]
		public void AssemblyLoaderShouldIgnoreInvalidDllsInLibraryFolder()
		{
			ResetLibrary();
			AssemblyLoader.ResetCache();

			string assemblyName = "aaa";

			string fileName = Path.Combine(LibraryPath, assemblyName + ".dll");
			File.WriteAllText(fileName, "NotReallyADll");

			AssemblyLoader.LoadAll(Environment.CurrentDirectory);

			Assert.IsFalse(IsAssemblyLoaded(assemblyName));
		}

		[TestMethod]
		public void AssemblyLoaderShouldIgnoreDllsWhosDependenciesAreNotSatisfied()
		{
			ResetLibrary();
			AssemblyLoader.ResetCache();

			string assemblyName = "AssemblyLoaderShouldIgnoreDllsWhosDependenciesAreNotSatisfied.TestCode";
			string assemblyFilename = Path.Combine(LibraryPath, assemblyName + ".dll");

			GenerateTestAssembly(assemblyName);
			FileSecurity settings = File.GetAccessControl(assemblyFilename);
			settings.AddAccessRule(new FileSystemAccessRule(Environment.UserDomainName + @"\" + Environment.UserName, FileSystemRights.ReadAndExecute, AccessControlType.Deny));
			File.SetAccessControl(assemblyFilename, settings);

			AssemblyLoader.LoadAll(Environment.CurrentDirectory);

			Assert.IsFalse(IsAssemblyLoaded(assemblyName));
		}

		[TestMethod]
		public void AssemblyLoaderReturnsFilteredListOfTypes()
		{
			ResetLibrary();
			AssemblyLoader.ResetCache();

			string assemblyName1 = "AssemblyLoaderReturnsFilteredListOfTypes.TestCode";
			GenerateTestAssembly(assemblyName1);
			Assert.IsFalse(IsAssemblyLoaded(assemblyName1));

			AssemblyLoader.LoadAll(Environment.CurrentDirectory);

			Type typeToMatch = Type.GetType("AssemblyLoaderReturnsFilteredListOfTypes.TestCode.ITestInterface," + assemblyName1, true);

			IList<Type> result = AssemblyLoader.GetTypesByInterface(typeToMatch);

			Assert.AreEqual(1, result.Count, "Only one type expected.");
			Assert.AreEqual("TestInterfaceClass", result[0].Name, "Unexpected type found.");
		}

		[TestMethod]
		public void AssemblyLoaderRemembersLoadedAssemblies()
		{
			ResetLibrary();
			AssemblyLoader.ResetCache();

			string assemblyName1 = "AssemblyLoaderRemembersLoadedAssemblies.TestCode1";
			GenerateTestAssembly(assemblyName1);
			Assert.IsFalse(IsAssemblyLoaded(assemblyName1));

			string assemblyName2 = "AssemblyLoaderRemembersLoadedAssemblies.TestCode2";
			GenerateTestAssembly(assemblyName2);
			Assert.IsFalse(IsAssemblyLoaded(assemblyName2));

			// We need the expected count to account for the fact that
			// other tests may have loaded assemblies that are now locked
			// and could not be cleaned up until the AppDomain is unloaded.
			// Instead we get the expected count and compare that.
			int expectedFileCount = Directory.GetFiles(LibraryPath, "*.dll").Length;

			AssemblyLoader.LoadAll(Environment.CurrentDirectory);

			Assert.AreEqual<int>(expectedFileCount, AssemblyLoader.LoadedAssemblies.Count);
		}

		//	Helper functions

		private bool IsAssemblyLoaded(string name)
		{
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

			foreach (Assembly assembly in assemblies)
			{
				AssemblyName assemblyName = assembly.GetName();

				if (String.Compare(assemblyName.Name, name, StringComparison.OrdinalIgnoreCase) == 0)
					return true;
			}
			return false;
		}

		private void ResetLibrary()
		{
			if (!Directory.Exists(LibraryPath))
				Directory.CreateDirectory(LibraryPath);

			string[] assemblyFilenames = Directory.GetFiles(LibraryPath, "*.dll");
			foreach (string filename in assemblyFilenames)
			{
				try
				{
					File.Delete(Path.Combine(LibraryPath, filename));
				}
				catch (UnauthorizedAccessException) { } // Ignore assemblies that have been loaded by previous tests.
			}

		}

		private void GenerateTestAssembly(string assemblyName)
		{
			string path = Path.Combine(LibraryPath, assemblyName + ".dll");

			CompilerResults results = DynamicCompilation.CompileAssemblyFromSource(
				@" 
				namespace " + assemblyName + @" {
					public class TestClass { public void TestMethod() { } }
					
					public interface ITestInterface { }

					public class TestInterfaceClass : ITestInterface { public void TestNewMethod() { } }
				}
				",
				path,
				new CSharp.CSharpCodeProvider(),
				new string[] { });
			Assert.IsTrue(File.Exists(path));
		}

		private string LibraryPath
		{
			get
			{
				return Path.Combine(Environment.CurrentDirectory, "Lib");
			}
		}
	}
}
