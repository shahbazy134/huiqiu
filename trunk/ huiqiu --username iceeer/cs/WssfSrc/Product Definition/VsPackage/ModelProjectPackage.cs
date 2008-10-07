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

using Microsoft.Practices.Modeling.CodeGeneration;
using Microsoft.Practices.Modeling.Common;
using Microsoft.Practices.Modeling.ExtensionProvider.Services;
using Microsoft.Practices.ServiceFactory.RecipeFramework.Extensions;
using Microsoft.Practices.ServiceFactory.RecipeFramework.Extensions.ProjectMapping;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Package;

namespace Microsoft.Practices.ServiceFactory.VsPkg
{
	/// A package load key is required to allow this package to load when the Visual Studio SDK is not installed.
	/// Package load keys may be obtained from http://msdn.microsoft.com/vstudio/extend.
	/// Consult the Visual Studio SDK documentation for more information.
    // [ProvideLoadKey("Standard", "3.1.0.0", "Web Service Software Factory: Modeling Edition", "Microsoft PLK", 1)]
	[ProvideProjectFactory(
		typeof(ModelProject.ModelProjectFactory),
	    "Service Factory : Modeling Edition", "Model Project(*.ssfproduct);*.ssfproduct",
		"ssfproduct", "ssfproduct", @"Templates\Projects.Cache",
		LanguageVsTemplate = "ServicesSoftwareFactoryProduct",
	    FolderGuid = "{DCF2A94A-45B0-11D1-ADBF-00C04FB6BE4C}")]
    [InstalledProductRegistration(false, "#110", "#112", "1.0", IconResourceID = 400)]
    [ProvideAutoLoad("ADFC4E64-0397-11D1-9F4E-00A0C911004F")]
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [DefaultRegistryRoot("Software\\Microsoft\\VisualStudio\\9.0")]
    [ProvideMenuResource(1000, 1)]
    [Guid(GuidList.guidWebServicesFactoryPkgString)]
	[ProvideService(typeof(ICodeGenerationService), ServiceName = "CodeGenerationService")]
	[ProvideService(typeof(IExtensionProviderService), ServiceName = "ExtensionProviderService")]
	public sealed class ModelProjectPackage : ProjectPackage, IDisposable
	{
		ProjectMappingManagerMonitor monitor;
		private bool disposed;

		public ModelProjectPackage()
		{
		}

		// FXCOP: False positive, methods have different generic signatures
		[SuppressMessage("Microsoft.Usage", "CA2223:MembersShouldDifferByMoreThanReturnType")]
		public T GetService<T>()
		{
			return (T)GetService(typeof(T));
		}

		// FXCOP: False positive, methods have different generic signatures
		[SuppressMessage("Microsoft.Usage", "CA2223:MembersShouldDifferByMoreThanReturnType")]
		public TInterface GetService<TInterface, TService>()
		{
			return (TInterface)GetService(typeof(TService));
		}

		protected override void Initialize()
		{
			base.Initialize();

            this.RegisterProjectFactory(new ModelProject.ModelProjectFactory(this));

			LoadAssemblies();
			InitializeServices();
			InitializePMM();
		}

		private void LoadAssemblies()
		{
			string basePath = 
				new ConfigurationService(
					ServiceFactoryGuidancePackage.PackageGuid,
					this).BasePath;

			//	Initialize the AssemblyLoader. This covers the case where the user opens an existing solution.
			//	In this scenario the model project packages gets loaded before the guidance package. The
			//	AssemblyLoader must be initialized before anything attempts to resolve the assemblies it loads.
			//
			AssemblyLoader.LoadAll(basePath);
		}

		private void InitializeServices()
		{
			IServiceContainer container = (IServiceContainer)this;
			ServiceCreatorCallback callback = new ServiceCreatorCallback(OnCreateService);
			container.AddService(typeof(ICodeGenerationService), callback, true);
			container.AddService(typeof(IExtensionProviderService), callback, true);
		}

		private void InitializePMM()
		{
			ProjectMappingManager.Initialize(this);
			monitor = new ProjectMappingManagerMonitor(this, this.GetService<IVsSolution>());
		}

		private object OnCreateService(IServiceContainer container, Type serviceType)
		{
			System.IServiceProvider serviceProvider = this as System.IServiceProvider;
			object serviceInstance = null;

			if(serviceType == typeof(ICodeGenerationService))
			{
				serviceInstance = new CodeGenerationService(serviceProvider);
			}
			else if(serviceType == typeof(IExtensionProviderService))
			{
				serviceInstance = new ExtensionProviderService();
			}

			if(serviceInstance is IComponent)
			{
				((IComponent)serviceInstance).Site = new Microsoft.Practices.ComponentModel.Site(
					serviceProvider,
					(IComponent)serviceInstance,
					serviceInstance.GetType().Name);
			}

			return serviceInstance;
		}

		#region IDisposable Members

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected override void Dispose(bool disposing)
		{
			try
			{
				if(!this.disposed)
				{
					if(disposing)
					{
						monitor.Dispose();
					}
				}
			}
			finally
			{
				base.Dispose(disposing);
				disposed = true;
			}
		}

		~ModelProjectPackage()
		{
			Dispose(false);
		}
		#endregion
	}
}