namespace Microsoft.Practices.ServiceFactory.Recipes.CreateWCFClientProxy.Presentation
{
    partial class SecureClientConfigBehaviorPage
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SecureClientConfigBehaviorPage));
			this.ClientCredentialsGroup = new System.Windows.Forms.GroupBox();
			this.BrowseClientCredentials = new System.Windows.Forms.Button();
			this.lbCredDesc = new System.Windows.Forms.Label();
			this.txDescription = new System.Windows.Forms.TextBox();
			this.ServiceCredentialsGroup = new System.Windows.Forms.GroupBox();
			this.BrowseServiceCredentials = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.txServiceDescription = new System.Windows.Forms.TextBox();
			this.toolTip = new System.Windows.Forms.ToolTip(this.components);
			this.lbEndMessage = new System.Windows.Forms.Label();
			this.ClientCredentialsGroup.SuspendLayout();
			this.ServiceCredentialsGroup.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
			this.SuspendLayout();
			// 
			// infoPanel
			// 
			resources.ApplyResources(this.infoPanel, "infoPanel");
			// 
			// ClientCredentialsGroup
			// 
			this.ClientCredentialsGroup.Controls.Add(this.BrowseClientCredentials);
			this.ClientCredentialsGroup.Controls.Add(this.lbCredDesc);
			this.ClientCredentialsGroup.Controls.Add(this.txDescription);
			resources.ApplyResources(this.ClientCredentialsGroup, "ClientCredentialsGroup");
			this.ClientCredentialsGroup.Name = "ClientCredentialsGroup";
			this.ClientCredentialsGroup.TabStop = false;
			// 
			// BrowseClientCredentials
			// 
			resources.ApplyResources(this.BrowseClientCredentials, "BrowseClientCredentials");
			this.BrowseClientCredentials.Name = "BrowseClientCredentials";
			this.toolTip.SetToolTip(this.BrowseClientCredentials, resources.GetString("BrowseClientCredentials.ToolTip"));
			this.BrowseClientCredentials.UseVisualStyleBackColor = true;
			this.BrowseClientCredentials.Click += new System.EventHandler(this.OnClientCredentialsClick);
			// 
			// lbCredDesc
			// 
			resources.ApplyResources(this.lbCredDesc, "lbCredDesc");
			this.lbCredDesc.Name = "lbCredDesc";
			// 
			// txDescription
			// 
			resources.ApplyResources(this.txDescription, "txDescription");
			this.txDescription.Name = "txDescription";
			this.txDescription.ReadOnly = true;
			this.toolTip.SetToolTip(this.txDescription, resources.GetString("txDescription.ToolTip"));
			// 
			// ServiceCredentialsGroup
			// 
			this.ServiceCredentialsGroup.Controls.Add(this.BrowseServiceCredentials);
			this.ServiceCredentialsGroup.Controls.Add(this.label2);
			this.ServiceCredentialsGroup.Controls.Add(this.txServiceDescription);
			resources.ApplyResources(this.ServiceCredentialsGroup, "ServiceCredentialsGroup");
			this.ServiceCredentialsGroup.Name = "ServiceCredentialsGroup";
			this.ServiceCredentialsGroup.TabStop = false;
			// 
			// BrowseServiceCredentials
			// 
			resources.ApplyResources(this.BrowseServiceCredentials, "BrowseServiceCredentials");
			this.BrowseServiceCredentials.Name = "BrowseServiceCredentials";
			this.toolTip.SetToolTip(this.BrowseServiceCredentials, resources.GetString("BrowseServiceCredentials.ToolTip"));
			this.BrowseServiceCredentials.UseVisualStyleBackColor = true;
			this.BrowseServiceCredentials.Click += new System.EventHandler(this.OnServiceCredentialsClick);
			// 
			// label2
			// 
			resources.ApplyResources(this.label2, "label2");
			this.label2.Name = "label2";
			// 
			// txServiceDescription
			// 
			resources.ApplyResources(this.txServiceDescription, "txServiceDescription");
			this.txServiceDescription.Name = "txServiceDescription";
			this.txServiceDescription.ReadOnly = true;
			this.toolTip.SetToolTip(this.txServiceDescription, resources.GetString("txServiceDescription.ToolTip"));
			// 
			// lbEndMessage
			// 
			resources.ApplyResources(this.lbEndMessage, "lbEndMessage");
			this.lbEndMessage.Name = "lbEndMessage";
			// 
			// SecureClientConfigBehaviorPage
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.lbEndMessage);
			this.Controls.Add(this.ClientCredentialsGroup);
			this.Controls.Add(this.ServiceCredentialsGroup);
			this.InfoRTBoxIcon = global::Microsoft.Practices.ServiceFactory.Properties.Resources.Information;
			this.InfoRTBoxSize = new System.Drawing.Size(372, 49);
			this.Name = "SecureClientConfigBehaviorPage";
			this.Controls.SetChildIndex(this.infoPanel, 0);
			this.Controls.SetChildIndex(this.ServiceCredentialsGroup, 0);
			this.Controls.SetChildIndex(this.ClientCredentialsGroup, 0);
			this.Controls.SetChildIndex(this.lbEndMessage, 0);
			this.ClientCredentialsGroup.ResumeLayout(false);
			this.ClientCredentialsGroup.PerformLayout();
			this.ServiceCredentialsGroup.ResumeLayout(false);
			this.ServiceCredentialsGroup.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox ClientCredentialsGroup;
        private System.Windows.Forms.Label lbCredDesc;
        private System.Windows.Forms.TextBox txDescription;
        private System.Windows.Forms.GroupBox ServiceCredentialsGroup;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txServiceDescription;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.Button BrowseServiceCredentials;
        private System.Windows.Forms.Button BrowseClientCredentials;
        private System.Windows.Forms.Label lbEndMessage;
    }
}
