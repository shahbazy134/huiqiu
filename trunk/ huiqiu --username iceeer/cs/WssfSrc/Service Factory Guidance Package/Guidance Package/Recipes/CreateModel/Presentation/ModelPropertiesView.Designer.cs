namespace Microsoft.Practices.ServiceFactory.Recipes.CreateModel.Presentation
{
	partial class ModelPropertiesView
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
			if(disposing && (components != null))
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ModelPropertiesView));
            this.lstModelTypes = new System.Windows.Forms.ListView();
            this.imageListLarge = new System.Windows.Forms.ImageList(this.components);
            this.imageListSmall = new System.Windows.Forms.ImageList(this.components);
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.btnSmallIcons = new System.Windows.Forms.Button();
            this.btnLargeIcons = new System.Windows.Forms.Button();
            this.lblModelType = new System.Windows.Forms.Label();
            this.lblDescription = new System.Windows.Forms.Label();
            this.lblModelName = new System.Windows.Forms.Label();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.txtModelName = new System.Windows.Forms.TextBox();
            this.lblExample = new System.Windows.Forms.Label();
            this.lblNamespacePrefix = new System.Windows.Forms.Label();
            this.txtNamespacePrefix = new System.Windows.Forms.TextBox();
            this.lblDescriptionContent = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // infoPanel
            // 
            resources.ApplyResources(this.infoPanel, "infoPanel");
            // 
            // lstModelTypes
            // 
            this.lstModelTypes.HideSelection = false;
            this.lstModelTypes.LargeImageList = this.imageListLarge;
            resources.ApplyResources(this.lstModelTypes, "lstModelTypes");
            this.lstModelTypes.MultiSelect = false;
            this.lstModelTypes.Name = "lstModelTypes";
            this.lstModelTypes.SmallImageList = this.imageListSmall;
            this.lstModelTypes.UseCompatibleStateImageBehavior = false;
            this.lstModelTypes.SelectedIndexChanged += new System.EventHandler(this.lstModelTypes_SelectedIndexChanged);
            // 
            // imageListLarge
            // 
            this.imageListLarge.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListLarge.ImageStream")));
            this.imageListLarge.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListLarge.Images.SetKeyName(0, "DataContractModel");
            this.imageListLarge.Images.SetKeyName(1, "ServiceModel");
            this.imageListLarge.Images.SetKeyName(2, "HostDesignerModel");
            // 
            // imageListSmall
            // 
            this.imageListSmall.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListSmall.ImageStream")));
            this.imageListSmall.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListSmall.Images.SetKeyName(0, "");
            this.imageListSmall.Images.SetKeyName(1, "");
            this.imageListSmall.Images.SetKeyName(2, "");
            // 
            // btnSmallIcons
            // 
            this.btnSmallIcons.Image = global::Microsoft.Practices.ServiceFactory.Properties.Resources.SmallIcons;
            resources.ApplyResources(this.btnSmallIcons, "btnSmallIcons");
            this.btnSmallIcons.Name = "btnSmallIcons";
            this.toolTip.SetToolTip(this.btnSmallIcons, resources.GetString("btnSmallIcons.ToolTip"));
            this.btnSmallIcons.UseVisualStyleBackColor = true;
            this.btnSmallIcons.Click += new System.EventHandler(this.btnSmallIcons_Click);
            // 
            // btnLargeIcons
            // 
            this.btnLargeIcons.Image = global::Microsoft.Practices.ServiceFactory.Properties.Resources.LargeIcons;
            resources.ApplyResources(this.btnLargeIcons, "btnLargeIcons");
            this.btnLargeIcons.Name = "btnLargeIcons";
            this.toolTip.SetToolTip(this.btnLargeIcons, resources.GetString("btnLargeIcons.ToolTip"));
            this.btnLargeIcons.UseVisualStyleBackColor = true;
            this.btnLargeIcons.Click += new System.EventHandler(this.btnLargeIcons_Click);
            // 
            // lblModelType
            // 
            resources.ApplyResources(this.lblModelType, "lblModelType");
            this.lblModelType.Name = "lblModelType";
            // 
            // lblDescription
            // 
            resources.ApplyResources(this.lblDescription, "lblDescription");
            this.lblDescription.Name = "lblDescription";
            // 
            // lblModelName
            // 
            resources.ApplyResources(this.lblModelName, "lblModelName");
            this.lblModelName.Name = "lblModelName";
            // 
            // errorProvider
            // 
            this.errorProvider.ContainerControl = this;
            // 
            // txtModelName
            // 
            resources.ApplyResources(this.txtModelName, "txtModelName");
            this.txtModelName.Name = "txtModelName";
            this.txtModelName.TextChanged += new System.EventHandler(this.txtModelName_TextChanged);
            // 
            // lblExample
            // 
            resources.ApplyResources(this.lblExample, "lblExample");
            this.lblExample.Name = "lblExample";
            // 
            // lblNamespacePrefix
            // 
            resources.ApplyResources(this.lblNamespacePrefix, "lblNamespacePrefix");
            this.lblNamespacePrefix.Name = "lblNamespacePrefix";
            // 
            // txtNamespacePrefix
            // 
            resources.ApplyResources(this.txtNamespacePrefix, "txtNamespacePrefix");
            this.txtNamespacePrefix.Name = "txtNamespacePrefix";
            this.txtNamespacePrefix.TextChanged += new System.EventHandler(this.txtNamespacePrefix_TextChanged);
            // 
            // lblDescriptionContent
            // 
            this.lblDescriptionContent.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lblDescriptionContent.CausesValidation = false;
            this.lblDescriptionContent.Cursor = System.Windows.Forms.Cursors.Default;
            resources.ApplyResources(this.lblDescriptionContent, "lblDescriptionContent");
            this.lblDescriptionContent.Name = "lblDescriptionContent";
            this.lblDescriptionContent.ReadOnly = true;
            this.lblDescriptionContent.TabStop = false;
            // 
            // ModelPropertiesView
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblDescriptionContent);
            this.Controls.Add(this.lblExample);
            this.Controls.Add(this.lblNamespacePrefix);
            this.Controls.Add(this.txtNamespacePrefix);
            this.Controls.Add(this.txtModelName);
            this.Controls.Add(this.lblModelName);
            this.Controls.Add(this.btnLargeIcons);
            this.Controls.Add(this.btnSmallIcons);
            this.Controls.Add(this.lblDescription);
            this.Controls.Add(this.lblModelType);
            this.Controls.Add(this.lstModelTypes);
            this.Name = "ModelPropertiesView";
            this.Load += new System.EventHandler(this.ModelPropertiesView_Load);
            this.Controls.SetChildIndex(this.infoPanel, 0);
            this.Controls.SetChildIndex(this.lstModelTypes, 0);
            this.Controls.SetChildIndex(this.lblModelType, 0);
            this.Controls.SetChildIndex(this.lblDescription, 0);
            this.Controls.SetChildIndex(this.btnSmallIcons, 0);
            this.Controls.SetChildIndex(this.btnLargeIcons, 0);
            this.Controls.SetChildIndex(this.lblModelName, 0);
            this.Controls.SetChildIndex(this.txtModelName, 0);
            this.Controls.SetChildIndex(this.txtNamespacePrefix, 0);
            this.Controls.SetChildIndex(this.lblNamespacePrefix, 0);
            this.Controls.SetChildIndex(this.lblExample, 0);
            this.Controls.SetChildIndex(this.lblDescriptionContent, 0);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ListView lstModelTypes;
		private System.Windows.Forms.ImageList imageListLarge;
		private System.Windows.Forms.ImageList imageListSmall;
		private System.Windows.Forms.ToolTip toolTip;
		private System.Windows.Forms.Label lblModelType;
		private System.Windows.Forms.Label lblDescription;
		private System.Windows.Forms.Button btnSmallIcons;
		private System.Windows.Forms.Button btnLargeIcons;
		private System.Windows.Forms.TextBox txtModelName;
		private System.Windows.Forms.Label lblModelName;
		private System.Windows.Forms.ErrorProvider errorProvider;
		private System.Windows.Forms.Label lblExample;
		private System.Windows.Forms.Label lblNamespacePrefix;
		private System.Windows.Forms.TextBox txtNamespacePrefix;
		private System.Windows.Forms.TextBox lblDescriptionContent;
	}
}
