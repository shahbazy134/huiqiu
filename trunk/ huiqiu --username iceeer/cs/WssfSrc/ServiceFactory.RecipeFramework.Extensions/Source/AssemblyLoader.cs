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
using System.Reflection;
using System.IO;
using System.Diagnostics;
using Microsoft.Practices.RecipeFramework.Library;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Microsoft.Practices.VisualStudio.Helper;

namespace Microsoft.Practices.ServiceFactory.RecipeFramework.Extensions
{
	/// <summary>
	/// Load assemblies in the guidance package \Lib folder and use an AppDomain AssemblyResolve
	/// event handler to return assemblies loaded from this folder.
	/// </summary>
	/// <remarks>
	/// This AssemblyLoader must be initialized by calling LoadAll before the guidance package 
	/// attempts to load any of the assemblies managed by the assembly loader.
	/// </remarks>
	public static class AssemblyLoader
	{
		private static bool loaded = false;
		private static IList<Assembly> loadedAssemblies = new List<Assembly>();
		private static object sync = new object();

		/// <summary>
		/// Load a list of assemblies and sink AssemblyResolve event.
		/// </summary>
		public static void LoadAll(string basePath)
		{
			Guard.ArgumentNotNullOrEmptyString(basePath, "basePath");

			lock (sync)
			{
				if (loaded)
					return;
				loaded = true;
			}

			basePath = Path.Combine(basePath, "Lib");

			string[] assemblyFilenames = Directory.GetFiles(basePath, "*.dll");

			foreach (string filename in assemblyFilenames)
			{
				string path = Path.Combine(basePath, filename);
				if (!File.Exists(path))
				{
					string message = String.Format(CultureInfo.CurrentUICulture, Properties.Resources.AssemblyNotFound, filename);
					Debug.WriteLine(message);
					throw new FileNotFoundException(message, path);
				}
				try
				{
					loadedAssemblies.Add(Assembly.LoadFrom(path));
				}
				catch (BadImageFormatException) { }		// Not a .NET DLL.
				catch (FileLoadException) { }			// Not a .NET DLL.
			}

			AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);
		}

		public static IList<Type> GetTypesByInterface(Type interfaceType)
		{
			List<Type> matches = new List<Type>();
			foreach (Assembly assembly in loadedAssemblies)
			{
				try
				{
					List<Type> assemblyTypes = new List<Type>(assembly.GetTypes());
					matches.AddRange(ReflectionHelper.GetTypesByInterface(assemblyTypes, interfaceType));
				}
				catch (ReflectionTypeLoadException) // Bad .NET DLL with missing dependencies
				{
					continue;
				}
			}

			return matches;
		}

		public static IList<Assembly> LoadedAssemblies
		{
			get
			{
				return loadedAssemblies;
			}
		}

		/// <summary>
		/// Marks the cache as not-loaded, primarily for unit testing.    
		/// </summary>
		public static void ResetCache()
		{
			loaded = false;
			loadedAssemblies = new List<Assembly>();
		}

		/// <summary>
		/// Attempt to resolve assemblies using the AssemblyResolver to look at loaded assemblies.
		/// </summary>
		/// <returns>Resolved assembly.</returns>
		/// <remarks>
		/// <seealso cref="http://msdn2.microsoft.com/en-us/library/system.appdomain.assemblyresolve.aspx"/>	
		/// <seealso cref="http://codeidol.com/csharp/net-framework/Assemblies,-Loading,-and-Deployment/Assembly-Loading/"/>	
		/// <seealso cref="http://blogs.msdn.com/suzcook/archive/2003/05/29/57143.aspx"/>
		/// </remarks>
		private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
		{
			Guard.ArgumentNotNull(args, "args");

			AssemblyResolver resolver = new AssemblyResolver();

			Assembly resolvedAssembly = resolver.Resolve(args.Name);
			Debug.WriteLineIf((resolvedAssembly != null), "Resolved assembly " + args.Name);

			return resolvedAssembly;
		}
	}
}
