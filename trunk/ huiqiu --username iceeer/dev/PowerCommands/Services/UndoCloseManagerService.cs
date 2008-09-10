/***************************************************************************

Copyright (c) 2008 Microsoft Corporation. All rights reserved.

***************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.PowerCommands.Extensions;

namespace Microsoft.PowerCommands.Services
{
    internal class UndoCloseManagerService : IUndoCloseManagerService, SUndoCloseManagerService
    {
        #region Fields
        private const int capacity = 20;

        FixedCapacityStack<IDocumentInfo> documents; 
        #endregion

        #region Constructors
        public UndoCloseManagerService()
        {
            documents = new FixedCapacityStack<IDocumentInfo>(capacity);
        } 
        #endregion

        #region Public Implementation
        public void PushDocument(IDocumentInfo document)
        {
            IDocumentInfo docInfo =
                documents.SingleOrDefault(
                    info => info.DocumentPath.Equals(document.DocumentPath));

            if(docInfo != null)
            {
                FixedCapacityStack<IDocumentInfo> temp =
                    new FixedCapacityStack<IDocumentInfo>(capacity);

                documents.Reverse().ForEach(
                    info =>
                    {
                        if(info.DocumentPath != document.DocumentPath)
                        {
                            temp.Push(info);
                        }
                    });

                temp.Push(document);
                documents = temp;
            }
            else
            {
                documents.Push(document);
            }
        }

        public IDocumentInfo PopDocument()
        {
            try
            {
                return documents.Pop();
            }
            catch(InvalidOperationException)
            {
                return null;
            }
        }

        public IDocumentInfo PopDocument(IDocumentInfo document)
        {
            IDocumentInfo docInfo =
                documents.SingleOrDefault(
                    info => info.DocumentPath.Equals(document.DocumentPath));

            if(docInfo != null)
            {
                FixedCapacityStack<IDocumentInfo> temp =
                    new FixedCapacityStack<IDocumentInfo>(capacity);

                documents.Reverse().ForEach(
                    info =>
                    {
                        if(info.DocumentPath != document.DocumentPath)
                        {
                            temp.Push(info);
                        }
                    });

                documents = temp;
            }

            return docInfo;
        }

        public IDocumentInfo CurrentDocument
        {
            get
            {
                try
                {
                    return documents.Peek();
                }
                catch(InvalidOperationException)
                {
                    return null;
                }
            }
        }

        public void ClearDocuments()
        {
            documents.Clear();
        }

        public IEnumerable<IDocumentInfo> GetDocuments()
        {
            return documents;
        } 
        #endregion
    }
}