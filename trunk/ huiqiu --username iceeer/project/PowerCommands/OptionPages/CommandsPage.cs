/***************************************************************************

Copyright (c) 2008 Microsoft Corporation. All rights reserved.

***************************************************************************/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.VisualStudio.Shell;

namespace Microsoft.PowerCommands.OptionPages
{
    /// <summary>
    /// Option page for enabling/disabling commands
    /// </summary>
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [Guid("7A9E9816-5ADD-4CBD-9C46-1901A492640D")]
    public class CommandsPage : DialogPage
    {
        #region Fields
        CommandsControl control;
        private IList<CommandID> disabledCommands = new List<CommandID>(); 
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the disabled commands.
        /// </summary>
        /// <value>The disabled commands.</value>
        [TypeConverter(typeof(DisabledCommandsDictionaryConverter))]
        public IList<CommandID> DisabledCommands
        {
            get
            {
                return disabledCommands;
            }
            set
            {
                disabledCommands = value;
            }
        } 
        #endregion

        #region Private Implementation
        /// <summary>
        /// </summary>
        /// <value></value>
        /// The window this dialog page will use for its UI.
        /// This window handle must be constant, so if you are
        /// returning a Windows Forms control you must make sure
        /// it does not recreate its handle.  If the window object
        /// implements IComponent it will be sited by the
        /// dialog page so it can get access to global services.
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        protected override IWin32Window Window
        {
            get
            {
                control = new CommandsControl();
                control.Location = new Point(0, 0);
                control.OptionPage = this;

                return control;
            }
        }
        #endregion	
    }
}