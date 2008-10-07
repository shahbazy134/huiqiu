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
using Microsoft.Practices.ServiceFactory.Library.Presentation.Models;
using Microsoft.Practices.ServiceFactory.Library.Presentation.Base;
using Microsoft.Practices.Modeling.Presentation.Models;

namespace Microsoft.Practices.ServiceFactory.Recipes.CreateModel.Presentation
{
	public interface IModelPropertiesModel : IModel
	{
		ModelType ModelType { get; set;}
		string ModelName { get; set;}
		IProjectModel ModelProject { get;}
		string NamespacePrefix { get; set;}
	}
}