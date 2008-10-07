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
using System.ComponentModel.Design;
using Microsoft.Practices.RecipeFramework;
using Microsoft.Practices.Common;
using Microsoft.Practices.RecipeFramework.Library;
using Microsoft.Practices.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Practices.ServiceFactory.ValueProviders
{
	[ServiceDependency(typeof(IDictionaryService))]
	public class SelectedProjectProviderBase : ValueProvider, IAttributesConfigurable
	{
		const string SelectedProjectAttributeName = "SelectedProject";
		string selectedProjectName;
		EnvDTE.Project selectedProject;

		protected virtual EnvDTE.Project SelectedProject
		{
			get
			{
				if (selectedProject == null &&
					!string.IsNullOrEmpty(selectedProjectName))
				{
					IDictionaryService dictservice = this.GetService<IDictionaryService>(true);
					selectedProject = dictservice.GetValue(selectedProjectName) as EnvDTE.Project;
				}
				return selectedProject;
			}
		}

		#region IAttributesConfigurable Members

		public virtual void Configure(System.Collections.Specialized.StringDictionary attributes)
		{
			Guard.ArgumentNotNull(attributes, "attributes");

			if (attributes.ContainsKey(SelectedProjectAttributeName))
			{
				this.selectedProjectName = attributes[SelectedProjectAttributeName];
			}
		}

		#endregion
	}
}
