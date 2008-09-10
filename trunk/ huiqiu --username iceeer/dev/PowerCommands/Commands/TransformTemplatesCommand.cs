/***************************************************************************

Copyright (c) 2008 Microsoft Corporation. All rights reserved.

***************************************************************************/

using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using EnvDTE;
using Microsoft.PowerCommands.Extensions;
using Microsoft.PowerCommands.Linq;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using VSLangProj;

namespace Microsoft.PowerCommands.Commands
{
    /// <summary>
    /// Command that executes the TextTemplatingFileGenerator custom tool
    /// </summary>
    [Guid("06743131-62C0-406A-8D14-0D487A579D5F")]
    [DisplayName("Transform Templates")]
    internal class TransformTemplatesCommand : DynamicCommand
    {
        #region Constants
        public const uint cmdidTransformTemplatesCommand = 0x19EF;

        private const string Extension = ".tt";
        private const string CustomToolName = "TextTemplatingFileGenerator";
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="TransformTemplatesCommand"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public TransformTemplatesCommand(IServiceProvider serviceProvider)
            : base(
                serviceProvider,
                OnExecute,
                new CommandID(
                    typeof(TransformTemplatesCommand).GUID,
                    (int)TransformTemplatesCommand.cmdidTransformTemplatesCommand))
        {
        }
        #endregion

        #region Private Implementation
        /// <summary>
        /// Determines whether this instance can execute the specified command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns>
        /// 	<c>true</c> if this instance can execute the specified command; otherwise, <c>false</c>.
        /// </returns>
        protected override bool CanExecute(OleMenuCommand command)
        {
            if(base.CanExecute(command))
            {
                ProjectItemIterator iterator = null;

                if(DynamicCommand.Dte.SelectedItems.Item(1).ProjectItem != null)
                {
                    //Executed at the folder level
                    iterator = new ProjectItemIterator(DynamicCommand.Dte.SelectedItems.Item(1).ProjectItem.ProjectItems);
                }
                else if(DynamicCommand.Dte.SelectedItems.Item(1).Project != null)
                {
                    //Executed at the project level
                    iterator = new ProjectItemIterator(DynamicCommand.Dte.SelectedItems.Item(1).Project.ProjectItems);
                }

                if(iterator != null)
                {
                    //Available if there is at least one ProjectItem with the .tt extension and the custom tool associated
                    return IsAtLeastOneTTProjectItem(iterator);
                }
            }

            return false;
        }

        private static void OnExecute(object sender, EventArgs e)
        {
            Microsoft.PowerCommands.Shell.StatusBar statusBar = new Microsoft.PowerCommands.Shell.StatusBar(DynamicCommand.ServiceProvider);
            Microsoft.PowerCommands.Shell.OutputWindow outputWindow = new Microsoft.PowerCommands.Shell.OutputWindow(DynamicCommand.ServiceProvider);
            ProjectItemIterator iterator = null;
            string message = string.Empty;
            bool tryToGenerate = true;
            string name = string.Empty;

            if(DynamicCommand.Dte.SelectedItems.Item(1).ProjectItem != null)
            {
                iterator = new ProjectItemIterator(DynamicCommand.Dte.SelectedItems.Item(1).ProjectItem.ProjectItems);
            }
            else if(DynamicCommand.Dte.SelectedItems.Item(1).Project != null)
            {
                iterator = new ProjectItemIterator(DynamicCommand.Dte.SelectedItems.Item(1).Project.ProjectItems);
            }

            if(iterator == null)
            {
                return;
            }

            outputWindow.ActivatePane(VSConstants.GUID_OutWindowGeneralPane);

            iterator
                .Where(item =>
                    item.Kind == Constants.vsProjectItemKindPhysicalFile &&
                    item.Properties.Item("Extension").Value.ToString().Equals(
                        Extension, StringComparison.OrdinalIgnoreCase) &&
                    item.Properties.Item("CustomTool").Value.ToString().Equals(
                        CustomToolName, StringComparison.OrdinalIgnoreCase))
                .ForEach(
                        item =>
                        {
                            try
                            {
                                try
                                {
                                    name = item.Properties.Item("FullPath").Value.ToString();
                                }
                                catch(ArgumentException)
                                {
                                    tryToGenerate = false;
                                }

                                if(!string.IsNullOrEmpty(name))
                                {
                                    if(!DynamicCommand.Dte.SourceControl.IsItemUnderSCC(name))
                                    {
                                        if(File.Exists(name))
                                        {
                                            FileAttributes attributes = File.GetAttributes(name);
                                            if((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                                            {
                                                tryToGenerate = false;

                                                message = string.Format(
                                                            Microsoft.PowerCommands.Properties.Resources.TransformTemplateReadOnly,
                                                            string.Format(
                                                                CultureInfo.CurrentCulture,
                                                                "{0}{1}",
                                                                Path.GetFileNameWithoutExtension(item.Name),
                                                                Extension));

                                                outputWindow.WriteMessage(message);
                                                statusBar.DisplayMessage(message);
                                            }
                                        }
                                    }
                                }

                                if(tryToGenerate)
                                {
                                    try
                                    {
                                        message =
                                            string.Format(
                                                CultureInfo.CurrentCulture,
                                                Microsoft.PowerCommands.Properties.Resources.TransformTemplateMessage,
                                                item.Name,
                                                CustomToolName);

                                        outputWindow.WriteMessage(message);
                                        statusBar.DisplayMessage(message);

                                        ((VSProjectItem)item.Object).RunCustomTool();
                                    }
                                    catch
                                    {
                                        message = string.Format(
                                                    CultureInfo.CurrentCulture,
                                                    Microsoft.PowerCommands.Properties.Resources.TransformTemplateMessageError,
                                                    item.Name);

                                        outputWindow.WriteMessage(message);
                                        statusBar.DisplayMessage(message);
                                    }
                                }
                            }
                            catch(ArgumentException)
                            {
                                //Do nothing, goto the next ProjectItem
                            }
                        });
        }

        private static bool IsAtLeastOneTTProjectItem(ProjectItemIterator iterator)
        {
            return iterator
                    .FirstOrDefault(item =>
                        item.Kind == Constants.vsProjectItemKindPhysicalFile &&
                        item.Properties.Item("Extension").Value.ToString().Equals(
                            Extension, StringComparison.OrdinalIgnoreCase) &&
                        item.Properties.Item("CustomTool").Value.ToString().Equals(
                            CustomToolName, StringComparison.OrdinalIgnoreCase))
                    != null;
        }

        #endregion
    }
}