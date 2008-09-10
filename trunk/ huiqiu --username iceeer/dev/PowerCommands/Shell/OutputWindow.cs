/***************************************************************************

Copyright (c) 2008 Microsoft Corporation. All rights reserved.

***************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.PowerCommands.Extensions;

namespace Microsoft.PowerCommands.Shell
{
    /// <summary>
    /// Class to interact with the VS output window
    /// </summary>
    public class OutputWindow
    {
        #region Fields
        IServiceProvider serviceProvider;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="OutputWindow"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public OutputWindow(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }
        #endregion

        #region Properties
        private IVsOutputWindow window;

        /// <summary>
        /// Gets the window.
        /// </summary>
        /// <value>The window.</value>
        protected IVsOutputWindow Window
        {
            get
            {
                if(window == null)
                {
                    window = this.serviceProvider.GetService<SVsOutputWindow, IVsOutputWindow>();
                }

                return window;
            }
        }
        #endregion

        #region Public Implementation
        /// <summary>
        /// Activates the pane.
        /// </summary>
        /// <param name="guidPane">The GUID pane.</param>
        public void ActivatePane(Guid guidPane)
        {
            IVsOutputWindowPane pane = null;

            if(ErrorHandler.Succeeded(GetPane(guidPane, out pane)) && (pane != null))
            {
                pane.Activate();
            }
        }

        /// <summary>
        /// Creates the pane.
        /// </summary>
        /// <param name="paneName">Name of the pane.</param>
        /// <param name="visible">if set to <c>true</c> [visible].</param>
        /// <param name="clearWithSolution">if set to <c>true</c> [clear with solution].</param>
        /// <returns></returns>
        public Guid CreatePane(string paneName, bool visible, bool clearWithSolution)
        {
            IVsOutputWindowPane pane = null;
            Guid guidPane = Guid.NewGuid();

            if(string.IsNullOrEmpty(paneName))
            {
                throw new ArgumentNullException("paneName");
            }

            if(ErrorHandler.Failed(GetPane(guidPane, out pane)) && (pane == null))
            {
                if(ErrorHandler.Succeeded(window.CreatePane(ref guidPane, paneName, visible ? 1 : 0, clearWithSolution ? 1 : 0)))
                {
                    window.GetPane(ref guidPane, out pane);
                }
            }
            else if(!visible)
            {
                pane.Hide();
            }
            else
            {
                pane.Activate();
            }

            if(pane == null)
            {
                throw new InvalidOperationException();
            }

            return guidPane;
        }

        /// <summary>
        /// Deletes the pane.
        /// </summary>
        /// <param name="guidPane">The GUID pane.</param>
        public void DeletePane(Guid guidPane)
        {
            IVsOutputWindowPane pane;

            if(guidPane == Guid.Empty)
            {
                throw new ArgumentNullException("guidPane");
            }

            if(ErrorHandler.Succeeded(GetPane(guidPane, out pane)) && pane != null)
            {
                Guid rguidPane = guidPane;

                this.window.DeletePane(ref rguidPane);
            }
        }

        /// <summary>
        /// Writes the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void WriteMessage(string message)
        {
            WriteMessage(Guid.Empty, message);
        }

        /// <summary>
        /// Writes the message.
        /// </summary>
        /// <param name="guidPane">The GUID pane.</param>
        /// <param name="message">The message.</param>
        public void WriteMessage(Guid guidPane, string message)
        {
            if(!string.IsNullOrEmpty(message))
            {
                IVsOutputWindowPane pane;

                message = FormatMessage(message);

                Guid guid = guidPane;

                if(guid == Guid.Empty)
                {
                    guid = VSConstants.GUID_OutWindowGeneralPane;
                }

                if(ErrorHandler.Failed(GetPane(guid, out pane)) || (pane == null))
                {
                    if(guid.Equals(VSConstants.GUID_OutWindowGeneralPane))
                    {
                        pane = this.serviceProvider.GetService<SVsGeneralOutputWindowPane, IVsOutputWindowPane>();
                    }
                }

                if(pane != null)
                {
                    pane.OutputStringThreadSafe(message);
                }
            }
        }

        /// <summary>
        /// Clears all messages
        /// </summary>
        /// <param name="guidPane">The GUID pane.</param>
        public void Clear(Guid guidPane)
        {
            IVsOutputWindowPane pane;
            Guid guid = guidPane;

            if(guid == Guid.Empty)
            {
                guid = VSConstants.GUID_OutWindowGeneralPane;
            }

            if(ErrorHandler.Succeeded(GetPane(guid, out pane)) && pane != null)
            {
                pane.Clear();
            }
        } 
        #endregion

        #region Private Implementation
        private int GetPane(Guid guidPane, out IVsOutputWindowPane pane)
        {
            if(this.Window == null)
            {
                throw new InvalidOperationException();
            }

            Guid rguidPane = guidPane;

            return this.Window.GetPane(ref rguidPane, out pane);
        }

        private static string FormatMessage(string message)
        {
            string formattedMessage = message;

            if(!formattedMessage.EndsWith("\r\n", StringComparison.OrdinalIgnoreCase))
            {
                formattedMessage = string.Concat(formattedMessage, "\r\n");
            }

            return formattedMessage;
        } 
        #endregion
    }
}