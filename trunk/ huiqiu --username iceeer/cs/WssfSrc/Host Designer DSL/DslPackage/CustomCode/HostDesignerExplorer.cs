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
using System.Diagnostics;
using System.Windows.Forms;
using Microsoft.VisualStudio.Modeling.Shell;
using System.ComponentModel.Design;
using System.Collections;
using Microsoft.Practices.Modeling.Dsl.Integration.Helpers;

namespace Microsoft.Practices.ServiceFactory.HostDesigner
{
	internal partial class HostDesignerExplorer : HostDesignerExplorerBase
	{
		ExplorerSynchronization explorerSynchrozationService;

		public override void AddCommandHandlers(IMenuCommandService menuCommandService)
		{
			base.AddCommandHandlers(menuCommandService);
		}
		internal void Initialize(ExplorerSynchronization explorerSynchronization)
		{
			ObjectModelBrowser.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(ObjectModelBrowser_AfterSelect);
			explorerSynchrozationService = explorerSynchronization;
		}

		void ObjectModelBrowser_AfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e)
		{
			ModelElementTreeNode elementTreeNode = e.Node as ModelElementTreeNode;

			if (elementTreeNode != null && explorerSynchrozationService != null)
			{
				explorerSynchrozationService.DoSelectedExplorerNodeChanged(elementTreeNode.ModelElement);
			}			
		}

		protected override Microsoft.VisualStudio.Modeling.ElementOperations ElementOperations
		{
			get
			{
				if (SelectedElement != null)
				{
					return new Microsoft.VisualStudio.Modeling.ElementOperations(this.ServiceProvider, this.SelectedElement.Store);
				}
				return null;
			}
		}

		protected override System.ComponentModel.Design.CommandID ContextMenuCommandId
		{
			get
			{
				return Constants.HostDesignerExplorerMenu;
			}
		}
	}
}
