namespace WindowsLiveIDClientSample
{
    partial class DetailsWindow
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
            this.labelIsDefaultUser = new System.Windows.Forms.Label();
            this.labelIsAuthed = new System.Windows.Forms.Label();
            this.labelPersist = new System.Windows.Forms.Label();
            this.labelcId = new System.Windows.Forms.Label();
            this.labelUserName = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // labelIsDefaultUser
            // 
            this.labelIsDefaultUser.AutoSize = true;
            this.labelIsDefaultUser.Location = new System.Drawing.Point(12, 130);
            this.labelIsDefaultUser.Name = "labelIsDefaultUser";
            this.labelIsDefaultUser.Size = new System.Drawing.Size(72, 13);
            this.labelIsDefaultUser.TabIndex = 14;
            this.labelIsDefaultUser.Text = "Default User: ";
            // 
            // labelIsAuthed
            // 
            this.labelIsAuthed.AutoSize = true;
            this.labelIsAuthed.Location = new System.Drawing.Point(12, 100);
            this.labelIsAuthed.Name = "labelIsAuthed";
            this.labelIsAuthed.Size = new System.Drawing.Size(112, 13);
            this.labelIsAuthed.TabIndex = 13;
            this.labelIsAuthed.Text = "IsAuthenticated: False";
            // 
            // labelPersist
            // 
            this.labelPersist.AutoSize = true;
            this.labelPersist.Location = new System.Drawing.Point(12, 70);
            this.labelPersist.Name = "labelPersist";
            this.labelPersist.Size = new System.Drawing.Size(111, 13);
            this.labelPersist.TabIndex = 12;
            this.labelPersist.Text = "Credentials Persisted: ";
            // 
            // labelcId
            // 
            this.labelcId.AutoSize = true;
            this.labelcId.Location = new System.Drawing.Point(12, 40);
            this.labelcId.Name = "labelcId";
            this.labelcId.Size = new System.Drawing.Size(29, 13);
            this.labelcId.TabIndex = 11;
            this.labelcId.Text = "CId: ";
            // 
            // labelUserName
            // 
            this.labelUserName.AutoSize = true;
            this.labelUserName.Location = new System.Drawing.Point(12, 10);
            this.labelUserName.Name = "labelUserName";
            this.labelUserName.Size = new System.Drawing.Size(63, 13);
            this.labelUserName.TabIndex = 10;
            this.labelUserName.Text = "UserName: ";
            // 
            // DetailsWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(340, 192);
            this.Controls.Add(this.labelIsDefaultUser);
            this.Controls.Add(this.labelIsAuthed);
            this.Controls.Add(this.labelPersist);
            this.Controls.Add(this.labelcId);
            this.Controls.Add(this.labelUserName);
            this.Name = "DetailsWindow";
            this.Text = "User Details";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelIsDefaultUser;
        private System.Windows.Forms.Label labelIsAuthed;
        private System.Windows.Forms.Label labelPersist;
        private System.Windows.Forms.Label labelcId;
        private System.Windows.Forms.Label labelUserName;
    }
}