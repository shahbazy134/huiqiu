/***************************************************************************

Copyright (c) 2008 Microsoft Corporation. All rights reserved.

***************************************************************************/

using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.VisualStudio.Shell;

namespace Microsoft.PowerCommands.OptionPages
{
    /// <summary>
    /// Option page for general settings
    /// </summary>
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [Guid("DF0D89F1-C9A3-47BF-B277-42E0C178F1A0")]
    public class GeneralPage : DialogPage
    {
        #region Fields
        GeneralControl control; 
        #endregion

        #region Properties
        private bool formatOnSave;
        /// <summary>
        /// Gets or sets a value indicating whether [format on save].
        /// </summary>
        /// <value><c>true</c> if [format on save]; otherwise, <c>false</c>.</value>
        public bool FormatOnSave
        {
            get
            {
                return formatOnSave;
            }
            set
            {
                formatOnSave = value;
            }
        } 

        private bool removeAndSortUsingsOnSave;

        /// <summary>
        /// Gets or sets a value indicating whether [remove and sort usings on save].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [remove and sort usings on save]; otherwise, <c>false</c>.
        /// </value>
        public bool RemoveAndSortUsingsOnSave
        {
            get
            {
                return removeAndSortUsingsOnSave;
            }
            set
            {
                removeAndSortUsingsOnSave = value;
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
                control = new GeneralControl();
                control.Location = new Point(0, 0);
                control.OptionPage = this;

                return control;
            }
        } 
        #endregion
    }
}