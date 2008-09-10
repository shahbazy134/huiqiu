/***************************************************************************

Copyright (c) 2008 Microsoft Corporation. All rights reserved.

***************************************************************************/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using EnvDTE;
using Microsoft.PowerCommands.Linq;
using Microsoft.VisualStudio.Shell;
using VSLangProj;

namespace Microsoft.PowerCommands.Commands
{
    /// <summary>
    /// Command that copies a class content to the clipboard
    /// </summary>
    [Guid("899EB090-8728-46DF-8CEB-FCA2E326FE63")]
    [DisplayName("Copy Class")]
    internal class CopyClassCommand : DynamicCommand
    {
        #region Constants
        public const uint cmdidCopyClassCommand = 0x0EA8;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="CopyClassCommand"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public CopyClassCommand(IServiceProvider serviceProvider)
            : base(
                serviceProvider,
                OnExecute,
                new CommandID(
                    typeof(CopyClassCommand).GUID,
                    (int)CopyClassCommand.cmdidCopyClassCommand))
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
                ProjectItem item = DynamicCommand.Dte.SelectedItems.Item(1).ProjectItem;

                if(item != null)
                {
                    if(item.ContainingProject.Kind == PrjKind.prjKindCSharpProject ||
                        item.ContainingProject.Kind == PrjKind.prjKindVBProject)
                    {
                        //Available only for C# or VB projects 

                        if(item.FileCodeModel != null)
                        {
                            CodeElementIterator iterator =
                                new CodeElementIterator(item.FileCodeModel.CodeElements);

                            //Available only for project items with only one Codeclass definition
                            return (iterator.OfType<CodeClass>().Count() == 1);
                        }
                    }
                }
            }

            return false;
        }

        private static void OnExecute(object sender, EventArgs e)
        {
            ProjectItem item = DynamicCommand.Dte.SelectedItems.Item(1).ProjectItem;

            if(item != null && item.FileCodeModel != null)
            {
                List<string> files = new List<string>();

                files.Add(string.Concat(
                    string.Format(Microsoft.PowerCommands.Common.Constants.ClassUri, item.ContainingProject.Kind),
                    item.get_FileNames(0)));

                foreach(ProjectItem subItem in item.ProjectItems)
                {
                    files.Add(string.Concat(
                        string.Format(Microsoft.PowerCommands.Common.Constants.ClassUri, item.ContainingProject.Kind),
                        subItem.get_FileNames(0)));
                }

                Clipboard.SetDataObject(
                    string.Join(Microsoft.PowerCommands.Common.Constants.QSSeparator, files.ToArray()), true);
            }
        }
        #endregion
    }
}