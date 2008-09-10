/***************************************************************************

Copyright (c) 2008 Microsoft Corporation. All rights reserved.

***************************************************************************/

using System;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.PowerCommands.Extensions;
using System.ComponentModel;
using EnvDTE80;
using System.Linq;

namespace Microsoft.PowerCommands.Commands
{
    /// <summary>
    /// Commands that clears all ouput window panes
    /// </summary>
    [Guid("8093C326-9C55-4ACC-96F4-B21525333D10")]
    [DisplayName("Clear All Panes")]
    internal class ClearAllPanesCommand : DynamicCommand
    {
        #region Constants
        public const uint cmdidClearAllPanesCommand = 0x2E52;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ClearAllPanesCommand"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public ClearAllPanesCommand(IServiceProvider serviceProvider)
            : base(
                serviceProvider,
                OnExecute,
                new CommandID(
                    typeof(ClearAllPanesCommand).GUID,
                    (int)ClearAllPanesCommand.cmdidClearAllPanesCommand))
        {
        }
        #endregion

        #region Private Implementation
        protected override bool CanExecute(OleMenuCommand command)
        {
            return base.CanExecute(command);
        }

        private static void OnExecute(object sender, EventArgs e)
        {
            ((DTE2)DynamicCommand.Dte).ToolWindows.OutputWindow.OutputWindowPanes
                .OfType<OutputWindowPane>()
                .ForEach(
                    pane => pane.Clear());
        }
        #endregion
    }
}