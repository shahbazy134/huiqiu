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


//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using DslShell = global::Microsoft.VisualStudio.Modeling.Shell;
using DslModeling = global::Microsoft.VisualStudio.Modeling;
using DslValidation = global::Microsoft.VisualStudio.Modeling.Validation;
using DslDiagrams = global::Microsoft.VisualStudio.Modeling.Diagrams;
using VSShellInterop = global::Microsoft.VisualStudio.Shell.Interop;


namespace Microsoft.Practices.ServiceFactory.DataContracts
{
	/// <summary>
	/// Double-derived class to allow easier code customization.
	/// </summary>
	internal partial class DataContractDslDocData : DataContractDslDocDataBase
	{
		/// <summary>
		/// Constructs a new DataContractDslDocData.
		/// </summary>
		public DataContractDslDocData(global::System.IServiceProvider serviceProvider, global::System.Guid editorFactoryId) 
			: base(serviceProvider, editorFactoryId)
		{
		}
	}

	/// <summary>
	/// Class which represents a DataContractDsl document in memory.
	/// </summary>
	internal abstract class DataContractDslDocDataBase : DslShell::ModelingDocData
	{

		#region Constraint ValidationController
		/// <summary>
		/// The controller for all validation that goes on in the package.
		/// </summary>
		private DslShell::VsValidationController validationController;
		private DslShell::ErrorListObserver errorListObserver;
		#endregion
		/// <summary>
		/// Document lock holder registered for the subordinate .diagram file.
		/// </summary>
		private DslShell::SubordinateDocumentLockHolder diagramDocumentLockHolder;

		/// <summary>
		/// Constructs a new DataContractDslDocDataBase.
		/// </summary>
		protected DataContractDslDocDataBase(global::System.IServiceProvider serviceProvider, global::System.Guid editorFactoryId) : base(serviceProvider, editorFactoryId)
		{
		}

		/// <summary>
		/// Returns a list of file format specifiers for the Save dialog box.
		/// </summary>
		protected override string FormatList
		{
			get
			{
                return global::Microsoft.Practices.ServiceFactory.DataContracts.DataContractDslDomainModel.SingletonResourceManager.GetString("FormatList"); 
			}
		}


		/// <summary>
		/// The controller for all validation that goes on in the package.
		/// </summary>
		public DslShell::VsValidationController ValidationController
		{
			get
			{
				if (this.validationController == null)
				{
					this.validationController = this.CreateValidationController();
					this.errorListObserver = new DslShell::ErrorListObserver(this.ServiceProvider);

					// register the observer so we can show the error/warning/msg in the VS output window.
					this.validationController.AddObserver(this.errorListObserver);
				}
				return this.validationController;
			}
		}

		/// <summary>
		/// Factory method to create a VSValidationController.
		/// </summary>
		protected virtual DslShell::VsValidationController CreateValidationController()
		{
			return new DslShell::VsValidationController(this.ServiceProvider, typeof(DataContractDslExplorerToolWindow));
		}
		
		/// <summary>
		/// When the doc data is closed, make sure we reset the valiation messages 
		/// (if there's any) from the ErrorList window.
		/// </summary>
		/// <param name="disposing"></param>
		protected override void Dispose(bool disposing)
		{
			try
			{
				if (this.validationController != null)
				{
					this.validationController.ClearMessages();
					// un-register our observer with the controller.
					this.validationController.RemoveObserver(this.errorListObserver);
					this.validationController = null;
					if ( this.errorListObserver != null )
					{
						this.errorListObserver.Dispose();
						this.errorListObserver = null;
					}
				}
				if (this.diagramDocumentLockHolder != null)
				{
					this.diagramDocumentLockHolder.Dispose();
					this.diagramDocumentLockHolder = null;
				}
			}
			finally
			{
				base.Dispose(disposing);
			}
		}

		/// <summary>
		/// Returns a collection of domain models to load into the store.
		/// </summary>
		protected override global::System.Collections.Generic.IList<global::System.Type> GetDomainModels()
		{
			return new global::System.Type[]
			{
				typeof(DslDiagrams::CoreDesignSurfaceDomainModel),
				typeof(global::Microsoft.Practices.ServiceFactory.DataContracts.DataContractDslDomainModel)
			};
		}

		/// <summary>
		/// Loads the given file.
		/// </summary>
		protected override void Load(string fileName, bool isReload)
		{
			DslModeling::SerializationResult serializationResult = new DslModeling::SerializationResult();
			global::Microsoft.Practices.ServiceFactory.DataContracts.DataContractModel modelRoot = null;
			DslModeling::ISchemaResolver schemaResolver = new DslShell::ModelingSchemaResolver(this.ServiceProvider);
			
			// Enable diagram fixup rules in our store, because we will load diagram data.
			global::Microsoft.Practices.ServiceFactory.DataContracts.DataContractDslDomainModel.EnableDiagramRules(this.Store);
			string diagramFileName = fileName + this.DiagramExtension;
			
			modelRoot = global::Microsoft.Practices.ServiceFactory.DataContracts.DataContractDslSerializationHelper.Instance.LoadModelAndDiagram(serializationResult, this.Store, fileName, diagramFileName, schemaResolver, this.ValidationController);

			// Report serialization messages.
			this.SuspendErrorListRefresh();
			try
			{
				foreach (DslModeling::SerializationMessage serializationMessage in serializationResult)
				{
					this.AddErrorListItem(new DslShell::SerializationErrorListItem(this.ServiceProvider, serializationMessage));
				}
			}
			finally
			{
				this.ResumeErrorListRefresh();
			}

			if (serializationResult.Failed)
			{	
				// Load failed, can't open the file.
				throw new global::System.InvalidOperationException(global::Microsoft.Practices.ServiceFactory.DataContracts.DataContractDslDomainModel.SingletonResourceManager.GetString("CannotOpenDocument"));
			}
			else
			{
				this.SetRootElement(modelRoot);
				if (this.Hierarchy != null && global::System.IO.File.Exists(diagramFileName))
				{
					// Add a lock to the subordinate diagram file.
					if (this.diagramDocumentLockHolder == null)
					{
						uint itemId = DslShell::SubordinateFileHelper.GetChildProjectItemId(this.Hierarchy, this.ItemId, this.DiagramExtension);
						if (itemId != global::Microsoft.VisualStudio.VSConstants.VSITEMID_NIL)
						{
							this.diagramDocumentLockHolder = DslShell::SubordinateFileHelper.LockSubordinateDocument(this.ServiceProvider, this, diagramFileName, itemId);
							if (this.diagramDocumentLockHolder == null)
							{
								throw new global::System.InvalidOperationException(string.Format(global::System.Globalization.CultureInfo.CurrentCulture,
													global::Microsoft.Practices.ServiceFactory.DataContracts.DataContractDslDomainModel.SingletonResourceManager.GetString("CannotCloseExistingDiagramDocument"),
													diagramFileName));
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// Called after the document is opened.
		/// </summary>
		/// <param name="e">Event Args.</param>
		protected override void OnDocumentLoaded(global::System.EventArgs e)
		{
			base.OnDocumentLoaded(e);
			this.OnDocumentLoaded();
		}

		/// <summary>
		/// Called after the document is reloaded.
		/// </summary>
		protected override void OnDocumentReloaded(global::System.EventArgs e)
		{
			base.OnDocumentReloaded(e);
			this.OnDocumentLoaded();
		}
		
		/// <summary>
		/// Called on both document load and reload.
		/// </summary>
		protected virtual void OnDocumentLoaded()
		{

			// Validate the document
			this.ValidationController.Validate(this.Store, DslValidation::ValidationCategories.Open);

			// Enable CompartmentItems events.
			global::Microsoft.Practices.ServiceFactory.DataContracts.DataContractModel modelRoot = this.RootElement as global::Microsoft.Practices.ServiceFactory.DataContracts.DataContractModel;
			if (modelRoot != null)
			{
				global::System.Collections.Generic.IList<DslDiagrams::PresentationElement> diagrams = DslDiagrams::PresentationViewsSubject.GetPresentation(modelRoot);
				if (diagrams.Count > 0)
				{
					global::Microsoft.Practices.ServiceFactory.DataContracts.DataContractDiagram diagram = diagrams[0] as global::Microsoft.Practices.ServiceFactory.DataContracts.DataContractDiagram;
					if (diagram != null)
					{
						diagram.SubscribeCompartmentItemsEvents();
					}
				}
			}
		}


		/// <summary>
		/// Validate the model before the file is saved.
		/// </summary>
		protected override bool CanSave(bool allowUserInterface)
		{
			// If a silent check then use a temporary ValidationController that is not connected to the error list to avoid any unwanted UI updates
			DslShell::VsValidationController vc = allowUserInterface ? this.ValidationController : this.CreateValidationController();
			if (vc == null)
			{
				return true;
			}

			// We check Load category first, because any violation in this category will cause the saved file to be unloadable justifying a special 
			// error message. If the Load category passes, we then check the normal Save category, and give the normal warning message if necessary.
			bool unloadableError = !vc.Validate(this.Store, DslValidation::ValidationCategories.Load) && vc.ErrorMessages.Count != 0;
			
			// Prompt user for confirmation if there are validation errors and this is not a silent save
			if (allowUserInterface)
			{
				vc.Validate(this.Store, DslValidation::ValidationCategories.Save);

				if (vc.ErrorMessages.Count != 0)
				{
					string errorMsg = (unloadableError ? "UnloadableSaveValidationFailed" : "SaveValidationFailed");
					global::System.Windows.Forms.DialogResult result = DslShell::PackageUtility.ShowMessageBox(this.ServiceProvider, global::Microsoft.Practices.ServiceFactory.DataContracts.DataContractDslDomainModel.SingletonResourceManager.GetString(errorMsg), VSShellInterop::OLEMSGBUTTON.OLEMSGBUTTON_YESNO, VSShellInterop::OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_SECOND, VSShellInterop::OLEMSGICON.OLEMSGICON_WARNING);
					return (result == global::System.Windows.Forms.DialogResult.Yes);
				}
			}
			
			return !unloadableError;
		}

			
		/// <summary>
		/// Handle when document has been saved
		/// </summary>
		/// <param name="e"></param>
		protected override void OnDocumentSaved(global::System.EventArgs e)
		{
			base.OnDocumentSaved(e);

			// Notify the Running Document Table that the subordinate has been saved
			// If this was a SaveAs, then let the subordinate document do this notification itself.
			// Otherwise VS will never ask the subordinate to save itself.
			DslShell::DocumentSavedEventArgs savedEventArgs = e as DslShell::DocumentSavedEventArgs;
			if (savedEventArgs != null && this.ServiceProvider != null)
			{
				if ( global::System.StringComparer.OrdinalIgnoreCase.Compare(savedEventArgs.OldFileName, savedEventArgs.NewFileName) == 0)
				{
					VSShellInterop::IVsRunningDocumentTable rdt = (VSShellInterop.IVsRunningDocumentTable)this.ServiceProvider.GetService(typeof(VSShellInterop::IVsRunningDocumentTable));
					if (rdt != null && this.diagramDocumentLockHolder != null && this.diagramDocumentLockHolder.SubordinateDocData != null)
					{
						global::Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(rdt.NotifyOnAfterSave(this.diagramDocumentLockHolder.SubordinateDocData.Cookie));
					}
				}
			}
		}

		/// <summary>
		/// Saves the given file.
		/// </summary>
		protected override void Save(string fileName)
		{
			DslModeling::SerializationResult serializationResult = new DslModeling::SerializationResult();
			global::Microsoft.Practices.ServiceFactory.DataContracts.DataContractModel modelRoot = (global::Microsoft.Practices.ServiceFactory.DataContracts.DataContractModel)this.RootElement;

			
			// Only save the diagrams if
			// a) There are any to save
			// b) This is NOT a SaveAs operation.  SaveAs should allow the subordinate document to control the save of its data as it is writing a new file.
			//    Except DO save the diagram on SaveAs if there isn't currently a diagram as there won't be a subordinate document yet to save it.

			bool saveAs = global::System.StringComparer.OrdinalIgnoreCase.Compare(fileName, this.FileName) != 0;

			global::System.Collections.Generic.IList<DslDiagrams::PresentationElement> diagrams = DslDiagrams::PresentationViewsSubject.GetPresentation(this.RootElement);
			if (diagrams.Count > 0 && (!saveAs || this.diagramDocumentLockHolder == null))
			{
				global::Microsoft.Practices.ServiceFactory.DataContracts.DataContractDiagram diagram = diagrams[0] as global::Microsoft.Practices.ServiceFactory.DataContracts.DataContractDiagram;
				if (diagram != null)
				{
					string diagramFileName = fileName + this.DiagramExtension;
					try
					{
						this.SuspendFileChangeNotification(diagramFileName);
						
						global::Microsoft.Practices.ServiceFactory.DataContracts.DataContractDslSerializationHelper.Instance.SaveModelAndDiagram(serializationResult, modelRoot, fileName, diagram, diagramFileName, this.Encoding, false);
					}
					finally
					{
						this.ResumeFileChangeNotification(diagramFileName);
					}
				}
			}
			else
			{
				global::Microsoft.Practices.ServiceFactory.DataContracts.DataContractDslSerializationHelper.Instance.SaveModel(serializationResult, modelRoot, fileName, this.Encoding, false);
			}
			// Report serialization messages.
			this.SuspendErrorListRefresh();
			try
			{
				foreach (DslModeling::SerializationMessage serializationMessage in serializationResult)
				{
					this.AddErrorListItem(new DslShell::SerializationErrorListItem(this.ServiceProvider, serializationMessage));
				}
			}
			finally
			{
				this.ResumeErrorListRefresh();
			}

			if (serializationResult.Failed)
			{	// Save failed.
				throw new global::System.InvalidOperationException(global::Microsoft.Practices.ServiceFactory.DataContracts.DataContractDslDomainModel.SingletonResourceManager.GetString("CannotSaveDocument"));
			}
		}
		/// <summary>
		/// Mark that the document has changed and thus a new backup should be created
		/// </summary>
		/// <remarks>
		/// Call this method when you change the document's content
		/// </remarks>
		public override void MarkDocumentChangedForBackup()
		{
			base.MarkDocumentChangedForBackup();

			// Also mark the subordinate document as changed
			if (this.diagramDocumentLockHolder != null &&
				this.diagramDocumentLockHolder.SubordinateDocData != null)
			{
				this.diagramDocumentLockHolder.SubordinateDocData.MarkDocumentChangedForBackup();
			}
		}
		
		#region Diagram file management
		
		/// <summary>
		/// Save the given document that is subordinate to this document.
		/// </summary>
		/// <param name="subordinateDocument"></param>
		/// <param name="fileName"></param>
		protected override void SaveSubordinateFile(DslShell::DocData subordinateDocument, string fileName)
		{
			// In this case, the only subordinate is the diagram.
			DslModeling::SerializationResult serializationResult = new DslModeling::SerializationResult();

			global::System.Collections.Generic.IList<DslDiagrams::PresentationElement> diagrams = DslDiagrams::PresentationViewsSubject.GetPresentation(this.RootElement);
			if (diagrams.Count > 0)
			{
				global::Microsoft.Practices.ServiceFactory.DataContracts.DataContractDiagram diagram = diagrams[0] as global::Microsoft.Practices.ServiceFactory.DataContracts.DataContractDiagram;
				if (diagram != null)
				{
					try
					{
						this.SuspendFileChangeNotification(fileName);
						
						global::Microsoft.Practices.ServiceFactory.DataContracts.DataContractDslSerializationHelper.Instance.SaveDiagram(serializationResult, diagram, fileName, this.Encoding, false);
					}
					finally
					{
						this.ResumeFileChangeNotification(fileName);
					}
				}
			}
			// Report serialization messages.
			this.SuspendErrorListRefresh();
			try
			{
				foreach (DslModeling::SerializationMessage serializationMessage in serializationResult)
				{
					this.AddErrorListItem(new DslShell::SerializationErrorListItem(this.ServiceProvider, serializationMessage));
				}
			}
			finally
			{
				this.ResumeErrorListRefresh();
			}
			
			if (!serializationResult.Failed)
			{
				// Notify the Running Document Table that the subordinate has been saved
				if (this.ServiceProvider != null)
				{
					VSShellInterop::IVsRunningDocumentTable rdt = (VSShellInterop.IVsRunningDocumentTable)this.ServiceProvider.GetService(typeof(VSShellInterop::IVsRunningDocumentTable));
					if (rdt != null && this.diagramDocumentLockHolder != null && this.diagramDocumentLockHolder.SubordinateDocData != null)
					{
						global::Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(rdt.NotifyOnAfterSave(this.diagramDocumentLockHolder.SubordinateDocData.Cookie));
					}
				}
			}
			else
			{	
				// Save failed.
				throw new global::System.InvalidOperationException(global::Microsoft.Practices.ServiceFactory.DataContracts.DataContractDslDomainModel.SingletonResourceManager.GetString("CannotSaveDocument"));
			}						
		}
		
		/// <summary>
		/// Provide a suffix for the diagram file
		/// </summary>
		protected virtual string DiagramExtension
		{
			get
			{
				return Constants.DefaultDiagramExtension;
			}
		}
		#endregion
		
		#region Base virtual overrides
		
		/// <summary>
		/// Return the model in XML format
		/// </summary>
		protected override string SerializedModel
		{
			get
			{
				global::Microsoft.Practices.ServiceFactory.DataContracts.DataContractModel modelRoot = this.RootElement as global::Microsoft.Practices.ServiceFactory.DataContracts.DataContractModel;
				string modelFile = string.Empty;
				if (modelRoot != null)
				{
					modelFile = global::Microsoft.Practices.ServiceFactory.DataContracts.DataContractDslSerializationHelper.Instance.GetSerializedModelString(modelRoot, this.Encoding);
				}
				return modelFile;
			}
		}
		#endregion
	}
}
