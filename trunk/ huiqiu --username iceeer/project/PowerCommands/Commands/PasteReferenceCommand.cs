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
using Microsoft.PowerCommands.Extensions;
using Microsoft.PowerCommands.Linq;
using Microsoft.VisualStudio.Shell;
using VSLangProj;

namespace Microsoft.PowerCommands.Commands
{
    /// <summary>
    /// Command that pastes references from the clipboard
    /// </summary>
    [Guid("14C14C76-3555-4D81-AFA6-2ADE1EE0D896")]
    [DisplayName("Paste Reference")]
    internal class PasteReferenceCommand : DynamicCommand
    {
        #region Constants
        public const uint cmdidPasteReferenceCommand = 0x7C09;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PasteReferenceCommand"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public PasteReferenceCommand(IServiceProvider serviceProvider)
            : base(
                serviceProvider,
                OnExecute,
                new CommandID(
                    typeof(PasteReferenceCommand).GUID,
                    (int)PasteReferenceCommand.cmdidPasteReferenceCommand))
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

                    if(text.StartsWith(Microsoft.PowerCommands.Common.Constants.ProjRefUri) ||
                        text.StartsWith(Microsoft.PowerCommands.Common.Constants.AssemblyRefUri))
                    {
                        string[] references = text.Split(
                            new string[] { 
                            Microsoft.PowerCommands.Common.Constants.QSSeparator }, StringSplitOptions.None);

                        command.Text = (references.Count() > 1) ? "Paste References" : "Paste Reference";

                        Project project = DynamicCommand.Dte.SelectedItems.Item(1).Project;

                        //Executed at the project level
                        if(project != null)
                        {
                            //At the project level is only available for VBProjects or WebSite projects
                            if(project.Kind == PrjKind.prjKindVBProject ||
                                project.Kind == VsWebSite.PrjKind.prjKindVenusProject)
                            {
                                if(references.Count() == 1)
                                {
                                    return CanExecuteCommandForSingleReference(references[0], project);
                                }
                                else
                                {
                                    return true;
                                }
                            }

                            return false;
                        }
                        else
                        {
                            //Executed at the reference level (only C# projects)
                            if(references.Count() == 1)
                            {
                                project = VSShellHelper.GetSelectedHierarchy().ToProject();

                                return CanExecuteCommandForSingleReference(references[0], project);
                            }
                            else
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        private static void OnExecute(object sender, EventArgs e)
        {
            Project selectedProject = VSShellHelper.GetSelectedHierarchy().ToProject();

            if(selectedProject != null && Clipboard.ContainsText())
            {
                VSProject vsProject = selectedProject.Object as VSProject;
                VsWebSite.VSWebSite webSiteProject = selectedProject.Object as VsWebSite.VSWebSite;

                if(vsProject != null || webSiteProject != null)
                {
                    string[] references =
                        Clipboard.GetDataObject().GetData(DataFormats.Text).ToString()
                            .Split(new string[] { Microsoft.PowerCommands.Common.Constants.QSSeparator }, StringSplitOptions.None);

                    foreach(string referenceUri in references)
                    {
                        string reference = GetReference(referenceUri);

                        if(IsProjectReference(referenceUri))
                        {
                            Project projectToReference =
                                new ProjectIterator(DynamicCommand.Dte.Solution)
                                    .SingleOrDefault(proj => proj.Name.Equals(reference, StringComparison.OrdinalIgnoreCase));

                            if(CanAddProjectReference(referenceUri, selectedProject, projectToReference))
                            {
                                if(vsProject != null)
                                {
                                    vsProject.References.AddProject(projectToReference);
                                }
                                else
                                {
                                    webSiteProject.References.AddFromProject(projectToReference);
                                }
                            }
                        }
                        else
                        {
                            if(!IsReferenceInPlace(referenceUri, selectedProject))
                            {
                                if(vsProject != null)
                                {
                                    vsProject.References.Add(reference);
                                }
                                else
                                {
                                    webSiteProject.References.AddFromFile(reference);
                                }
                            }
                        }
                    }
                }
            }
        }

        private static bool IsProjectReference(string referenceUri)
        {
            return referenceUri.StartsWith(Microsoft.PowerCommands.Common.Constants.ProjRefUri);
        }

        private static string GetReference(string referenceUri)
        {
            string reference = string.Empty;

            if(IsProjectReference(referenceUri))
            {
                reference =
                    referenceUri.Replace(
                        Microsoft.PowerCommands.Common.Constants.ProjRefUri,
                        string.Empty);
            }
            else
            {
                reference =
                    referenceUri.Replace(
                        Microsoft.PowerCommands.Common.Constants.AssemblyRefUri,
                        string.Empty);
            }

            return reference;
        }

        private bool CanExecuteCommandForSingleReference(string referenceUri, Project project)
        {
            if(IsProjectReference(referenceUri))
            {
                Project projectToReference =
                    new ProjectIterator(DynamicCommand.Dte.Solution)
                        .SingleOrDefault(proj => proj.Name.Equals(GetReference(referenceUri), StringComparison.OrdinalIgnoreCase));

                return CanAddProjectReference(referenceUri, project, projectToReference);
            }
            else
            {
                return !IsReferenceInPlace(referenceUri, project);
            }
        }

        private static bool CanAddProjectReference(string referenceUri, Project sourceProject, Project projectToReference)
        {
            if(!IsSelfReference(referenceUri, sourceProject) &&
                !IsReferenceInPlace(referenceUri, sourceProject))
            {
                if(projectToReference.Object is VSProject)
                {
                    if(!IsReferenceInPlace(sourceProject.Name, projectToReference.Object as VSProject)) //Circular dependencies
                    {
                        return true;
                    }
                }
                else
                {
                    if(!IsReferenceInPlace(sourceProject.Name, projectToReference.Object as VsWebSite.VSWebSite)) //Circular dependencies
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static bool IsReferenceInPlace(string referenceUri, Project project)
        {
            if(project.Object is VSProject)
            {
                return IsReferenceInPlace(referenceUri, project.Object as VSProject);
            }
            else
            {
                return IsReferenceInPlace(referenceUri, project.Object as VsWebSite.VSWebSite);
            }
        }

        private static bool IsReferenceInPlace(string referenceUri, VSProject project)
        {
            string reference = GetReference(referenceUri);

            Reference @ref = project.References.OfType<Reference>()
                .SingleOrDefault(ref1 => ref1.Name.Equals(reference, StringComparison.OrdinalIgnoreCase));

            return @ref != null;
        }

        private static bool IsReferenceInPlace(string referenceUri, VsWebSite.VSWebSite website)
        {
            string reference = GetReference(referenceUri);

            VsWebSite.AssemblyReference @ref = website.References.OfType<VsWebSite.AssemblyReference>()
                .SingleOrDefault(ref1 => ref1.Name.Equals(reference, StringComparison.OrdinalIgnoreCase));

            return @ref != null;
        }

        private static bool IsSelfReference(string referenceUri, Project project)
        {
            string reference = GetReference(referenceUri);

            if(IsProjectReference(referenceUri))
            {
                if(project.Name == reference)
                {
                    return true;
                }
            }

            return false;
        }
        #endregion
    }
}