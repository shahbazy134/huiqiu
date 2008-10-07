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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Microsoft.VisualStudio.Modeling;
using System.Diagnostics;
using Microsoft.VisualStudio.Modeling.Diagrams;
using System.Resources;
using Microsoft.VisualStudio.Modeling.Validation;

namespace Microsoft.Practices.ServiceFactory.HostDesigner
{
	public partial class HostDesignerUserControl : UserControl, IDisposable
	{
		private HostDesignerDocView documentView;
		private ModelElement currentElement;
		private ExplorerSynchronization explorerSynchronization;
		private EventHandler<SelectedExplorerNodeChangedEventArgs> selectedExplorerNodeChanged;
		private HostDesignerValidationMessageObserverSync validationSync;
		
		public HostDesignerUserControl()
		{
			InitializeComponent();
		}

		internal void Initialize(HostDesignerDocView view)
		{
			Debug.Assert(view != null);

			documentView = view;

			//	Sync with store to detect model changes
			documentView.DocData.Store.EventManagerDirectory.ElementPropertyChanged.Add(
				new EventHandler<ElementPropertyChangedEventArgs>(delegate(object o, ElementPropertyChangedEventArgs args)
				{
					this.currentElement = args.ModelElement;
					UpdateDesigner();
				}));

			explorerSynchronization = view.DocData.Store.GetService(typeof(ExplorerSynchronization)) as ExplorerSynchronization;
			if (explorerSynchronization != null)
			{
				selectedExplorerNodeChanged = new EventHandler<SelectedExplorerNodeChangedEventArgs>(explorerSynchrozation_SelectedExplorerNodeChanged);
				explorerSynchronization.SelectedExplorerNodeChanged += selectedExplorerNodeChanged;
			}
			
			
			validationSync = new HostDesignerValidationMessageObserverSync(view, this);
			view.ValidationControllerAccess.Controller.AddObserver(validationSync);
			
			SetSelection(documentView.DocData.RootElement);
		}

		void explorerSynchrozation_SelectedExplorerNodeChanged(object sender, SelectedExplorerNodeChangedEventArgs e)
		{
			SetSelection(e.SelectedElement);
		}

		private void SetSelection(ModelElement selectedElement)
		{
			currentElement = selectedElement;
			UpdateDesigner();
		}

		private void UpdateDesigner()
		{
			if (CanUpdateDesigner())
			{
				UpdateCaption(this.currentElement);
				UpdateActions(this.currentElement);
				UpdateGuidance(this.currentElement);
			}
		}

		private bool CanUpdateDesigner()
		{
			DomainClassInfo info = null;
			try
			{
				info = this.currentElement.GetDomainClass();
			}
			catch (NullReferenceException) 
			{}

			return info != null;
		}

		private void UpdateGuidance(ModelElement modelElement)
		{
			lblGuidanceNodeType.Text = modelElement.GetDomainClass().DisplayName;
		
			if (typeof(HostApplication).IsAssignableFrom(modelElement.GetType()))
			{
				lblGuidance.Text = Resources.HostApplicationGuidance;
			} 
			else if (typeof(ClientApplication).IsAssignableFrom(modelElement.GetType()))
			{
				lblGuidance.Text = Resources.ClientApplicationGuidance;
			} 
			else if (typeof(ServiceReference).IsAssignableFrom(modelElement.GetType()))
			{
				lblGuidance.Text = Resources.ServiceReferenceGuidance;
			}
			else if (typeof(Proxy).IsAssignableFrom(modelElement.GetType()))
			{
				lblGuidance.Text = Resources.ProxyGuidance;
			}
			else if (typeof(Endpoint).IsAssignableFrom(modelElement.GetType()))
			{
				lblGuidance.Text = Resources.EndpointGuidance;
			}
			else
			{
				lblGuidance.Text = Resources.HostModelGuidandace;
			} 
		}

		private void UpdateActions(ModelElement modelElement)
		{
		
			documentView.IsModelValid();
			//calling this method here, safes time in subsequent calls.
			//and reduces delays in updating the UI.
			  
			if (documentView.CanGenerateCode(modelElement))
			{
				linkGenerateCode.Visible = lblGenerateCode.Visible = true;
				linkGenerateCode.Enabled = documentView.IsElementValidForCodeGen(modelElement);
			} 
			else
			{
				linkGenerateCode.Visible = lblGenerateCode.Visible = false;
			}
			
			if (documentView.CanGenerateProxy(modelElement))
			{
				linkGenerateProxy.Visible = lblGenerateProxy.Visible = true;
				linkGenerateProxy.Enabled = documentView.IsElementValidForProxyGen(modelElement);
			}
			else
			{
				linkGenerateProxy.Visible = lblGenerateProxy.Visible = false;
			}
			
			lblNoAction.Visible = !(linkGenerateProxy.Visible | linkGenerateCode.Visible);
			linkValidate.Visible = lblModelInvalid.Visible = (!lblNoAction.Visible && !documentView.IsModelValid());
		}

		private void UpdateCaption(ModelElement modelElement)
		{
			lblElementName.Text = GetElementName(modelElement);
			lblElementType.Text = modelElement.GetDomainClass().DisplayName;

			ResourceManager resourceManager = HostDesignerDomainModel.SingletonResourceManager;
			
			if (typeof(ClientApplication).IsAssignableFrom(modelElement.GetType()))
			{
				picIcon.Image =	ImageHelper.GetImage(resourceManager.GetObject("ClientApplicationExplorerImage"));
			}
			else if (typeof(Proxy).IsAssignableFrom(modelElement.GetType()))
			{
				picIcon.Image = ImageHelper.GetImage(resourceManager.GetObject("ProxyExplorerImage"));
			}
			else if (typeof(HostApplication).IsAssignableFrom(modelElement.GetType()))
			{
				picIcon.Image = ImageHelper.GetImage(resourceManager.GetObject("HostApplicationExplorerImage"));
			}
			else if (typeof(ServiceReference).IsAssignableFrom(modelElement.GetType()))
			{
				picIcon.Image = ImageHelper.GetImage(resourceManager.GetObject("ServiceReferenceExplorerImage"));
			} 
			else if (typeof(Endpoint).IsAssignableFrom(modelElement.GetType()))
			{
				picIcon.Image = ImageHelper.GetImage(resourceManager.GetObject("EndpointExplorerImage"));
			} 
			else //defualt icon
			{
				picIcon.Image = Resources.DefaultHostModelViewIcon;
			}
		}

		private string GetElementName(ModelElement element)
		{
			if (element == null) return string.Empty;
			
			string elementName = string.Empty;
			if (!DomainClassInfo.TryGetName(element, out elementName))
			{
				DomainClassInfo classInfo = element.GetDomainClass();
				elementName = classInfo.DisplayName;
			}
			
			return elementName;
		}

		#region IDisposable Members

		void IDisposable.Dispose()
		{
			try
			{
				if (explorerSynchronization != null && selectedExplorerNodeChanged != null)
				{
					explorerSynchronization.SelectedExplorerNodeChanged -= selectedExplorerNodeChanged;
				}
				if (documentView != null && documentView.ValidationControllerAccess != null)
				{
					if (validationSync != null)
					{
						documentView.ValidationControllerAccess.Controller.RemoveObserver(validationSync);
					}
				}
			}
			finally
			{
				base.Dispose();
			}
		}

		#endregion

		private void linkGenerateCode_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			documentView.ExecuteCodeGeneration(this.currentElement);
		}

		private void linkGenerateProxy_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			documentView.ExecuteGenerateProxy(this.currentElement);
		}

		private void linkValidate_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			documentView.ExecuteValidateModelAndUpdateTaskWindow();
			
			UpdateActions(currentElement);
		}

		private class HostDesignerValidationMessageObserverSync : ValidationMessageObserver
		{
			HostDesignerDocView designerView;
			HostDesignerUserControl designerUI;
			
			public HostDesignerValidationMessageObserverSync(HostDesignerDocView designerView, HostDesignerUserControl designerUI)
			{
				this.designerView = designerView;
				this.designerUI = designerUI;
			}

			protected override void OnValidationEnded(ValidationContext context)
			{
				base.OnValidationEnded(context);
				
				if (context.CurrentViolations.Count > 0)
				{
					designerView.SetValidationStateDirect(false);
				} 
				else
				{
					bool menuValidation = 0 != (context.Categories & ValidationCategories.Menu);
					bool allModelElementsValidated = context.ValidationSubjects.Count == designerView.DocData.Store.ElementDirectory.AllElements.Count;
					if (menuValidation && allModelElementsValidated)
					{
						designerView.SetValidationStateDirect(true);
					}
					
				}
				
				designerUI.UpdateActions(designerUI.currentElement);
			}
		}
	}
}
