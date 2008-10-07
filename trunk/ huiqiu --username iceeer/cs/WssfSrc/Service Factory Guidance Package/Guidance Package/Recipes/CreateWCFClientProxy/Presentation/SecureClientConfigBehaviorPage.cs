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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Microsoft.Practices.WizardFramework;
using System.Windows.Forms.Design;
using Microsoft.Practices.RecipeFramework.Extensions.Dialogs;
using System.ComponentModel.Design;
using System.ServiceModel.Description;
using System.ServiceModel.Security;
using System.ServiceModel.Configuration;
using System.Security.Cryptography.X509Certificates;

namespace Microsoft.Practices.ServiceFactory.Recipes.CreateWCFClientProxy.Presentation
{
    /// <summary>
    /// Wizard page for performing the metadata exchange with the specified service and 
    /// gathering any required additional data for generating the application configuration file.
    /// </summary>
    public partial class SecureClientConfigBehaviorPage : CustomWizardPage, ISecureClientConfigBehaviorView
    {
        private SecureClientConfigBehaviorPresenter presenter;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:SecureClientConfigBehaviorPage"/> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        public SecureClientConfigBehaviorPage(WizardForm parent)
            : base(parent)
        {
            InitializeComponent();
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.UserControl.Load"></see> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"></see> that contains the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!this.Site.DesignMode)
            {
                this.InfoRTBoxSize = new System.Drawing.Size(this.Wizard.ActivePage.Width, 85);
            }
        }

        /// <summary>
        /// Called when [activate].
        /// </summary>
        /// <returns></returns>
        public override bool OnActivate()
        {
            presenter = new SecureClientConfigBehaviorPresenter(this);
            presenter.ProcessEndpoint();
            InitControls();
            return base.OnActivate();
        }

        private void InitControls()
        {
            this.ServiceCredentialsGroup.Visible = showServiceCredentials;
            this.ClientCredentialsGroup.Visible = showClientCredentials;
            this.lbEndMessage.Visible = !(showServiceCredentials || showClientCredentials);
        }

        /// <summary>
        /// Sets the certificate validation mode.
        /// </summary>
        /// <value>The certificate validation mode.</value>
        [RecipeArgument]
        public X509CertificateValidationMode CertificateValidationMode
        {
            get
            {
                IDictionaryService dictionary = (IDictionaryService)GetService(typeof(IDictionaryService));
                return (X509CertificateValidationMode)dictionary.GetValue("CertificateValidationMode"); 
            }
            set 
			{
				IDictionaryService dictionary = (IDictionaryService)GetService(typeof(IDictionaryService));
				dictionary.SetValue("CertificateValidationMode", value);
			}
        }

        /// <summary>
        /// The <see cref="BehaviorElement"/> object that will be passed to the executing actions.
        /// </summary>
        /// <value>The behavior element.</value>
        [RecipeArgument]
        public EndpointBehaviorElement BehaviorElement
        {
			get { return behaviorElement; }
			set { behaviorElement = value; }
        }
		private EndpointBehaviorElement behaviorElement;

        /// <summary>
        /// Gets or sets the dictionary service.
        /// </summary>
        /// <value>The dictionary service.</value>
        public IDictionaryService DictionaryService
        {
            get { return (IDictionaryService)GetService(typeof(IDictionaryService)); }
        }

        /// <summary>
        /// Gets the service endpoint.
        /// </summary>
        /// <value>The endpoint.</value>
        public ServiceEndpoint Endpoint
        {
            get
            {
                IDictionaryService dictservice = (IDictionaryService)GetService(typeof(IDictionaryService));
                return (ServiceEndpoint)dictservice.GetValue("ServiceEndpoint");
            }
        }

        /// <summary>
        /// Gets or sets the name of the client certificate subject.
        /// </summary>
        /// <value>The name of the client certificate subject.</value>
        public string ClientCertificateSubjectName
        {
            get { return txDescription.Text; }
            set { txDescription.Text = value; }
        }

        /// <summary>
        /// Gets or sets the name of the service certificate subject.
        /// </summary>
        /// <value>The name of the service certificate subject.</value>
        public string ServiceCertificateSubjectName
        {
            get { return txServiceDescription.Text; }
            set { txServiceDescription.Text = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show service credentials].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [show service credentials]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowServiceCredentials
        {
            get { return showServiceCredentials; }
            set { showServiceCredentials = value; }
        } private bool showServiceCredentials;

        /// <summary>
        /// Gets or sets a value indicating whether [show client credentials].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [show client credentials]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowClientCredentials
        {
            get { return showClientCredentials; }
            set { showClientCredentials = value; }
        } private bool showClientCredentials;

        private void OnClientCredentialsClick(object sender, EventArgs e)
        {
            IUIService svc = (IUIService)this.GetService(typeof(IUIService));
            using (X509CertificatePickerDialog dialog = new X509CertificatePickerDialog())
            {
                if (dialog.ShowDialog(svc.GetDialogOwnerWindow()) == System.Windows.Forms.DialogResult.OK)
                {
                    presenter.UpdateClientCertificate(dialog.Certificate.SubjectName.Name,
                        X509FindType.FindBySubjectDistinguishedName,
                        dialog.StoreLocation,
                        dialog.StoreName);
                }
            }
        }

        private void OnServiceCredentialsClick(object sender, EventArgs e)
        {
            IUIService svc = (IUIService)this.GetService(typeof(IUIService));
            using (X509CertificatePickerDialog dialog = new X509CertificatePickerDialog())
            {
                if (dialog.ShowDialog(svc.GetDialogOwnerWindow()) == System.Windows.Forms.DialogResult.OK)
                {
                    presenter.UpdateServiceCertificate(dialog.Certificate.SubjectName.Name,
                        X509FindType.FindBySubjectDistinguishedName,
                        dialog.StoreLocation,
                        dialog.StoreName);
                }
            }
        }
    }
}
