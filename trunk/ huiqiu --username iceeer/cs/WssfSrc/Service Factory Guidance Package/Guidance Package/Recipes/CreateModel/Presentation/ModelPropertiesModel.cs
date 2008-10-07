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
using Microsoft.Practices.ServiceFactory.Library.Presentation.Base;
using Microsoft.Practices.ServiceFactory.Library.Presentation.Models;
using Microsoft.Practices.Modeling.Presentation.Models;

namespace Microsoft.Practices.ServiceFactory.Recipes.CreateModel.Presentation
{
	public class ModelPropertiesModel : ModelBase, IModelPropertiesModel
	{
		private const string ModelTypeKey = "ModelType";
		private const string ModelNameKey = "ModelName";
		private const string ModelProjectKey = "ModelProject";
		private const string NamespacePrefixKey = "NamespacePrefix";

		IProjectModel projectModel;

		public ModelPropertiesModel(IDictionaryService dictionary, IProjectModel projectModel)
			: base(dictionary)
		{
			this.projectModel = projectModel;
		}

		public ModelType ModelType
		{
			get 
			{
				if(ModelTypeDictionary.GetValue(ModelTypeKey) == null)
				{
					return ModelType.ServiceModel;
				}
				else 
				{
					return (ModelType)ModelTypeDictionary.GetValue(ModelTypeKey);
				}
			}

			set { ModelTypeDictionary.SetValue(ModelTypeKey, value); }
		}

		public string ModelName
		{
			get { return ModelTypeDictionary.GetValue(ModelNameKey) as string; }
			set { ModelTypeDictionary.SetValue(ModelNameKey, value); }
		}

		public string NamespacePrefix
		{
			get { return ModelTypeDictionary.GetValue(NamespacePrefixKey) as string; }
			set { ModelTypeDictionary.SetValue(NamespacePrefixKey, value); }
		}

		public IProjectModel ModelProject 
		{
			get
			{
				return this.projectModel;
			}
		}

		public override bool IsValid
		{
			get 
			{
				return (!string.IsNullOrEmpty(ModelName) && 
					!string.IsNullOrEmpty(NamespacePrefix));
			}
		}
	}
}