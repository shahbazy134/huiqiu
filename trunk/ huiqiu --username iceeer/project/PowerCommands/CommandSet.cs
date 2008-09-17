/***************************************************************************

Copyright (c) 2008 Microsoft Corporation. All rights reserved.

***************************************************************************/

using System;
using System.ComponentModel.Design;
using Microsoft.PowerCommands.Commands;
using Microsoft.PowerCommands.Extensions;
using Microsoft.PowerCommands.Services;
using Microsoft.VisualStudio.Shell;

namespace Microsoft.PowerCommands
{
    /// <summary>
    /// Class that contains all defined commands
    /// </summary>
    internal class CommandSet
    {
        private IServiceProvider serviceProvider;
        OleMenuCommandService menuCommandService;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandSet"/> class.
        /// </summary>
        /// <param name="provider">The provider.</param>
        public CommandSet(IServiceProvider provider)
        {
            this.serviceProvider = provider;
            menuCommandService = this.serviceProvider.GetService<IMenuCommandService, OleMenuCommandService>();
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        public void Initialize()
        {
            RegisterMenuCommands();
        }

        private void RegisterMenuCommands()
        {
            ICommandManagerService commandManager =
                this.serviceProvider.GetService<SCommandManagerService, ICommandManagerService>();

            OleMenuCommand openVSCommandPromptCommand = new OpenVSCommandPromptCommand(this.serviceProvider);
            menuCommandService.AddCommand(openVSCommandPromptCommand);
            commandManager.RegisterCommand(openVSCommandPromptCommand);

            OleMenuCommand transformTemplatesCommand = new TransformTemplatesCommand(this.serviceProvider);
            menuCommandService.AddCommand(transformTemplatesCommand);
            commandManager.RegisterCommand(transformTemplatesCommand);

            OleMenuCommand collapseProjectsCommand = new CollapseProjectsCommand(this.serviceProvider);
            menuCommandService.AddCommand(collapseProjectsCommand);
            commandManager.RegisterCommand(collapseProjectsCommand);

            OleMenuCommand openContainingFolderCommand = new OpenContainingFolderCommand(this.serviceProvider);
            menuCommandService.AddCommand(openContainingFolderCommand);
            commandManager.RegisterCommand(openContainingFolderCommand);

            OleMenuCommand extractToConstantCommand = new ExtractToConstantCommand(this.serviceProvider);
            menuCommandService.AddCommand(extractToConstantCommand);
            commandManager.RegisterCommand(extractToConstantCommand);

            OleMenuCommand editProjectFileCommand = new EditProjectFileCommand(this.serviceProvider);
            menuCommandService.AddCommand(editProjectFileCommand);
            commandManager.RegisterCommand(editProjectFileCommand);

            OleMenuCommand copyAsProjectReferenceCommand = new CopyAsProjectReferenceCommand(this.serviceProvider);
            menuCommandService.AddCommand(copyAsProjectReferenceCommand);
            commandManager.RegisterCommand(copyAsProjectReferenceCommand);

            OleMenuCommand copyReferenceCommand = new CopyReferenceCommand(this.serviceProvider);
            menuCommandService.AddCommand(copyReferenceCommand);
            commandManager.RegisterCommand(copyReferenceCommand);

            OleMenuCommand copyReferencesCommand = new CopyReferencesCommand(this.serviceProvider);
            menuCommandService.AddCommand(copyReferencesCommand);
            commandManager.RegisterCommand(copyReferencesCommand);

            OleMenuCommand pasteReferenceCommand = new PasteReferenceCommand(this.serviceProvider);
            menuCommandService.AddCommand(pasteReferenceCommand);
            commandManager.RegisterCommand(pasteReferenceCommand);

            OleMenuCommand clearRecentFileListCommand = new ClearRecentFileListCommand(this.serviceProvider);
            menuCommandService.AddCommand(clearRecentFileListCommand);
            commandManager.RegisterCommand(clearRecentFileListCommand);

            OleMenuCommand clearRecentProjectListCommand = new ClearRecentProjectListCommand(this.serviceProvider);
            menuCommandService.AddCommand(clearRecentProjectListCommand);
            commandManager.RegisterCommand(clearRecentProjectListCommand);

            OleMenuCommand reloadProjectsCommand = new ReloadProjectsCommand(this.serviceProvider);
            menuCommandService.AddCommand(reloadProjectsCommand);
            commandManager.RegisterCommand(reloadProjectsCommand);

            OleMenuCommand unloadProjectsCommand = new UnloadProjectsCommand(this.serviceProvider);
            menuCommandService.AddCommand(unloadProjectsCommand);
            commandManager.RegisterCommand(unloadProjectsCommand);

            OleMenuCommand removeSortUsingsCommand = new RemoveSortUsingsCommand(this.serviceProvider);
            menuCommandService.AddCommand(removeSortUsingsCommand);
            commandManager.RegisterCommand(removeSortUsingsCommand);

            OleMenuCommand copyClassCommand = new CopyClassCommand(this.serviceProvider);
            menuCommandService.AddCommand(copyClassCommand);
            commandManager.RegisterCommand(copyClassCommand);

            OleMenuCommand pasteClassCommand = new PasteClassCommand(this.serviceProvider);
            menuCommandService.AddCommand(pasteClassCommand);
            commandManager.RegisterCommand(pasteClassCommand);

            OleMenuCommand closeAllDocumentsCommand = new CloseAllDocumentsCommand(this.serviceProvider);
            menuCommandService.AddCommand(closeAllDocumentsCommand);
            commandManager.RegisterCommand(closeAllDocumentsCommand);

            OleMenuCommand copyPathCommand = new CopyPathCommand(this.serviceProvider);
            menuCommandService.AddCommand(copyPathCommand);
            commandManager.RegisterCommand(copyPathCommand);

            OleMenuCommand emailCodeSnippetCommand = new EmailCodeSnippetCommand(this.serviceProvider);
            menuCommandService.AddCommand(emailCodeSnippetCommand);
            commandManager.RegisterCommand(emailCodeSnippetCommand);

            OleMenuCommand insertGuidAttributeCommand = new InsertGuidAttributeCommand(this.serviceProvider);
            menuCommandService.AddCommand(insertGuidAttributeCommand);
            commandManager.RegisterCommand(insertGuidAttributeCommand);

            OleMenuCommand undoCloseCommand = new UndoCloseCommand(this.serviceProvider);
            menuCommandService.AddCommand(undoCloseCommand);
            commandManager.RegisterCommand(undoCloseCommand);

            OleMenuCommand clearAllPanesCommand = new ClearAllPanesCommand(this.serviceProvider);
            menuCommandService.AddCommand(clearAllPanesCommand);
            commandManager.RegisterCommand(clearAllPanesCommand);

            OleMenuCommand showAllFilesCommand = new ShowAllFilesCommand(this.serviceProvider);
            menuCommandService.AddCommand(showAllFilesCommand);
            commandManager.RegisterCommand(showAllFilesCommand);

            OleMenuCommand showUndoCloseToolWindowCommand = new ShowUndoCloseToolWindowCommand(this.serviceProvider);
            menuCommandService.AddCommand(showUndoCloseToolWindowCommand);
        }
    }
}