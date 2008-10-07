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
using Microsoft.Practices.ComponentModel;
using Microsoft.Practices.RecipeFramework;
using System.ServiceModel.Description;
using Microsoft.Practices.ServiceFactory.Description;
using EnvDTE;
using System.CodeDom;

namespace Microsoft.Practices.ServiceFactory.Recipes.CreateWCFClientProxy
{
	[ServiceDependency(typeof(DTE))]
	public class UpdateClientConfigAction : ActionBase
	{
		private ServiceEndpoint serviceEndpoint;
		private ContractGenerationOptions options;

		/// <summary>
		/// Gets or sets the service endpoint.
		/// </summary>
		/// <value>The service endpoint.</value>
		[Input(Required = true)]
		public ServiceEndpoint ServiceEndpoint
		{
			get { return serviceEndpoint; }
			set { serviceEndpoint = value; }
		}

		/// <summary>
		/// Gets or sets the CLR namespace.
		/// </summary>
		/// <value>The CLR namespace.</value>
		[Input(Required = true)]
		public ContractGenerationOptions ContractGenerationOptions
		{
			get { return options; }
			set { options = value; }
		}

		/// <summary>
		/// See <see cref="M:Microsoft.Practices.RecipeFramework.IAction.Execute"/>.
		/// </summary>
		public override void Execute()
		{
			if (this.IsValidModel())
			{
				ContractGenerator proxyGenerator = new ContractGenerator(options);
				// refresh metadata in order to avoid the config error (customBinding)
				MetadataDiscovery disco = new MetadataDiscovery(this.serviceEndpoint.Address.Uri);

				proxyGenerator.Generate(disco.InspectMetadata());
				 
				// clear logs
				base.ClearLogs();
			}
			else
			{
				throw new InvalidOperationException(Properties.Resources.ValidationException);
			}
		}
	}
}
