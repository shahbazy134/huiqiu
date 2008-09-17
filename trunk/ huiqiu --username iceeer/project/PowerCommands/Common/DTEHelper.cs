/***************************************************************************

Copyright (c) 2008 Microsoft Corporation. All rights reserved.

***************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EnvDTE;
using EnvDTE80;
using Microsoft.PowerCommands.Linq;
using Microsoft.PowerCommands.Services;

namespace Microsoft.PowerCommands.Common
{
    /// <summary>
    /// Helper class for DTE common operations
    /// </summary>
    public class DTEHelper
    {
        /// <summary>
        /// Restarts the VS.
        /// </summary>
        /// <param name="dte">The DTE.</param>
        public static void RestartVS(DTE dte)
        {
            System.Diagnostics.Process vs = new System.Diagnostics.Process();

            string[] args = Environment.GetCommandLineArgs();

            vs.StartInfo.FileName = Path.GetFullPath(args[0]);
            vs.StartInfo.Arguments = string.Join(" ", args, 1, args.Length - 1);
            vs.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Maximized;
            vs.Start();

            dte.Quit();
        }

        /// <summary>
        /// Compiles a project.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <returns></returns>
        public static int CompileProject(Project project)
        {
            string assembly;
            return CompileProject(project, out assembly);
        }

        /// <summary>
        /// Compiles a project.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="assemblyFile">The assembly file.</param>
        /// <returns></returns>
        public static int CompileProject(Project project, out string assemblyFile)
        {
            project.DTE.Solution.SolutionBuild.BuildProject(
                project.DTE.Solution.SolutionBuild.ActiveConfiguration.Name,
                project.UniqueName,
                true);

            if(project.DTE.Solution.SolutionBuild.LastBuildInfo == 0)
            {
                string outputPath = project.ConfigurationManager.ActiveConfiguration.Properties.Item("OutputPath").Value.ToString();
                string buildPath = project.Properties.Item("LocalPath").Value.ToString();
                string targetName = project.Properties.Item("OutputFileName").Value.ToString();
                assemblyFile = Path.Combine(buildPath, Path.Combine(outputPath, targetName));
            }
            else
            {
                assemblyFile = null;
            }

            return project.DTE.Solution.SolutionBuild.LastBuildInfo;
        }

        /// <summary>
        /// Gets the project and solution folders nodes.
        /// </summary>
        /// <param name="solution">The solution.</param>
        /// <returns></returns>
        public static IEnumerable<UIHierarchyItem> GetUIProjectAndSolutionFoldersNodes(Solution solution)
        {
            string solutionName = solution.Properties.Item("Name").Value.ToString();

            UIHierarchy solutionExplorer = ((DTE2)solution.DTE).ToolWindows.SolutionExplorer;

            var projects =
                new UIHierarchyItemIterator(solutionExplorer.GetItem(solutionName).UIHierarchyItems)
                .Where(item =>
                    item.Object is Project ||
                    (item.Object is ProjectItem && ((ProjectItem)item.Object).Object is Project))
                 .Select(item => (UIHierarchyItem)item);

            return projects;
        }

        /// <summary>
        /// Determines whether [is UI solution node] [the specified item].
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>
        /// 	<c>true</c> if [is UI solution node] [the specified item]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsUISolutionNode(UIHierarchyItem item)
        {
            return ((item.Object is Project && ((Project)item.Object).Kind == ProjectKinds.vsProjectKindSolutionFolder) ||
                    (item.Object is ProjectItem && ((ProjectItem)item.Object).Object is Project &&
                        ((Project)((ProjectItem)item.Object).Object).Kind == ProjectKinds.vsProjectKindSolutionFolder));
        }

        /// <summary>
        /// Determines whether [is project node] [the specified item].
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>
        /// 	<c>true</c> if [is project node] [the specified item]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsProjectNode(UIHierarchyItem item)
        {
            return ((item.Object is Project && ((Project)item.Object).Kind != ProjectKinds.vsProjectKindSolutionFolder) ||
                    (item.Object is ProjectItem && ((ProjectItem)item.Object).Object is Project &&
                        ((Project)((ProjectItem)item.Object).Object).Kind != ProjectKinds.vsProjectKindSolutionFolder));
        }

        /// <summary>
        /// Gets the UI project nodes.
        /// </summary>
        /// <param name="solution">The solution.</param>
        /// <returns></returns>
        public static IEnumerable<UIHierarchyItem> GetUIProjectNodes(Solution solution)
        {
            string solutionName = solution.Properties.Item("Name").Value.ToString();
            UIHierarchy solutionExplorer = ((DTE2)solution.DTE).ToolWindows.SolutionExplorer;

            return GetUIProjectNodes(solutionExplorer.GetItem(solutionName).UIHierarchyItems);
        }

        /// <summary>
        /// Gets the UI project nodes.
        /// </summary>
        /// <param name="root">The root.</param>
        /// <returns></returns>
        public static IEnumerable<UIHierarchyItem> GetUIProjectNodes(UIHierarchyItems root)
        {
            var projects =
                new UIHierarchyItemIterator(root)
                .Where(item =>
                    (item.Object is Project && ((Project)item.Object).Kind != ProjectKinds.vsProjectKindSolutionFolder) ||
                    (item.Object is ProjectItem && ((ProjectItem)item.Object).Object is Project &&
                        ((Project)((ProjectItem)item.Object).Object).Kind != ProjectKinds.vsProjectKindSolutionFolder))
                 .Select(item => (UIHierarchyItem)item);

            return projects;
        }

        /// <summary>
        /// Opens a document.
        /// </summary>
        /// <param name="dte">The DTE.</param>
        /// <param name="docInfo">The doc info.</param>
        public static void OpenDocument(DTE dte, IDocumentInfo docInfo)
        {
            if(File.Exists(docInfo.DocumentPath))
            {
                Window window = dte.OpenFile(docInfo.DocumentViewKind, docInfo.DocumentPath);

                if(window != null)
                {
                    window.Visible = true;
                    window.Activate();

                    if(docInfo.CursorLine > 1 || docInfo.CursorColumn > 1)
                    {
                        TextSelection selection = window.Document.Selection as TextSelection;

                        if(selection != null)
                        {
                            //Move cursor
                            selection.MoveTo(docInfo.CursorLine, docInfo.CursorColumn, true);
                            //Clear selection
                            selection.Cancel();
                        }
                    }
                }
            }
        }
    }
}