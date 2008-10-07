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
using System.ServiceModel.Configuration;
using System.ComponentModel;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.ComponentModel.Design;
using System.ServiceModel.Description;
using System.Runtime.Remoting.Messaging;
using System.ServiceModel.Channels;
using Microsoft.Practices.RecipeFramework.Extensions.CommonHelpers;
using Microsoft.Practices.EnterpriseLibrary.Validation;
using System.Windows.Forms;
using Microsoft.Practices.ServiceFactory.Description;
using Microsoft.Practices.ServiceFactory.Library.Validation;
using RecLib = Microsoft.Practices.RecipeFramework.Library;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Practices.VisualStudio.Helper;
using Microsoft.Practices.ServiceFactory.Recipes.CreateASMXClientProxy;
using System.Net;
using System.CodeDom;

namespace Microsoft.Practices.ServiceFactory.Recipes.CreateWCFClientProxy.Presentation
{
	/// <summary>
	/// The presenter class for pages that implements <see cref="ISecureClientConfigView"/> interface.
	/// </summary>
	public class SecureClientConfigPresenter
	{
		private ISecureClientConfigView view;
        private bool eventsCancelled;
        private Dictionary<string, ServiceEndpoint> serviceEndpoints;
        private MetadataDiscovery metadata;
        private SecureClientConfigModel model;
		
        public event EventHandler ViewLoaded;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:SecureClientConfigPresenter"/> class.
		/// </summary>
		/// <param name="view">The view.</param>
		public SecureClientConfigPresenter(ISecureClientConfigView view, SecureClientConfigModel model)
		{
			RecLib.Guard.ArgumentNotNull(view, "view");
			RecLib.Guard.ArgumentNotNull(model, "model");
            
            this.view = view;
            this.model = model;

            this.serviceEndpoints = new Dictionary<string, ServiceEndpoint>();
            this.metadata = new MetadataDiscovery(view.ServiceAddress);
            this.metadata.InspectMetadataCompleted += OnInspectMetadataCompleted;
            view.ViewLoading += OnViewLoading;
            view.ViewClosing += OnViewClosing;
            view.SelectedEndpointChanging += OnSelectedEndpointChanging;
            view.RequestIsDataValid += OnRequestIsDataValid;
        }

        private void OnViewLoading(object sender, EventArgs e)
        {
            eventsCancelled = false;
            view.ClearEndpoints();
            view.SetProgress(true);
            metadata.InspectMetadataAsync();
        }

        private void OnViewClosing(object sender, FormClosingEventArgs e)
        {
            this.eventsCancelled = true;
        }

        private void OnSelectedEndpointChanging(object sender, ValidationEventArgs e)
        {
			if (!string.IsNullOrEmpty(view.SelectedEndpoint) &&
				this.serviceEndpoints.ContainsKey(view.SelectedEndpoint))
			{
				this.model.ServiceEndpoint = this.serviceEndpoints[view.SelectedEndpoint];
				this.model.ContractGenerationOptions.ImportedEndpointNames.Clear();
				this.model.ContractGenerationOptions.ImportedEndpointNames.Add(this.model.ServiceEndpoint.Name);
			}
			else
			{
				this.model.ServiceEndpoint = null;
			}
        }

        private void OnRequestIsDataValid(object sender, RequestDataEventArgs<bool> e)
        {
            e.Value = this.model.IsValid;
        }

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
		private void OnInspectMetadataCompleted(object sender, InspectMetadataCompletedEventArgs e)
        {
			try
			{
				if (!eventsCancelled)
				{
					view.SetProgress(false);

					if (e.Exception != null)
					{
						view.ShowMessage(ExceptionHelper.BuildErrorMessage(e.Exception), MessageBoxIcon.Error);
						return;
					}

					ContractGenerationOptions options = new ContractGenerationOptions();
					options.CodeProvider = this.model.CodeDomProvider;
					options.ClrNamespace = this.model.ClientProjectNamespace;
					options.OutputConfiguration = model.Configuration;
					// Add the build output path to the current AppDomain probing path
					options.AssemblyResolvePath = model.BuildPath;

					ServiceEndpointCollection serviceEndpoints = GetServiceEndPoints(options, e.Metadata);

					if (serviceEndpoints != null &&
						serviceEndpoints.Count > 0)
					{
						AddEndpoints(serviceEndpoints);
						// select the first item
						view.SelectedEndpoint = serviceEndpoints[0].Name;
					}
				}
			}
			catch (Exception exception)
			{
				view.ShowMessage(ExceptionHelper.BuildErrorMessage(exception), MessageBoxIcon.Error);
			}
            finally
            {
                if (ViewLoaded != null)
                {
                    ViewLoaded(this, new EventArgs());
                }
            }
        }

        /// <summary>
        /// Adds the endpoints.
        /// </summary>
        /// <param name="endpoints">The endpoints.</param>
        private void AddEndpoints(ServiceEndpointCollection endpoints)
        {
            this.serviceEndpoints.Clear();

            foreach (ServiceEndpoint endpoint in endpoints)
            {
				if (!this.serviceEndpoints.ContainsKey(endpoint.Name))
				{
					string endpointName = endpoint.Name;
					this.view.AddEndpoint(endpointName);
					this.serviceEndpoints.Add(endpointName, endpoint);
				}
            }
        }

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
		private ServiceEndpointCollection GetServiceEndPoints(
            ContractGenerationOptions options, MetadataSet metadata)
        {
            ServiceEndpointCollection serviceEndpoints = null;
            ContractGenerator generator = new ContractGenerator(options);

            try
            {
                WsdlImporter importer = generator.CreateWsdlImporter(metadata);
                generator.Generate(importer, false, false);
                serviceEndpoints = importer.ImportAllEndpoints();
                this.model.ContractGenerationOptions = options;
				this.model.CodeCompileUnit = generator.CodeCompileUnit;
            }
            catch (FileNotFoundException importerNotFound)
            {
                string importerFile = this.view.GetImporterFile(importerNotFound.Message, options.AssemblyResolvePath);
                if (!string.IsNullOrEmpty(importerFile))
                {
                    options.ReferencedAssemblies.Add(importerFile);
                    serviceEndpoints = GetServiceEndPoints(options, metadata);
                }
            }
            catch (Exception exception)
            {
				view.ShowMessage(ExceptionHelper.BuildErrorMessage(exception), MessageBoxIcon.Error);
            }

            return serviceEndpoints;
        }
	}
}
