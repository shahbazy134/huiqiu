/***************************************************************************

Copyright (c) 2008 Microsoft Corporation. All rights reserved.

***************************************************************************/

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.VisualStudio.Shell;

namespace Microsoft.PowerCommands.Services
{
    internal class CommandManagerService : ICommandManagerService, SCommandManagerService
    {
        #region Fields
        private IList<OleMenuCommand> registeredCommands; 
        #endregion

        #region Constructors
        public CommandManagerService()
        {
            registeredCommands = new List<OleMenuCommand>();
        } 
        #endregion

        #region Public Implementation
        public void RegisterCommand(OleMenuCommand command)
        {
            if(registeredCommands.SingleOrDefault(
                cmd => cmd.CommandID.Guid.Equals(command.CommandID.Guid) &&
                    cmd.CommandID.ID.Equals(command.CommandID.ID)) == null)
            {
                registeredCommands.Add(command);
            }
        }

        public void UnRegisterCommand(OleMenuCommand command)
        {
            if(registeredCommands.SingleOrDefault(
                cmd => cmd.CommandID.Guid.Equals(command.CommandID.Guid) &&
                    cmd.CommandID.ID.Equals(command.CommandID.ID)) != null)
            {
                registeredCommands.Remove(command);
            }
        }

        public IEnumerable<OleMenuCommand> GetRegisteredCommands()
        {
            return new ReadOnlyCollection<OleMenuCommand>(registeredCommands);
        } 
        #endregion
    }
}