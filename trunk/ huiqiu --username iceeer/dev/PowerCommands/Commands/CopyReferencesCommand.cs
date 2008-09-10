/***************************************************************************

Copyright (c) 2008 Microsoft Corporation. All rights reserved.

***************************************************************************/

using System;
using System.Collections.Generic;
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
    /// Command that copies all project references to the clipboard
    /// </summary>
    [Guid("C91EA546-A349-47B1-AA69-7A1529B58C57")]
    [DisplayName("Copy References")]
    internal class CopyReferencesCommand : DynamicCommand
    {
        #region Constants
        public const uint cmdidCopyReferencesCommand = 0x2673;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="CopyReferencesCommand"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public CopyReferencesCommand(IServiceProvider serviceProvider)
            : base(
                serviceProvider,
                OnExecute,
                new CommandID(
                    typeof(CopyReferencesCommand).GUID,
                    (int)CopyReferencesCommand.cmdidCopyReferencesCommand))
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
                Project selectedProject = VSShellHelper.GetSelectedHierarchy().ToProject();

                if(selectedProject != null)
                {
                    VSProject vsProject = selectedProject.Object as VSProject;

                    return (vsProject.References.Count > 1); // > 1 because of mscorlib.dll
                }
            }

            return false;
        }

        private static void OnExecute(object sender, EventArgs e)
        {
            List<string> references = new List<string>();

            Project selectedProject = VSShellHelper.GetSelectedHierarchy().ToProject();

            if(selectedProject != null)
            {
                VSProject vsProject = selectedProject.Object as VSProject;

                vsProject.References.OfType<Reference>()
                    .ForEach(reference =>
                        {
                            if(new ProjectIterator(DynamicCommand.Dte.Solution)
                                .SingleOrDefault(project => project.Name.Equals(reference.Name, StringComparison.OrdinalIgnoreCase)) != null)
                            {
                                references.Add(string.Concat(Microsoft.PowerCommands.Common.Constants.ProjRefUri, reference.Name));
                            }
                            else
                            {
                                references.Add(string.Concat(Microsoft.PowerCommands.Common.Constants.AssemblyRefUri, reference.Name));
                            }
                        });

                Clipboard.SetDataObject(
                    string.Join(Microsoft.PowerCommands.Common.Constants.QSSeparator, references.ToArray()), true);
            }
        }
        #endregion
    }
}