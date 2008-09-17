/***************************************************************************

Copyright (c) 2008 Microsoft Corporation. All rights reserved.

***************************************************************************/

using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using VSLangProj;

namespace Microsoft.PowerCommands.Commands
{
    /// <summary>
    /// Command that copies one or more project references to the clipboard
    /// </summary>
    [Guid("88822172-82D1-48ff-A566-72400006A992")]
    [DisplayName("Copy As Project Reference")]
    internal class CopyAsProjectReferenceCommand : DynamicCommand
    {
        #region Constants
        public const uint cmdidCopyAsProjectReferenceCommand = 0x0810;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="CopyAsProjectReferenceCommand"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public CopyAsProjectReferenceCommand(IServiceProvider serviceProvider)
            : base(
                serviceProvider,
                OnExecute,
                new CommandID(
                    typeof(CopyAsProjectReferenceCommand).GUID,
                    (int)CopyAsProjectReferenceCommand.cmdidCopyAsProjectReferenceCommand))
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
                Project project = DynamicCommand.Dte.SelectedItems.Item(1).Project;

                if(project != null)
                {
                    if(project.Kind == PrjKind.prjKindCSharpProject || project.Kind == PrjKind.prjKindVBProject)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static void OnExecute(object sender, EventArgs e)
        {
            Project project = DynamicCommand.Dte.SelectedItems.Item(1).Project;

            if(project != null)
            {
                Clipboard.SetDataObject(string.Concat(Microsoft.PowerCommands.Common.Constants.ProjRefUri, project.Name));
            }
        }
        #endregion
    }
}