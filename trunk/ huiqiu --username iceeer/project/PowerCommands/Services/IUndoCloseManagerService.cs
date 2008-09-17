/***************************************************************************

Copyright (c) 2008 Microsoft Corporation. All rights reserved.

***************************************************************************/

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Microsoft.PowerCommands.Services
{
    /// <summary>
    /// Service for managing undo close documents
    /// </summary>
    [Guid("AF0C3D86-775F-4BEF-AB72-87D18E36873D")]
    [ComVisible(true)]
    public interface IUndoCloseManagerService
    {
        /// <summary>
        /// Pushes a document.
        /// </summary>
        /// <param name="document">The document.</param>
        void PushDocument(IDocumentInfo document);

        /// <summary>
        /// Pops a document.
        /// </summary>
        /// <returns></returns>
        IDocumentInfo PopDocument();

        /// <summary>
        /// Pops a particular document.
        /// </summary>
        /// <returns></returns>
        IDocumentInfo PopDocument(IDocumentInfo document);

        /// <summary>
        /// Gets the current document.
        /// </summary>
        /// <value>The current document.</value>
        IDocumentInfo CurrentDocument {get;}

        /// <summary>
        /// Gets the documents.
        /// </summary>
        /// <returns></returns>
        IEnumerable<IDocumentInfo> GetDocuments();

        /// <summary>
        /// Clears the documents.
        /// </summary>
        void ClearDocuments();
    }
}
