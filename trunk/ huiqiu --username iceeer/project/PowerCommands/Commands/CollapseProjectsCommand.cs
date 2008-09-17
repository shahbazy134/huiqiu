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

namespace Microsoft.PowerCommands.Commands
{
    /// <summary>
    /// Command that collapses all projects
    /// </summary>    
    [Guid("C4C895C3-F940-424C-B158-2923AE5B7B80")]
    [DisplayName("Collapse Projects")]
    internal class CollapseProjectsCommand : DynamicCommand
    {
        #region Constants
        public const uint cmdidCollapseProjectsCommand = 0x2910;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="CollapseProjectsCommand"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public CollapseProjectsCommand(IServiceProvider serviceProvider)
            : base(
                serviceProvider,
                OnExecute,
                new CommandID(
                    typeof(CollapseProjectsCommand).GUID,
                    (int)CollapseProjectsCommand.cmdidCollapseProjectsCommand))
        {
        }
        #endregion

        #region Properties
        private static UIHierarchy solutionExplorer;

        protected static UIHierarchy SolutionExplorer
        {
            get
            {
                if(solutionExplorer == null)
                {
                    solutionExplorer = ((DTE2)DynamicCommand.Dte).ToolWindows.SolutionExplorer;
                }

                return solutionExplorer;
            }
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
                    if(project.Kind == ProjectKinds.vsProjectKindSolutionFolder)
                    {
                        //Executed at the solution folder level
                        command.Text = "Collapse Projects";

                        return IsAtLeastOneProjectExpanded(GetSelectedUIHierarchy().UIHierarchyItems);
                    }
                    else
                    {
                        //Executed at the project level
                        command.Text = "Collapse Project";

                        return IsSelectedProjectExpanded();
                    }
                }
                else if(new ProjectIterator(DynamicCommand.Dte.Solution).Count() > 0)
                {
                    //Executed at the solution level
                    command.Text = "Collapse Projects";

                    return IsAtLeastOneProjectExpanded(SolutionExplorer.UIHierarchyItems);
                }
            }

            return false;
        }
        
        private static void OnExecute(object sender, EventArgs e)
        {
            Project project = DynamicCommand.Dte.SelectedItems.Item(1).Project;

            if(project != null)
            {
                UIHierarchyItem selectedUIHierarchy = GetSelectedUIHierarchy();

                if(selectedUIHierarchy != null)
                {
                    //Executed at the project/solution folder level
                    CollapseProject(selectedUIHierarchy);
                }
            }
            else
            {
                //Executed at the solution level
                DTEHelper.GetUIProjectNodes(DynamicCommand.Dte.Solution)
                    .ForEach<UIHierarchyItem>(uiHier =>
                        {
                            CollapseProject(uiHier);
                        }
                            );

                SolutionExplorer.GetItem(DynamicCommand.Dte.Solution.Properties.Item("Name").Value.ToString())
                    .Select(vsUISelectionType.vsUISelectionTypeSelect);
            }
        }

        private static UIHierarchyItem GetSelectedUIHierarchy()
        {
            object[] selection = SolutionExplorer.SelectedItems as object[];

            if(selection != null && selection.Length == 1)
            {
                return selection[0] as UIHierarchyItem;
            }

            return null;
        }

        private static bool IsAtLeastOneProjectExpanded(UIHierarchyItems root)
        {
            UIHierarchyItem project =
                DTEHelper.GetUIProjectNodes(root)
                    .FirstOrDefault(hier => hier.UIHierarchyItems.Expanded);

            return (project != null);
        }

        private static bool IsSelectedProjectExpanded()
        {
            UIHierarchyItem selectedUIHierarchy = GetSelectedUIHierarchy();

            if(selectedUIHierarchy != null)
            {
                return selectedUIHierarchy.UIHierarchyItems.Expanded;
            }

            return false;
        }

        private static void CollapseProject(UIHierarchyItem project)
        {
            new UIHierarchyItemIterator(project.UIHierarchyItems)
                .ForEach<UIHierarchyItem>(subUiHier =>
                    {
                        PerformCollapse(subUiHier);
                    });

            PerformCollapse(project);
            project.Select(vsUISelectionType.vsUISelectionTypeSelect);
        }

        private static void PerformCollapse(UIHierarchyItem project)
        {
            if(!DTEHelper.IsUISolutionNode(project) && project.UIHierarchyItems.Expanded)
            {
                project.UIHierarchyItems.Expanded = false;
            }

            if(DTEHelper.IsProjectNode(project) &&
                (project.Object as ProjectItem) != null &&
                (project.Object as ProjectItem).ContainingProject.Kind == ProjectKinds.vsProjectKindSolutionFolder &&
                project.UIHierarchyItems.Expanded)
            {
                project.Select(vsUISelectionType.vsUISelectionTypeSelect);
                SolutionExplorer.DoDefaultAction();
            }
        }
        #endregion
    }
}