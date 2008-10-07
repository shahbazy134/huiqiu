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
using System.Windows.Forms;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Modeling.Shell;
using Microsoft.VisualStudio.Shell;
using System.ComponentModel.Design;
using System.Diagnostics;
using Microsoft.VisualStudio.Modeling;
using Microsoft.Practices.RecipeFramework.Services;
using Microsoft.Practices.RecipeFramework;
using System.Collections.Specialized;
using Microsoft.Practices.Modeling.CodeGeneration;
using Microsoft.Practices.Modeling.CodeGeneration.Artifacts;
using System.Collections;
using System.Windows.Forms.Design;
using Microsoft.VisualStudio.Modeling.Validation;
using Microsoft.Practices.Modeling.CodeGeneration.Logging;
using System.Globalization;

namespace Microsoft.Practices.ServiceFactory.HostDesigner
{
	internal partial class HostDesignerDocView : HostDesignerDocViewBase
	{
		private const string ServicesGuigancePackageName = "ServicesGuidancePackage";
		private static readonly Guid AsmxImplementationTechnologyID = new Guid("FF107115-D18B-4F82-9B82-D6F8C77B8693");
		private static readonly Guid WcfImplementationTechnologyID = new Guid("44E8A8C3-3651-4932-BAE3-1FAF3684E2F3");
		private HostDesignerUserControl designerControl = null;
		private GuidancePackage servicesGuidancePackage = null;
		private ExplorerSynchronization explorerSynchronization = null;
		private EventHandler<SelectedExplorerNodeChangedEventArgs> selectedExplorerNodeChanged = null;
		private ModelViewValidationState validationState = ModelViewValidationState.Uncertain;
		private EventHandler<TransactionCommitEventArgs> transactionCommitHandler = null;
		private IValidationControllerAccesor validationControllerAccess;
		
		protected override void Initialize()
		{
			base.Initialize();

			explorerSynchronization = this.GetService(typeof(ExplorerSynchronization)) as ExplorerSynchronization;
			if (explorerSynchronization != null)
			{
				selectedExplorerNodeChanged = new EventHandler<SelectedExplorerNodeChangedEventArgs>(explorerSynchronization_SelectedExplorerNodeChanged);
				explorerSynchronization.SelectedExplorerNodeChanged += selectedExplorerNodeChanged;
			}
		}

		void explorerSynchronization_SelectedExplorerNodeChanged(object sender, SelectedExplorerNodeChangedEventArgs e)
		{
			SelectedElements.Clear();
			SelectedElements.Add(e.SelectedElement);
		}

		protected override bool LoadView()
		{
			EnsureWindowVisible(StandardToolWindows.PropertyBrowser);
			EnsureWindowVisible(new Guid(Constants.HostDesignerModelExplorerToolWindowId));
			
			if (base.LoadView())
			{
				transactionCommitHandler = new EventHandler<TransactionCommitEventArgs>(delegate(object sender, TransactionCommitEventArgs args)
				{
					this.validationState = ModelViewValidationState.Uncertain;
				});

				
				DocData.Store.EventManagerDirectory.TransactionCommitted.Add(transactionCommitHandler);
				validationControllerAccess = DocData as IValidationControllerAccesor;

				designerControl.Initialize(this);

				
				return LoadGuidancePackage();
			}
			

			
			return false;
		}

		private bool LoadGuidancePackage()
		{
			IRecipeManagerService manager = (IRecipeManagerService)this.DocData.Store.GetService(typeof(IRecipeManagerService));
			Debug.Assert(manager != null, "Unable to get IRecipeManagerService service");
			if (manager == null)
			{
				return false;
			}

			servicesGuidancePackage = manager.GetPackage(ServicesGuigancePackageName);
			if (servicesGuidancePackage == null)
			{
				try
				{
					manager.EnablePackage(ServicesGuigancePackageName);
				}
				catch(ActionExecutionException executionException)
				{
					IUIService uiService = this.GetService(typeof(IUIService)) as IUIService;
					if (uiService != null)
					{
						uiService.ShowError(executionException, 
							String.Format(CultureInfo.CurrentCulture, Resources.GuidancePackageLoedFailure, ServicesGuigancePackageName));
					}
				}
				servicesGuidancePackage = manager.GetPackage(ServicesGuigancePackageName);
			}

			if (servicesGuidancePackage == null)
			{
				return false;
			}
				
			return true;
		}

		public override IWin32Window Window
		{
			get
			{
				if (designerControl == null)
					designerControl = new HostDesignerUserControl();
				return designerControl;
			}
		}

		internal IValidationControllerAccesor ValidationControllerAccess
		{
			get { return validationControllerAccess; }
		}
		

		internal void EnsureWindowVisible(Guid guid)
		{
			IVsWindowFrame frame = null;

			IVsUIShell service = this.ServiceProvider.GetService(typeof(IVsUIShell)) as IVsUIShell;
			if (service != null)
			{
				ErrorHandler.ThrowOnFailure(service.FindToolWindow(0x80000, ref guid, out frame));
			}
			if (frame != null)
			{
				frame.Show();
			}
		}

		protected override void Dispose(bool disposing)
		{
			try
			{
				if (disposing)
				{
					if (explorerSynchronization != null && 
						selectedExplorerNodeChanged != null)
					{
						explorerSynchronization.SelectedExplorerNodeChanged -= selectedExplorerNodeChanged;
					}
				}
			}
			finally
			{
				base.Dispose(disposing);
			}
		}


		#region  Execute XXX
		internal void ExecuteGenerateProxy(ModelElement modelElement)
		{
			Proxy proxy = modelElement as Proxy;
			if (proxy != null && proxy.ClientApplication.ImplementationTechnology != null)
			{
				Guid implementationTechnology = proxy.ClientApplication.ImplementationTechnology.Id;
				if (implementationTechnology == AsmxImplementationTechnologyID)
				{
					ExecuteRecipe("CreateASMXClientProxy", new HybridDictionary());
				} 
				else if (implementationTechnology == WcfImplementationTechnologyID)
				{
					ExecuteRecipe("CreateWCFClientProxy", new HybridDictionary());
				} 
				else 
				{
					Debug.Assert(false, "Unknown implementation technology. action should be added here.");
				}
			}
		}
		
		internal void ExecuteCodeGeneration(ModelElement element)
		{
			HybridDictionary arguments = new HybridDictionary();
			arguments.Add("SelectedElement", element);
		
			ExecuteRecipe("GenerateCode", arguments);
		}
		
		private void ExecuteRecipe(string recipeName, IDictionary arguments)
		{
			try
			{
				servicesGuidancePackage.Execute(recipeName, new HybridDictionary());
			}
			catch (Exception ex)
			{
				Logger.Write(ex.InnerException ?? ex, 
					String.Format(CultureInfo.CurrentCulture, Resources.RecipeExecutionError, recipeName));
			}
		}

		internal void ExecuteValidateModelAndUpdateTaskWindow()
		{
			if (validationControllerAccess == null || validationControllerAccess.Controller == null) throw new InvalidOperationException("Model does not support validation");

			bool allElementsValid = validationControllerAccess.Controller.Validate(
				DocData.Store.ElementDirectory.AllElements, ValidationCategories.Menu);

			validationState = allElementsValid ? ModelViewValidationState.Valid : ModelViewValidationState.Invalid;
		}
		
		#endregion

		#region CanGenenrate
		internal bool CanGenerateCode(ModelElement element)
		{
			return typeof(ServiceReference).IsAssignableFrom(element.GetType());
		}
		
		internal bool CanGenerateProxy(ModelElement modelElement)
		{
			return typeof(Proxy).IsAssignableFrom(modelElement.GetType());
		}
		
		internal bool IsModelValid()
		{
			if (validationState == ModelViewValidationState.Uncertain)
			{
				//we should perform validation
				VsValidationController temporaryValidationController = new VsValidationController(this.ServiceProvider, typeof(HostDesignerExplorerToolWindow));
				bool allElementsValid = temporaryValidationController.Validate(DocData.Store.ElementDirectory.AllElements, Microsoft.VisualStudio.Modeling.Validation.ValidationCategories.Menu);
				
				validationState = allElementsValid? ModelViewValidationState.Valid : ModelViewValidationState.Invalid;
			}
			
			return validationState == ModelViewValidationState.Valid;
		}

		internal bool IsElementValidForCodeGen(ModelElement modelElement)
		{
			IArtifactLinkContainer container = ModelCollector.GetArtifacts(modelElement);
			if (container == null || container.ArtifactLinks == null || container.ArtifactLinks.Count == 0) return false;
			return IsModelValid();
		}

		internal bool IsElementValidForProxyGen(ModelElement modelElement)
		{
			Proxy proxy = modelElement as Proxy;
			if (proxy == null) return false;
			if (proxy.Endpoint == null) return false;
			if (proxy.ClientApplication.ImplementationTechnology == null) return false;
			if (proxy.ClientApplication.ImplementationProject == null) return false;

			return IsModelValid();
		}
		
		#endregion
		
		private enum ModelViewValidationState
		{
			Uncertain,
			Valid,
			Invalid
		}

		internal void SetValidationStateDirect(bool valid)
		{
			validationState = valid ? ModelViewValidationState.Valid : ModelViewValidationState.Invalid;
		}
	}
}
