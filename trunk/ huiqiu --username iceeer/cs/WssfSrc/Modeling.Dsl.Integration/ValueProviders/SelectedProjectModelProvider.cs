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
using Microsoft.Practices.RecipeFramework;
using Microsoft.VisualStudio.Modeling.Shell;
using Microsoft.Practices.Modeling.Dsl.Integration.Helpers;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.Practices.ComponentModel;
using Microsoft.Practices.VisualStudio.Helper;
using EnvDTE;

namespace Microsoft.Practices.Modeling.Dsl.Integration.ValueProviders
{
	[ServiceDependency(typeof(IVsSolution))]
	public class SelectedProjectModelProvider : ValueProvider
	{
		public override bool OnBeginRecipe(object currentValue, out object newValue)
		{
			newValue = null;
			ModelingDocView docView = DesignerHelper.GetModelingDocView(this.Site);
			IVsSolution solution = this.GetService<IVsSolution>(true);
			using (HierarchyNode hNode = new HierarchyNode(solution, docView.DocData.Hierarchy))
			{
				newValue = hNode.ExtObject;
				return (newValue is EnvDTE.Project);
			}
		}
	}
}
