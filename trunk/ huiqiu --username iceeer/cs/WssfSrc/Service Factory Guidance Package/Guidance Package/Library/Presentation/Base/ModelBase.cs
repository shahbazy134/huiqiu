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
using Microsoft.Practices.RecipeFramework.Library;

namespace Microsoft.Practices.ServiceFactory.Library.Presentation.Base
{
	public abstract class ModelBase : IModel
	{
		private IDictionaryService modelTypeDictionary;

		protected ModelBase(IDictionaryService dictionary)
		{
			Guard.ArgumentNotNull(dictionary, "dictionary");

			this.modelTypeDictionary = dictionary;
		}

		public abstract bool IsValid
		{
			get;
		}

		protected IDictionaryService ModelTypeDictionary
		{
			get { return this.modelTypeDictionary; }	
		}
	}
}