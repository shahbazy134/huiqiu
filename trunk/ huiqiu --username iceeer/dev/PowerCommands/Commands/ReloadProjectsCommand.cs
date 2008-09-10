/***************************************************************************

Copyright (c) 2008 Microsoft Corporation. All rights reserved.

***************************************************************************/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Runtime.InteropServices;
using EnvDTE;
using EnvDTE80;
using Microsoft.PowerCommands.Extensions;
using Microsoft.PowerCommands.Linq;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace Microsoft.PowerCommands.Commands
{
    /// <summary>
    /// Command that reloads all projects in a solution
    /// </summary>
    [Guid("9759B1F3-64EF-41AF-B383-170CE3FC7277")]
    [DisplayName("Reload Projects")]
    internal class ReloadProjectsCommand : DynamicCommand
    {
        #region Constants
        public const uint cmdidReloadProjectsCommand = 0x5325;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ReloadProjectsCommand"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public ReloadProjectsCommand(IServiceProvider serviceProvider)
            : base(
                serviceProvider,
                OnExecute,
                new CommandID(
                    typeof(ReloadProjectsCommand).GUID,
                    (int)ReloadProjectsCommand.cmdidReloadProjectsCommand))
        {
        }
        #endregion

        #region Properties
        private static IVsSolution solution;

        protected static IVsSolution Solution
        {
            get
            {
                if(solution == null)
                {
                    solution = DynamicCommand.ServiceProvider.GetService<SVsSolution, IVsSolution>();
                }

                return solution;
            }
        }

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
                return IsAtLeastOneProjectToReload();
            }

            return false;
        }

        private static void OnExecute(object sender, EventArgs e)
        {
            List<UIHierarchyItem> projectsToReload = GetProjectsToReload();

            projectsToReload
                .ForEach<UIHierarchyItem>(
                    item =>
                    {
                        item.Select(vsUISelectionType.vsUISelectionTypeSelect);

                        try
                        {
                            DynamicCommand.Dte.ExecuteCommand("Project.ReloadProject", string.Empty);
                        }
                        catch(COMException)
                        {
                            //Do nothing, go to the next project
                        }
                    });
        }

        private static IEnumerable<string> GetProjectNamesToReload()
        {
            HierarchyNodeIterator nodeIterator = new HierarchyNodeIterator(Solution);

            //HAAK: The projects to be reloaded are found by looking for "(unavailable)" in the node caption
            return nodeIterator
                .Where(node =>
                     node.GetProperty<string>(__VSHPROPID.VSHPROPID_Caption).IndexOf(
                        "(unavailable)", StringComparison.OrdinalIgnoreCase) > -1)
                .Select(node => node.Name)
                .Distinct();
        }

        private static bool IsAtLeastOneProjectToReload()
        {
            HierarchyNodeIterator nodeIterator = new HierarchyNodeIterator(Solution);

            //HAAK: The projects to be reloaded are found by looking for "(unavailable)" in the node caption
            return (nodeIterator
                    .FirstOrDefault(node =>
                         node.GetProperty<string>(__VSHPROPID.VSHPROPID_Caption).IndexOf(
                            "(unavailable)", StringComparison.OrdinalIgnoreCase) > -1)
                    != null);
        }

        private static List<UIHierarchyItem> GetProjectsToReload()
        {
            string solutionName = DynamicCommand.Dte.Solution.Properties.Item("Name").Value.ToString();

            ProjectIterator iterator = new ProjectIterator(DynamicCommand.Dte.Solution);

            var projectNames = GetProjectNamesToReload();

            List<UIHierarchyItem> projectsToReload =
                new UIHierarchyItemIterator(SolutionExplorer.GetItem(solutionName).UIHierarchyItems)
                .Where(item => 
                    {
                        try
                        {
                           return projectNames.SingleOrDefault(name => name.Equals(item.Name)) != null;
                        }
                        catch(ArgumentException)
                        {
                            return false;
                        }
                    }
                    )
                    .ToList();

            return projectsToReload;
        }
        #endregion
    }
}