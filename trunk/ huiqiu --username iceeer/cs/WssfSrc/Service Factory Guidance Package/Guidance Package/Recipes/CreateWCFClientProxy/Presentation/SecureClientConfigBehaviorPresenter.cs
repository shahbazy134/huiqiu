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
using System.ServiceModel.Security.Tokens;
using Microsoft.Practices.RecipeFramework.Extensions.CommonHelpers;
using Microsoft.Practices.RecipeFramework.Library;
using System.ServiceModel.Channels;
using Microsoft.Practices.ServiceFactory.Description;
using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Claims;

namespace Microsoft.Practices.ServiceFactory.Recipes.CreateWCFClientProxy.Presentation
{
    /// <summary>
    /// The presenter class for pages that implements <see cref="ISecureClientConfigBehaviorView"/> interface.
    /// </summary>
    public class SecureClientConfigBehaviorPresenter
    {
        public const string BehaviorElementKeyName = "BehaviorElement";

        private ISecureClientConfigBehaviorView view;
        private ClientCredentialsElement clientCredentials;
        private EndpointBehaviorElement behaviorElement;
        private bool addClientCredentialsSection;

        private enum SupportedBindings
        {
            BasicHttpBinding,
            WSHttpBinding,
            CustomBinding,
            Unknown
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:SecureClientConfigBehaviorPresenter"/> class.
        /// </summary>
        /// <param name="view">The view.</param>
		public SecureClientConfigBehaviorPresenter(ISecureClientConfigBehaviorView view)
        {
            Guard.ArgumentNotNull(view, "view");

            this.view = view;
            
            // Set default values.
			clientCredentials = new ClientCredentialsElement();
            clientCredentials.ServiceCertificate.Authentication.CertificateValidationMode = view.CertificateValidationMode;
            clientCredentials.ServiceCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;
            behaviorElement = null;
            addClientCredentialsSection = false;
        }

        /// <summary>
        /// Processes the <see cref="Endpoint"/>.
        /// </summary>
        public void ProcessEndpoint()
        {
            bool showServiceCredentials = false;
            bool showClientCredentials = false;
            this.addClientCredentialsSection = false;

            if (view.Endpoint == null)
            {
                throw new ArgumentNullException(Properties.Resources.NullEndpointException);
            }

            if (view.Endpoint.Binding == null)
            {
                throw new ArgumentNullException(Properties.Resources.NullBindingException);
            }

            switch (GetSupportedBinding(view.Endpoint.Binding))
            {
                case SupportedBindings.BasicHttpBinding:
                    BasicHttpBinding basicBinding = (BasicHttpBinding)view.Endpoint.Binding;
                    bool needToAskForCertificates = (basicBinding.Security.Message.ClientCredentialType ==
                                                     BasicHttpMessageCredentialType.Certificate);
                    showServiceCredentials = needToAskForCertificates;
                    showClientCredentials = needToAskForCertificates;
                    this.addClientCredentialsSection = needToAskForCertificates;
                    break;
                case SupportedBindings.WSHttpBinding:
                    WSHttpBinding wsBinding = (WSHttpBinding)view.Endpoint.Binding;
                    showServiceCredentials = 
                        !wsBinding.Security.Message.NegotiateServiceCredential &&
                        wsBinding.Security.Message.ClientCredentialType != MessageCredentialType.Windows;
                    showClientCredentials =
                        (wsBinding.Security.Message.ClientCredentialType ==
                        System.ServiceModel.MessageCredentialType.Certificate);
                    this.addClientCredentialsSection =
                        (wsBinding.Security.Message.ClientCredentialType !=
                        System.ServiceModel.MessageCredentialType.Windows);
                    break;
                case SupportedBindings.CustomBinding:
                    CustomBinding customBinding = (CustomBinding)view.Endpoint.Binding;
                    SecurityBindingElement security = customBinding.Elements.Find<SecurityBindingElement>();
                    if (security != null)
                    {
                        showServiceCredentials = IsServiceCredentialsRequired(security);
                        showClientCredentials = IsClientCredentialsRequired(security);
						this.addClientCredentialsSection = showServiceCredentials ||
														   showClientCredentials ||
														   HasServiceCertificate(view.Endpoint);														   
                    }
                    break;
                default:
                    //we don't need any further security data.
                    break;
            }

            behaviorElement = new EndpointBehaviorElement(view.Endpoint.Name);

            if (!showServiceCredentials &&
                !showClientCredentials)
            {
                UpdateViewArguments();
            }

            view.ShowServiceCredentials = showServiceCredentials;
            view.ShowClientCredentials = showClientCredentials;
        }

        /// <summary>
        /// Updates the view arguments.
        /// </summary>
        public void UpdateViewArguments()
        {
            IDictionaryService dictservice = view.DictionaryService;
            if (behaviorElement != null &&
                (addClientCredentialsSection ||
                 view.ShowServiceCredentials ||
                 view.ShowClientCredentials) )
            {
                ServiceModelConfigurationManager.UpdateBehaviorExtensionSection<ClientCredentialsElement>(behaviorElement, clientCredentials);
                // check if we need to update the identity element
                if (view.ShowServiceCredentials &&
                   GetSupportedBinding(view.Endpoint.Binding) == SupportedBindings.BasicHttpBinding)
                {
                    ServiceEndpoint endpoint = dictservice.GetValue(SecureClientConfigModel.ServiceEndpointKeyName) as ServiceEndpoint;
                    if (endpoint != null)
                    {
                        EndpointIdentity identity = EndpointIdentity.CreateDnsIdentity(GetDns(clientCredentials.ServiceCertificate.DefaultCertificate.FindValue));
                        endpoint.Address = new EndpointAddress(endpoint.Address.Uri, identity, endpoint.Address.Headers);
                        dictservice.SetValue(SecureClientConfigModel.ServiceEndpointKeyName, endpoint);
                    }
                }
            }
            dictservice.SetValue(BehaviorElementKeyName, behaviorElement);
        }

        /// <summary>
        /// Updates the client certificate.
        /// </summary>
        /// <param name="subjectName">Name of the subject.</param>
        /// <param name="x509FindType">Type of the X509 find.</param>
        /// <param name="storeLocation">The store location.</param>
        /// <param name="storeName">Name of the store.</param>
        public void UpdateClientCertificate(string subjectName,
            X509FindType x509FindType, StoreLocation storeLocation, StoreName storeName)
        {
            if (!IsValidSubjectName(subjectName))
            {
                return;
            }

            subjectName = X509CertificateHelper.GetSubjectDistinguishedName(subjectName);
			clientCredentials.ClientCertificate.X509FindType = x509FindType;
            clientCredentials.ClientCertificate.StoreLocation = storeLocation;
            clientCredentials.ClientCertificate.StoreName = storeName;
            clientCredentials.ClientCertificate.FindValue = subjectName;

            view.ClientCertificateSubjectName = subjectName;

            if (!view.ShowServiceCredentials ||
                 view.ServiceCertificateSubjectName.Length != 0)
            {
                UpdateViewArguments();
            }
        }

        /// <summary>
        /// Updates the service certificate.
        /// </summary>
        /// <param name="subjectName">Name of the subject.</param>
        /// <param name="x509FindType">Type of the X509 find.</param>
        /// <param name="storeLocation">The store location.</param>
        /// <param name="storeName">Name of the store.</param>
        public void UpdateServiceCertificate(string subjectName,
            X509FindType x509FindType, StoreLocation storeLocation, StoreName storeName)
        {
            if(!IsValidSubjectName(subjectName))
            {
                return;
            }

            subjectName = X509CertificateHelper.GetSubjectDistinguishedName(subjectName);
			clientCredentials.ServiceCertificate.DefaultCertificate.X509FindType = x509FindType;
            clientCredentials.ServiceCertificate.DefaultCertificate.StoreLocation = storeLocation;
            clientCredentials.ServiceCertificate.DefaultCertificate.StoreName = storeName;
            clientCredentials.ServiceCertificate.DefaultCertificate.FindValue = subjectName;

            view.ServiceCertificateSubjectName = subjectName;

            if (!view.ShowClientCredentials ||
                 view.ClientCertificateSubjectName.Length != 0)
            {
                UpdateViewArguments();
            }
        }

        #region Private Implementation

		private bool HasServiceCertificate(ServiceEndpoint endpoint)
		{
			return endpoint.Address.Identity.IdentityClaim.ClaimType == ClaimTypes.Thumbprint ||
				   endpoint.Address.Identity.IdentityClaim.ClaimType == ClaimTypes.X500DistinguishedName;
		}

        private string GetDns(string distinguishedName)
        {
            return distinguishedName.Replace("CN=", string.Empty);
        }

        private SupportedBindings GetSupportedBinding(Binding binding)
        {
            string bindingName = binding.GetType().Name;
            if(Enum.IsDefined(typeof(SupportedBindings), bindingName))
            {
                return (SupportedBindings)Enum.Parse(typeof(SupportedBindings), bindingName);
            }
            return SupportedBindings.Unknown;
        }

        private bool IsValidSubjectName(string subjectName)
        {
            return !string.IsNullOrEmpty(subjectName);
        }

		private bool IsServiceCredentialsRequired(SecurityBindingElement sbe)
        {
            SecurityBindingElement bootstrapSecurity;
			if (IsSecureConversationBinding(sbe, out bootstrapSecurity))
            {
				sbe = bootstrapSecurity;
            }
			return IsMutualCertificateBinding(sbe) ||
				   IsUserNameForCertificateBinding(sbe);
        }

		private bool IsClientCredentialsRequired(SecurityBindingElement sbe)
        {
            SecurityBindingElement bootstrapSecurity;
			if (IsSecureConversationBinding(sbe, out bootstrapSecurity))
            {
				sbe = bootstrapSecurity;
            }
			return IsMutualCertificateBinding(sbe) ||
				   IsSslAndRequireClientCertificate(sbe);               
        }

        private bool IsSecureConversationBinding(SecurityBindingElement sbe, out SecurityBindingElement bootstrapSecurity)
        {
            bootstrapSecurity = null;
            SymmetricSecurityBindingElement symm = sbe as SymmetricSecurityBindingElement;
            if (symm != null)
            {
                if (symm.RequireSignatureConfirmation)
                {
                    return false;
                }
                SecureConversationSecurityTokenParameters secConv = symm.ProtectionTokenParameters as SecureConversationSecurityTokenParameters;
                if (secConv == null)
                {
                    return false;
                }
                bootstrapSecurity = secConv.BootstrapSecurityBindingElement;
            }
            else
            {
                if (!sbe.IncludeTimestamp)
                {
                    return false;
                }
                if (!(sbe is TransportSecurityBindingElement))
                {
                    return false;
                }
                SupportingTokenParameters suppToken = sbe.EndpointSupportingTokenParameters;
                if (((suppToken.Signed.Count != 0) ||
                    (suppToken.SignedEncrypted.Count != 0)) ||
                    ((suppToken.Endorsing.Count != 1) ||
                    (suppToken.SignedEndorsing.Count != 0)))
                {
                    return false;
                }
                SecureConversationSecurityTokenParameters secConvToken = suppToken.Endorsing[0] as SecureConversationSecurityTokenParameters;
                if (secConvToken == null)
                {
                    return false;
                }
                bootstrapSecurity = secConvToken.BootstrapSecurityBindingElement;
            }
            return (bootstrapSecurity != null);
        }

        private bool IsSslAndRequireClientCertificate(SecurityBindingElement sbe)
        {
            SymmetricSecurityBindingElement symm = sbe as SymmetricSecurityBindingElement;
            if (symm != null)
            {
                if (!IsTokenParameterEmpty(sbe.EndpointSupportingTokenParameters))
                {
                    return false;
                }
                SslSecurityTokenParameters ssl = symm.ProtectionTokenParameters as SslSecurityTokenParameters;
                if (ssl == null)
                {
                    return false;               
                }
                return ssl.RequireClientCertificate;
            }
            return false;
        }

        private bool IsMutualCertificateBinding(SecurityBindingElement sbe)
        {
            AsymmetricSecurityBindingElement asymm = sbe as AsymmetricSecurityBindingElement;
            if (asymm != null)
            {
                X509SecurityTokenParameters x509Token = asymm.RecipientTokenParameters as X509SecurityTokenParameters;
                if (((x509Token == null) ||
                    (x509Token.X509ReferenceStyle != X509KeyIdentifierClauseType.Any)) ||
                    (x509Token.InclusionMode != SecurityTokenInclusionMode.Never))
                {
                    return false;
                }
                X509SecurityTokenParameters x509Params = asymm.InitiatorTokenParameters as X509SecurityTokenParameters;
                if (((x509Params == null) ||
                    (x509Params.X509ReferenceStyle != X509KeyIdentifierClauseType.Any)) ||
                    (x509Params.InclusionMode != SecurityTokenInclusionMode.AlwaysToRecipient))
                {
                    return false;
                }
                if (!IsTokenParameterEmpty(sbe.EndpointSupportingTokenParameters))
                {
                    return false;
                }
            }
            else
            {
                SymmetricSecurityBindingElement symm = sbe as SymmetricSecurityBindingElement;
                if (symm == null)
                {
                    return false;
                }
                X509SecurityTokenParameters x509TokenPrms = symm.ProtectionTokenParameters as X509SecurityTokenParameters;
                if (((x509TokenPrms == null) ||
                    (x509TokenPrms.X509ReferenceStyle != X509KeyIdentifierClauseType.Thumbprint)) ||
                    (x509TokenPrms.InclusionMode != SecurityTokenInclusionMode.Never))
                {
                    return false;
                }
                SupportingTokenParameters suppToken = sbe.EndpointSupportingTokenParameters;
                if (((suppToken.Signed.Count != 0) ||
                    (suppToken.SignedEncrypted.Count != 0)) ||
                    ((suppToken.Endorsing.Count != 1) ||
                    (suppToken.SignedEndorsing.Count != 0)))
                {
                    return false;
                }
                x509TokenPrms = suppToken.Endorsing[0] as X509SecurityTokenParameters;
                if (((x509TokenPrms == null) ||
                    (x509TokenPrms.X509ReferenceStyle != X509KeyIdentifierClauseType.Thumbprint)) ||
                    (x509TokenPrms.InclusionMode != SecurityTokenInclusionMode.AlwaysToRecipient))
                {
                    return false;
                }
            }
            return true;
        }

        private bool IsUserNameForCertificateBinding(SecurityBindingElement sbe)
        {
            SymmetricSecurityBindingElement symm = sbe as SymmetricSecurityBindingElement;
            if (symm == null)
            {
                return false;
            }

            X509SecurityTokenParameters x509Token = symm.ProtectionTokenParameters as X509SecurityTokenParameters;
            if (((x509Token == null) ||
                (x509Token.X509ReferenceStyle != X509KeyIdentifierClauseType.Thumbprint)) ||
                (x509Token.InclusionMode != SecurityTokenInclusionMode.Never))
            {
                return false;
            }

            SupportingTokenParameters supportingToken = sbe.EndpointSupportingTokenParameters;
            if (((supportingToken.Signed.Count != 0) ||
                (supportingToken.SignedEncrypted.Count != 1)) ||
                ((supportingToken.Endorsing.Count != 0) ||
                (supportingToken.SignedEndorsing.Count != 0)))
            {
                return false;
            }
            if (!(supportingToken.SignedEncrypted[0] is UserNameSecurityTokenParameters))
            {
                return false;
            }
            return true;
        }

        private bool IsTokenParameterEmpty(SupportingTokenParameters token)
        {
            if (token.Signed.Count == 0 && 
                token.SignedEncrypted.Count == 0 && 
                token.Endorsing.Count == 0)
            {
                return (token.SignedEndorsing.Count == 0);
            }
            return false;
        }

        #endregion
    }
}
