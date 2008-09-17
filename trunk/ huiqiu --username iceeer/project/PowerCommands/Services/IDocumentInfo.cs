/***************************************************************************

Copyright (c) 2008 Microsoft Corporation. All rights reserved.

***************************************************************************/

namespace Microsoft.PowerCommands.Services
{
    /// <summary>
    /// Interface that represents a document for the undo close command
    /// </summary>
    public interface IDocumentInfo
    {
        /// <summary>
        /// Gets or sets the document path.
        /// </summary>
        /// <value>The document path.</value>
        string DocumentPath { get; set; }
        /// <summary>
        /// Gets or sets the cursor line.
        /// </summary>
        /// <value>The cursor line.</value>
        int CursorLine { get; set; }
        /// <summary>
        /// Gets or sets the cursor column.
        /// </summary>
        /// <value>The cursor column.</value>
        int CursorColumn { get; set; }
        /// <summary>
        /// Gets or sets the kind of the document view.
        /// </summary>
        /// <value>The kind of the document view.</value>
        string DocumentViewKind { get; set; }
    }
}
