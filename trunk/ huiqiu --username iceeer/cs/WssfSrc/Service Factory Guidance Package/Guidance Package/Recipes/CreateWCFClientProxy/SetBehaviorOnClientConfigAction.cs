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
using EnvDTE;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using Microsoft.Practices.RecipeFramework.Configuration;
using Microsoft.Practices.RecipeFramework;
using Microsoft.Practices.ComponentModel;
using System.ServiceModel;
using Microsoft.Practices.ServiceFactory.Actions;
using Microsoft.Practices.ServiceFactory.Description;

namespace Microsoft.Practices.ServiceFactory.Recipes.CreateWCFClientProxy
{
	/// <summary>
	/// Sets the behavior on the client configuration file.
	/// </summary>
	public class SetBehaviorOnClientConfigAction : ActionBase
	{
		/// <summary>
		/// Gets or sets the host project.
		/// </summary>
		/// <value>The host project.</value>
		[Input(Required = true)]
		public System.Configuration.Configuration Configuration
		{
			get { return configuration; }
			set { configuration = value; }
		} private System.Configuration.Configuration configuration;

		/// <summary>
		/// Gets or sets the behavior element.
		/// </summary>
		/// <value>The behavior element.</value>
		[Input(Required = true)]
		public EndpointBehaviorElement BehaviorElement
		{
			get { return behaviorElement; }
			set { behaviorElement = value; }
		} private EndpointBehaviorElement behaviorElement;

		/// <summary>
		/// Gets or sets the service endpoint.
		/// </summary>
		/// <value>The service endpoint.</value>
		[Input(Required = true)]
		public ServiceEndpoint ServiceEndpoint
		{
			get { return serviceEndpoint; }
			set { serviceEndpoint = value; }
		} private ServiceEndpoint serviceEndpoint;

		/// <summary>
		/// See <see cref="M:Microsoft.Practices.RecipeFramework.IAction.Execute"/>.
		/// </summary>
		public override void Execute()
		{
			if (behaviorElement != null &&
				behaviorElement.Count > 0)
			{
				ServiceModelConfigurationManager configurationManager = new ServiceModelConfigurationManager(configuration);

				// Update the behavior name endpoint
				ClientSection client = configurationManager.GetClient();
				int endpointIndex = GetAddedEndpointIndex(client.Endpoints);
				ChannelEndpointElement endpoint = client.Endpoints[endpointIndex];

				// Set the behavior BehaviorConfiguration values
				behaviorElement.Name = endpoint.Name;
				endpoint.BehaviorConfiguration = behaviorElement.Name;

				// Add this behavior to the current configuration
				configurationManager.UpdateBehavior(behaviorElement);

				// Update the Identity value in case the Wizard changed its value 
				// (i.e. BasicHttp with some Pattern)
				if (serviceEndpoint.Address.Identity != null)
				{
					client.Endpoints[endpointIndex].Identity.InitializeFrom(serviceEndpoint.Address.Identity);
				}
			}
		}

		private int GetAddedEndpointIndex(ChannelEndpointElementCollection endpoints)
		{
			int endpointIndex = -1;

			// look for a matching endpoint address and get the latest endpoint in the collection
			for (int index = 0; index < endpoints.Count; index++)
			{
				if (endpoints[index].Address.Equals(serviceEndpoint.Address.Uri) &&
					endpoints[index].Contract.EndsWith(serviceEndpoint.Contract.Name, StringComparison.OrdinalIgnoreCase))
				{
					endpointIndex = index;
				}
			}

			if (endpointIndex == -1)
			{
				throw new EntryPointNotFoundException();
			}

			return endpointIndex;
		}
	}
}

