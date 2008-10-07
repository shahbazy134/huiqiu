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
using System.ServiceModel.Security;
using System.Runtime.InteropServices;
using System.ServiceModel.Description;
using System.Windows.Forms;
using Microsoft.Practices.ServiceFactory.Library.Validation;

namespace Microsoft.Practices.ServiceFactory.Recipes.CreateWCFClientProxy.Presentation
{
	/// <summary>
	/// The contract class for the <see cref="Microsoft.Practices.ServiceFactory.WCF.CustomWizardPages.SecureClientConfigPage"/> view.
	/// </summary>
	[ComVisible(false)]
	public interface ISecureClientConfigView
	{
        /// <summary>
        /// Gets the service URI.
        /// </summary>
        /// <value>The service URI.</value>
        string ServiceAddress { get;}

        /// <summary>
        /// Gets or sets the selected endpoint.
        /// </summary>
        /// <value>The selected endpoint.</value>
        string SelectedEndpoint { get; set; }

        /// <summary>
        /// Adds the endpoint.
        /// </summary>
        /// <param name="name">The endpoint name.</param>
        void AddEndpoint(string endpoint);

        /// <summary>
        /// Clears the endpoints.
        /// </summary>
        void ClearEndpoints();

        /// <summary>
        /// Sets the progress.
        /// </summary>
        /// <param name="value">if set to <c>true</c> [value].</param>
        void SetProgress(bool value);

		/// <summary>
		/// Shows the message.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="icon">The icon.</param>
        void ShowMessage(string message, MessageBoxIcon icon);

        /// <summary>
        /// Gets the importer file.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="initialDirectory">The initial directory.</param>
        /// <returns></returns>
        string GetImporterFile(string message, string initialDirectory);

        event EventHandler ViewLoading;
        event EventHandler<FormClosingEventArgs> ViewClosing;
        event EventHandler<ValidationEventArgs> SelectedEndpointChanging;
        event EventHandler<RequestDataEventArgs<bool>> RequestIsDataValid;
	}
}
