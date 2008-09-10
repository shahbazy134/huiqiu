/***************************************************************************

Copyright (c) 2008 Microsoft Corporation. All rights reserved.

***************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EnvDTE;

namespace Microsoft.PowerCommands.Linq
{
    /// <summary>
    /// Iterator for ProjectItem hierachy
    /// </summary>
    public sealed class ProjectItemIterator : IEnumerable<ProjectItem>
    {
        #region Fields
        ProjectItems items; 
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectItemIterator"/> class.
        /// </summary>
        /// <param name="items">The items.</param>
        public ProjectItemIterator(ProjectItems items)
        {
            if(items == null)
            {
                throw new ArgumentNullException("items");
            }

            this.items = items;
        } 
        #endregion

        #region Public Implementation
        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<ProjectItem> GetEnumerator()
        {
            return (Enumerate(items)
                .Select(item => item)).GetEnumerator();
        }

        IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion

        #region Private Implementation
        private IEnumerable<ProjectItem> Enumerate(ProjectItems items)
        {
            foreach(ProjectItem item in items)
            {
                yield return item;

                foreach(ProjectItem subItem in Enumerate(item.ProjectItems))
                {
                    yield return subItem;
                }
            }
        }
        #endregion
    }
}