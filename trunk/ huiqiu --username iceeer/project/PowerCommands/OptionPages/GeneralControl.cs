/***************************************************************************

Copyright (c) 2008 Microsoft Corporation. All rights reserved.

***************************************************************************/

using System;
using System.Windows.Forms;

namespace Microsoft.PowerCommands.OptionPages
{
    /// <summary>
    /// UserControl fro the General option page
    /// </summary>
    public partial class GeneralControl : UserControl
    {
        #region Properties
        private GeneralPage optionPage;

        /// <summary>
        /// Gets or sets the option page.
        /// </summary>
        /// <value>The option page.</value>
        public GeneralPage OptionPage
        {
            get
            {
                return optionPage;
            }
            set
            {
                optionPage = value;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="GeneralControl"/> class.
        /// </summary>
        public GeneralControl()
        {
            InitializeComponent();
        } 
        #endregion

        #region Event Handlers
        private void chkFormatOnSave_CheckedChanged(object sender, EventArgs e)
        {
            OptionPage.FormatOnSave = chkFormatOnSave.Checked;
        }

        private void RemoveAndSortUsingsOnSave_CheckedChanged(object sender, EventArgs e)
        {
            OptionPage.RemoveAndSortUsingsOnSave = chkRemoveAndSortUsingsOnSave.Checked;
        }

        private void GeneralControl_Load(object sender, EventArgs e)
        {
            chkFormatOnSave.Checked = OptionPage.FormatOnSave;
            chkRemoveAndSortUsingsOnSave.Checked = OptionPage.RemoveAndSortUsingsOnSave;
        }
        #endregion
    }
}