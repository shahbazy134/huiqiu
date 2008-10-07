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
using Microsoft.VisualStudio.Modeling;

namespace Microsoft.Practices.ServiceFactory.HostDesigner
{
	class ExplorerSynchronization
	{
		public event EventHandler<SelectedExplorerNodeChangedEventArgs> SelectedExplorerNodeChanged;

		internal void DoSelectedExplorerNodeChanged(ModelElement selectedExplorerNode)
		{
			if (SelectedExplorerNodeChanged != null)
			{
				SelectedExplorerNodeChanged(this, new SelectedExplorerNodeChangedEventArgs(selectedExplorerNode));
			}
		}
	}

	sealed class SelectedExplorerNodeChangedEventArgs : EventArgs
	{
		readonly ModelElement selectedElement;
		public SelectedExplorerNodeChangedEventArgs(ModelElement selectedElement)
		{
			this.selectedElement = selectedElement;
		}

		public ModelElement SelectedElement
		{
			get{return selectedElement;}
		}
	}

}
