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
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.ComponentModel;

namespace Microsoft.Practices.ServiceFactory.ServiceContracts
{
	public class XsdElementPickerEditor : UITypeEditor
	{
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			using(XsdElementPickerForm form = new XsdElementPickerForm(provider))
			{
				IWindowsFormsEditorService editorService =
							   (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));

				if(editorService == null)
				{
					return value;
				}

				if(editorService.ShowDialog(form) == DialogResult.OK)
				{
					value = form.XsdElementUri;
				}
			}

			return value;
		}

		public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
		{
			return UITypeEditorEditStyle.Modal;
		}
	}
}