/***************************************************************************

Copyright (c) 2008 Microsoft Corporation. All rights reserved.

***************************************************************************/

using System.ComponentModel;
using System.ComponentModel.Design;

namespace Microsoft.PowerCommands.OptionPages
{
    /// <summary>
    /// Class that represents a row in the Commands grid
    /// </summary>
    public class RowItem
    {
        #region Properties
        /// <summary>
        /// Gets or sets the command text.
        /// </summary>
        /// <value>The command text.</value>
        [DisplayName("Command")]
        public string CommandText { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="RowItem"/> is enabled.
        /// </summary>
        /// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets the command.
        /// </summary>
        /// <value>The command.</value>
        [Browsable(false)]
        public CommandID Command { get; set; } 
        #endregion
    }
}