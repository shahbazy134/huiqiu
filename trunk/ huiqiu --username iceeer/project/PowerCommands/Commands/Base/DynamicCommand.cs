/***************************************************************************

Copyright (c) 2008 Microsoft Corporation. All rights reserved.

***************************************************************************/

using System;
using System.ComponentModel.Design;
using System.Linq;
using EnvDTE;
using Microsoft.PowerCommands.Extensions;
using Microsoft.VisualStudio.Shell;

namespace Microsoft.PowerCommands.Commands
{
    /// <summary>
    /// Base class that represents a dynamic command
    /// </summary>
    public abstract class DynamicCommand : OleMenuCommand
    {
        #region Fields
        private static DTE dte;
        private static IServiceProvider serviceProvider;
        private static PowerCommandsPackage powerCommandsPackage;
        #endregion

        #region Properties
        /// <summary>
        /// The ServiceProvider
        /// </summary>
        protected static IServiceProvider ServiceProvider
        {
            get
            {
                return serviceProvider;
            }
        }

        /// <summary>
        /// VS
        /// </summary>
        protected static DTE Dte
        {
            get
            {
                if(dte == null)
                {
                    dte = CollapseProjectsCommand.ServiceProvider.GetService<DTE>();
                }

                return dte;
            }
        }

        /// <summary>
        /// CommandManagerService
        /// </summary>
        protected static PowerCommandsPackage PowerCommandsPackage
        {
            get
            {
                if(powerCommandsPackage == null)
                {
                    powerCommandsPackage = ServiceProvider.GetService<PowerCommandsPackage>();
                }

                return powerCommandsPackage;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicCommand"/> class.
        /// </summary>
        /// <param name="provider">The service provider.</param>
        /// <param name="onExecute">The on execute delegate.</param>
        /// <param name="id">The command id.</param>
        public DynamicCommand(IServiceProvider provider, EventHandler onExecute, CommandID id)
            : base(onExecute, id)
        {
            this.BeforeQueryStatus += new EventHandler(OnBeforeQueryStatus);
            CollapseProjectsCommand.serviceProvider = provider;
        }
        #endregion

        #region Protected Implementation
        /// <summary>
        /// Called when [before query status].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void OnBeforeQueryStatus(object sender, EventArgs e)
        {
            OleMenuCommand command = sender as OleMenuCommand;

            command.Enabled = command.Visible = command.Supported = CanExecute(command);
        }

        /// <summary>
        /// Determines whether this instance can execute the specified command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns>
        /// 	<c>true</c> if this instance can execute the specified command; otherwise, <c>false</c>.
        /// </returns>
        protected virtual bool CanExecute(OleMenuCommand command)
        {
            return PowerCommandsPackage.CommandsPage.DisabledCommands.SingleOrDefault(
                cmd => cmd.Guid.Equals(command.CommandID.Guid) &&
                    cmd.ID.Equals(command.CommandID.ID)) == null;
        }
        #endregion
    }
}