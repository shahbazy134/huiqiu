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
using Microsoft.Practices.RecipeFramework.Extensions.TypeEditor;
using Microsoft.Practices.RecipeFramework.Library.CodeModel.Editors;

namespace Microsoft.Practices.ServiceFactory.RecipeFramework.Extensions.Editors.TypeEditor
{
	/// <summary>
	/// Configuration filter for public primitive types only for use with the <see cref="CLRTypeEditor"/>.
	/// </summary>
	public class PrimitiveCLRTypeEditorConfig : ICLRTypeEditorConfig
	{
		#region ICLRTypeEditorConfig Members

		/// <summary>
		/// <seealso cref="ICLRTypeEditorConfig.BrowseRoot"/>
		/// </summary>
		public CodeModelEditor.BrowseRoot BrowseRoot
		{
			get { return CodeModelEditor.BrowseRoot.Solution; }
		}

		/// <summary>
		/// <seealso cref="ICLRTypeEditorConfig.BrowseKind"/>
		/// </summary>
		public CodeModelEditor.BrowseKind BrowseKind
		{
			get { return CodeModelEditor.BrowseKind.Class; }
		}

		/// <summary>
		/// <seealso cref="ICLRTypeEditorConfig.Filter"/>
		/// </summary>
		public Type Filter
		{
			get { return typeof(PrimitiveTypesCodeModelEditorFilter); }
		}

		#endregion
	}
}
