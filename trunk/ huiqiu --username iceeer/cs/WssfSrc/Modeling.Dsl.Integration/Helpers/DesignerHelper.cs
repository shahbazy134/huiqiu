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
using Microsoft.VisualStudio.Modeling.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.Practices.VisualStudio.Helper;
using System.IO;
using Microsoft.Practices.RecipeFramework.Library;

namespace Microsoft.Practices.Modeling.Dsl.Integration.Helpers
{
	public static class DesignerHelper
	{
		private static ModelingDocView GetWindowFrameProperty(IServiceProvider provider, __VSFPROPID propertyId)
		{
			IVsMonitorSelection selection = (IVsMonitorSelection)provider.GetService(typeof(IVsMonitorSelection));
			if(selection != null)
			{
				object frameObject;
				selection.GetCurrentElementValue(2, out frameObject);

				IVsWindowFrame windowFrame = frameObject as IVsWindowFrame;
				if(windowFrame != null)
				{
					object propertyValue;
					windowFrame.GetProperty((int)propertyId, out propertyValue);
					return propertyValue as ModelingDocView;
				}
			}

			return null;
		}

		public static ModelingDocView GetModelingDocView(IServiceProvider provider)
		{
			Guard.ArgumentNotNull(provider, "provider");			
			return GetWindowFrameProperty(provider, __VSFPROPID.VSFPROPID_DocView);
		}

		public static void EnsureValidPath(string path, ModelingDocData docData)
		{
			Guard.ArgumentNotNullOrEmptyString(path, "path");
			Guard.ArgumentNotNull(docData, "docData");
			
			// bypass auto save files
			if (Path.GetFileName(path).StartsWith("~", StringComparison.OrdinalIgnoreCase))
			{
				return;
			}

			IVsSolution solution = (IVsSolution)docData.Store.GetService(typeof(SVsSolution));
			using (HierarchyNode rootNode = new HierarchyNode(solution, docData.Hierarchy))
			{
				if (!Path.GetDirectoryName(path).StartsWith(
						Path.GetDirectoryName(rootNode.Path), StringComparison.OrdinalIgnoreCase))
				{
					throw new InvalidOperationException(Properties.Resources.SaveOutsideProjectDirException);
				}
			}
		}

		public static SingleDiagramDocView GetDiagramDocView(IServiceProvider provider)
		{
			return GetWindowFrameProperty(provider, __VSFPROPID.VSFPROPID_DocView) as SingleDiagramDocView;
		}
	}
}
