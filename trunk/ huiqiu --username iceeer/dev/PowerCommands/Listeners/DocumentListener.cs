/***************************************************************************

Copyright (c) 2008 Microsoft Corporation. All rights reserved.

***************************************************************************/

using System;
using EnvDTE;
using Microsoft.PowerCommands.Extensions;

namespace Microsoft.PowerCommands.Listeners
{
    /// <summary>
    /// Listener for document events
    /// </summary>
    public class DocumentListener : IDisposable
    {
        #region Delegates / Events
        /// <summary>
        /// 
        /// </summary>
        public delegate void DocumentClosingEventHandler(Document Document);

        /// <summary>
        /// Occurs when [document closing].
        /// </summary>
        public event DocumentClosingEventHandler DocumentClosing;
        #endregion

        #region Fields
        private static volatile object Mutex;

        private bool isDisposed;
        private IServiceProvider serviceProvider;
        private DTE dte;
        #endregion

        #region Properties
        private DocumentEvents documentEvents;

        private DocumentEvents DocumentEvents
        {
            get
            {
                if(documentEvents == null)
                {
                    documentEvents = dte.Events.GetObject("DocumentEvents") as DocumentEvents;
                }

                return documentEvents;
            }
        }
        #endregion

        #region Constructors
        static DocumentListener()
        {
            Mutex = new object();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentListener"/> class.
        /// </summary>
        /// <param name="provider">The provider.</param>
        public DocumentListener(IServiceProvider provider)
        {
            this.serviceProvider = provider;
        }
        #endregion

        #region Public Implementation
        /// <summary>
        /// Initializes this instance.
        /// </summary>
        public void Initialize()
        {
            dte = this.serviceProvider.GetService<DTE>();

            if(DocumentEvents != null)
            {
                DocumentEvents.DocumentClosing += new _dispDocumentEvents_DocumentClosingEventHandler(OnDocumentClosing);
            }
        }

        /// <summary>
        /// Called when [document closing].
        /// </summary>
        /// <param name="Document">The document.</param>
        public void OnDocumentClosing(Document Document)
        {
            if(DocumentClosing != null)
            {
                DocumentClosing(Document);
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Private Implementation
        private void Dispose(bool disposing)
        {
            if(!this.isDisposed)
            {
                lock(Mutex)
                {
                    if(DocumentEvents != null)
                    {
                        DocumentEvents.DocumentClosing -= OnDocumentClosing;
                    }

                    this.isDisposed = true;
                }
            }
        }
        #endregion
    }
}
