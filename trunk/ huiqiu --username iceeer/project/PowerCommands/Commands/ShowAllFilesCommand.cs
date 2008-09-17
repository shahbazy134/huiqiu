/***************************************************************************

Copyright (c) 2008 Microsoft Corporation. All rights reserved.

***************************************************************************/

using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Runtime.InteropServices;
using EnvDTE;
using EnvDTE80;
using Microsoft.PowerCommands.Common;
using Microsoft.PowerCommands.Extensions;
using Microsoft.PowerCommands.Linq;
using Microsoft.VisualStudio.Shell;
using VSLangProj;
using System.Diagnostics;

namespace Microsoft.PowerCommands.Commands
{
    /// <summary>
    /// Commands that show all hidden files in a solution
    /// </summary>
    [Guid("97104711-1B56-4AD3-92C9-557353BC78E0")]
    [DisplayName("Show All Files")]
    internal class ShowAllFilesCommand : DynamicCommand
    {
        #region Constants
        public const uint cmdidShowAllFilesCommand = 0x2A19;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ShowAllFilesCommand"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public ShowAllFilesCommand(IServiceProvider serviceProvider)
            : base(
                serviceProvider,
                OnExecute,
                new CommandID(
                    typeof(ShowAllFilesCommand).GUID,
                    (int)ShowAllFilesCommand.cmdidShowAllFilesCommand))
        {
        }
        #endregion

        #region Private Implementation
        protected override bool CanExecute(OleMenuCommand command)
        {
            if(base.CanExecute(command))
            {
                UIHierarchy solutionExplorer = ((DTE2)Dte).ToolWindows.SolutionExplorer;

                if(solutionExplorer.UIHierarchyItems.Item(1).IsSelected)
                {
                    return IsAtLeastOneCSharpOrVBProject();
                }
            }

            return false;
        }

        private static void OnExecute(object sender, EventArgs e)
        {
            new ProjectIterator(DynamicCommand.Dte.Solution)
                .Where(prj => prj.Kind == PrjKind.prjKindCSharpProject || prj.Kind == PrjKind.prjKindVBProject)
                .ForEach(
                    prj =>
                    {
                        UIHierarchyItem item = DTEHelper.GetUIProjectNodes(Dte.Solution)
                            .FirstOrDefault(
                                ui =>
                                {
                                    if(ui.Object is Project)
                                    {
                                        return ((Project)ui.Object).FullName.Equals(prj.FullName);
                                    }
                                    else
                                    {
                                        return ((Project)((ProjectItem)ui.Object).Object).FullName.Equals(prj.FullName);
                                    }
                                });

                        if(item != null)
                        {
                            item.Select(vsUISelectionType.vsUISelectionTypeSelect);

                            Dte.ExecuteCommand("Project.ShowAllFiles", string.Empty);
                        }
                    });

            //Select solution again
            UIHierarchy solutionExplorer = ((DTE2)Dte).ToolWindows.SolutionExplorer;
            solutionExplorer.UIHierarchyItems.Item(1).Select(vsUISelectionType.vsUISelectionTypeSelect);
        }

        private static bool IsAtLeastOneCSharpOrVBProject()
        {
            return (new ProjectIterator(DynamicCommand.Dte.Solution)
                        .FirstOrDefault(
                            prj =>
                                (prj.Kind == PrjKind.prjKindCSharpProject || prj.Kind == PrjKind.prjKindVBProject))
                    != null);
        }

        #endregion
    }
}