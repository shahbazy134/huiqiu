/***************************************************************************

Copyright (c) 2008 Microsoft Corporation. All rights reserved.

***************************************************************************/

using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.PowerCommands.Common;
using Microsoft.PowerCommands.Extensions;
using Microsoft.PowerCommands.Services;
using Microsoft.VisualStudio.Shell;

namespace Microsoft.PowerCommands.Commands
{
    /// <summary>
    /// Command that re opens a closed document
    /// </summary>
    [Guid("184DD6C2-6301-49E8-A6C9-D8D026444172")]
    [DisplayName("Undo Close")]
    internal class UndoCloseCommand : DynamicCommand
    {
        #region Constants
        public const uint cmdidUndoCloseCommand = 0x667D;
        #endregion

        private static IUndoCloseManagerService undoCloseManager;

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="UndoCloseCommand"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public UndoCloseCommand(IServiceProvider serviceProvider)
            : base(
                serviceProvider,
                OnExecute,
                new CommandID(
                    typeof(UndoCloseCommand).GUID,
                    (int)UndoCloseCommand.cmdidUndoCloseCommand))
        {
            undoCloseManager = ServiceProvider.GetService<SUndoCloseManagerService, IUndoCloseManagerService>();
        }
        #endregion

        #region Private Implementation
        /// <summary>
        /// Determines whether this instance can execute the specified command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns>
        /// 	<c>true</c> if this instance can execute the specified command; otherwise, <c>false</c>.
        /// </returns>
        protected override bool CanExecute(OleMenuCommand command)
        {
            if(base.CanExecute(command))
            {
                return (undoCloseManager.CurrentDocument != null);
            }

            return false;
        }

        private static void OnExecute(object sender, EventArgs e)
        {
            IDocumentInfo docInfo = undoCloseManager.PopDocument();

            if(File.Exists(docInfo.DocumentPath))
            {
                try
                {
                    DTEHelper.OpenDocument(Dte, docInfo);
                }
                catch(COMException)
                { }

                PowerCommandsPackage.UndoCloseToolWindow.Control.UpdateDocumentList();
            }
            else
            {
                //The file was moved/renamed/deleted
                //Go to the next item on the stack

                if(undoCloseManager.CurrentDocument != null)
                {
                    UndoCloseCommand.OnExecute(sender, e);
                }
            }
        }
        #endregion
    }
}