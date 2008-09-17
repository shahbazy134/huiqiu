namespace WindowsLiveIDClientSample
{
    partial class BlogWindow
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
            this.labelSpaceUrl = new System.Windows.Forms.Label();
            this.textBoxSpaceUrl = new System.Windows.Forms.TextBox();
            this.labelBlogSubject = new System.Windows.Forms.Label();
            this.textBoxBlogSubject = new System.Windows.Forms.TextBox();
            this.labelBlogBody = new System.Windows.Forms.Label();
            this.textBoxBlogBody = new System.Windows.Forms.TextBox();
            this.buttonViewBlog = new System.Windows.Forms.Button();
            this.buttonPostBlog = new System.Windows.Forms.Button();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.labelBlogInstructions = new System.Windows.Forms.Label();
            this.buttonClear = new System.Windows.Forms.Button();
            this.labelSpaceURLHelper = new System.Windows.Forms.Label();
            this.labelSpaceURLExample = new System.Windows.Forms.Label();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelSpaceUrl
            // 
            this.labelSpaceUrl.AutoSize = true;
            this.labelSpaceUrl.Location = new System.Drawing.Point(3, 78);
            this.labelSpaceUrl.Name = "labelSpaceUrl";
            this.labelSpaceUrl.Size = new System.Drawing.Size(72, 13);
            this.labelSpaceUrl.TabIndex = 23;
            this.labelSpaceUrl.Text = "Space Name:";
            // 
            // textBoxSpaceUrl
            // 
            this.textBoxSpaceUrl.Location = new System.Drawing.Point(81, 75);
            this.textBoxSpaceUrl.MaxLength = 256;
            this.textBoxSpaceUrl.Name = "textBoxSpaceUrl";
            this.textBoxSpaceUrl.Size = new System.Drawing.Size(267, 20);
            this.textBoxSpaceUrl.TabIndex = 18;
            this.textBoxSpaceUrl.TextChanged += new System.EventHandler(this.textBoxSpaceUrl_TextChanged);
            // 
            // labelBlogSubject
            // 
            this.labelBlogSubject.AutoSize = true;
            this.labelBlogSubject.Location = new System.Drawing.Point(29, 130);
            this.labelBlogSubject.Name = "labelBlogSubject";
            this.labelBlogSubject.Size = new System.Drawing.Size(46, 13);
            this.labelBlogSubject.TabIndex = 22;
            this.labelBlogSubject.Text = "Subject:";
            // 
            // textBoxBlogSubject
            // 
            this.textBoxBlogSubject.Location = new System.Drawing.Point(81, 127);
            this.textBoxBlogSubject.MaxLength = 256;
            this.textBoxBlogSubject.Name = "textBoxBlogSubject";
            this.textBoxBlogSubject.Size = new System.Drawing.Size(267, 20);
            this.textBoxBlogSubject.TabIndex = 19;
            this.textBoxBlogSubject.TextChanged += new System.EventHandler(this.textBoxBlogSubject_TextChanged);
            // 
            // labelBlogBody
            // 
            this.labelBlogBody.AutoSize = true;
            this.labelBlogBody.Location = new System.Drawing.Point(41, 156);
            this.labelBlogBody.Name = "labelBlogBody";
            this.labelBlogBody.Size = new System.Drawing.Size(34, 13);
            this.labelBlogBody.TabIndex = 21;
            this.labelBlogBody.Text = "Body:";
            // 
            // textBoxBlogBody
            // 
            this.textBoxBlogBody.Location = new System.Drawing.Point(81, 153);
            this.textBoxBlogBody.Multiline = true;
            this.textBoxBlogBody.Name = "textBoxBlogBody";
            this.textBoxBlogBody.Size = new System.Drawing.Size(267, 138);
            this.textBoxBlogBody.TabIndex = 20;
            this.textBoxBlogBody.TextChanged += new System.EventHandler(this.textBoxBlogBody_TextChanged);
            // 
            // buttonViewBlog
            // 
            this.buttonViewBlog.Location = new System.Drawing.Point(273, 297);
            this.buttonViewBlog.Name = "buttonViewBlog";
            this.buttonViewBlog.Size = new System.Drawing.Size(75, 23);
            this.buttonViewBlog.TabIndex = 25;
            this.buttonViewBlog.Text = "View Blog";
            this.buttonViewBlog.UseVisualStyleBackColor = true;
            this.buttonViewBlog.Click += new System.EventHandler(this.buttonViewBlog_Click);
            // 
            // buttonPostBlog
            // 
            this.buttonPostBlog.Location = new System.Drawing.Point(192, 297);
            this.buttonPostBlog.Name = "buttonPostBlog";
            this.buttonPostBlog.Size = new System.Drawing.Size(75, 23);
            this.buttonPostBlog.TabIndex = 24;
            this.buttonPostBlog.Text = "Post Blog";
            this.buttonPostBlog.UseVisualStyleBackColor = true;
            this.buttonPostBlog.Click += new System.EventHandler(this.buttonPostBlog_Click);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.labelBlogInstructions);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(29, 9);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(278, 60);
            this.flowLayoutPanel1.TabIndex = 26;
            // 
            // labelBlogInstructions
            // 
            this.labelBlogInstructions.AutoSize = true;
            this.labelBlogInstructions.Location = new System.Drawing.Point(3, 0);
            this.labelBlogInstructions.Name = "labelBlogInstructions";
            this.labelBlogInstructions.Size = new System.Drawing.Size(271, 52);
            this.labelBlogInstructions.TabIndex = 0;
            this.labelBlogInstructions.Text = "To post a blog, first enter the name of your Spaces Web site into the Space Name " +
                "text box. For example, if your space URL is http://mysite.spaces.live.com, type " +
                "mysite.";
            // 
            // buttonClear
            // 
            this.buttonClear.Location = new System.Drawing.Point(81, 296);
            this.buttonClear.Name = "buttonClear";
            this.buttonClear.Size = new System.Drawing.Size(75, 23);
            this.buttonClear.TabIndex = 27;
            this.buttonClear.Text = "Clear";
            this.buttonClear.UseVisualStyleBackColor = true;
            this.buttonClear.Click += new System.EventHandler(this.buttonClear_Click);
            // 
            // labelSpaceURLHelper
            // 
            this.labelSpaceURLHelper.AutoSize = true;
            this.labelSpaceURLHelper.Location = new System.Drawing.Point(9, 103);
            this.labelSpaceURLHelper.Name = "labelSpaceURLHelper";
            this.labelSpaceURLHelper.Size = new System.Drawing.Size(66, 13);
            this.labelSpaceURLHelper.TabIndex = 28;
            this.labelSpaceURLHelper.Text = "Space URL:";
            // 
            // labelSpaceURLExample
            // 
            this.labelSpaceURLExample.AutoSize = true;
            this.labelSpaceURLExample.Location = new System.Drawing.Point(78, 102);
            this.labelSpaceURLExample.Name = "labelSpaceURLExample";
            this.labelSpaceURLExample.Size = new System.Drawing.Size(114, 13);
            this.labelSpaceURLExample.TabIndex = 29;
            this.labelSpaceURLExample.Text = "http://spaces.live.com";
            // 
            // BlogWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(363, 336);
            this.Controls.Add(this.labelSpaceURLExample);
            this.Controls.Add(this.labelSpaceURLHelper);
            this.Controls.Add(this.buttonClear);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.buttonViewBlog);
            this.Controls.Add(this.buttonPostBlog);
            this.Controls.Add(this.labelSpaceUrl);
            this.Controls.Add(this.textBoxSpaceUrl);
            this.Controls.Add(this.labelBlogSubject);
            this.Controls.Add(this.textBoxBlogSubject);
            this.Controls.Add(this.labelBlogBody);
            this.Controls.Add(this.textBoxBlogBody);
            this.Name = "BlogWindow";
            this.Text = "Blog";
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelSpaceUrl;
        private System.Windows.Forms.TextBox textBoxSpaceUrl;
        private System.Windows.Forms.Label labelBlogSubject;
        private System.Windows.Forms.TextBox textBoxBlogSubject;
        private System.Windows.Forms.Label labelBlogBody;
        private System.Windows.Forms.TextBox textBoxBlogBody;
        private System.Windows.Forms.Button buttonViewBlog;
        private System.Windows.Forms.Button buttonPostBlog;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Label labelBlogInstructions;
        private System.Windows.Forms.Button buttonClear;
        private System.Windows.Forms.Label labelSpaceURLHelper;
        private System.Windows.Forms.Label labelSpaceURLExample;
    }
}