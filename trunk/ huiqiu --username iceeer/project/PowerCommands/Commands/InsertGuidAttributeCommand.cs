/***************************************************************************

Copyright (c) 2008 Microsoft Corporation. All rights reserved.

***************************************************************************/

using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Runtime.InteropServices;
using EnvDTE;
using Microsoft.PowerCommands.Linq;
using Microsoft.VisualStudio.Shell;

namespace Microsoft.PowerCommands.Commands
{
    /// <summary>
    /// Command that copies to the clipboard a Guid Attribute
    /// </summary>
    [Guid("D99DE366-5426-4F39-A444-23698B9B5D89")]
    [DisplayName("Insert Guid Attribute")]
    internal class InsertGuidAttributeCommand : DynamicCommand
    {
        #region Constants
        public const uint cmdidInsertGuidAttributeCommand = 0x265A;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="InsertGuidAttributeCommand"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public InsertGuidAttributeCommand(IServiceProvider serviceProvider)
            : base(
                serviceProvider,
                OnExecute,
                new CommandID(
                    typeof(InsertGuidAttributeCommand).GUID,
                    (int)InsertGuidAttributeCommand.cmdidInsertGuidAttributeCommand))
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
                if(DynamicCommand.Dte.ActiveDocument != null &&
                    DynamicCommand.Dte.ActiveDocument.ProjectItem != null &&
                    DynamicCommand.Dte.ActiveDocument.ProjectItem.FileCodeModel != null)
                {
                    TextSelection selection = DynamicCommand.Dte.ActiveDocument.Selection as TextSelection;

                    try
                    {
                        CodeClass @class =
                            DynamicCommand.Dte.ActiveDocument.ProjectItem.FileCodeModel.CodeElementFromPoint(
                                selection.ActivePoint, vsCMElement.vsCMElementClass) as CodeClass;

                        if(@class != null)
                        {
                            CodeElementIterator iterator = new CodeElementIterator(@class.Children);

                            if(iterator.OfType<CodeAttribute>()
                                .SingleOrDefault(
                                    att => att.Name.IndexOf("Guid") > -1) == null)
                            {
                                return true;
                            }
                        }
                    }
                    catch(COMException)
                    {
                        //Ocurred when right clicking out of a class because of vsCMElement.vsCMElementClass
                        return false;
                    }
                }
            }

            return false;
        }

        private static void OnExecute(object sender, EventArgs e)
        {
            if(DynamicCommand.Dte.ActiveDocument != null &&
                DynamicCommand.Dte.ActiveDocument.ProjectItem != null &&
                DynamicCommand.Dte.ActiveDocument.ProjectItem.FileCodeModel != null)
            {
                try
                {
                    TextSelection selection = DynamicCommand.Dte.ActiveDocument.Selection as TextSelection;

                    CodeClass @class =
                        DynamicCommand.Dte.ActiveDocument.ProjectItem.FileCodeModel.CodeElementFromPoint(
                            selection.ActivePoint, vsCMElement.vsCMElementClass) as CodeClass;

                    if(@class != null)
                    {
                        @class.AddAttribute(
                            typeof(GuidAttribute).FullName,
                            string.Format("\"{0}\"", Guid.NewGuid().ToString("D").ToUpper()),
                            -1);
                    }
                }
                catch(COMException)
                {
                    //Ocurred when right clicking out of a class because of vsCMElement.vsCMElementClass
                }
            }
        }
        #endregion
    }
}