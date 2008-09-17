/***************************************************************************

Copyright (c) 2008 Microsoft Corporation. All rights reserved.

***************************************************************************/

using System;
using Microsoft.PowerCommands.Extensions;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

namespace Microsoft.PowerCommands.Listeners
{
    /// <summary>
    /// Listener for solution events
    /// </summary>
    public class SolutionListener : IDisposable, IVsSolutionEvents3
    {
        #region Delegates / Events
        /// <summary>
        /// 
        /// </summary>
        public delegate int OnAfterCloseSolutionHandler(object pUnkReserved);
        /// <summary>
        /// 
        /// </summary>
        public delegate int OnAfterOpenSolutionHandler(object pUnkReserved, int fNewSolution);

        /// <summary>
        /// Occurs when [after close solution].
        /// </summary>
        public event OnAfterCloseSolutionHandler AfterCloseSolution;

        /// <summary>
        /// Occurs when [after open solution].
        /// </summary>
        public event OnAfterOpenSolutionHandler AfterOpenSolution;
        #endregion

        #region Fields
        private static volatile object Mutex;

        private bool isDisposed;
        private IServiceProvider serviceProvider;
        private uint eventsCookie;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the solution.
        /// </summary>
        /// <value>The solution.</value>
        public IVsSolution Solution { get; private set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes the <see cref="RDTListener"/> class.
        /// </summary>
        static SolutionListener()
        {
            Mutex = new object();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RDTListener"/> class.
        /// </summary>
        /// <param name="provider">The provider.</param>
        public SolutionListener(IServiceProvider provider)
        {
            this.serviceProvider = provider;
            this.Solution = provider.GetService<SVsSolution, IVsSolution>();
        }
        #endregion

        #region Public Implementation
        /// <summary>
        /// Initializes this instance.
        /// </summary>
        public void Initialize()
        {
            this.Solution.AdviseSolutionEvents(this, out this.eventsCookie);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Called when [after close solution].
        /// </summary>
        /// <param name="pUnkReserved">The p unk reserved.</param>
        /// <returns></returns>
        public int OnAfterCloseSolution(object pUnkReserved)
        {
            if(AfterCloseSolution != null)
            {
                return AfterCloseSolution(pUnkReserved);
            }

            return VSConstants.S_OK;
        }

        /// <summary>
        /// Called when [after closing children].
        /// </summary>
        /// <param name="pHierarchy">The p hierarchy.</param>
        /// <returns></returns>
        public int OnAfterClosingChildren(IVsHierarchy pHierarchy)
        {
            return VSConstants.S_OK;
        }

        /// <summary>
        /// Called when [after load project].
        /// </summary>
        /// <param name="pStubHierarchy">The p stub hierarchy.</param>
        /// <param name="pRealHierarchy">The p real hierarchy.</param>
        /// <returns></returns>
        public int OnAfterLoadProject(IVsHierarchy pStubHierarchy, IVsHierarchy pRealHierarchy)
        {
            return VSConstants.S_OK;
        }

        /// <summary>
        /// Called when [after merge solution].
        /// </summary>
        /// <param name="pUnkReserved">The p unk reserved.</param>
        /// <returns></returns>
        public int OnAfterMergeSolution(object pUnkReserved)
        {
            return VSConstants.S_OK;
        }

        /// <summary>
        /// Called when [after open project].
        /// </summary>
        /// <param name="pHierarchy">The p hierarchy.</param>
        /// <param name="fAdded">The f added.</param>
        /// <returns></returns>
        public int OnAfterOpenProject(IVsHierarchy pHierarchy, int fAdded)
        {
            return VSConstants.S_OK;
        }

        /// <summary>
        /// Called when [after open solution].
        /// </summary>
        /// <param name="pUnkReserved">The p unk reserved.</param>
        /// <param name="fNewSolution">The f new solution.</param>
        /// <returns></returns>
        public int OnAfterOpenSolution(object pUnkReserved, int fNewSolution)
        {
            if(AfterOpenSolution != null)
            {
                return AfterOpenSolution(pUnkReserved, fNewSolution);
            }

            return VSConstants.S_OK;
        }

        /// <summary>
        /// Called when [after opening children].
        /// </summary>
        /// <param name="pHierarchy">The p hierarchy.</param>
        /// <returns></returns>
        public int OnAfterOpeningChildren(IVsHierarchy pHierarchy)
        {
            return VSConstants.S_OK;
        }

        /// <summary>
        /// Called when [before close project].
        /// </summary>
        /// <param name="pHierarchy">The p hierarchy.</param>
        /// <param name="fRemoved">The f removed.</param>
        /// <returns></returns>
        public int OnBeforeCloseProject(IVsHierarchy pHierarchy, int fRemoved)
        {
            return VSConstants.S_OK;
        }

        /// <summary>
        /// Called when [before close solution].
        /// </summary>
        /// <param name="pUnkReserved">The p unk reserved.</param>
        /// <returns></returns>
        public int OnBeforeCloseSolution(object pUnkReserved)
        {
            return VSConstants.S_OK;
        }

        /// <summary>
        /// Called when [before closing children].
        /// </summary>
        /// <param name="pHierarchy">The p hierarchy.</param>
        /// <returns></returns>
        public int OnBeforeClosingChildren(IVsHierarchy pHierarchy)
        {
            return VSConstants.S_OK;
        }

        /// <summary>
        /// Called when [before opening children].
        /// </summary>
        /// <param name="pHierarchy">The p hierarchy.</param>
        /// <returns></returns>
        public int OnBeforeOpeningChildren(IVsHierarchy pHierarchy)
        {
            return VSConstants.S_OK;
        }

        /// <summary>
        /// Called when [before unload project].
        /// </summary>
        /// <param name="pRealHierarchy">The p real hierarchy.</param>
        /// <param name="pStubHierarchy">The p stub hierarchy.</param>
        /// <returns></returns>
        public int OnBeforeUnloadProject(IVsHierarchy pRealHierarchy, IVsHierarchy pStubHierarchy)
        {
            return VSConstants.S_OK;
        }

        /// <summary>
        /// Called when [query close project].
        /// </summary>
        /// <param name="pHierarchy">The p hierarchy.</param>
        /// <param name="fRemoving">The f removing.</param>
        /// <param name="pfCancel">The pf cancel.</param>
        /// <returns></returns>
        public int OnQueryCloseProject(IVsHierarchy pHierarchy, int fRemoving, ref int pfCancel)
        {
            return VSConstants.S_OK;
        }

        /// <summary>
        /// Called when [query close solution].
        /// </summary>
        /// <param name="pUnkReserved">The p unk reserved.</param>
        /// <param name="pfCancel">The pf cancel.</param>
        /// <returns></returns>
        public int OnQueryCloseSolution(object pUnkReserved, ref int pfCancel)
        {
            return VSConstants.S_OK;
        }

        /// <summary>
        /// Called when [query unload project].
        /// </summary>
        /// <param name="pRealHierarchy">The p real hierarchy.</param>
        /// <param name="pfCancel">The pf cancel.</param>
        /// <returns></returns>
        public int OnQueryUnloadProject(IVsHierarchy pRealHierarchy, ref int pfCancel)
        {
            return VSConstants.S_OK;
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
                        this.Solution.AdviseSolutionEvents(this, out this.eventsCookie);
                        this.eventsCookie = 0;
                    }

                    this.isDisposed = true;
                }
            }
        }
        #endregion
    }
}