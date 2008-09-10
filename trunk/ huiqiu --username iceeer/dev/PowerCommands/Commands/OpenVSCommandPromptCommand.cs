/***************************************************************************

Copyright (c) 2008 Microsoft Corporation. All rights reserved.

***************************************************************************/

using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.PowerCommands.Common;
using Microsoft.PowerCommands.Extensions;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.Win32;

namespace Microsoft.PowerCommands.Commands
{
    /// <summary>
    /// Command that opens a VS command prompt
    /// </summary>
    [Guid("A9902F9B-09E2-418D-B3D0-EA771B908B65")]
    [DisplayName("Open Command Prompt")]
    internal class OpenVSCommandPromptCommand : DynamicCommand
    {
        #region Constants
        public const uint cmdidOpenVSCommandPromptCommand = 0x731C;

        private const string VS90COMNTOOLS = "VS90COMNTOOLS";
        private const string cmd = "cmd.exe";
        private const string vsvars32 = "vsvars32.bat";
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="OpenVSCommandPromptCommand"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public OpenVSCommandPromptCommand(IServiceProvider serviceProvider)
            : base(
                serviceProvider,
                OnExecute,
                new CommandID(
                    typeof(OpenVSCommandPromptCommand).GUID,
                    (int)OpenVSCommandPromptCommand.cmdidOpenVSCommandPromptCommand))
        {
        }
        #endregion

        #region Private Implementation
        protected override bool CanExecute(OleMenuCommand command)
        {
            if(base.CanExecute(command))
            {
                return DynamicCommand.Dte.Solution.IsOpen;
            }

            return false;
        }

        private static void OnExecute(object sender, EventArgs e)
        {
            string currentDirectory = string.Empty;
            string vsvars32Path = GetVsVarsPath();

            if(DynamicCommand.Dte.SelectedItems.Item(1).Project != null)
            {
                //Executed at the project level
                currentDirectory =
                    Path.GetDirectoryName(DynamicCommand.Dte.SelectedItems.Item(1).Project.FullName);
            }
            else if(DynamicCommand.Dte.SelectedItems.Item(1).ProjectItem != null)
            {
                //Executed at the folder level / item level
                currentDirectory =
                    Path.GetDirectoryName(DynamicCommand.Dte.SelectedItems.Item(1).ProjectItem.get_FileNames(0));
            }
            else
            {
                //Executed at the solution level
                currentDirectory =
                    Path.GetDirectoryName(DynamicCommand.Dte.Solution.FullName);
            }

            Directory.SetCurrentDirectory(currentDirectory);

            if(File.Exists(vsvars32Path))
            {
                System.Diagnostics.Process.Start(cmd, string.Concat("/k \"" + IOHelper.SanitizePath(vsvars32Path)));
            }
        }

        private static string GetVsVarsPath()
        {
            string vsvars32Path;

            if(Environment.GetEnvironmentVariable(VS90COMNTOOLS) != null)
            {
                vsvars32Path = Path.Combine(
                    Environment.GetEnvironmentVariable(VS90COMNTOOLS),
                    vsvars32);
            }
            else
            {
                string registryKey;
                ILocalRegistry2 localRegistry =
                    DynamicCommand.ServiceProvider.GetService<SLocalRegistry, ILocalRegistry2>();

                localRegistry.GetLocalRegistryRoot(out registryKey);

                using(RegistryKey key = Registry.LocalMachine.OpenSubKey(registryKey))
                {
                    string installDir = key.GetValue(Microsoft.PowerCommands.Common.Constants.InstallDirKey).ToString();

                    vsvars32Path = Path.Combine(
                        string.Concat(installDir, @"\..\Tools\"),
                        vsvars32);
                }
            }

            return vsvars32Path;
        }
        #endregion
    }
}