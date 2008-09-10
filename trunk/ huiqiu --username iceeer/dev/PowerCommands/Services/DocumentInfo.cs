/***************************************************************************

Copyright (c) 2008 Microsoft Corporation. All rights reserved.

***************************************************************************/

namespace Microsoft.PowerCommands.Services
{
    internal class DocumentInfo : IDocumentInfo
    {
        #region Properties
        public string DocumentPath { get; set; }

        public int CursorLine { get; set; }

        public int CursorColumn { get; set; }

        public string DocumentViewKind { get; set; } 
        #endregion

        #region Constructors
        public DocumentInfo(string document, int cursorLine, int cursorColumn, string viewKind)
        {
            this.DocumentPath = document;
            this.CursorLine = cursorLine;
            this.CursorColumn = cursorColumn;
            this.DocumentViewKind = viewKind;
        } 
        #endregion
    }
}