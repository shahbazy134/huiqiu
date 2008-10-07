namespace Microsoft.Practices.ServiceFactory.Recipes.CreateWCFClientProxy.Presentation
{
	partial class SecureClientConfigPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SecureClientConfigPage));
            this.EndpointsGroup = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.Endpoints = new System.Windows.Forms.ComboBox();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.ProgressBarUpdateTimer = new System.Windows.Forms.Timer(this.components);
            this.ProgressPanel = new System.Windows.Forms.Panel();
            this.ProgressLabel = new System.Windows.Forms.Label();
            this.ProgressBar = new System.Windows.Forms.ProgressBar();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.EndpointsGroup.SuspendLayout();
            this.ProgressPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // infoPanel
            // 
            resources.ApplyResources(this.infoPanel, "infoPanel");
            // 
            // EndpointsGroup
            // 
            this.EndpointsGroup.Controls.Add(this.label1);
            this.EndpointsGroup.Controls.Add(this.Endpoints);
            resources.ApplyResources(this.EndpointsGroup, "EndpointsGroup");
            this.EndpointsGroup.Name = "EndpointsGroup";
            this.EndpointsGroup.TabStop = false;
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // Endpoints
            // 
            resources.ApplyResources(this.Endpoints, "Endpoints");
            this.Endpoints.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.Endpoints.FormattingEnabled = true;
            this.Endpoints.Name = "Endpoints";
            this.toolTip.SetToolTip(this.Endpoints, resources.GetString("Endpoints.ToolTip"));
            this.Endpoints.SelectedIndexChanged += new System.EventHandler(this.OnEndpointsSelectedIndexChanged);
            // 
            // ProgressPanel
            // 
            this.ProgressPanel.Controls.Add(this.ProgressLabel);
            this.ProgressPanel.Controls.Add(this.ProgressBar);
            resources.ApplyResources(this.ProgressPanel, "ProgressPanel");
            this.ProgressPanel.Name = "ProgressPanel";
            // 
            // ProgressLabel
            // 
            resources.ApplyResources(this.ProgressLabel, "ProgressLabel");
            this.ProgressLabel.Name = "ProgressLabel";
            // 
            // ProgressBar
            // 
            resources.ApplyResources(this.ProgressBar, "ProgressBar");
            this.ProgressBar.Name = "ProgressBar";
            this.ProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            // 
            // errorProvider
            // 
            this.errorProvider.ContainerControl = this;
            // 
            // openFileDialog
            // 
            this.openFileDialog.FileName = "openFileDialog";
            // 
            // SecureClientConfigPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ProgressPanel);
            this.Controls.Add(this.EndpointsGroup);
            this.InfoRTBoxSize = new System.Drawing.Size(372, 49);
            this.Name = "SecureClientConfigPage";
            this.Skippable = false;
            this.Controls.SetChildIndex(this.infoPanel, 0);
            this.Controls.SetChildIndex(this.EndpointsGroup, 0);
            this.Controls.SetChildIndex(this.ProgressPanel, 0);
            this.EndpointsGroup.ResumeLayout(false);
            this.EndpointsGroup.PerformLayout();
            this.ProgressPanel.ResumeLayout(false);
            this.ProgressPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion

        private System.Windows.Forms.GroupBox EndpointsGroup;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox Endpoints;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.Timer ProgressBarUpdateTimer;
        private System.Windows.Forms.Panel ProgressPanel;
        private System.Windows.Forms.ProgressBar ProgressBar;
        private System.Windows.Forms.Label ProgressLabel;
        private System.Windows.Forms.ErrorProvider errorProvider;
        private System.Windows.Forms.OpenFileDialog openFileDialog;

	}
}
