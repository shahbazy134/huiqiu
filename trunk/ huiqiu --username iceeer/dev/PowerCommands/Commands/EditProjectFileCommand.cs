/***************************************************************************

Copyright (c) 2008 Microsoft Corporation. All rights reserved.

***************************************************************************/

using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.IO;
using System.Runtime.InteropServices;
using EnvDTE;
using Microsoft.VisualStudio.Shell;

namespace Microsoft.PowerCommands.Commands
{
    /// <summary>
    /// Commands that opens the project file inside Visual Studio
    /// </summary>
    [Guid("888DA324-B21F-4658-B663-F22884A3AF1D")]
    [DisplayName("Edit Project File")]
    internal class EditProjectFileCommand : DynamicCommand
    {
        #region Constants
        public const uint cmdidEditProjectCommand = 0x0121;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="EditProjectFileCommand"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public EditProjectFileCommand(IServiceProvider serviceProvider)
            : base(
                serviceProvider,
                OnExecute,
                new CommandID(
                    typeof(EditProjectFileCommand).GUID,
                    (int)EditProjectFileCommand.cmdidEditProjectCommand))
        {
        }
        #endregion

        #region Private Implementation
        protected override bool CanExecute(OleMenuCommand command)
        {
            if(base.CanExecute(command))
            {
                Project project = DynamicCommand.Dte.SelectedItems.Item(1).Project;

                if(project != null)
                {
                    return true;
                }
            }

            return false;
        }

        private static void OnExecute(object sender, EventArgs e)
        {
            Project project = DynamicCommand.Dte.SelectedItems.Item(1).Project;

            if(project != null)
            {
                string projectFile = project.FullName;

                //HAAK: This is needed for web projects
                if(File.Exists(projectFile))
                {
                    DynamicCommand.Dte.ExecuteCommand("Project.UnloadProject", string.Empty);

                    Window window = DynamicCommand.Dte.OpenFile(Constants.vsViewKindTextView, projectFile);
                    window.Visible = true;
                    window.Activate();
                }
            }
        }
        #endregion
    }
}