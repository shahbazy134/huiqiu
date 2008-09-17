/***************************************************************************

Copyright (c) 2008 Microsoft Corporation. All rights reserved.

***************************************************************************/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using Microsoft.PowerCommands.Extensions;
using Microsoft.PowerCommands.Services;

namespace Microsoft.PowerCommands.OptionPages
{
    /// <summary>
    /// 
    /// </summary>
    public partial class CommandsControl : UserControl
    {
        #region Fields
        ICommandManagerService commandManagerService;
        IList<RowItem> items; 
        #endregion

        #region Properties
        private CommandsPage optionPage;

        /// <summary>
        /// Gets or sets the option page.
        /// </summary>
        /// <value>The option page.</value>
        public CommandsPage OptionPage
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
        /// Initializes a new instance of the <see cref="CommandsControl"/> class.
        /// </summary>
        public CommandsControl()
        {
            InitializeComponent();
        } 
        #endregion

        #region Event Handlers
        private void CommandsControl_Load(object sender, EventArgs e)
        {
            commandManagerService = optionPage.Site.GetService<SCommandManagerService, ICommandManagerService>();

            items = commandManagerService.GetRegisteredCommands()
                    .OrderBy(command => command.GetType().Name)
                    .Select(
                        command =>
                        new RowItem()
                        {
                            Command = command.CommandID,
                            CommandText = GetDisplayName(command.GetType()),
                            Enabled = OptionPage.DisabledCommands.SingleOrDefault(
                                        cmd => cmd.Guid.Equals(command.CommandID.Guid) &&
                                        cmd.ID.Equals(command.CommandID.ID)) == null
                        }).ToList();

            gridVisibility.DataSource = items;
            gridVisibility.Columns[0].Width = 200;
            gridVisibility.Columns[0].ReadOnly = true;
        }

        private void gridVisibility_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if(e.ColumnIndex == 1)
            {
                RowItem item = gridVisibility.CurrentRow.DataBoundItem as RowItem;

                optionPage.DisabledCommands.Remove(item.Command);

                if(!item.Enabled)
                {
                    optionPage.DisabledCommands.Add(item.Command);
                }
            }
        }

        private void gridVisibility_MouseLeave(object sender, EventArgs e)
        {
            gridVisibility.EndEdit();
        }
        #endregion

        #region Private Implementation
        private string GetDisplayName(Type command)
        {
            string displayName = string.Empty;

            DisplayNameAttribute att =
                TypeDescriptor.GetAttributes(command)
                .OfType<DisplayNameAttribute>()
                .FirstOrDefault();

            if(att != null)
            {
                displayName = att.DisplayName;
            }

            return displayName;
        } 
        #endregion
    }
}