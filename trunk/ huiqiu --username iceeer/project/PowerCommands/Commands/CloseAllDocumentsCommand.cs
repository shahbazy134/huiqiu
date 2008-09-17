/***************************************************************************

Copyright (c) 2008 Microsoft Corporation. All rights reserved.

***************************************************************************/

using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;

namespace Microsoft.PowerCommands.Commands
{
    /// <summary>
    /// Commands that opens the project file inside Visual Studio
    /// </summary>
    [Guid("31BE3723-DCBD-4D19-BB8D-55F6087F6B88")]
    [DisplayName("Close All")]
    internal class CloseAllDocumentsCommand : DynamicCommand
    {
        #region Constants
        public const uint cmdidCloseAllDocumentsCommand = 0x098F;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="EditProjectFileCommand"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public CloseAllDocumentsCommand(IServiceProvider serviceProvider)
            : base(
                serviceProvider,
                OnExecute,
                new CommandID(
                    typeof(CloseAllDocumentsCommand).GUID,
                    (int)CloseAllDocumentsCommand.cmdidCloseAllDocumentsCommand))
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
            DynamicCommand.Dte.ExecuteCommand("Window.CloseAllDocuments", string.Empty);
        }
        #endregion
    }
}