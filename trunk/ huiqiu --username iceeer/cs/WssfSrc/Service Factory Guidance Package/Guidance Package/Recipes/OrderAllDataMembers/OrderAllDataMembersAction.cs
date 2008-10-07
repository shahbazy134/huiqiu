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
using Microsoft.Practices.ServiceFactory.Actions;
using Microsoft.Practices.RecipeFramework;
using Microsoft.VisualStudio.Modeling;
using Microsoft.Practices.ServiceFactory.DataContracts;
using Microsoft.Practices.Modeling.ExtensionProvider.Helpers;
using Microsoft.Practices.Modeling.ExtensionProvider.Extension;
using System.Reflection;

namespace Microsoft.Practices.ServiceFactory.Recipes.OrderAllDataMembers
{
	public class OrderAllDataMembersAction : ActionBase
	{
		private ModelElement selectedElement;

		[Input(Required = true)]
		public ModelElement SelectedElement
		{
			get { return selectedElement; }
			set { selectedElement = value; }
		}

		protected override void OnExecute()
		{
			OrderDataMembers();
		}

		protected void OrderDataMembers()
		{
			using (Transaction transaction = selectedElement.Store.TransactionManager.BeginTransaction())
			{
				foreach (DataContract data in selectedElement.Store.ElementDirectory.FindElements<DataContract>())
				{
					int index = 0;
					foreach (DataMember member in data.DataMembers)
					{
						SetOrder(member.ObjectExtender, index++);
					}
					if (index > 0)
					{
						OrderedDataMember(data.Name, index);
					}
					SetOrderParts(data.ObjectExtender);
				}

				foreach (FaultContract fault in selectedElement.Store.ElementDirectory.FindElements<FaultContract>())
				{
					int index = 0;
					foreach (DataMember member in fault.DataMembers)
					{
						SetOrder(member.ObjectExtender, index++);
					}
					if (index > 0)
					{
						OrderedDataMember(fault.Name, index);
					}
					SetOrderParts(fault.ObjectExtender);
				}

				transaction.Commit();
			}
		}

		protected virtual void OrderedDataMember(string name, int index)
		{
			base.Trace(Properties.Resources.OrderedDataMembers, name, index);
		}

		private void SetOrder(object extender, int index)
		{
			if (extender != null)
			{
				// extender will be a WCFDataElement or a ASMXDataElement
				// we reflect the object to avoid coupling with extenders
				PropertyInfo info = extender.GetType().GetProperty("Order");
				if (info != null)
				{
					info.SetValue(extender, index, null);
				}
			}
		}

		private void SetOrderParts(object extender)
		{
			if (extender != null)
			{
				// this property will be available only for ASMXDataElement
				PropertyInfo info = extender.GetType().GetProperty("OrderParts");
				if (info != null)
				{
					info.SetValue(extender, true, null);
				}
			}
		}

	}
}
