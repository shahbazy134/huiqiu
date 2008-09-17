/***************************************************************************

Copyright (c) 2008 Microsoft Corporation. All rights reserved.

***************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.PowerCommands.Commands.UI
{
    /// <summary>
    /// Interface for modal dialog
    /// </summary>
    public interface IModalView
    {
        /// <summary>
        /// Method associared with the OK button.
        /// </summary>
        void OK();

        /// <summary>
        /// Method associared with the Cancel button.
        /// </summary>
        void Cancel();
    }
}
