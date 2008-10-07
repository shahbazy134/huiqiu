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
using System.Diagnostics.CodeAnalysis;
using Microsoft.Practices.Common;
using System.Collections.Specialized;
using Microsoft.Practices.RecipeFramework.Library;
using EnvDTE;
using System.ComponentModel.Design;
using Microsoft.Practices.ComponentModel;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.Practices.VisualStudio.Helper;
using System.IO;

namespace Microsoft.Practices.ServiceFactory.ValueProviders
{
	[ServiceDependency(typeof(IDictionaryService))]
	[ServiceDependency(typeof(IVsSolution))]
	public class ModelFilesProvider : ValueProvider, IAttributesConfigurable
	{
		private const string ModelProjectAttributeName = "ModelProject";
		private const string ExtensionAttributeName = "Extension";
		private string modelProjectName;
		private string extension;

		/// <summary>
		/// Contains code that will be called when recipe execution begins. This is the first method in the lifecycle.
		/// </summary>
		/// <param name="currentValue">An <see cref="T:System.Object"/> that contains the current value of the argument.</param>
		/// <param name="newValue">When this method returns, contains an <see cref="T:System.Object"/> that contains
		/// the new value of the argument, if the returned value
		/// of the method is <see langword="true"/>. Otherwise, it is ignored.</param>
		/// <returns>
		/// 	<see langword="true"/> if the argument value should be replaced with
		/// the value in <paramref name="newValue"/>; otherwise, <see langword="false"/>.
		/// </returns>
		/// <remarks>By default, always returns <see langword="false"/>, unless overriden by a derived class.</remarks>
		public override bool OnBeginRecipe(object currentValue, out object newValue)
		{
			List<string> modelfiles = new List<string>();
			Project modelProject = GetModelProject(modelProjectName);
			IVsSolution solution = (IVsSolution)ServiceHelper.GetService(this, typeof(IVsSolution));
			using (HierarchyNode node = new HierarchyNode(solution, modelProject.UniqueName))
			{
				node.RecursiveForEach(delegate(HierarchyNode func)
				{
					if (Path.GetExtension(func.Name).Equals(extension, StringComparison.OrdinalIgnoreCase))
					{
						modelfiles.Add(func.Name);
					}
				});
			}
			newValue = modelfiles.ToArray();
			return (modelfiles.Count > 0);
		}

		#region IAttributesConfigurable Members

		/// <summary>
		/// Configures the component using the dictionary of attributes specified
		/// in the configuration file.
		/// </summary>
		/// <param name="attributes">The attributes in the configuration element.</param>
		public void Configure(StringDictionary attributes)
		{
			Guard.ArgumentNotNull(attributes, "attributes");

			if (attributes.ContainsKey(ModelProjectAttributeName))
			{
				modelProjectName = attributes[ModelProjectAttributeName];
			}

			if (attributes.ContainsKey(ExtensionAttributeName))
			{
				extension = attributes[ExtensionAttributeName];
			}
			Guard.ArgumentNotNullOrEmptyString(extension, ExtensionAttributeName);
		}

		#endregion

		private Project GetModelProject(string projectArgumentName)
		{
			IDictionaryService dictionaryService = (IDictionaryService)ServiceHelper.GetService(
				this, typeof(IDictionaryService));
			Project project = dictionaryService.GetValue(projectArgumentName) as Project;
			
			Guard.ArgumentNotNull(project, ModelProjectAttributeName);
			
			return project;
		}
	}
}
