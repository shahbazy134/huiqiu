/***************************************************************************

Copyright (c) 2008 Microsoft Corporation. All rights reserved.

***************************************************************************/

using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using EnvDTE;
using Microsoft.PowerCommands.Common;
using Microsoft.PowerCommands.Extensions;
using Microsoft.PowerCommands.Linq;
using Microsoft.PowerCommands.Services;
using Microsoft.VisualStudio.Shell.Interop;
using System.Runtime.InteropServices;

namespace Microsoft.PowerCommands.ToolWindows
{
    /// <summary>
    /// User Control for the Undo Close Window
    /// </summary>
    public partial class UndoCloseControl : UserControl
    {
        #region Fields
        IServiceProvider serviceProvider;
        IUndoCloseManagerService undoCloseManager;
        ImageList images = new ImageList();
        ListViewGroup rootGroup = new ListViewGroup(Properties.Resources.RecentlyClosedDocuments);
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="UndoCloseControl"/> class.
        /// </summary>
        public UndoCloseControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UndoCloseControl"/> class.
        /// </summary>
        /// <param name="provider">The provider.</param>
        public UndoCloseControl(IServiceProvider provider)
        {
            this.serviceProvider = provider;
            InitializeComponent();
        }
        #endregion

        #region Event Handlers
        private void UndoCloseControl_Load(object sender, EventArgs e)
        {
            undoCloseManager = this.serviceProvider.GetService<SUndoCloseManagerService, IUndoCloseManagerService>();
            InitializeList();
        }

        private void lstDocuments_DoubleClick(object sender, EventArgs e)
        {
            OpenDocument();
        }

        private void lstDocuments_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar == (char)13)
            {
                OpenDocument();
            }
        }
        #endregion

        #region Public Implementation
        /// <summary>
        /// Updates the document list.
        /// </summary>
        public void UpdateDocumentList()
        {
            lstDocuments.BeginUpdate();

            ClearDocumentList();

            undoCloseManager.GetDocuments().ForEach(
                info =>
                {
                    string imageKey;
                    Icon icon = null;

                    imageKey = Path.GetExtension(info.DocumentPath);

                    if(!lstDocuments.SmallImageList.Images.ContainsKey(imageKey))
                    {
                        icon = NativeMethods.GetIcon(info.DocumentPath);

                        if(icon != null)
                        {
                            lstDocuments.SmallImageList.Images.Add(imageKey, icon);
                        }
                    }

                    ListViewItem item =
                        new ListViewItem(info.DocumentPath, rootGroup) { Tag = info, ImageKey = imageKey };

                    lstDocuments.Items.Add(item);
                });

            lstDocuments.EndUpdate();
        }

        /// <summary>
        /// Clears the document list.
        /// </summary>
        public void ClearDocumentList()
        {
            lstDocuments.Items.Clear();
        }
        #endregion

        #region Private Implementation
        private void OpenDocument()
        {
            ListViewItem item = lstDocuments.SelectedItems.OfType<ListViewItem>().First();
            DocumentInfo docInfo = item.Tag as DocumentInfo;

            if(docInfo != null)
            {
                undoCloseManager.PopDocument(docInfo);
                lstDocuments.Items.Remove(item);

                if(File.Exists(docInfo.DocumentPath))
                {
                    DTE dte = serviceProvider.GetService<SDTE, DTE>();

                    try
                    {
                        DTEHelper.OpenDocument(dte, docInfo);
                    }
                    catch(COMException) 
                    { }
                }
            }
        }

        private HierarchyNode GetHierarchy(string file)
        {
            IVsSolution solution = serviceProvider.GetService<SVsSolution, IVsSolution>();

            HierarchyNodeIterator iterator = new HierarchyNodeIterator(solution);

            return iterator.SingleOrDefault(
                node => node.FullName.Equals(file));
        }

        private void InitializeList()
        {
            images.ColorDepth = ColorDepth.Depth32Bit;
            lstDocuments.SmallImageList = images;
            lstDocuments.Columns[0].Width = lstDocuments.Width - 8;
            lstDocuments.Groups.Add(rootGroup);
        }
        #endregion
    }
}