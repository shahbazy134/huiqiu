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
using Microsoft.Practices.ComponentModel;
using Microsoft.Practices.RecipeFramework.Library.CodeModel.Editors;
using EnvDTE;

namespace Microsoft.Practices.ServiceFactory.RecipeFramework.Extensions.Editors.TypeEditor
{
	public class PrimitiveTypesCodeModelEditorFilter : SitedComponent, ICodeModelEditorFilter
	{
		#region ICodeModelEditorFilter Members

		public bool Filter(CodeElement codeElement)
		{
			//if (codeElement is CodeClass)
			//{
				return false;
			//}
			//return true;
		}

		#endregion
	}
}
