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
using System.ComponentModel.Design;
using System.ServiceModel.Configuration;
using System.Windows.Forms.Design;
using System.Collections;
using System.ServiceModel.Description;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel.Security;
using Microsoft.Practices.RecipeFramework.Extensions.Dialogs;
using Microsoft.Practices.RecipeFramework.Extensions.CommonHelpers;
using EnvDTE;
using System.Reflection;
using Microsoft.Practices.ServiceFactory.Library.Validation;
using Microsoft.Practices.ServiceFactory.Helpers;
using Microsoft.Practices.ServiceFactory.Library.Presentation.Base;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Practices.ServiceFactory.Recipes.CreateWCFClientProxy.Presentation
{
    /// <summary>
    /// Wizard page for performing the metadata exchange with the specified service and 
    /// gathering any required additional data for generating the application configuration file.
    /// </summary>
    public sealed partial class SecureClientConfigPage : ViewBase, ISecureClientConfigView
	{
        private string serviceAddress;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:SecureClientConfigPage"/> class.
		/// </summary>
		/// <param name="parent">The parent.</param>
		public SecureClientConfigPage(WizardForm parent)
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
			this.Wizard.FormClosing += new FormClosingEventHandler(OnWizardFormClosing);
		}

        /// <summary>
        /// Called when [activate].
        /// </summary>
        /// <returns></returns>
        public override bool OnActivate()
        {
            InitializePresenter();
            return base.OnActivate();
        }

		/// <summary>
		/// Raises the <see cref="E:System.Windows.Forms.Control.Leave"></see> event.
		/// </summary>
		/// <param name="e">An <see cref="T:System.EventArgs"></see> that contains the event data.</param>
		protected override void OnLeave(EventArgs e)
		{
			base.OnLeave(e);
            OnViewClosing(this, new FormClosingEventArgs(CloseReason.UserClosing, false));
		}

		// cancel any pending event so when the wizard form is gone,
		// we don't get any late event and therefore VS may blow out 
		// likely because of unhandled exceptions.
		void OnWizardFormClosing(object sender, FormClosingEventArgs e)
		{
            OnViewClosing(sender, e);
		}

        public override bool IsDataValid
        {
            get
            {
                RequestDataEventArgs<bool> args = new RequestDataEventArgs<bool>();
                OnRequestIsDataValid(this, args);
                if (args.ValueProvided)
                {
                    return args.Value;
                }
                return base.IsDataValid;
            }
        }

        #region GAX Interface properties

        /// <summary>
        /// Sets the service endpoint.
        /// </summary>
        /// <value>The service endpoint.</value>
        [RecipeArgument]
        public ServiceEndpoint ServiceEndpoint
        {
			get { return serviceEndpoint; }
			set { serviceEndpoint = value; }
        }
		private ServiceEndpoint serviceEndpoint;

        /// <summary>
        /// Sets the endpoint.
        /// </summary>
        /// <value>The endpoint.</value>
        //[RecipeArgument]
        //This argument is already used in a previous page
        public string ServiceAddress
        {
            get
            {
                IDictionaryService dict = (IDictionaryService)GetService(typeof(IDictionaryService));
                serviceAddress = dict.GetValue("ServiceAddress") as string;
                return serviceAddress;
            }
        }

        #endregion

        #region View Interface implementation

        string ISecureClientConfigView.ServiceAddress
        {
            get { return this.serviceAddress; }
        }

        string ISecureClientConfigView.SelectedEndpoint
        {
            get { return Endpoints.SelectedItem.ToString(); }
            set
            {
                Endpoints.SelectedItem = value;
                EndpointsGroup.Enabled = !string.IsNullOrEmpty(value);
            }
        }

        void ISecureClientConfigView.AddEndpoint(string endpoint)
        {
            Endpoints.Items.Add(endpoint);
        }

        void ISecureClientConfigView.ClearEndpoints()
        {
            Endpoints.Items.Clear();
            EndpointsGroup.Enabled = false;
        }

        void ISecureClientConfigView.SetProgress(bool value)
        {
            if (value)
            {
                this.Wizard.Cursor = Cursors.AppStarting;
                this.ProgressPanel.Visible = true;
            }
            else
            {
                this.Wizard.Cursor = Cursors.Default;
                this.ProgressPanel.Visible = false;
            }
        }

        void ISecureClientConfigView.ShowMessage(string message, MessageBoxIcon icon)
        {
            // The UIService may not work in a background thread like this.
            //IUIService uiService = (IUIService)this.GetService(typeof(IUIService));
            //uiService.ShowError(ExceptionHelper.BuildErrorMessage(error));
            MessageBox.Show(this, 
                message, 
                Properties.Resources.ErrorMsgBoxTitle, 
                MessageBoxButtons.OK, 
                icon, 
                MessageBoxDefaultButton.Button1,
                CustomPageHelper.GetRtlMessageBoxOptionsToShutUpFxCop(this));
        }

        string ISecureClientConfigView.GetImporterFile(string message, string initialDirectory)
        {
            openFileDialog.FileName = ExtracFileNameFromMessage(message);
            openFileDialog.InitialDirectory = initialDirectory;
            openFileDialog.AddExtension = true;
            openFileDialog.CheckFileExists = true;
            openFileDialog.AddExtension = true;
            openFileDialog.CheckPathExists = true;
            openFileDialog.DefaultExt = ".dll";
            openFileDialog.Filter = "dll files (*.dll)|*.dll";
            openFileDialog.ValidateNames = true;
            openFileDialog.Title = Properties.Resources.SelectImporterFile;
            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                return openFileDialog.FileName;
            }
            return null;
        }

        public event EventHandler ViewLoading;
        public event EventHandler<FormClosingEventArgs> ViewClosing;
        public event EventHandler<ValidationEventArgs> SelectedEndpointChanging;
        public event EventHandler<RequestDataEventArgs<bool>> RequestIsDataValid;

        #endregion

        #region Event Helpers

        private void OnViewLoading(object sender, EventArgs e)
        {
            if (ViewLoading != null)
            {
                ViewLoading(sender, e);
            }
        }

		private void OnViewClosing(object sender, FormClosingEventArgs e)
        {
            if (ViewClosing != null)
            {
                ViewClosing(sender, e);
            }
        }

		private void OnSelectedEndpointChanging(object sender, ValidationEventArgs e)
        {
            if (SelectedEndpointChanging != null)
            {
                SelectedEndpointChanging(sender, e);
            }
        }

		private void OnRequestIsDataValid(object sender, RequestDataEventArgs<bool> e)
        {
            if (RequestIsDataValid != null)
            {
                RequestIsDataValid(sender, e);
            }
        }

        #endregion

		[SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults")]
		private void InitializePresenter()
        {
            if (this.ServiceAddress != null)
            {
                ClearEvents();
                IDictionaryService dict = (IDictionaryService)GetService(typeof(IDictionaryService));
                SecureClientConfigModel model = new SecureClientConfigModel(dict);   
				// we don't need the use this instance since the communication will be through events.
                new SecureClientConfigPresenter(this, model);
                OnViewLoading(this, new EventArgs());
            }
        }

        private void ClearEvents()
        {
            ViewLoading = null;
            ViewClosing = null;
            SelectedEndpointChanging = null;
            RequestIsDataValid = null;
        }

        private void OnEndpointsSelectedIndexChanged(object sender, EventArgs e)
        {
            this.toolTip.SetToolTip(Endpoints, Endpoints.SelectedItem.ToString());
            OnSelectedEndpointChanging(sender, CreateValidationEventArgs((Control)sender, errorProvider));
        }

        private string ExtracFileNameFromMessage(string message)
        {
            int start = message.IndexOf("'", StringComparison.Ordinal) + 1;
            if (start > 0)
            {
                string name = message.Substring(start, message.LastIndexOf("'", StringComparison.Ordinal) - start);
                AssemblyName assemblyName = new AssemblyName(name);
                return assemblyName.Name;
            }
            return string.Empty;
        }
    }
}
