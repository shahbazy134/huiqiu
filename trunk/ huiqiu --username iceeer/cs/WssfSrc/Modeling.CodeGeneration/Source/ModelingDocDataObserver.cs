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
using Microsoft.VisualStudio.Modeling.Shell;
using Microsoft.VisualStudio.Modeling;
using Microsoft.Practices.Modeling.ExtensionProvider.Extension;
using Microsoft.Practices.Modeling.ExtensionProvider.Helpers;
using Microsoft.Practices.RecipeFramework.Library;
using System.Diagnostics;
using Microsoft.Practices.Modeling.CodeGeneration.Artifacts;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Forms;
using Microsoft.VisualStudio.Modeling.Validation;
using Microsoft.VisualStudio.Shell.Interop;
using System.Collections.ObjectModel;

namespace Microsoft.Practices.Modeling.CodeGeneration
{
	public class ModelingDocDataObserver : IDisposable
	{
		private ModelingDocData docData;

		public ModelingDocDataObserver(ModelingDocData docData)
		{
			Guard.ArgumentNotNull(docData, "docData");
			this.docData = docData;
			this.docData.DocumentLoaded += new EventHandler(OnDocumentLoaded);
			this.docData.DocumentClosed += new EventHandler(OnDocumentClosed);
		}

		public bool CanSave<TModel>(ValidationController validationController, bool allowUserInterface, string errorLoadMessage, string errorSaveMessage) where TModel : ModelElement
		{
			Guard.ArgumentNotNull(validationController, "validationController");

			List<ModelElement> elementList = new List<ModelElement>();
			FullDepthElementWalker elementWalker = new FullDepthElementWalker(new ModelElementVisitor(elementList), new EmbeddingReferenceVisitorFilter(), false);

			TModel model = docData.Store.ElementDirectory.FindElements<TModel>()[0];
			elementWalker.DoTraverse(model);

			DialogResult result = DialogResult.Yes;

			if(allowUserInterface)
			{
				bool unloadableError = false;

				if(!validationController.Validate(elementList, ValidationCategories.Load) && validationController.ErrorMessages.Count != 0)
				{
					unloadableError = true;
				}
				if((!validationController.Validate(elementList, ValidationCategories.Save) && validationController.ErrorMessages.Count != 0) || unloadableError)
				{
					string errorMessage = (unloadableError ? errorLoadMessage : errorSaveMessage);

					result = PackageUtility.ShowMessageBox(
						docData.Store as IServiceProvider,
						errorMessage,
						OLEMSGBUTTON.OLEMSGBUTTON_YESNO,
						OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_SECOND,
						OLEMSGICON.OLEMSGICON_WARNING);
				}
			}

			return (result == DialogResult.Yes);
		}

		private void OnDocumentLoaded(object sender, EventArgs e)
		{
			IList<ModelElement> extenderRoots = new List<ModelElement>();

			DepthFirstElementWalker elementWalker = new DepthFirstElementWalker(
					new TypeMatchingElementVisitor<ITechnologyProvider>(extenderRoots),
					new EmbeddingVisitorFilter()
					);

			elementWalker.DoTraverse(docData.RootElement);

			foreach (ModelElement extenderRoot in extenderRoots)
			{
				IExtensionProvider provider = ((ITechnologyProvider)extenderRoot).ImplementationTechnology;

				if (provider == null)
					break;

				IList<ModelElement> extensibleObjects = new List<ModelElement>();

				DepthFirstElementWalker extensibleElementWalker = new DepthFirstElementWalker(
					new TypeMatchingElementVisitor<IExtensibleObject>(extensibleObjects),
					new EmbeddingVisitorFilter()
				);
				extensibleElementWalker.DoTraverse(extenderRoot);

				foreach (ModelElement extensibleObject in extensibleObjects)
				{
					ExtensionProviderHelper extensionProviderHelper = new ExtensionProviderHelper((IExtensibleObject)extensibleObject);
					extensionProviderHelper.SetObjectExtender(extensionProviderHelper.GetObjectExtender(provider));
				}
			}
			
			AdviseEvents(docData.RootElement.Store);
		}

		private object GetService(Type serviceType)
		{
			return docData.Store.GetService(serviceType);
		}

		private T GetService<T>()
		{
			return (T)GetService(typeof(T));
		}

		private void OnDocumentClosed(object sender, EventArgs e)
		{
			UnadviseEvents(docData.RootElement.Store);
		}

		private void OnElementDeleted(object sender, ElementDeletedEventArgs e)
		{
			IArtifactLinkContainer artifactContainer = ModelCollector.GetArtifacts(e.ModelElement);
			if(artifactContainer != null && artifactContainer.ArtifactLinks != null)
			{
				ICodeGenerationService codeGenerationService = GetService<ICodeGenerationService>();
				codeGenerationService.ValidateDeleteFromCollection(artifactContainer.ArtifactLinks);
			}
		}

		private void OnElementChanged(object sender, ElementPropertyChangedEventArgs e)
		{
			if(e.DomainProperty.DisplayName.Equals("Name", StringComparison.OrdinalIgnoreCase))
			{
				string oldName = e.OldValue.ToString();
				string newName = e.NewValue.ToString();
				IArtifactLinkContainer artifactContainer = ModelCollector.GetArtifacts(e.ModelElement);
				if(artifactContainer != null && artifactContainer.ArtifactLinks != null)
				{
					ICodeGenerationService codeGenerationService = GetService<ICodeGenerationService>();
					codeGenerationService.ValidateRenameFromCollection(artifactContainer.ArtifactLinks, newName, oldName);
				}
			}
		}

		private void AdviseEvents(Store store)
		{
			store.EventManagerDirectory.ElementDeleted.Add(new EventHandler<ElementDeletedEventArgs>(OnElementDeleted));
			store.EventManagerDirectory.ElementPropertyChanged.Add(new EventHandler<ElementPropertyChangedEventArgs>(OnElementChanged));
		}

		private void UnadviseEvents(Store store)
		{
			store.EventManagerDirectory.ElementDeleted.Remove(new EventHandler<ElementDeletedEventArgs>(OnElementDeleted));
			store.EventManagerDirectory.ElementPropertyChanged.Remove(new EventHandler<ElementPropertyChangedEventArgs>(OnElementChanged));
		}

		#region IDisposable

		private bool disposed;

		// Implement IDisposable.
		// Do not make this method virtual.
		// A derived class should not be able to override this method.
		public void Dispose()
		{
			Dispose(true);
			// Take yourself off the Finalization queue 
			// to prevent finalization code for this object
			// from executing a second time.
			GC.SuppressFinalize(this);
		}

		// Dispose(bool disposing) executes in two distinct scenarios.
		// If disposing equals true, the method has been called directly
		// or indirectly by a user's code. Managed and unmanaged resources
		// can be disposed.
		// If disposing equals false, the method has been called by the 
		// runtime from inside the finalizer and you should not reference 
		// other objects. Only unmanaged resources can be disposed.
		protected virtual void Dispose(bool disposing)
		{
			// Check to see if Dispose has already been called.
			if(!this.disposed)
			{
				// If disposing equals true, dispose all managed 
				// and unmanaged resources.
				if(disposing)
				{
					if(this.docData != null)
					{
						this.docData.DocumentLoaded -= new EventHandler(OnDocumentLoaded);
						this.docData.DocumentClosed -= new EventHandler(OnDocumentClosed);
						this.docData = null;
					}
				}
				// Release unmanaged resources. If disposing is false, 
				// only the following code is executed.

				// Note that this is not thread safe.
				// Another thread could start disposing the object
				// after the managed resources are disposed,
				// but before the disposed flag is set to true.
				// If thread safety is necessary, it must be
				// implemented by the client.

			}
			disposed = true;
		}

		#endregion
	}
}
