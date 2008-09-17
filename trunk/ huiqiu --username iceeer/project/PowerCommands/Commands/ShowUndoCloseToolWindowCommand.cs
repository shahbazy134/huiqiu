/***************************************************************************

Copyright (c) 2008 Microsoft Corporation. All rights reserved.

***************************************************************************/

using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace Microsoft.PowerCommands.Commands
{
    /// <summary>
    /// Command that shows the Undo Close toolwindow
    /// </summary>
    [Guid("E8F31AE2-1186-4936-9A54-B5D10E6AB0F8")]
    [DisplayName("Copy Path")]
    internal class ShowUndoCloseToolWindowCommand : DynamicCommand
    {
        #region Constants
        public const uint cmdidShowUndoCloseToolWindowCommand = 0x4213;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ShowUndoCloseToolWindowCommand"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public ShowUndoCloseToolWindowCommand(IServiceProvider serviceProvider)
            : base(
                serviceProvider,
                OnExecute,
                new CommandID(
                    typeof(ShowUndoCloseToolWindowCommand).GUID,
                    (int)ShowUndoCloseToolWindowCommand.cmdidShowUndoCloseToolWindowCommand))
        {
        }
        #endregion

        #region Private Implementation
        protected override bool CanExecute(OleMenuCommand command)
        {
            if(base.CanExecute(command))
            {
                if(PowerCommandsPackage.CommandsPage.DisabledCommands.SingleOrDefault(
                    cmd => cmd.Guid.Equals(typeof(UndoCloseCommand).GUID) &&
                        cmd.ID.Equals((int)UndoCloseCommand.cmdidUndoCloseCommand)) == null)
                {
                    return true;
                }
            }

            return false;
        }

        private static void OnExecute(object sender, EventArgs e)
        {
            ToolWindowPane window = PowerCommandsPackage.UndoCloseToolWindow;

            if((window == null) || (window.Frame == null))
            {
                throw new COMException(Microsoft.PowerCommands.Properties.Resources.CannotCreateToolwindow);
            }

            IVsWindowFrame windowFrame = (IVsWindowFrame)window.Frame;
            ErrorHandler.ThrowOnFailure(windowFrame.Show());
        }
        #endregion
    }
}