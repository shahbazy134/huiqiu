using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.PowerCommands.Commands.UI
{
    /// <summary>
    /// Class that represents the model for the ClearListView
    /// </summary>
    public class ClearListModel
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ClearListModel"/> class.
        /// </summary>
        public ClearListModel()
        {
            ListEntries = new List<KeyValue>();
            SelectedListEntries = new List<KeyValue>();
        } 
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the list entries.
        /// </summary>
        /// <value>The list entries.</value>
        public List<KeyValue> ListEntries { get; set; }

        /// <summary>
        /// Gets or sets the selected list entries.
        /// </summary>
        /// <value>The selected list entries.</value>
        public List<KeyValue> SelectedListEntries { get; set; } 
        #endregion
    }
}