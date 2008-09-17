/***************************************************************************

Copyright (c) 2008 Microsoft Corporation. All rights reserved.

***************************************************************************/

using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.VisualStudio.Shell;

namespace Microsoft.PowerCommands.ToolWindows
{
    /// <summary>
    /// Undo Close Toolwindow
    /// </summary>
    [Guid("ECCC9E97-FD3B-4C15-AF76-EF71A71D8B17")]
    public class UndoCloseToolWindow : ToolWindowPane
    {
        #region Fields
        private UndoCloseControl control; 
        #endregion

        #region Properties
        /// <summary>
        /// Gets the control.
        /// </summary>
        /// <value>The control.</value>
        public UndoCloseControl Control
        {
            get
            {
                return control;
            }
        } 
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="UndoCloseToolWindow"/> class.
        /// </summary>
        public UndoCloseToolWindow() :
            base(null)
        {
            this.Caption = Properties.Resources.UndoCloseCaption;
            this.BitmapResourceID = 10969;
            this.BitmapIndex = 0;

            control = new UndoCloseControl(this);
        } 
        #endregion

        #region Public Implementation
        /// <summary>
        /// </summary>
        /// <value></value>
        /// The window this dialog page will use for its UI.
        /// This window handle must be constant, so if you are
        /// returning a Windows Forms control you must make sure
        /// it does not recreate its handle.  If the window object
        /// implements IComponent it will be sited by the
        /// dialog page so it can get access to global services.
        public override IWin32Window Window
        {
            get
            {
                return (IWin32Window)control;
            }
        } 
        #endregion
    }
}