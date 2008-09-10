/***************************************************************************

Copyright (c) 2008 Microsoft Corporation. All rights reserved.

***************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using EnvDTE;
using EnvDTE80;

namespace Microsoft.PowerCommands.Linq
{
    /// <summary>
    /// Iterator for Project hierarchy
    /// </summary>
    public sealed class ProjectIterator : IEnumerable<Project>
    {
        #region Fields
        Solution solution;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectIterator"/> class.
        /// </summary>
        /// <param name="solution">The solution.</param>
        public ProjectIterator(Solution solution)
        {
            if(solution == null)
            {
                throw new ArgumentNullException("solution");
            }

            this.solution = solution;
        }
        #endregion

        #region Public Implementation
        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<Project> GetEnumerator()
        {
            foreach(Project project in this.solution.Projects)
            {
                if(project.Kind != ProjectKinds.vsProjectKindSolutionFolder)
                {
                    yield return project;
                }
                else
                {
                    foreach(Project subProject in Enumerate(project))
                    {
                        yield return subProject;
                    }
                }
            }
        }

        IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion

        #region Private Implementation
        private IEnumerable<Project> Enumerate(Project project)
        {
            foreach(ProjectItem item in project.ProjectItems)
            {
                if(item.Object is Project)
                {
                    if(((Project)item.Object).Kind != ProjectKinds.vsProjectKindSolutionFolder)
                    {
                        yield return (Project)item.Object;
                    }
                    else
                    {
                        foreach(Project subProject in Enumerate((Project)item.Object))
                        {
                            yield return subProject;
                        }
                    }
                }
            }
        }
        #endregion
    }
}