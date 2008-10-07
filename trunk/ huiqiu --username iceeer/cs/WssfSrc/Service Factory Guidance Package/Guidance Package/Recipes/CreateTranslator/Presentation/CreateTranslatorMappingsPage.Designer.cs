namespace Microsoft.Practices.ServiceFactory.Recipes.CreateTranslator.Presentation
{
    partial class CreateTranslatorMappingsPage
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CreateTranslatorMappingsPage));
			this.toolTip = new System.Windows.Forms.ToolTip(this.components);
			this.removeMappingButton = new System.Windows.Forms.Button();
			this.createMappingButton = new System.Windows.Forms.Button();
			this.mappedProperties = new System.Windows.Forms.ListBox();
			this.secondProperties = new System.Windows.Forms.ListBox();
			this.firstProperties = new System.Windows.Forms.ListBox();
			this.label3 = new System.Windows.Forms.Label();
			this.secondPropertiesLabel = new System.Windows.Forms.Label();
			this.firstPropertiesLabel = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
			this.SuspendLayout();
			// 
			// infoPanel
			// 
			resources.ApplyResources(this.infoPanel, "infoPanel");
			// 
			// removeMappingButton
			// 
			resources.ApplyResources(this.removeMappingButton, "removeMappingButton");
			this.removeMappingButton.Name = "removeMappingButton";
			this.toolTip.SetToolTip(this.removeMappingButton, resources.GetString("removeMappingButton.ToolTip"));
			this.removeMappingButton.UseVisualStyleBackColor = true;
			this.removeMappingButton.Click += new System.EventHandler(this.removeMappingButton_Click);
			// 
			// createMappingButton
			// 
			resources.ApplyResources(this.createMappingButton, "createMappingButton");
			this.createMappingButton.Name = "createMappingButton";
			this.toolTip.SetToolTip(this.createMappingButton, resources.GetString("createMappingButton.ToolTip"));
			this.createMappingButton.UseVisualStyleBackColor = true;
			this.createMappingButton.Click += new System.EventHandler(this.createMappingButton_Click);
			// 
			// mappedProperties
			// 
			this.mappedProperties.FormattingEnabled = true;
			resources.ApplyResources(this.mappedProperties, "mappedProperties");
			this.mappedProperties.Name = "mappedProperties";
			this.toolTip.SetToolTip(this.mappedProperties, resources.GetString("mappedProperties.ToolTip"));
			this.mappedProperties.SelectedIndexChanged += new System.EventHandler(this.mappedProperties_SelectedIndexChanged);
			// 
			// secondProperties
			// 
			this.secondProperties.FormattingEnabled = true;
			resources.ApplyResources(this.secondProperties, "secondProperties");
			this.secondProperties.Name = "secondProperties";
			this.toolTip.SetToolTip(this.secondProperties, resources.GetString("secondProperties.ToolTip"));
			this.secondProperties.SelectedIndexChanged += new System.EventHandler(this.secondProperties_SelectedIndexChanged);
			// 
			// firstProperties
			// 
			this.firstProperties.FormattingEnabled = true;
			resources.ApplyResources(this.firstProperties, "firstProperties");
			this.firstProperties.Name = "firstProperties";
			this.toolTip.SetToolTip(this.firstProperties, resources.GetString("firstProperties.ToolTip"));
			this.firstProperties.SelectedIndexChanged += new System.EventHandler(this.firstProperties_SelectedIndexChanged);
			// 
			// label3
			// 
			resources.ApplyResources(this.label3, "label3");
			this.label3.Name = "label3";
			// 
			// secondPropertiesLabel
			// 
			resources.ApplyResources(this.secondPropertiesLabel, "secondPropertiesLabel");
			this.secondPropertiesLabel.Name = "secondPropertiesLabel";
			// 
			// firstPropertiesLabel
			// 
			resources.ApplyResources(this.firstPropertiesLabel, "firstPropertiesLabel");
			this.firstPropertiesLabel.Name = "firstPropertiesLabel";
			// 
			// CreateTranslatorMappingsPage
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.label3);
			this.Controls.Add(this.secondPropertiesLabel);
			this.Controls.Add(this.firstPropertiesLabel);
			this.Controls.Add(this.removeMappingButton);
			this.Controls.Add(this.createMappingButton);
			this.Controls.Add(this.mappedProperties);
			this.Controls.Add(this.secondProperties);
			this.Controls.Add(this.firstProperties);
			this.InfoRTBoxIcon = global::Microsoft.Practices.ServiceFactory.Properties.Resources.Information;
			this.Name = "CreateTranslatorMappingsPage";
			this.Controls.SetChildIndex(this.infoPanel, 0);
			this.Controls.SetChildIndex(this.firstProperties, 0);
			this.Controls.SetChildIndex(this.secondProperties, 0);
			this.Controls.SetChildIndex(this.mappedProperties, 0);
			this.Controls.SetChildIndex(this.createMappingButton, 0);
			this.Controls.SetChildIndex(this.removeMappingButton, 0);
			this.Controls.SetChildIndex(this.firstPropertiesLabel, 0);
			this.Controls.SetChildIndex(this.secondPropertiesLabel, 0);
			this.Controls.SetChildIndex(this.label3, 0);
			((System.ComponentModel.ISupportInitialize)(this)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

		private System.Windows.Forms.ToolTip toolTip;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label secondPropertiesLabel;
		private System.Windows.Forms.Label firstPropertiesLabel;
		private System.Windows.Forms.Button removeMappingButton;
		private System.Windows.Forms.Button createMappingButton;
		private System.Windows.Forms.ListBox mappedProperties;
		private System.Windows.Forms.ListBox secondProperties;
		private System.Windows.Forms.ListBox firstProperties;
    }
}
