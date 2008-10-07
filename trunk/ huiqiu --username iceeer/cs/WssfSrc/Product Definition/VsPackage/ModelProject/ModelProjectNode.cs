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
using Microsoft.VisualStudio.Shell;
using Microsoft.Practices.ServiceFactory;
using System.Runtime.InteropServices;
using VsCommands = Microsoft.VisualStudio.VSConstants.VSStd97CmdID;
using VsCommands2K = Microsoft.VisualStudio.VSConstants.VSStd2KCmdID;
using Microsoft.VisualStudio;
using Microsoft.Build.BuildEngine;
using System.Windows.Forms;
using System.Drawing;

namespace Microsoft.Practices.ServiceFactory.VsPkg.ModelProject
{
    [ComVisible(true)]
    // FXCOP: This type does not need to be created directly by COM.
    [SuppressMessage("Microsoft.Interoperability", "CA1409:ComVisibleTypesShouldBeCreatable")]
    [Guid("4C2CCFB2-D3D0-4E26-A8F4-746AC78E5A02")]
    public class ModelProjectNode : ProjectNode, IModelProject
    {

        #region Enum for image list
        internal enum ModelProjectImageName
        {
            Project = 0,
        }
        #endregion

        #region Constants
        internal const string ProjectTypeName = "ModelProject";
        #endregion

        #region Fields
        //private ModelProjectPackage package;
        internal static int imageOffset;
        private static ImageList imageList = Microsoft.VisualStudio.Package.Utilities.GetImageList(typeof(ModelProjectNode).Assembly.GetManifestResourceStream("Microsoft.Practices.ServiceFactory.VsPkg.Resources.ModelProjectImageList.bmp"));
        private VSLangProj.VSProject vsProject;
        #endregion

        #region Constructors
        public ModelProjectNode()
            : base()
        {
            CanProjectDeleteItems = true;
            CanFileNodesHaveChilds = true;

            InitializeImageList();

            // Add Category IDs mapping in order to support properties for project items
            AddCATIDMapping(typeof(ModelNodeProperties), typeof(ModelNodeProperties).GUID);
            AddCATIDMapping(typeof(ProjectNodeProperties), typeof(ProjectNodeProperties).GUID);
        }
        #endregion

        #region Properties
        public static ImageList ImageList
        {
            get
            {
                return imageList;
            }
            set
            {
                imageList = value;
            }
        }

        protected internal VSLangProj.VSProject VSProject
        {
            get
            {
                if (vsProject == null)
                {
                    vsProject = new Microsoft.VisualStudio.Package.Automation.OAVSProject(this);
                }

                return vsProject;
            }
        }
        #endregion

        #region override

		/// <summary>
		/// Set the configuration in MSBuild.
		/// This does not get persisted and is used to evaluate msbuild conditions
		/// which are based on the $(Configuration) property.
		/// </summary>
		protected override void SetCurrentConfiguration()
		{			
			try
			{				
				base.SetCurrentConfiguration();
			}
			catch (COMException)
			{
				// Catch the COMException that may throw 'Utilities.GetActiveConfigurationName'
				this.SetConfiguration("");
			}
		}
	
        protected override bool IsItemTypeFileType(string type)
        {
            if (String.Compare(type, BuildAction.Model.ToString(), StringComparison.OrdinalIgnoreCase) == 0
                || String.Compare(type, BuildAction.Content.ToString(), StringComparison.OrdinalIgnoreCase) == 0
                || String.Compare(type, BuildAction.None.ToString(), StringComparison.OrdinalIgnoreCase) == 0)
                return true;

            // we don't know about this type, so ignore it.
            return false;
        }

        public override FileNode CreateFileNode(ProjectElement item)
        {
            return new ModelNode(this, item);
        }

        public override int ImageIndex
        {
            get
            {
                return imageOffset + (int)ModelProjectImageName.Project;
            }
        }

        protected override QueryStatusResult QueryStatusCommandFromOleCommandTarget(Guid cmdGroup, uint cmd, out bool handled)
        {
            if (cmdGroup == Microsoft.VisualStudio.Package.VsMenus.guidStandardCommandSet97)
            {
                switch ((VsCommands)cmd)
                {
                    case VsCommands.BuildOrder:
                    case VsCommands.AddExistingItem:
                    case VsCommands.AddNewItem:
                    case VsCommands.SetStartupProject:
                    case VsCommands.ProjectSettings:
                    case VsCommands.ProjectProperties:
                    case VsCommands.PropSheetOrProperties:
                    case VsCommands.ProjectDependencies:
                    case VsCommands.Properties:
                    case VsCommands.PropertyPages:
                    case VsCommands.DebugOptions:
                    case VsCommands.NewFolder:
                    case VsCommands.StartNoDebug:
                        handled = true;
                        return QueryStatusResult.NOTSUPPORTED | QueryStatusResult.INVISIBLE;
                }
            }
            else if (cmdGroup == Microsoft.VisualStudio.Package.VsMenus.guidStandardCommandSet2K)
            {
                switch ((VsCommands2K)cmd)
                {
                    case VsCommands2K.FOLDER:
                    case VsCommands2K.DEPENDENCIES:
                    case VsCommands2K.PROJSETTINGS:
                    case VsCommands2K.ADDREFERENCE:
                    case VsCommands2K.PROJSTARTDEBUG:
                    case VsCommands2K.PROJSTEPINTO:
                    case VsCommands2K.Debug:
                        handled = true;
                        return QueryStatusResult.NOTSUPPORTED | QueryStatusResult.INVISIBLE;
                }
            }
            return base.QueryStatusCommandFromOleCommandTarget(cmdGroup, cmd, out handled);
        }

        protected override NodeProperties CreatePropertiesObject()
        {
            return new ModelProjectNodeProperties(this);
        }


        /// <summary>
        /// Model projects cannot have references, so we don't process any.
        /// </summary>
        protected override void ProcessReferences()
        {
        }

        public override object GetAutomationObject()
        {
            return new OAModelProject(this);
        }

        public override Guid ProjectGuid
        {
            get { return typeof(ModelProjectFactory).GUID; }
        }

        public override string ProjectType
        {
            get { return ProjectTypeName; }
        }

        protected override void AddNewFileNodeToHierarchy(HierarchyNode parentNode, string fileName)
        {
            HierarchyNode discoveredParentNode = FindParentNode(parentNode, fileName);
            base.AddNewFileNodeToHierarchy(discoveredParentNode, fileName);
        }

        protected virtual HierarchyNode FindParentNode(HierarchyNode parentNode, string fileName)
        {
            if (this.CanFileNodesHaveChilds)
            {
                string newFile = System.IO.Path.GetFileNameWithoutExtension(fileName);

                // Search only sibling nodes for name similarities, if exist assume fileName is dependent upon
                HierarchyNode potentialParent = parentNode.FirstChild;
                while (potentialParent != null)
                {
                    FileNode fileNode = potentialParent as FileNode;
                    if (fileNode != null && newFile.Equals(fileNode.FileName, StringComparison.OrdinalIgnoreCase))
                    {
                        return potentialParent;
                    }
                    potentialParent = potentialParent.NextSibling;
                }
            }

            return parentNode;
        }

        #endregion


        #region IModelProject Members

        public string Namespace
        {
            get
            {
                if (this.BuildProject != null && this.BuildProject.EvaluatedProperties != null)
                {
                    BuildPropertyGroup properties = this.BuildProject.EvaluatedProperties;
                    if (properties["Rootnamespace"] != null)
                    {
                        return properties["Rootnamespace"].Value;
                    }
                }
                return string.Empty;
            }
        }

        #endregion

        #region Private implementation
        private void InitializeImageList()
        {
            imageOffset = this.ImageHandler.ImageList.Images.Count;

            foreach (Image img in ImageList.Images)
            {
                this.ImageHandler.AddImage(img);
            }
        }
        #endregion
    }
}
