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
using Microsoft.Practices.ServiceFactory.Library.Presentation.Base;

namespace Microsoft.Practices.ServiceFactory.Recipes.CreateWCFClientProxy.Presentation
{
	/// <summary>
	/// The contract class for the <see cref="Microsoft.Practices.ServiceFactory.WCF.CustomWizardPages.SecureClientConfigBehaviorPage"/> view.
	/// </summary>
	[ComVisible(false)]
	public interface ISecureClientConfigBehaviorView : IView
	{
        /// <summary>
        /// Gets the endpoint.
        /// </summary>
        /// <value>The endpoint.</value>
		ServiceEndpoint Endpoint { get; }

		/// <summary>
		/// Gets or sets the name of the client certificate subject.
		/// </summary>
		/// <value>The name of the client certificate subject.</value>
		string ClientCertificateSubjectName { get; set; }

		/// <summary>
		/// Gets or sets the name of the service certificate subject.
		/// </summary>
		/// <value>The name of the service certificate subject.</value>
		string ServiceCertificateSubjectName { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether [show service credentials].
		/// </summary>
		/// <value>
		/// 	<c>true</c> if [show service credentials]; otherwise, <c>false</c>.
		/// </value>
		bool ShowServiceCredentials { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether [show client credentials].
		/// </summary>
		/// <value>
		/// 	<c>true</c> if [show client credentials]; otherwise, <c>false</c>.
		/// </value>
		bool ShowClientCredentials { get; set; }

		/// <summary>
		/// Gets the certificate validation mode.
		/// </summary>
		/// <value>The certificate validation mode.</value>
		X509CertificateValidationMode CertificateValidationMode { get; }
	}
}
