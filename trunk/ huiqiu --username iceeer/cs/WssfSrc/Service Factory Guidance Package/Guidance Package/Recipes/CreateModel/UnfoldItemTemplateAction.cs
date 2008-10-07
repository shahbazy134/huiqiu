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
using Microsoft.Practices.RecipeFramework;
using Microsoft.Practices.Common.Services;
using System.IO;
using EnvDTE;
using System.ComponentModel.Design;
using EnvDTE80;
using System.Globalization;
using Microsoft.Practices.RecipeFramework.Library;

namespace Microsoft.Practices.ServiceFactory.Recipes.CreateModel
{
	public class UnfoldItemTemplateAction : ConfigurableAction
	{
		private string itemName;

		[Input(Required = true)]
		public string ItemName
		{
			get
			{
				return this.itemName;
			}
			set
			{
				this.itemName = value;
			}
		}

		private Project project;

		[Input(Required = true)]
		public Project Project
		{
			get
			{
				return this.project;
			}
			set
			{
				this.project = value;
			}
		}

		private string template;

		[Input(Required = true)]
		public string Template
		{
			get
			{
				if(!File.Exists(this.template))
				{
					TypeResolutionService service = (TypeResolutionService)this.GetService(typeof(ITypeResolutionService));
					if(service != null)
					{
						this.template = new FileInfo(Path.Combine(Path.Combine(service.BasePath, @"Templates\"), this.template)).FullName;
					}
				}
				return this.template;
			}
			set
			{
				this.template = value;
			}
		}

		private ProjectItem newItem;

		[Output]
		public ProjectItem NewItem
		{
			get
			{
				return this.newItem;
			}
			set
			{
				this.newItem = value;
			}
		}

		public override void Execute()
		{
			if(this.Project != null)
			{
				this.NewItem = this.Project.ProjectItems.AddFromTemplate(this.Template, this.ItemName);
			}
		}

		public override void Undo()
		{
		}
	}
}