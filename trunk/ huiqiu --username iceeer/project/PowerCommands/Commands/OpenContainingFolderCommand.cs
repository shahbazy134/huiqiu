/***************************************************************************

Copyright (c) 2008 Microsoft Corporation. All rights reserved.

***************************************************************************/

using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using EnvDTE;
using Microsoft.PowerCommands.Common;
using Microsoft.VisualStudio.Shell;

namespace Microsoft.PowerCommands.Commands
{
    [Guid("5C199E63-E4F4-4B27-8955-75844A35066A")]
    [DisplayName("Open Containing Folder")]
    internal class OpenContainingFolderCommand : DynamicCommand
    {
        #region Constants
        public const uint cmdidOpenContainingFolderCommand = 0x39B9;

        private const string Explorer = "explorer.exe";
        #endregion

        #region Constructors
        public OpenContainingFolderCommand(IServiceProvider serviceProvider)
            : base(
                serviceProvider,
                OnExecute,
                new CommandID(
                    typeof(OpenContainingFolderCommand).GUID,
                    (int)OpenContainingFolderCommand.cmdidOpenContainingFolderCommand))
        {
        }
        #endregion

        #region Private Implementation
        protected override bool CanExecute(OleMenuCommand command)
        {
            if(base.CanExecute(command))
            {
                ProjectItem item = DynamicCommand.Dte.SelectedItems.Item(1).ProjectItem;

                if(item != null)
                {
                    return true;
                }
            }

            return false;
        }
        
        private static void OnExecute(object sender, EventArgs e)
        {
            ProjectItem item = DynamicCommand.Dte.SelectedItems.Item(1).ProjectItem;

            if(item != null)
            {
                string fileName = IOHelper.SanitizePath(item.get_FileNames(0));

                System.Diagnostics.Process.Start(
                    Explorer,
                    string.Format("/e,/select,{0}", fileName));
            }
        }
        #endregion
    }
}