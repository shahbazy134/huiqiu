/***************************************************************************

Copyright (c) 2008 Microsoft Corporation. All rights reserved.

***************************************************************************/

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;

namespace Microsoft.PowerCommands.Services
{
    /// <summary>
    /// Service for managing registered commands
    /// </summary>
    [Guid("31A6E058-A531-41CC-B385-B6FAB536066A")]
    [ComVisible(true)]
    public interface ICommandManagerService
    {
        /// <summary>
        /// Registers the command.
        /// </summary>
        /// <param name="command">The command.</param>
        void RegisterCommand(OleMenuCommand command);
        /// <summary>
        /// Uns the register command.
        /// </summary>
        /// <param name="command">The command.</param>
        void UnRegisterCommand(OleMenuCommand command);
        /// <summary>
        /// Gets the registered commands.
        /// </summary>
        /// <returns></returns>
        IEnumerable<OleMenuCommand> GetRegisteredCommands();
    }
}