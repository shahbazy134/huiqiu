/***************************************************************************

Copyright (c) 2008 Microsoft Corporation. All rights reserved.

***************************************************************************/

using System;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace Microsoft.PowerCommands.Listeners
{
    /// <summary>
    /// Listener for RDT events
    /// </summary>
    public class RDTListener : IDisposable, IVsRunningDocTableEvents3
    {
        #region Delegates / Events
        /// <summary>
        /// 
        /// </summary>
        public delegate int OnAfterAttributeChangeEventHandler(uint docCookie, uint grfAttribs);
        /// <summary>
        /// 
        /// </summary>
        public delegate int OnAfterDocumentWindowHideEventHandler(uint docCookie, IVsWindowFrame pFrame);
        /// <summary>
        /// 
        /// </summary>
        public delegate int OnAfterFirstDocumentLockEventHandler(uint docCookie, uint dwRDTLockType, uint dwReadLocksRemaining, uint dwEditLocksRemaining);
        /// <summary>
        /// 
        /// </summary>
        public delegate int OnAfterSaveEventHandler(uint docCookie);
        /// <summary>
        /// 
        /// </summary>
        public delegate int OnBeforeDocumentWindowShowEventHandler(uint docCookie, int fFirstShow, IVsWindowFrame pFrame);
        /// <summary>
        /// 
        /// </summary>
        public delegate int OnBeforeLastDocumentUnlockEventHandler(uint docCookie, uint dwRDTLockType, uint dwReadLocksRemaining, uint dwEditLocksRemaining);
        /// <summary>
        /// 
        /// </summary>
        public delegate int OnAfterAttributeChangeExHandler(uint docCookie, uint grfAttribs, IVsHierarchy pHierOld, uint itemidOld, string pszMkDocumentOld, IVsHierarchy pHierNew, uint itemidNew, string pszMkDocumentNew);
        /// <summary>
        /// 
        /// </summary>
        public delegate int OnBeforeSaveHandler(uint docCookie);

        /// <summary>
        /// Occurs when [after attribute change].
        /// </summary>
        public event OnAfterAttributeChangeEventHandler AfterAttributeChange;
        /// <summary>
        /// Occurs when [after document window hide].
        /// </summary>
        public event OnAfterDocumentWindowHideEventHandler AfterDocumentWindowHide;
        /// <summary>
        /// Occurs when [after first document lock].
        /// </summary>
        public event OnAfterFirstDocumentLockEventHandler AfterFirstDocumentLock;
        /// <summary>
        /// Occurs when [after save].
        /// </summary>
        public event OnAfterSaveEventHandler AfterSave;
        /// <summary>
        /// Occurs when [before document window show].
        /// </summary>
        public event OnBeforeDocumentWindowShowEventHandler BeforeDocumentWindowShow;
        /// <summary>
        /// Occurs when [before last document unlock].
        /// </summary>
        public event OnBeforeLastDocumentUnlockEventHandler BeforeLastDocumentUnlock;
        /// <summary>
        /// Occurs when [after attribute change ex].
        /// </summary>
        public event OnAfterAttributeChangeExHandler AfterAttributeChangeEx;
        /// <summary>
        /// Occurs when [before save].
        /// </summary>
        public event OnBeforeSaveHandler BeforeSave;
        #endregion

        #region Fields
        private static volatile object Mutex;

        private bool isDisposed;
        private IServiceProvider serviceProvider;
        private uint eventsCookie;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes the <see cref="RDTListener"/> class.
        /// </summary>
        static RDTListener()
        {
            Mutex = new object();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RDTListener"/> class.
        /// </summary>
        /// <param name="provider">The provider.</param>
        public RDTListener(IServiceProvider provider)
        {
            this.serviceProvider = provider;
            this.RunningDocumentTable = new RunningDocumentTable(provider);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the running document table.
        /// </summary>
        /// <value>The running document table.</value>
        public RunningDocumentTable RunningDocumentTable { get; private set; }
        #endregion

        #region Public Implementation
        /// <summary>
        /// Initializes this instance.
        /// </summary>
        public void Initialize()
        {
            this.eventsCookie = this.RunningDocumentTable.Advise(this);
        }

        /// <summary>
        /// Called when [after attribute change].
        /// </summary>
        /// <param name="docCookie">The doc cookie.</param>
        /// <param name="grfAttribs">The GRF attribs.</param>
        /// <returns></returns>
        public int OnAfterAttributeChange(uint docCookie, uint grfAttribs)
        {
            if(AfterAttributeChange != null)
            {
                return AfterAttributeChange(docCookie, grfAttribs);
            }

            return VSConstants.S_OK;
        }

        /// <summary>
        /// Called when [after document window hide].
        /// </summary>
        /// <param name="docCookie">The doc cookie.</param>
        /// <param name="pFrame">The p frame.</param>
        /// <returns></returns>
        public int OnAfterDocumentWindowHide(uint docCookie, IVsWindowFrame pFrame)
        {
            if(AfterDocumentWindowHide != null)
            {
                return AfterDocumentWindowHide(docCookie, pFrame);
            }

            return VSConstants.S_OK;
        }

        /// <summary>
        /// Called when [after first document lock].
        /// </summary>
        /// <param name="docCookie">The doc cookie.</param>
        /// <param name="dwRDTLockType">Type of the dw RDT lock.</param>
        /// <param name="dwReadLocksRemaining">The dw read locks remaining.</param>
        /// <param name="dwEditLocksRemaining">The dw edit locks remaining.</param>
        /// <returns></returns>
        public int OnAfterFirstDocumentLock(uint docCookie, uint dwRDTLockType, uint dwReadLocksRemaining, uint dwEditLocksRemaining)
        {
            if(AfterFirstDocumentLock != null)
            {
                return AfterFirstDocumentLock(docCookie, dwRDTLockType, dwReadLocksRemaining, dwEditLocksRemaining);
            }

            return VSConstants.S_OK;
        }

        /// <summary>
        /// Called when [after save].
        /// </summary>
        /// <param name="docCookie">The doc cookie.</param>
        /// <returns></returns>
        public int OnAfterSave(uint docCookie)
        {
            if(AfterSave != null)
            {
                return AfterSave(docCookie);
            }

            return VSConstants.S_OK;
        }

        /// <summary>
        /// Called when [before document window show].
        /// </summary>
        /// <param name="docCookie">The doc cookie.</param>
        /// <param name="fFirstShow">The f first show.</param>
        /// <param name="pFrame">The p frame.</param>
        /// <returns></returns>
        public int OnBeforeDocumentWindowShow(uint docCookie, int fFirstShow, IVsWindowFrame pFrame)
        {
            if(BeforeDocumentWindowShow != null)
            {
                return BeforeDocumentWindowShow(docCookie, fFirstShow, pFrame);
            }

            return VSConstants.S_OK;
        }

        /// <summary>
        /// Called when [before last document unlock].
        /// </summary>
        /// <param name="docCookie">The doc cookie.</param>
        /// <param name="dwRDTLockType">Type of the dw RDT lock.</param>
        /// <param name="dwReadLocksRemaining">The dw read locks remaining.</param>
        /// <param name="dwEditLocksRemaining">The dw edit locks remaining.</param>
        /// <returns></returns>
        public int OnBeforeLastDocumentUnlock(uint docCookie, uint dwRDTLockType, uint dwReadLocksRemaining, uint dwEditLocksRemaining)
        {
            if(BeforeLastDocumentUnlock != null)
            {
                return BeforeLastDocumentUnlock(docCookie, dwRDTLockType, dwReadLocksRemaining, dwEditLocksRemaining);
            }

            return VSConstants.S_OK;
        }

        /// <summary>
        /// Called when [after attribute change ex].
        /// </summary>
        /// <param name="docCookie">The doc cookie.</param>
        /// <param name="grfAttribs">The GRF attribs.</param>
        /// <param name="pHierOld">The p hier old.</param>
        /// <param name="itemidOld">The itemid old.</param>
        /// <param name="pszMkDocumentOld">The PSZ mk document old.</param>
        /// <param name="pHierNew">The p hier new.</param>
        /// <param name="itemidNew">The itemid new.</param>
        /// <param name="pszMkDocumentNew">The PSZ mk document new.</param>
        /// <returns></returns>
        public int OnAfterAttributeChangeEx(uint docCookie, uint grfAttribs, IVsHierarchy pHierOld, uint itemidOld, string pszMkDocumentOld, IVsHierarchy pHierNew, uint itemidNew, string pszMkDocumentNew)
        {
            if(AfterAttributeChange != null)
            {
                return AfterAttributeChangeEx(docCookie, grfAttribs, pHierOld, itemidOld, pszMkDocumentOld, pHierNew, itemidNew, pszMkDocumentNew);
            }

            return VSConstants.S_OK;
        }

        /// <summary>
        /// Called when [before save].
        /// </summary>
        /// <param name="docCookie">The doc cookie.</param>
        /// <returns></returns>
        public int OnBeforeSave(uint docCookie)
        {
            if(BeforeSave != null)
            {
                return BeforeSave(docCookie);
            }

            return VSConstants.S_OK;
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
                    if(disposing && (this.eventsCookie != 0))
                    {
                        this.RunningDocumentTable.Unadvise(this.eventsCookie);
                        this.eventsCookie = 0;
                    }

                    this.isDisposed = true;
                }
            }
        }
        #endregion
    }
}