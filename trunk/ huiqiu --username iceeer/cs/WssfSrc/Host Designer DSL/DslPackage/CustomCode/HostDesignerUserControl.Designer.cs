namespace Microsoft.Practices.ServiceFactory.HostDesigner
{
	partial class HostDesignerUserControl
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HostDesignerUserControl));
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
			this.lblNoAction = new System.Windows.Forms.Label();
			this.linkGenerateCode = new System.Windows.Forms.LinkLabel();
			this.lblGenerateCode = new System.Windows.Forms.Label();
			this.linkGenerateProxy = new System.Windows.Forms.LinkLabel();
			this.lblGenerateProxy = new System.Windows.Forms.Label();
			this.lblModelInvalid = new System.Windows.Forms.Label();
			this.panel1 = new System.Windows.Forms.Panel();
			this.panel2 = new System.Windows.Forms.Panel();
			this.lblNodeType = new System.Windows.Forms.Label();
			this.lblNodeName = new System.Windows.Forms.Label();
			this.lblElementType = new System.Windows.Forms.Label();
			this.lblElementName = new System.Windows.Forms.Label();
			this.picIcon = new System.Windows.Forms.PictureBox();
			this.lblGuidance = new System.Windows.Forms.Label();
			this.lblGuidanceNodeType = new System.Windows.Forms.Label();
			this.linkValidate = new System.Windows.Forms.LinkLabel();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.flowLayoutPanel1.SuspendLayout();
			this.panel1.SuspendLayout();
			this.panel2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.picIcon)).BeginInit();
			this.SuspendLayout();
			// 
			// splitContainer1
			// 
			this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			resources.ApplyResources(this.splitContainer1, "splitContainer1");
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.flowLayoutPanel1);
			this.splitContainer1.Panel1.Controls.Add(this.panel1);
			// 
			// splitContainer1.Panel2
			// 
			resources.ApplyResources(this.splitContainer1.Panel2, "splitContainer1.Panel2");
			this.splitContainer1.Panel2.BackColor = System.Drawing.SystemColors.Info;
			this.splitContainer1.Panel2.Controls.Add(this.lblGuidance);
			this.splitContainer1.Panel2.Controls.Add(this.lblGuidanceNodeType);
			// 
			// flowLayoutPanel1
			// 
			this.flowLayoutPanel1.Controls.Add(this.lblNoAction);
			this.flowLayoutPanel1.Controls.Add(this.linkGenerateCode);
			this.flowLayoutPanel1.Controls.Add(this.lblGenerateCode);
			this.flowLayoutPanel1.Controls.Add(this.linkGenerateProxy);
			this.flowLayoutPanel1.Controls.Add(this.lblGenerateProxy);
			this.flowLayoutPanel1.Controls.Add(this.lblModelInvalid);
			this.flowLayoutPanel1.Controls.Add(this.linkValidate);
			resources.ApplyResources(this.flowLayoutPanel1, "flowLayoutPanel1");
			this.flowLayoutPanel1.Name = "flowLayoutPanel1";
			// 
			// lblNoAction
			// 
			resources.ApplyResources(this.lblNoAction, "lblNoAction");
			this.lblNoAction.ForeColor = System.Drawing.SystemColors.ControlDark;
			this.lblNoAction.Name = "lblNoAction";
			// 
			// linkGenerateCode
			// 
			resources.ApplyResources(this.linkGenerateCode, "linkGenerateCode");
			this.linkGenerateCode.Name = "linkGenerateCode";
			this.linkGenerateCode.TabStop = true;
			this.linkGenerateCode.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkGenerateCode_LinkClicked);
			// 
			// lblGenerateCode
			// 
			resources.ApplyResources(this.lblGenerateCode, "lblGenerateCode");
			this.lblGenerateCode.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
			this.lblGenerateCode.Name = "lblGenerateCode";
			// 
			// linkGenerateProxy
			// 
			resources.ApplyResources(this.linkGenerateProxy, "linkGenerateProxy");
			this.linkGenerateProxy.Name = "linkGenerateProxy";
			this.linkGenerateProxy.TabStop = true;
			this.linkGenerateProxy.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkGenerateProxy_LinkClicked);
			// 
			// lblGenerateProxy
			// 
			resources.ApplyResources(this.lblGenerateProxy, "lblGenerateProxy");
			this.lblGenerateProxy.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
			this.lblGenerateProxy.Name = "lblGenerateProxy";
			// 
			// lblModelInvalid
			// 
			resources.ApplyResources(this.lblModelInvalid, "lblModelInvalid");
			this.lblModelInvalid.ForeColor = System.Drawing.SystemColors.ControlDark;
			this.lblModelInvalid.Name = "lblModelInvalid";
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.panel2);
			resources.ApplyResources(this.panel1, "panel1");
			this.panel1.Name = "panel1";
			// 
			// panel2
			// 
			this.panel2.Controls.Add(this.lblNodeType);
			this.panel2.Controls.Add(this.lblNodeName);
			this.panel2.Controls.Add(this.lblElementType);
			this.panel2.Controls.Add(this.lblElementName);
			this.panel2.Controls.Add(this.picIcon);
			resources.ApplyResources(this.panel2, "panel2");
			this.panel2.Name = "panel2";
			// 
			// lblNodeType
			// 
			resources.ApplyResources(this.lblNodeType, "lblNodeType");
			this.lblNodeType.Name = "lblNodeType";
			// 
			// lblNodeName
			// 
			resources.ApplyResources(this.lblNodeName, "lblNodeName");
			this.lblNodeName.Name = "lblNodeName";
			// 
			// lblElementType
			// 
			resources.ApplyResources(this.lblElementType, "lblElementType");
			this.lblElementType.Name = "lblElementType";
			// 
			// lblElementName
			// 
			resources.ApplyResources(this.lblElementName, "lblElementName");
			this.lblElementName.Name = "lblElementName";
			// 
			// picIcon
			// 
			resources.ApplyResources(this.picIcon, "picIcon");
			this.picIcon.Name = "picIcon";
			this.picIcon.TabStop = false;
			// 
			// lblGuidance
			// 
			this.lblGuidance.AutoEllipsis = true;
			resources.ApplyResources(this.lblGuidance, "lblGuidance");
			this.lblGuidance.Name = "lblGuidance";
			// 
			// lblGuidanceNodeType
			// 
			resources.ApplyResources(this.lblGuidanceNodeType, "lblGuidanceNodeType");
			this.lblGuidanceNodeType.Name = "lblGuidanceNodeType";
			// 
			// linkValidate
			// 
			resources.ApplyResources(this.linkValidate, "linkValidate");
			this.linkValidate.Name = "linkValidate";
			this.linkValidate.TabStop = true;
			this.linkValidate.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkValidate_LinkClicked);
			// 
			// HostDesignerUserControl
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.splitContainer1);
			this.Name = "HostDesignerUserControl";
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.ResumeLayout(false);
			this.flowLayoutPanel1.ResumeLayout(false);
			this.flowLayoutPanel1.PerformLayout();
			this.panel1.ResumeLayout(false);
			this.panel2.ResumeLayout(false);
			this.panel2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.picIcon)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.Label lblGuidance;
		private System.Windows.Forms.Label lblGuidanceNodeType;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
		private System.Windows.Forms.Label lblNoAction;
		private System.Windows.Forms.LinkLabel linkGenerateCode;
		private System.Windows.Forms.Label lblGenerateCode;
		private System.Windows.Forms.LinkLabel linkGenerateProxy;
		private System.Windows.Forms.Label lblGenerateProxy;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.Label lblElementType;
		private System.Windows.Forms.Label lblElementName;
		private System.Windows.Forms.PictureBox picIcon;
		private System.Windows.Forms.Label lblNodeName;
		private System.Windows.Forms.Label lblNodeType;
		private System.Windows.Forms.Label lblModelInvalid;
		private System.Windows.Forms.LinkLabel linkValidate;
	}
}
