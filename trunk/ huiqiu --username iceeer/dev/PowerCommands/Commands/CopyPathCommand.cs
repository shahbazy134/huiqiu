/***************************************************************************

Copyright (c) 2008 Microsoft Corporation. All rights reserved.

***************************************************************************/

using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.PowerCommands.Common;
using Microsoft.VisualStudio.Shell;

namespace Microsoft.PowerCommands.Commands
{
    /// <summary>
    /// Command that copies an item path to the clipboard
    /// </summary>
    [Guid("7F95D8FB-4996-4763-AF41-A2154A831F77")]
    [DisplayName("Copy Path")]
    internal class CopyPathCommand : DynamicCommand
    {
        #region Constants
        public const uint cmdidCopyPathCommand = 0x5A09;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="CopyPathCommand"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public CopyPathCommand(IServiceProvider serviceProvider)
            : base(
                serviceProvider,
                OnExecute,
                new CommandID(
                    typeof(CopyPathCommand).GUID,
                    (int)CopyPathCommand.cmdidCopyPathCommand))
        {
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
                return DynamicCommand.Dte.Solution.IsOpen;
            }

            return false;
        }

        private static void OnExecute(object sender, EventArgs e)
        {
            string path = string.Empty;

            if(DynamicCommand.Dte.SelectedItems.Item(1).Project != null)
            {
                //Executed at the project level
                path = DynamicCommand.Dte.SelectedItems.Item(1).Project.FullName;
            }
            else if(DynamicCommand.Dte.SelectedItems.Item(1).ProjectItem != null)
            {
                //Executed at the folder level / item level
                path = DynamicCommand.Dte.SelectedItems.Item(1).ProjectItem.get_FileNames(0);
            }
            else
            {
                //Executed at the solution level
                path = DynamicCommand.Dte.Solution.FullName;
            }

            Clipboard.SetDataObject(IOHelper.SanitizePath(path), true);
        }
        #endregion
    }
}