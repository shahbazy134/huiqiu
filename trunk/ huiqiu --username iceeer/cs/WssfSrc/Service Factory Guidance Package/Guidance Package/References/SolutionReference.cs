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

using EnvDTE;
using Microsoft.Practices.RecipeFramework.VisualStudio.Templates;
using System;
using System.Runtime.Serialization;

namespace Microsoft.Practices.ServiceFactory.References
{
	/// <summary>
	/// UnboundTemplateReference for a solution or a solution folder
	/// </summary>
	[Serializable]
	public class SolutionReference : UnboundTemplateReference
	{
		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="SolutionOrSolutionFolderReference"/> class.
		/// </summary>
		/// <param name="recipe">The recipe.</param>
		public SolutionReference(String recipe)
			: base(recipe)
		{
		}
		#endregion

		#region Public Implementation
		/// <summary>
		/// Determines whether the reference is enabled for a particular target item,
		/// based on the condition contained in the reference.
		/// </summary>
		/// <param name="target">The <see cref="T:System.Object"/> to check for references.</param>
		/// <returns>
		/// 	<see langword="true"/> if the reference is enabled for the given <paramref name="target"/>.
		/// Otherwise, <see langword="false"/>.
		/// </returns>
		public override bool IsEnabledFor(object target)
		{
			return (target is Solution);
		}

		/// <summary>
		/// See <see cref="P:Microsoft.Practices.RecipeFramework.IAssetReference.AppliesTo"/>.
		/// </summary>
		/// <value></value>
		public override String AppliesTo
		{
			get { return Properties.Resources.SolutionReference; }
		}
		#endregion

		#region ISerializable Members

		/// <summary>
		/// Required constructor for deserialization.
		/// </summary>
		protected SolutionReference(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		#endregion ISerializable Members
	}
}