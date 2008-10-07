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
using System.ComponentModel.Design;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using EnvDTE;
using Microsoft.Practices.Common;
using Microsoft.Practices.ComponentModel;
using Microsoft.Practices.RecipeFramework;
using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Modeling.Diagnostics;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Modeling.Utilities;
using Microsoft.Practices.Modeling.Dsl.Integration.Helpers;
using Microsoft.VisualStudio.Modeling.Shell;
using Microsoft.VisualStudio.Modeling.Diagrams;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Practices.Modeling.Dsl.Integration.ValueProviders
{
	public class DesignerSelectedElementProvider : ValueProvider
	{
		// FxCop: This code is calling into external libraries and we cannot know what exceptions 
		//		  might be thrown.
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
		public override bool OnBeginRecipe(object currentValue, out object newValue)
		{
			newValue = null;

            try
            {
                newValue = DomainModelHelper.GetSelectedElement(this.Site);

                return true;
            }
            catch
            {
                return false;
            }
		}

		/// <summary>
		/// Gets the service.
		/// </summary>
		/// <returns></returns>
		protected TInterface GetService<TInterface, TImpl>()
		{
			return (TInterface)GetService(typeof(TImpl));
		}
	}
}
