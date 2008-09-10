/***************************************************************************

Copyright (c) 2008 Microsoft Corporation. All rights reserved.

***************************************************************************/

using System;
using System.ComponentModel.Design;
using System.Linq;
using System.Runtime.InteropServices;
using EnvDTE;
using Microsoft.PowerCommands.Commands;
using Microsoft.PowerCommands.Listeners;
using Microsoft.PowerCommands.OptionPages;
using Microsoft.PowerCommands.Services;
using Microsoft.PowerCommands.ToolWindows;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using VSLangProj;

namespace Microsoft.PowerCommands
{
    /// <summary>
    /// Class that implements the Package class
    /// </summary>        
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [ProvideLoadKey("Standard", "1.0", "PowerCommands for Visual Studio 2008", "Microsoft PLK", 1)]
    [InstalledProductRegistration(true, "#9394", "#25288", "1.0", IconResourceID = 57077, LanguageIndependentName = "PowerCommands for Visual Studio 2008")]
    [DefaultRegistryRoot(@"Software\Microsoft\VisualStudio\9.0")]
    [ProvideMenuResource(1000, 1)]
    [ProvideAutoLoad("{ADFC4E64-0397-11D1-9F4E-00A0C911004F}")]
    [ProvideService(typeof(SCommandManagerService), ServiceName = "CommandManagerService")]
    [ProvideService(typeof(SUndoCloseManagerService), ServiceName = "UndoCloseManagerService")]
    [ProvideProfileAttribute(typeof(CommandsPage), "PowerCommands", "Commands", 15600, 1912, true, DescriptionResourceID = 197)]
    [ProvideOptionPageAttribute(typeof(CommandsPage), "PowerCommands", "Commands", 15600, 1912, true)]
    [ProvideProfileAttribute(typeof(GeneralPage), "PowerCommands", "General", 15600, 4606, true, DescriptionResourceID = 2891)]
    [ProvideOptionPageAttribute(typeof(GeneralPage), "PowerCommands", "General", 15600, 4606, true)]
    [ProvideToolWindowVisibility(typeof(UndoCloseToolWindow), "{F1536EF8-92EC-443C-9ED7-FDADF150DA82}")]
    [ProvideToolWindow(typeof(UndoCloseToolWindow), MultiInstances = false, Style = VsDockStyle.Tabbed, Orientation = ToolWindowOrientation.Top, Transient = true, Window = "{D78612C7-9962-4B83-95D9-268046DAD23A}")]
    [Guid("24E33DBF-CADF-4DA8-ACFE-566366FC8468")]
    public sealed class PowerCommandsPackage : Package, IVsInstalledProduct
    {
        #region Fields
        DocumentInfo docInfo;
        RDTListener RDTListener;
        SolutionListener solutionListener;
        DocumentListener documentListener;
        #endregion

        #region Properties
        private CommandsPage commandsPage;
        /// <summary>
        /// Gets the commands page.
        /// </summary>
        /// <value>The commands page.</value>
        public CommandsPage CommandsPage
        {
            get
            {
                if(commandsPage == null)
                {
                    commandsPage =
                        this.GetDialogPage(typeof(CommandsPage)) as CommandsPage;
                }

                return commandsPage;
            }
        }

        private GeneralPage generalPage;
        /// <summary>
        /// Gets the general page.
        /// </summary>
        /// <value>The general page.</value>
        public GeneralPage GeneralPage
        {
            get
            {
                if(generalPage == null)
                {
                    generalPage =
                        this.GetDialogPage(typeof(GeneralPage)) as GeneralPage;
                }

                return generalPage;
            }
        }

        private IUndoCloseManagerService undoCloseManager;
        /// <summary>
        /// Gets the undo close manager.
        /// </summary>
        /// <value>The undo close manager.</value>
        public IUndoCloseManagerService UndoCloseManager
        {
            get
            {
                if(undoCloseManager == null)
                {
                    undoCloseManager = this.GetService(typeof(SUndoCloseManagerService)) as IUndoCloseManagerService;
                }

                return undoCloseManager;
            }
        }

        private DTE dte;
        /// <summary>
        /// Gets the DTE.
        /// </summary>
        /// <value>The DTE.</value>
        public DTE Dte
        {
            get
            {
                if(dte == null)
                {
                    dte = this.GetService(typeof(DTE)) as DTE;
                }

                return dte;
            }
        }

        private UndoCloseToolWindow undoCloseToolWindow;
        /// <summary>
        /// Gets the undo close tool window.
        /// </summary>
        /// <value>The undo close tool window.</value>
        public UndoCloseToolWindow UndoCloseToolWindow
        {
            get
            {
                if(undoCloseToolWindow == null)
                {
                    undoCloseToolWindow =
                        this.FindToolWindow(typeof(UndoCloseToolWindow), 0, true) as UndoCloseToolWindow;
                }

                return undoCloseToolWindow;
            }
        }
        #endregion

        #region Public Implementation
        /// <summary>
        /// Returns the id for bmp of the splash screen
        /// </summary>
        /// <returns></returns>
        public int IdBmpSplash(out uint pIdBmp)
        {
            pIdBmp = 0;
            return Microsoft.VisualStudio.VSConstants.S_OK;
        }

        /// <summary>
        /// Returns the Id for the ico logo for aboutbox.
        /// </summary>
        /// <returns></returns>
        public int IdIcoLogoForAboutbox(out uint pIdIco)
        {
            pIdIco = 57077;
            return Microsoft.VisualStudio.VSConstants.S_OK;
        }

        /// <summary>
        /// Returns the official name
        /// </summary>
        /// <returns></returns>
        public int OfficialName(out string pbstrName)
        {
            pbstrName = GetResourceString("9394");
            return Microsoft.VisualStudio.VSConstants.S_OK;
        }

        /// <summary>
        /// Returns the product details.
        /// </summary>
        /// <returns></returns>
        public int ProductDetails(out string pbstrProductDetails)
        {
            pbstrProductDetails = GetResourceString("25288");
            return Microsoft.VisualStudio.VSConstants.S_OK;
        }

        /// <summary>
        /// Returns the product id.
        /// </summary>
        /// <returns></returns>
        public int ProductID(out string pbstrPID)
        {
            pbstrPID = GetResourceString("41182");
            return Microsoft.VisualStudio.VSConstants.S_OK;
        }
        #endregion

        #region Protected Implementation
        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put any initialization code that relies on services provided by Visual Studio.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            (this as IServiceContainer).AddService(
                typeof(SCommandManagerService),
                new ServiceCreatorCallback(CreateCommandManagerService),
                true);

            (this as IServiceContainer).AddService(
                typeof(SUndoCloseManagerService),
                new ServiceCreatorCallback(CreateUndoCloseManagerService),
                true);

            CommandSet commandSet = new CommandSet(this);
            commandSet.Initialize();

            RDTListener = new RDTListener(this);
            RDTListener.Initialize();
            RDTListener.BeforeSave += new RDTListener.OnBeforeSaveHandler(RDTListener_BeforeSave);
            RDTListener.AfterDocumentWindowHide += new RDTListener.OnAfterDocumentWindowHideEventHandler(RDTListener_AfterDocumentWindowHide);

            solutionListener = new SolutionListener(this);
            solutionListener.Initialize();
            solutionListener.AfterCloseSolution += new SolutionListener.OnAfterCloseSolutionHandler(solutionListener_AfterCloseSolution);
            solutionListener.AfterOpenSolution += new SolutionListener.OnAfterOpenSolutionHandler(solutionListener_AfterOpenSolution);

            documentListener = new DocumentListener(this);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            RDTListener.Dispose();
            solutionListener.Dispose();
            documentListener.Dispose();
        }
        #endregion

        #region Event Handlers
        int RDTListener_AfterDocumentWindowHide(uint docCookie, IVsWindowFrame pFrame)
        {
            if(CommandsPage.DisabledCommands.SingleOrDefault(
                cmd => cmd.Guid.Equals(typeof(UndoCloseCommand).GUID) &&
                    cmd.ID.Equals((int)UndoCloseCommand.cmdidUndoCloseCommand)) == null)
            {
                //Prevent being called twice
                if(UndoCloseManager.CurrentDocument == null ||
                    !UndoCloseManager.CurrentDocument.DocumentPath.Equals(docInfo.DocumentPath))
                {
                    UndoCloseManager.PushDocument(docInfo);
                    UndoCloseToolWindow.Control.UpdateDocumentList();
                }
            }

            return Microsoft.VisualStudio.VSConstants.S_OK;
        }

        int RDTListener_BeforeSave(uint docCookie)
        {
            if(GeneralPage.FormatOnSave || GeneralPage.RemoveAndSortUsingsOnSave)
            {
                Document activeDocument = Dte.ActiveDocument;
                Document documentToBeSaved = GetDocumentToBeSaved(docCookie);

                if(documentToBeSaved.ProjectItem.FileCodeModel != null &&
                    (documentToBeSaved.ProjectItem.ContainingProject.Kind == PrjKind.prjKindCSharpProject ||
                        documentToBeSaved.ProjectItem.ContainingProject.Kind == PrjKind.prjKindVBProject))
                {
                    //HAAK: This is for the Save All command
                    //To fire the Edit.FormatDocument/Edit.RemoveAndSort command the document needs to be active
                    if(!activeDocument.Equals(documentToBeSaved))
                    {
                        documentToBeSaved.Activate();
                    }

                    if(GeneralPage.FormatOnSave)
                    {
                        Dte.ExecuteCommand("Edit.FormatDocument", string.Empty);
                    }

                    if(documentToBeSaved.ProjectItem.ContainingProject.Kind == PrjKind.prjKindCSharpProject)
                    {
                        if(GeneralPage.RemoveAndSortUsingsOnSave)
                        {
                            Dte.ExecuteCommand("Edit.RemoveAndSort", string.Empty);
                        }
                    }

                    activeDocument.Activate();
                }
            }

            return Microsoft.VisualStudio.VSConstants.S_OK;
        }

        void documentListener_DocumentClosing(Document Document)
        {
            int cursorLine = 1;
            int cursorOffset = 1;

            TextSelection selection = Document.Selection as TextSelection;

            if(selection != null)
            {
                cursorLine = selection.ActivePoint.Line;
                cursorOffset = selection.ActivePoint.LineCharOffset;
            }

            if(docInfo != null && !docInfo.DocumentPath.Equals(Document.FullName))
            {
                if(Document.FullName.IndexOf(docInfo.DocumentPath) == -1)
                {
                    //Prevent adding code behind documents
                    docInfo = new DocumentInfo(Document.FullName, cursorLine, cursorOffset, GetViewKind(Document));
                }
            }
            else
            {
                docInfo = new DocumentInfo(Document.FullName, cursorLine, cursorOffset, GetViewKind(Document));
            }
        }

        int solutionListener_AfterOpenSolution(object pUnkReserved, int fNewSolution)
        {
            documentListener.Initialize();
            documentListener.DocumentClosing += new DocumentListener.DocumentClosingEventHandler(documentListener_DocumentClosing);

            return Microsoft.VisualStudio.VSConstants.S_OK;
        }

        int solutionListener_AfterCloseSolution(object pUnkReserved)
        {
            UndoCloseManager.ClearDocuments();
            UndoCloseToolWindow.Control.ClearDocumentList();

            return Microsoft.VisualStudio.VSConstants.S_OK;
        }
        #endregion

        #region Private Implementation
        private Document GetDocumentToBeSaved(uint docCookie)
        {
            RunningDocumentTable rdt = new RunningDocumentTable(this);

            return Dte.Documents.OfType<Document>()
                    .Single(
                        document => document.FullName.Equals(
                            rdt.GetDocumentInfo(docCookie).Moniker));
        }

        private object CreateCommandManagerService(IServiceContainer container, Type serviceType)
        {
            if(container != this)
            {
                return null;
            }

            if(typeof(SCommandManagerService) == serviceType)
            {
                return new CommandManagerService();
            }

            return null;
        }

        private object CreateUndoCloseManagerService(IServiceContainer container, Type serviceType)
        {
            if(container != this)
            {
                return null;
            }

            if(typeof(SUndoCloseManagerService) == serviceType)
            {
                return new UndoCloseManagerService();
            }

            return null;
        }

        private string GetResourceString(string resourceName)
        {
            string resourceValue;

            Microsoft.VisualStudio.Shell.Interop.IVsResourceManager resourceManager =
                (Microsoft.VisualStudio.Shell.Interop.IVsResourceManager)GetService(typeof(Microsoft.VisualStudio.Shell.Interop.SVsResourceManager));

            if(resourceManager == null)
            {
                throw new InvalidOperationException(
                    "Could not get SVsResourceManager service. Make sure that the package is sited before calling this method");
            }

            Guid packageGuid = this.GetType().GUID;
            int hr = resourceManager.LoadResourceString(
                ref packageGuid, -1, resourceName, out resourceValue);

            Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(hr);

            return resourceValue;
        }

        private string GetViewKind(Document document)
        {
            if(document.Selection == null)
            {
                //TODO: Find is there is another to get the DocView type
                return EnvDTE.Constants.vsViewKindDesigner;
            }
            else
            {
                return EnvDTE.Constants.vsViewKindPrimary;
            }
        }        
        #endregion
    }
}