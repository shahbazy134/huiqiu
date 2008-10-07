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
using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
using Microsoft.Practices.ServiceFactory.RecipeFramework.Extensions.Properties;
using Microsoft.Practices.RecipeFramework.Library;
using Microsoft.Practices.RecipeFramework.Services;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell;

namespace Microsoft.Practices.ServiceFactory.RecipeFramework.Extensions
{
	/// <summary>
	/// Substitude for IConfigurationService Service when this is not available to the container.
	/// </summary>
	public class ConfigurationService
	{
		//	Private implementation of IServiceProvider that passes through to the
		//	global service provider. 
		//
		private class GlobalServiceProvider : IServiceProvider
		{
			#region IServiceProvider Members

			public object GetService(Type serviceType)
			{
				return Package.GetGlobalService(serviceType);
			}

			#endregion
		}

		private const string packageSubKey = @"Packages";
		private const string packageMenuSubKey = @"Menus";

		private IServiceProvider serviceProvider;
		private Guid packageId;

		/// <summary>
		/// Constructor.
		/// </summary>
		public ConfigurationService(Guid packageId)
			: this(packageId, new GlobalServiceProvider())
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <remarks>
		///	Used by unit tests to inject a mock service provider.
		/// </remarks>
		/// <param name="serviceProvider"></param>
		public ConfigurationService(Guid packageId, IServiceProvider serviceProvider)
		{
			if (serviceProvider == null)
				throw new InvalidOperationException(Resources.ServiceProviderIsNull);

			this.packageId = packageId;
			this.serviceProvider = serviceProvider;
		}

		/// <summary>
		/// Get an instance of the registry accessor.
		/// </summary>
		/// <remarks>
		/// Used by unit tests to provide a mock registry. Override this to return a MockRegistry.
		/// </remarks>
		/// <returns>Registry instance.</returns>
		protected virtual IAccessRegistry GetRegistryInstance()
		{
			return new RegistryAccessor();
		}

		/// <summary>
		/// Provide the same functionality as IConfigurationService.BasePath property.
		/// </summary>
		public string BasePath
		{
			get
			{
				if (serviceProvider == null)
					throw new InvalidOperationException(Resources.ServiceProviderIsNull);

				string basePath = String.Empty;
				basePath = InferBasePathFromRegistry();

				return basePath;
			}
		}

		/// <summary>
		/// Infer the guidance package base path from its registry settings.
		/// </summary>
		/// <returns>The full path to the guidance package install folder.</returns>
		private string InferBasePathFromRegistry()
		{
			string registryRoot = GetLocalRegistryRoot();
			string packageKey = Path.Combine(registryRoot, packageSubKey);
			packageKey = Path.Combine(packageKey, packageId.ToString("B"));

			string packageName = GetRegistryKeyValue(packageKey, String.Empty);
			if (String.IsNullOrEmpty(packageName))
				throw new InvalidOperationException(Resources.ConfigurationServiceGuidancePackageNotFound);

			string packageMenuKey = Path.Combine(registryRoot, packageMenuSubKey);

			string packagePath = GetRegistryKeyValue(packageMenuKey, packageName);
			if (String.IsNullOrEmpty(packagePath))
				throw new InvalidOperationException(Resources.ConfigurationServiceGuidancePackageNotFound);

			packagePath = Directory.GetParent(packagePath).FullName;
			if (!Directory.Exists(packagePath))
				throw new InvalidOperationException(Resources.ConfigurationServiceGuidancePackageNotFound);

			return packagePath;
		}

		/// <summary>
		/// Read a string value from the registry.
		/// </summary>
		/// <param name="registryKey">Key.</param>
		/// <param name="registryKey">Value name.</param>
		/// <returns>The registry data.</returns>
		private string GetRegistryKeyValue(string registryKey, string registryValueName)
		{
			Debug.Assert(!String.IsNullOrEmpty(registryKey));

			IAccessRegistry registry;

			using (registry = GetRegistryInstance())
			{
				registry.Open(Registry.LocalMachine, registryKey, false);
				return registry.GetValue(registryValueName, String.Empty);
			}
		}

		/// <summary>
		/// Get the registry path for the current hive root for the current instance of Visual studio.
		/// </summary>
		/// <returns>The registry path for the hive root.</returns>
		private string GetLocalRegistryRoot()
		{
			Debug.Assert(serviceProvider != null);

			string registryRoot = string.Empty;

			ILocalRegistry3 localRegistry = serviceProvider.GetService(typeof(SLocalRegistry)) as ILocalRegistry3;
			if (localRegistry != null)
			{
				localRegistry.GetLocalRegistryRoot(out registryRoot);
			}

			if (String.IsNullOrEmpty(registryRoot))
			{
				throw new InvalidOperationException(Resources.HiveRootNotFound);
			}
			return registryRoot;
		}
	}
}
