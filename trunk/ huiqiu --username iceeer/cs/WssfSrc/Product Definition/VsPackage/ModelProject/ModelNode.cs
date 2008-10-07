//===============================================================================
// Microsoft patterns & practices
// Web Service Software Factory
//===============================================================================
// Copyright � Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================
// The example companies, organizations, products, domain names,
// e-mail addresses, logos, people, places, and events depicted
// herein are fictitious.  No association with any real company,
// organization, product, domain name, email address, logo, person,
// places, or events is intended or should be inferred.
//===============================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Microsoft.VisualStudio.Package;
using System.Runtime.InteropServices;
using Microsoft.Practices.ServiceFactory.Actions;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Modeling.Shell;
using Microsoft.Practices.Modeling.CodeGeneration.Artifacts;
using Logging = Microsoft.Practices.Modeling.CodeGeneration.Logging;
using System.Diagnostics;
using Microsoft.Practices.Modeling.CodeGeneration;
using Microsoft.VisualStudio.Modeling;
using VsCommands = Microsoft.VisualStudio.VSConstants.VSStd97CmdID;
using VsCommands2K = Microsoft.VisualStudio.VSConstants.VSStd2KCmdID;
using Microsoft.VisualStudio.Shell;

namespace Microsoft.Practices.ServiceFactory.VsPkg.ModelProject
{
    public class ModelNode : FileNode
    {
        private OAModelProjectFileNode automationObject;

        public ModelNode(ProjectNode root, ProjectElement item)
            : base(root, item)
        {
            if (item != null)
            {
				string desiredItemName = BuildAction.Model.ToString();
				if (string.Compare(desiredItemName, item.ItemName, StringComparison.OrdinalIgnoreCase) != 0)
				{
					item.ItemName = desiredItemName;
				}
            }
        }

		#region Overriden implementation

		public override void Remove(bool removeFromStorage)
		{
			OnModelDetach();
			base.Remove(removeFromStorage);
		}

		protected override int ExcludeFromProject()
		{
			OnModelDetach();
			return base.ExcludeFromProject();
		}

		protected override int QueryStatusOnNode(Guid cmdGroup, uint cmd, IntPtr pCmdText, ref QueryStatusResult result)
		{
			if (cmdGroup == Microsoft.VisualStudio.Package.VsMenus.guidStandardCommandSet97)
			{
				switch ((VsCommands)cmd)
				{
					case VsCommands.Copy:
					case VsCommands.Cut:
					case VsCommands.Paste:
						result = QueryStatusResult.NOTSUPPORTED | QueryStatusResult.INVISIBLE;
						return VSConstants.S_OK;
				}
			}
			else if (cmdGroup == Microsoft.VisualStudio.Package.VsMenus.guidStandardCommandSet2K)
			{
				switch ((VsCommands2K)cmd)
				{
					case VsCommands2K.COPY:
					case VsCommands2K.CUT:
					case VsCommands2K.PASTE:
					case VsCommands2K.RUNCUSTOMTOOL:
						result = QueryStatusResult.NOTSUPPORTED | QueryStatusResult.INVISIBLE;
						return VSConstants.S_OK;
				}
			}
			return base.QueryStatusOnNode(cmdGroup, cmd, pCmdText, ref result);
		}

		protected override StringBuilder PrepareSelectedNodesForClipBoard()
		{
			return new StringBuilder();
		}


		protected override NodeProperties CreatePropertiesObject()
		{
			return new ModelNodeProperties(this);
		}

		public override object GetAutomationObject()
		{
			if (automationObject == null)
			{
				automationObject = new OAModelProjectFileNode(this.ProjectMgr.GetAutomationObject() as OAModelProject, this);
			}

			return automationObject;
		}

		#endregion

        private void OnModelDetach()
        {
            FileDocumentManager fileManager = (FileDocumentManager)this.GetDocumentManager();
            fileManager.Open(false, false, WindowFrameShowAction.DontShow);
            try
            {
                ModelingDocData docData = DocData;
                if (docData != null && docData.RootElement != null && docData.RootElement.Store != null)
                {
                    IArtifactLinkContainer links = ModelCollector.GetArtifacts(docData.RootElement.Store);
                    ICodeGenerationService codeGenerationService = GetService<ICodeGenerationService>();
                    codeGenerationService.ValidateDeleteFromCollection(links.ArtifactLinks);
                }
            }
            finally
            {
                fileManager.Close(__FRAMECLOSE.FRAMECLOSE_NoSave);
            }
        }

        private ModelingDocData DocData
        {
            get
            {
                IVsRunningDocumentTable rdt = GetService<IVsRunningDocumentTable, SVsRunningDocumentTable>();
                if (rdt == null)
                {
                    return null;
                }
                IVsHierarchy vsHierarchy = null;
                uint itemid = VSConstants.VSITEMID_NIL;
                IntPtr pDocData = IntPtr.Zero;
                uint docCookie = 0;
                Marshal.ThrowExceptionForHR(
                    rdt.FindAndLockDocument((uint)_VSRDTFLAGS.RDT_NoLock,
                        this.Url,
                        out vsHierarchy,
                        out itemid, out pDocData, out docCookie));
                if (pDocData == IntPtr.Zero)
                {
                    return null;
                }
                return Marshal.GetObjectForIUnknown(pDocData) as ModelingDocData;
            }
        }

        private T GetService<T>()
        {
            return (T) this.GetService(typeof(T));
        }

        private TInterface GetService<TInterface, TService>()
        {
          if (this.ProjectMgr.Site == null)
          {
              return default(TInterface);
          }

            return (TInterface)this.ProjectMgr.Site.GetService(typeof(TService));
        }
    }
}
