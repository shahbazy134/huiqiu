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
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace Microsoft.PowerCommands.Commands
{
    /// <summary>
    /// Command that unloads all projects
    /// </summary>
    [Guid("86155E5B-99D3-48A8-B3D7-99860F8FDCA9")]
    [DisplayName("Unload Projects")]
    internal class UnloadProjectsCommand : DynamicCommand
    {
        #region Constants
        public const uint cmdidUnloadProjectsCommand = 0x1FE9;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="UnloadProjectsCommand"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public UnloadProjectsCommand(IServiceProvider serviceProvider)
            : base(
                serviceProvider,
                OnExecute,
                new CommandID(
                    typeof(UnloadProjectsCommand).GUID,
                    (int)UnloadProjectsCommand.cmdidUnloadProjectsCommand))
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
                return IsAtLeastOneProjectToUnload();
            }

            return false;
        }

        private static void OnExecute(object sender, EventArgs e)
        {
            var projectsToUnload = GetProjectsToUnload().ToList();

            projectsToUnload.ForEach(hierarchyNode =>
                {
                    ErrorHandler.ThrowOnFailure(
                        Solution.CloseSolutionElement((uint)__VSSLNCLOSEOPTIONS.SLNCLOSEOPT_UnloadProject, hierarchyNode.Hierarchy, 0));
                }
                );
        }

        private static bool IsAtLeastOneProjectToUnload()
        {
            HierarchyNodeIterator nodeIterator = new HierarchyNodeIterator(Solution);

            HierarchyNode projectToUnload = nodeIterator
                .FirstOrDefault(node =>
                     (node.ExtObject is Project && 
                     ((Project)node.ExtObject) != null &&
                        (((Project)node.ExtObject).Kind != ProjectKinds.vsProjectKindSolutionFolder &&
                        ((Project)node.ExtObject).Kind != EnvDTE.Constants.vsProjectKindMisc)));

            return (projectToUnload != null);
        }

        private static IEnumerable<HierarchyNode> GetProjectsToUnload()
        {
            HierarchyNodeIterator nodeIterator = new HierarchyNodeIterator(Solution);

            var projectsToUnload = nodeIterator
                .Where(node =>
                     (node.ExtObject is Project &&
                     ((Project)node.ExtObject) != null &&
                        (((Project)node.ExtObject).Kind != ProjectKinds.vsProjectKindSolutionFolder &&
                        ((Project)node.ExtObject).Kind != EnvDTE.Constants.vsProjectKindMisc)))
                .Select(node => node)
                .Distinct();

            return projectsToUnload;
        }

        #endregion
    }
}