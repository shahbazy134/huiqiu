/***************************************************************************

Copyright (c) 2008 Microsoft Corporation. All rights reserved.

***************************************************************************/

using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using EnvDTE;
using Microsoft.PowerCommands.Common;
using Microsoft.PowerCommands.Linq;
using Microsoft.VisualStudio.Shell;

namespace Microsoft.PowerCommands.Commands
{
    /// <summary>
    /// Command that pastes class content from the clipboard
    /// </summary>
    [Guid("C328650B-8F49-4883-8D83-3A9103458095")]
    [DisplayName("Paste Class")]
    internal class PasteClassCommand : DynamicCommand
    {
        #region Constants
        public const uint cmdidPasteClassCommand = 0x201A;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PasteClassCommand"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public PasteClassCommand(IServiceProvider serviceProvider)
            : base(
                serviceProvider,
                OnExecute,
                new CommandID(
                    typeof(PasteClassCommand).GUID,
                    (int)PasteClassCommand.cmdidPasteClassCommand))
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
                if(Clipboard.ContainsText())
                {
                    string text = Clipboard.GetDataObject().GetData(DataFormats.Text).ToString();

                    if(text.StartsWith(Microsoft.PowerCommands.Common.Constants.ClassIdentifier))
                    {
                        string[] fileNames = text.Split(
                            new string[] { Microsoft.PowerCommands.Common.Constants.DoubleForwardSlash }, StringSplitOptions.None);

                        string kind = fileNames[0].Split(new String[] { ":" }, StringSplitOptions.None).ElementAt(1);

                        Project project = DynamicCommand.Dte.SelectedItems.Item(1).Project;

                        if(project != null)
                        {
                            //Executed at the project level
                            return project.Kind.Equals(kind);
                        }
                        else
                        {
                            ProjectItem folder = DynamicCommand.Dte.SelectedItems.Item(1).ProjectItem;

                            if(folder != null)
                            {
                                //Executed at the folder level
                                return folder.ContainingProject.Kind.Equals(kind);
                            }
                        }

                        return true;
                    }
                }
            }

            return false;
        }

        private static void OnExecute(object sender, EventArgs e)
        {
            Project project = DynamicCommand.Dte.SelectedItems.Item(1).Project;

            if(project != null)
            {
                //Executed at the project level
                Process(project, project.ProjectItems);
            }
            else
            {
                ProjectItem folder = DynamicCommand.Dte.SelectedItems.Item(1).ProjectItem;

                if(folder != null)
                {
                    //Executed at the folder level
                    Process(folder.ContainingProject, folder.ProjectItems);
                }
            }
        }

        private static void Process(Project project, ProjectItems projectItems)
        {
            string[] fileNames =
                Clipboard.GetDataObject().GetData(DataFormats.Text).ToString()
                    .Split(new string[] { Microsoft.PowerCommands.Common.Constants.QSSeparator }, StringSplitOptions.None);

            string fileName = fileNames[0].Split(new String[] { Microsoft.PowerCommands.Common.Constants.DoubleForwardSlash }, StringSplitOptions.None).ElementAt(1);

            ProjectItem item = AddClass(
                fileName,
                project,
                projectItems);

            if(fileNames.Count() > 1)
            {
                for(int i = 1; i < fileNames.Count(); i++)
                {
                    fileName = fileNames[i].Split(new String[] { Microsoft.PowerCommands.Common.Constants.DoubleForwardSlash }, StringSplitOptions.None).ElementAt(1);

                    AddClass(
                        fileName,
                        IOHelper.GetFileWithoutExtension(item.get_FileNames(0)),
                        project,
                        item.ProjectItems);
                }
            }
        }

        private static ProjectItem AddClass(string fileName, Project project, ProjectItems projectItems)
        {
            return AddClass(fileName, GetIdentifierName(project, fileName), project, projectItems);
        }

        private static ProjectItem AddClass(string fileName, string identifierName, Project project, ProjectItems projectItems)
        {
            ProjectItem item =
                projectItems.AddFromTemplate(
                    fileName,
                    string.Format("{0}{1}", identifierName, IOHelper.GetFileExtension(fileName)));

            CodeElementIterator iterator = new CodeElementIterator(item.FileCodeModel.CodeElements);

            CodeClass codeClass =
                iterator.OfType<CodeClass>()
                    .First();

            if(codeClass != null)
            {
                codeClass.Name = identifierName;
            }

            item.Open(EnvDTE.Constants.vsViewKindTextView).Activate();

            return item;
        }

        private static string GetIdentifierName(Project project, string fileName)
        {
            ProjectItemIterator itemIterator =
                new ProjectItemIterator(project.ProjectItems);

            string fileNameWithoutExtension = IOHelper.GetFileWithoutExtension(fileName);

            var names = itemIterator
                            .Where(it => it.Name.StartsWith(fileNameWithoutExtension))
                            .Select(it => it.Name);

            return string.Concat(fileNameWithoutExtension, names.Count());
        }
        #endregion
    }
}