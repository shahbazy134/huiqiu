namespace Microsoft.Practices.ServiceFactory.Recipes.CreateTranslator.Presentation
{
    partial class CreateTranslatorPropertiesPage
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CreateTranslatorPropertiesPage));
			this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
			this.toolTip = new System.Windows.Forms.ToolTip(this.components);
			this.secondClassValueEditor = new Microsoft.Practices.WizardFramework.ValueEditor();
			this.mappingClassNamespaceTextBox = new System.Windows.Forms.TextBox();
			this.mappingClassNameTextBox = new System.Windows.Forms.TextBox();
			this.firstClassValueEditor = new Microsoft.Practices.WizardFramework.ValueEditor();
			this.panel1 = new System.Windows.Forms.Panel();
			this.firstTypeLabel = new System.Windows.Forms.Label();
			this.mappingClassNameLabel = new System.Windows.Forms.Label();
			this.secondTypeLabel = new System.Windows.Forms.Label();
			this.mappingClassNamespace = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.secondClassValueEditor)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.firstClassValueEditor)).BeginInit();
			this.panel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
			this.SuspendLayout();
			// 
			// infoPanel
			// 
			resources.ApplyResources(this.infoPanel, "infoPanel");
			// 
			// errorProvider
			// 
			this.errorProvider.ContainerControl = this;
			// 
			// secondClassValueEditor
			// 
			resources.ApplyResources(this.secondClassValueEditor, "secondClassValueEditor");
			this.secondClassValueEditor.BackColor = System.Drawing.SystemColors.Window;
			this.secondClassValueEditor.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.secondClassValueEditor.ConverterInstance = null;
			this.secondClassValueEditor.EditorInstance = null;
			this.secondClassValueEditor.EditorType = null;
			this.secondClassValueEditor.ForeColor = System.Drawing.SystemColors.WindowText;
			this.secondClassValueEditor.Name = "secondClassValueEditor";
			this.secondClassValueEditor.ReadOnly = false;
			this.secondClassValueEditor.ToolTip = "The second class to map";
			this.toolTip.SetToolTip(this.secondClassValueEditor, resources.GetString("secondClassValueEditor.ToolTip"));
			this.secondClassValueEditor.ValueRequired = false;
			this.secondClassValueEditor.ValueType = null;
			// 
			// mappingClassNamespaceTextBox
			// 
			resources.ApplyResources(this.mappingClassNamespaceTextBox, "mappingClassNamespaceTextBox");
			this.mappingClassNamespaceTextBox.Name = "mappingClassNamespaceTextBox";
			this.toolTip.SetToolTip(this.mappingClassNamespaceTextBox, resources.GetString("mappingClassNamespaceTextBox.ToolTip"));
			// 
			// mappingClassNameTextBox
			// 
			resources.ApplyResources(this.mappingClassNameTextBox, "mappingClassNameTextBox");
			this.mappingClassNameTextBox.Name = "mappingClassNameTextBox";
			this.toolTip.SetToolTip(this.mappingClassNameTextBox, resources.GetString("mappingClassNameTextBox.ToolTip"));
			// 
			// firstClassValueEditor
			// 
			resources.ApplyResources(this.firstClassValueEditor, "firstClassValueEditor");
			this.firstClassValueEditor.BackColor = System.Drawing.SystemColors.Window;
			this.firstClassValueEditor.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.firstClassValueEditor.ConverterInstance = null;
			this.firstClassValueEditor.EditorInstance = null;
			this.firstClassValueEditor.EditorType = null;
			this.firstClassValueEditor.ForeColor = System.Drawing.SystemColors.WindowText;
			this.firstClassValueEditor.Name = "firstClassValueEditor";
			this.firstClassValueEditor.ReadOnly = false;
			this.firstClassValueEditor.ToolTip = "The first class to map";
			this.toolTip.SetToolTip(this.firstClassValueEditor, resources.GetString("firstClassValueEditor.ToolTip"));
			this.firstClassValueEditor.ValueRequired = false;
			this.firstClassValueEditor.ValueType = null;
			// 
			// panel1
			// 
			resources.ApplyResources(this.panel1, "panel1");
			this.panel1.Controls.Add(this.firstTypeLabel);
			this.panel1.Controls.Add(this.mappingClassNameLabel);
			this.panel1.Controls.Add(this.secondTypeLabel);
			this.panel1.Controls.Add(this.mappingClassNamespace);
			this.panel1.Controls.Add(this.secondClassValueEditor);
			this.panel1.Controls.Add(this.mappingClassNamespaceTextBox);
			this.panel1.Controls.Add(this.mappingClassNameTextBox);
			this.panel1.Controls.Add(this.firstClassValueEditor);
			this.panel1.Name = "panel1";
			// 
			// firstTypeLabel
			// 
			resources.ApplyResources(this.firstTypeLabel, "firstTypeLabel");
			this.firstTypeLabel.Name = "firstTypeLabel";
			// 
			// mappingClassNameLabel
			// 
			resources.ApplyResources(this.mappingClassNameLabel, "mappingClassNameLabel");
			this.mappingClassNameLabel.Name = "mappingClassNameLabel";
			// 
			// secondTypeLabel
			// 
			resources.ApplyResources(this.secondTypeLabel, "secondTypeLabel");
			this.secondTypeLabel.Name = "secondTypeLabel";
			// 
			// mappingClassNamespace
			// 
			resources.ApplyResources(this.mappingClassNamespace, "mappingClassNamespace");
			this.mappingClassNamespace.Name = "mappingClassNamespace";
			// 
			// CreateTranslatorPropertiesPage
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.panel1);
			this.InfoRTBoxIcon = global::Microsoft.Practices.ServiceFactory.Properties.Resources.Information;
			this.Name = "CreateTranslatorPropertiesPage";
			this.Controls.SetChildIndex(this.infoPanel, 0);
			this.Controls.SetChildIndex(this.panel1, 0);
			((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.secondClassValueEditor)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.firstClassValueEditor)).EndInit();
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

		private System.Windows.Forms.ErrorProvider errorProvider;
        private System.Windows.Forms.ToolTip toolTip;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Label firstTypeLabel;
		private System.Windows.Forms.Label mappingClassNameLabel;
		private System.Windows.Forms.Label secondTypeLabel;
		private System.Windows.Forms.Label mappingClassNamespace;
		private Microsoft.Practices.WizardFramework.ValueEditor secondClassValueEditor;
		private System.Windows.Forms.TextBox mappingClassNamespaceTextBox;
		private System.Windows.Forms.TextBox mappingClassNameTextBox;
		private Microsoft.Practices.WizardFramework.ValueEditor firstClassValueEditor;
    }
}
