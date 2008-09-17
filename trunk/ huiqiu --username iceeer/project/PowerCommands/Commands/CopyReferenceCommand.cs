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
using Microsoft.PowerCommands.Extensions;
using Microsoft.PowerCommands.Linq;
using Microsoft.VisualStudio.Shell;

namespace Microsoft.PowerCommands.Commands
{
    /// <summary>
    /// Command that copies one or more project references to the clipboard
    /// </summary>
    [Guid("D88EF4B1-587E-4A9F-AE08-F3CEDDBF028A")]
    [DisplayName("Copy Reference")]
    internal class CopyReferenceCommand : DynamicCommand
    {
        #region Constants
        public const uint cmdidCopyReferenceCommand = 0x08E2;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="CopyReferenceCommand"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public CopyReferenceCommand(IServiceProvider serviceProvider)
            : base(
                serviceProvider,
                OnExecute,
                new CommandID(
                    typeof(CopyReferenceCommand).GUID,
                    (int)CopyReferenceCommand.cmdidCopyReferenceCommand))
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
            command.Text =
                (DynamicCommand.Dte.SelectedItems.OfType<SelectedItem>().Count() > 1) ? "Copy References" : "Copy Reference";
            
            return base.CanExecute(command);
        }
        
        private static void OnExecute(object sender, EventArgs e)
        {
            List<string> references = new List<string>();

            DynamicCommand.Dte.SelectedItems.OfType<SelectedItem>()
                .ForEach(item =>
                    {
                        if(!string.IsNullOrEmpty(item.Name))
                        {
                            if(new ProjectIterator(DynamicCommand.Dte.Solution)
                                .SingleOrDefault(project => project.Name.Equals(item.Name, StringComparison.OrdinalIgnoreCase)) != null)
                            {
                                references.Add(string.Concat(Microsoft.PowerCommands.Common.Constants.ProjRefUri, item.Name));
                            }
                            else
                            {
                                references.Add(string.Concat(Microsoft.PowerCommands.Common.Constants.AssemblyRefUri, item.Name));
                            }
                        }
                    });

            Clipboard.SetDataObject(
                string.Join(Microsoft.PowerCommands.Common.Constants.QSSeparator, references.ToArray()), true);
        }
        #endregion
    }
}