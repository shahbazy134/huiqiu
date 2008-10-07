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
using System.Text;
using Microsoft.Practices.ServiceFactory.HostDesigner;
using Microsoft.Build.BuildEngine;
using Microsoft.Practices.RecipeFramework.Library;
using EnvDTE;
using Microsoft.Practices.ComponentModel;
using Microsoft.Practices.Modeling.Dsl.Integration.ValueProviders;
using Microsoft.Practices.RecipeFramework.Extensions.CommonHelpers;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.Practices.VisualStudio.Helper;

namespace Microsoft.Practices.ServiceFactory.ValueProviders
{
	[ServiceDependency(typeof(SVsSolution))]
	public class HostModelClientProjectProvider : DesignerSelectedElementProvider
	{
		public override bool OnBeginRecipe(object currentValue, out object newValue)
		{
			object modelElement = null; 
			if (base.OnBeginRecipe(currentValue, out modelElement))
			{
				Proxy proxy = modelElement as Proxy;
				if (proxy != null &&
					!string.IsNullOrEmpty(proxy.ClientApplication.ImplementationProject))
				{
					IVsSolution solution = GetService<IVsSolution, SVsSolution>();
					using (HierarchyNode projectNode = new HierarchyNode(solution, proxy.ClientApplication.ImplementationProject))
					{
						if (projectNode != null)
						{
							newValue = projectNode.ExtObject as EnvDTE.Project;
							return true;
						}
					}
				}
			}
			newValue = null;
			return false;
		}
	}
}
