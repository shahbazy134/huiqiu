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
using System.Runtime.Serialization;
using EnvDTE;
using EnvDTE80;
using Microsoft.Practices.RecipeFramework;
using Microsoft.Practices.RecipeFramework.Extensions.CommonHelpers;

namespace Microsoft.Practices.ServiceFactory.References
{
	[Serializable]
	public class SolutionFolderHasProjectsReference : UnboundRecipeReference
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SolutionFolderHasProjectsReference"/> class.
		/// </summary>
		/// <param name="recipe">The recipe.</param>
		public SolutionFolderHasProjectsReference(string recipe)
			: base(recipe)
		{
		}

		/// <summary>
		/// 	<seealso cref="P:Microsoft.Practices.RecipeFramework.IAssetReference.AppliesTo"/>
		/// </summary>
		/// <value></value>
		public override string AppliesTo
		{
			get { return Properties.Resources.SolutionFolderHasProjectsReference; }
		}

		/// <summary>
		/// 	<seealso cref="M:Microsoft.Practices.RecipeFramework.IUnboundAssetReference.IsEnabledFor(System.Object)"/>
		/// </summary>
		/// <param name="target"></param>
		/// <returns></returns>
		public override bool IsEnabledFor(object target)
		{
			Project project = target as Project;
			if (project != null && 
				project.Object is SolutionFolder)
			{
				foreach (ProjectItem item in project.ProjectItems)
				{
					if (item.Object is Project)
					{
						return true;
					}
				}
			}
			return false;
		}

		#region ISerializable Members

		protected SolutionFolderHasProjectsReference(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		#endregion ISerializable Members
	}
}
