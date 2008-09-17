namespace WindowsLiveIDClientSample
{
    partial class MainWindow
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.buttonSignInOrOut = new System.Windows.Forms.Button();
            this.buttonOptions = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.buttonBlog = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.buttonDetails = new System.Windows.Forms.Button();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.labelMainInstructionText = new System.Windows.Forms.Label();
            this.LinkLabelDocumentation = new System.Windows.Forms.LinkLabel();
            this.linkLabelSpaces = new System.Windows.Forms.LinkLabel();
            this.statusStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonSignInOrOut
            // 
            this.buttonSignInOrOut.Location = new System.Drawing.Point(244, 56);
            this.buttonSignInOrOut.Name = "buttonSignInOrOut";
            this.buttonSignInOrOut.Size = new System.Drawing.Size(75, 23);
            this.buttonSignInOrOut.TabIndex = 1;
            this.buttonSignInOrOut.Text = "Sign In";
            this.buttonSignInOrOut.UseVisualStyleBackColor = true;
            this.buttonSignInOrOut.Click += new System.EventHandler(this.buttonSignInOrOut_Click);
            // 
            // buttonOptions
            // 
            this.buttonOptions.Location = new System.Drawing.Point(244, 56);
            this.buttonOptions.Name = "buttonOptions";
            this.buttonOptions.Size = new System.Drawing.Size(75, 23);
            this.buttonOptions.TabIndex = 2;
            this.buttonOptions.Text = "Options";
            this.buttonOptions.UseVisualStyleBackColor = true;
            this.buttonOptions.Click += new System.EventHandler(this.buttonOptions_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 456);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(350, 22);
            this.statusStrip1.TabIndex = 3;
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(60, 17);
            this.toolStripStatusLabel1.Text = "Signed Out";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.flowLayoutPanel2);
            this.groupBox1.Controls.Add(this.buttonSignInOrOut);
            this.groupBox1.Location = new System.Drawing.Point(12, 83);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(325, 85);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Sign In";
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.Controls.Add(this.label1);
            this.flowLayoutPanel2.Location = new System.Drawing.Point(6, 19);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(300, 31);
            this.flowLayoutPanel2.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(287, 26);
            this.label1.TabIndex = 2;
            this.label1.Text = "To use the features of this application, you must first sign in using your Window" +
                "s Live ID.";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.buttonOptions);
            this.groupBox2.Location = new System.Drawing.Point(11, 174);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(325, 85);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Personal Options";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 20);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(187, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "View or change your personal options.";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.linkLabelSpaces);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.buttonBlog);
            this.groupBox3.Location = new System.Drawing.Point(12, 265);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(325, 85);
            this.groupBox3.TabIndex = 6;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Blog";
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(7, 20);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(311, 33);
            this.label3.TabIndex = 2;
            this.label3.Text = "View or post a blog to your Spaces account. For more information, see:";
            // 
            // buttonBlog
            // 
            this.buttonBlog.Location = new System.Drawing.Point(244, 56);
            this.buttonBlog.Name = "buttonBlog";
            this.buttonBlog.Size = new System.Drawing.Size(75, 23);
            this.buttonBlog.TabIndex = 2;
            this.buttonBlog.Text = "Blog";
            this.buttonBlog.UseVisualStyleBackColor = true;
            this.buttonBlog.Click += new System.EventHandler(this.buttonBlog_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.label4);
            this.groupBox4.Controls.Add(this.buttonDetails);
            this.groupBox4.Location = new System.Drawing.Point(12, 365);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(325, 85);
            this.groupBox4.TabIndex = 7;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Details";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 20);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(131, 13);
            this.label4.TabIndex = 2;
            this.label4.Text = "View your account details.";
            // 
            // buttonDetails
            // 
            this.buttonDetails.Location = new System.Drawing.Point(244, 56);
            this.buttonDetails.Name = "buttonDetails";
            this.buttonDetails.Size = new System.Drawing.Size(75, 23);
            this.buttonDetails.TabIndex = 2;
            this.buttonDetails.Text = "View Details";
            this.buttonDetails.UseVisualStyleBackColor = true;
            this.buttonDetails.Click += new System.EventHandler(this.buttonDetails_Click);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.labelMainInstructionText);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(11, 12);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(325, 47);
            this.flowLayoutPanel1.TabIndex = 9;
            // 
            // labelMainInstructionText
            // 
            this.labelMainInstructionText.AutoSize = true;
            this.labelMainInstructionText.Location = new System.Drawing.Point(3, 0);
            this.labelMainInstructionText.Name = "labelMainInstructionText";
            this.labelMainInstructionText.Size = new System.Drawing.Size(317, 39);
            this.labelMainInstructionText.TabIndex = 0;
            this.labelMainInstructionText.Text = "To locate shortcuts to this application and its source code, click Start->Program" +
                "s->Windows Live ID Client SDK, and then click the desired shortcut.";
            // 
            // LinkLabelDocumentation
            // 
            this.LinkLabelDocumentation.AutoSize = true;
            this.LinkLabelDocumentation.Location = new System.Drawing.Point(14, 63);
            this.LinkLabelDocumentation.Name = "LinkLabelDocumentation";
            this.LinkLabelDocumentation.Size = new System.Drawing.Size(210, 13);
            this.LinkLabelDocumentation.TabIndex = 4;
            this.LinkLabelDocumentation.TabStop = true;
            this.LinkLabelDocumentation.Text = "Click here to access online documentation.";
            this.LinkLabelDocumentation.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // linkLabelSpaces
            // 
            this.linkLabelSpaces.AutoSize = true;
            this.linkLabelSpaces.Location = new System.Drawing.Point(7, 53);
            this.linkLabelSpaces.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.linkLabelSpaces.Name = "linkLabelSpaces";
            this.linkLabelSpaces.Size = new System.Drawing.Size(114, 13);
            this.linkLabelSpaces.TabIndex = 3;
            this.linkLabelSpaces.TabStop = true;
            this.linkLabelSpaces.Text = "http://spaces.live.com";
            this.linkLabelSpaces.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelSpaces_LinkClicked);
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(350, 478);
            this.Controls.Add(this.LinkLabelDocumentation);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.statusStrip1);
            this.Name = "MainWindow";
            this.Text = "Windows Live ID Client Sample";
            this.Load += new System.EventHandler(this.MainWindow_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.flowLayoutPanel2.ResumeLayout(false);
            this.flowLayoutPanel2.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonSignInOrOut;
        private System.Windows.Forms.Button buttonOptions;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button buttonBlog;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button buttonDetails;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Label labelMainInstructionText;
        private System.Windows.Forms.LinkLabel LinkLabelDocumentation;
        private System.Windows.Forms.LinkLabel linkLabelSpaces;
    }
}

