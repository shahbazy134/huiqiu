/***************************************************************************

Copyright (c) 2008 Microsoft Corporation. All rights reserved.

***************************************************************************/

using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Runtime.InteropServices;
using EnvDTE;
using Microsoft.PowerCommands.Common;
using Microsoft.PowerCommands.Extensions;
using Microsoft.PowerCommands.Linq;
using Microsoft.PowerCommands.Shell;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using VSLangProj;

namespace Microsoft.PowerCommands.Commands
{
    /// <summary>
    /// Command that removes and sorts using
    /// </summary>
    [Guid("453783B0-8DB7-4F1C-B7CE-5319D3915E8E")]
    [DisplayName("Remove and Sort Usings")]
    internal class RemoveSortUsingsCommand : DynamicCommand
    {
        #region Constants
        public const uint cmdidRemoveSortUsingsCommand = 0x0DBE;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="RemoveSortUsingsCommand"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public RemoveSortUsingsCommand(IServiceProvider serviceProvider)
            : base(
                serviceProvider,
                OnExecute,
                new CommandID(
                    typeof(RemoveSortUsingsCommand).GUID,
                    (int)RemoveSortUsingsCommand.cmdidRemoveSortUsingsCommand))
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
                    //Executed at the project level
                    if(project.Kind == PrjKind.prjKindCSharpProject)
                    {
                        //Available for C# projects only
                        return true;
                    }
                }
                else
                {
                    //Executed at the solution level
                    return IsAtLeastOneCSharpProject();
                }
            }

            return false;
        }

        private static void OnExecute(object sender, EventArgs e)
        {
            Project project = DynamicCommand.Dte.SelectedItems.Item(1).Project;

            if(project != null && project.Kind == PrjKind.prjKindCSharpProject)
            {
                //Executed at the project level
                ProcessProject(project);
            }
            else
            {
                //Executed at the solution level
                new ProjectIterator(DynamicCommand.Dte.Solution)
                    .Where(prj => prj.Kind == PrjKind.prjKindCSharpProject)
                    .ForEach(prj => ProcessProject(prj));
            }
        }

        private static bool IsAtLeastOneCSharpProject()
        {
            return (new ProjectIterator(DynamicCommand.Dte.Solution)
                        .FirstOrDefault(prj => prj.Kind == PrjKind.prjKindCSharpProject)
                    != null);
        }

        private static void ProcessProject(Project project)
        {
            string fileName;

            if(project != null)
            {
                int result = DTEHelper.CompileProject(project);

                if(result != VSConstants.S_OK)
                {
                    ErrorListWindow errorList =
                        new ErrorListWindow(DynamicCommand.ServiceProvider);

                    errorList.Show();

                    return;
                }

                RunningDocumentTable docTable =
                    new RunningDocumentTable(DynamicCommand.ServiceProvider);

                var alreadyOpenFiles = docTable.Select(info => info.Moniker).ToList();

                //HAAK the item.FileCodeModel call opens the item in invisible mode
                new ProjectItemIterator(project.ProjectItems)
                    .Where(item => item.FileCodeModel != null)
                    .ForEach(item =>
                    {
                        fileName = item.get_FileNames(0);

                        Window window = DynamicCommand.Dte.OpenFile(EnvDTE.Constants.vsViewKindTextView, fileName);
                        window.Activate();

                        try
                        {
                            DynamicCommand.Dte.ExecuteCommand("Edit.RemoveAndSort", string.Empty);
                        }
                        catch(COMException)
                        {
                            //Do nothing, go to the next item
                        }

                        if(alreadyOpenFiles.SingleOrDefault(
                                file => file.Equals(fileName, StringComparison.OrdinalIgnoreCase)) != null)
                        {
                            DynamicCommand.Dte.ActiveDocument.Save(fileName);
                        }
                        else
                        {
                            window.Close(vsSaveChanges.vsSaveChangesYes);
                        }
                    });
            }
        }
        #endregion
    }
}