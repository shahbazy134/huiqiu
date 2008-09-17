/***************************************************************************

Copyright (c) 2008 Microsoft Corporation. All rights reserved.

***************************************************************************/

using System;
using Microsoft.VisualStudio.Shell;

namespace Microsoft.PowerCommands.Shell
{
    /// <summary>
    /// Class to interact with the VS error list window
    /// </summary>
    public class ErrorListWindow
    {
        #region Fields
        IServiceProvider serviceProvider;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorListWindow"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public ErrorListWindow(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }
        #endregion

        #region Properties
        private ErrorListProvider errorListProvider;

        /// <summary>
        /// Gets the error list provider.
        /// </summary>
        /// <value>The error list provider.</value>
        protected ErrorListProvider ErrorListProvider
        {
            get
            {
                if(errorListProvider == null)
                {
                    errorListProvider = new ErrorListProvider(this.serviceProvider);
                }

                return errorListProvider;
            }
        }
        #endregion

        #region Public Implementation
        /// <summary>
        /// Shows the errors.
        /// </summary>
        public void Show()
        {
            this.ErrorListProvider.Show();        
        }

        /// <summary>
        /// Writes the error.
        /// </summary>
        /// <param name="message">The message.</param>
        public void WriteError(string message)
        {
            ErrorTask errorTask = new ErrorTask();

            errorTask.CanDelete = false;
            errorTask.ErrorCategory = TaskErrorCategory.Error;
            errorTask.Text = message;

            this.ErrorListProvider.Tasks.Add(errorTask);
            Show();
        }

        /// <summary>
        /// Clears the errors.
        /// </summary>
        public void ClearErrors()
        {
            this.ErrorListProvider.Tasks.Clear();
        }

        #endregion
    }
}
