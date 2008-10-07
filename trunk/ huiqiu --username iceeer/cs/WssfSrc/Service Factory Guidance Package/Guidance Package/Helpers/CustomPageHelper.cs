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
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Practices.ServiceFactory.Helpers
{
	static class CustomPageHelper
	{
		/// <summary>
		/// This handler can be applied in any DataGridView containing checkbox columns in order to
		/// auto-commit these cells just when they are clicked
		/// </summary>

		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		public static void AutoCommitCheckboxCurrentCellDirtyStateChangeHandler(object sender, EventArgs e)
		{
			DataGridView dataGridView = (DataGridView)sender;
			if(dataGridView.IsCurrentCellDirty &&
				dataGridView.Columns[dataGridView.CurrentCellAddress.X] is DataGridViewCheckBoxColumn)
			{
				dataGridView.CommitEdit(DataGridViewDataErrorContexts.Commit);
			}
		}

		public static MessageBoxOptions GetRtlMessageBoxOptionsToShutUpFxCop(Control control)
		{
			if(control == null)
			{
				return (MessageBoxOptions)0;
			}

			if(control.RightToLeft == System.Windows.Forms.RightToLeft.Yes)
			{
				return MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading;
			}

			if(control.RightToLeft == System.Windows.Forms.RightToLeft.Inherit)
			{
				return GetRtlMessageBoxOptionsToShutUpFxCop(control.Parent);
			}

			return (MessageBoxOptions)0;
		}
	}
}