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
using Microsoft.Practices.ServiceFactory.Actions;
using Microsoft.Practices.ServiceFactory.RecipeFramework.Extensions.ProjectMapping;

namespace Microsoft.Practices.ServiceFactory.Recipes.CreateSolution
{
	public class CreateMappingFileAction : ActionBase
	{
		public override void Execute()
		{
			ProjectMappingManager.Instance.CreateMappingFile().Dispose();
		}

		public override void Undo()
		{
			// not implemented
		}
	}
}
