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
using Microsoft.Practices.ServiceFactory.Recipes.CreateModel.Presentation;
using Microsoft.Practices.ServiceFactory.Recipes.CreateModel;
using Microsoft.Practices.ServiceFactory.Library.Presentation.Models;
using Microsoft.Practices.ServiceFactory.Library.Validation;
using Microsoft.Practices.EnterpriseLibrary.Validation;
using Microsoft.Practices.Modeling.Presentation.Models;

namespace ServicesGuidancePackage.Tests.Library.Presentation.CreateModel
{
	internal class MockModelPropertiesView : IModelPropertiesView
	{
		string currentModelName;
		string currentNamespacePrefix;
		ModelType currentModelType;
		IProjectModel currentModelProject;

		#region IModelPropertiesView Members

		public event EventHandler<ValidationEventArgs> ModelNameChanged;

		public event EventHandler ModelTypeChanged;

		public string CurrentModelName
		{
			get { return currentModelName; }
			set { currentModelName = value; }
		}

		public ModelType CurrentModelType
		{
			get { return currentModelType; }
			set { currentModelType = value; }
		}

		public IProjectModel CurrentModelProject
		{
			get { return currentModelProject; }
			set { currentModelProject = value; }
		}

		public event EventHandler<ValidationEventArgs> NamespacePrefixChanged;

		public string CurrentNamespacePrefix
		{
			get { return currentNamespacePrefix; }
			set { currentNamespacePrefix = value; }
		}

		#endregion

		public ValidationResults FireNamespacePrefixChanged(string currentNamespacePrefix)
		{
			CurrentNamespacePrefix = currentNamespacePrefix;
			if(CurrentNamespacePrefix != null)
			{
				ValidationEventArgs args = new ValidationEventArgs(new DefaultValidationResultReport());
				NamespacePrefixChanged("CurrentNamespacePrefix", args);
				return args.ValidationResults;
			}
			return null;
		}

		public ValidationResults FireModelNameChanged(string modelName)
		{
			CurrentModelName = modelName;
			if (ModelNameChanged != null)
			{
				ValidationEventArgs args = new ValidationEventArgs(new DefaultValidationResultReport());
				ModelNameChanged("CurrentModelName", args);
				return args.ValidationResults;
			}
			return null;
		}

		public ValidationResults FireModelTypeChanged(ModelType modelType)
		{
			CurrentModelType = modelType;
			if (ModelTypeChanged != null)
			{
				ValidationEventArgs args = new ValidationEventArgs(new DefaultValidationResultReport());
				ModelTypeChanged("CurrentModelType", args);
				return args.ValidationResults;
			}
			return null;
		}
	}
}