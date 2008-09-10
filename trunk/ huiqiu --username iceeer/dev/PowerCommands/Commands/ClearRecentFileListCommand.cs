/***************************************************************************

Copyright (c) 2008 Microsoft Corporation. All rights reserved.

***************************************************************************/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using Microsoft.PowerCommands.Commands.UI;
using Microsoft.PowerCommands.Common;
using Microsoft.PowerCommands.Extensions;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.Win32;

namespace Microsoft.PowerCommands.Commands
{
    [Guid("5DC1F44A-F045-4E82-9A6A-D576BD672DB3")]
    [DisplayName("Clear Recent File List")]
    internal class ClearRecentFileListCommand : DynamicCommand
    {
        #region Constants
        public const uint cmdidClearRecentFileListCommand = 0x0F7D;
        #endregion

        #region Constructors
        public ClearRecentFileListCommand(IServiceProvider serviceProvider)
            : base(
                serviceProvider,
                OnExecute,
                new CommandID(
                    typeof(ClearRecentFileListCommand).GUID,
                    (int)ClearRecentFileListCommand.cmdidClearRecentFileListCommand))
        {
        }
        #endregion

        #region Private Implementation
        protected override bool CanExecute(OleMenuCommand command)
        {
            if(base.CanExecute(command))
            {
                using(RegistryKey rootKey = GetRecentRootKey())
                {
                    if(rootKey != null)
                    {
                        return (rootKey.GetValueNames().Length > 0);
                    }
                }
            }

            return false;
        }

        private static void OnExecute(object sender, EventArgs e)
        {
            ClearListView view = new ClearListView();

            using(RegistryKey key = GetRecentRootKey())
            {
                key.GetValueNames().ForEach(
                    valueName => view.Model.ListEntries.Add(new KeyValue(valueName, key.GetValue(valueName).ToString())));

                if((bool)view.ShowDialog())
                {
                    DynamicCommand.Dte.ExecuteCommand("File.SaveAll", string.Empty);
                    DeleteRecentFileList(view.Model.SelectedListEntries);
                    ReEnumerateFileList();
                    DTEHelper.RestartVS(DynamicCommand.Dte);
                }
            }
        }

        private static void DeleteRecentFileList(List<KeyValue> entriesToDelete)
        {
            using(RegistryKey key = GetRecentRootKey())
            {
                if(key != null)
                {
                    entriesToDelete.ForEach(
                        entry => key.DeleteValue(entry.Key));
                }
            }
        }

        private static void ReEnumerateFileList()
        {
            int fileCounter = 1;

            using(RegistryKey key = GetRecentRootKey())
            {
                string[] valueNames = key.GetValueNames();

                valueNames.ForEach(
                    valueName =>
                    {
                        key.SetValue(string.Concat(valueName, "_"), key.GetValue(valueName));
                        key.DeleteValue(valueName);
                    });


                valueNames = key.GetValueNames();

                valueNames.ForEach(
                    valueName =>
                    {
                        key.SetValue(string.Format("File{0}", fileCounter), key.GetValue(valueName));
                        key.DeleteValue(valueName);
                        fileCounter++;
                    });
            }
        }

        private static RegistryKey GetRecentRootKey()
        {
            string registryKey;
            ILocalRegistry2 localRegistry =
                DynamicCommand.ServiceProvider.GetService<SLocalRegistry, ILocalRegistry2>();

            localRegistry.GetLocalRegistryRoot(out registryKey);

            return Registry.CurrentUser.OpenSubKey(
                    string.Format(@"{0}\{1}\",
                        registryKey,
                        Microsoft.PowerCommands.Common.Constants.FileMRUListKey), true);
        }
        #endregion
    }
}