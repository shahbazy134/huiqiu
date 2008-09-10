/***************************************************************************

Copyright (c) 2008 Microsoft Corporation. All rights reserved.

***************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Effects;
using System.ComponentModel;

namespace Microsoft.PowerCommands.Commands.UI
{
    /// <summary>
    /// View for the extract to constant command
    /// </summary>
    public partial class ExtractToConstantView : Window, IModalView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExtractToConstantView"/> class.
        /// </summary>
        public ExtractToConstantView()
        {
            InitializeComponent();
        }

        #region IExtractToConstantView Members

        void IModalView.OK()
        {
            this.DialogResult = true;
            this.Close();
        }

        void IModalView.Cancel()
        {
            this.DialogResult = false;
            this.Close();
        }
        #endregion
    }
}