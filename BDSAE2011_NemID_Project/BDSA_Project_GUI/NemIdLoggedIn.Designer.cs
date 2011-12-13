namespace BDSA_Project_GUI
{
    partial class NemIdLoggedIn
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.label1 = new System.Windows.Forms.Label();
            this.LogOut = new System.Windows.Forms.Button();
            this.optionsGroupBox = new System.Windows.Forms.GroupBox();
            this.continueToExternalInfo = new System.Windows.Forms.Label();
            this.DeleteAccountButton = new System.Windows.Forms.Button();
            this.optionsLabel = new System.Windows.Forms.Label();
            this.ContinueToExternalSiteButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.optionsGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.BackgroundImage = global::BDSA_Project_GUI.Properties.Resources.nemidlogo;
            this.splitContainer1.Panel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.label1);
            this.splitContainer1.Panel2.Controls.Add(this.LogOut);
            this.splitContainer1.Panel2.Controls.Add(this.optionsGroupBox);
            this.splitContainer1.Size = new System.Drawing.Size(500, 500);
            this.splitContainer1.SplitterDistance = 125;
            this.splitContainer1.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(73, 61);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(366, 16);
            this.label1.TabIndex = 4;
            this.label1.Text = "You have succesfully been authenticated by NemID!";
            // 
            // LogOut
            // 
            this.LogOut.Location = new System.Drawing.Point(375, 301);
            this.LogOut.Name = "LogOut";
            this.LogOut.Size = new System.Drawing.Size(75, 23);
            this.LogOut.TabIndex = 3;
            this.LogOut.Text = "Log Out";
            this.LogOut.UseVisualStyleBackColor = true;
            // 
            // optionsGroupBox
            // 
            this.optionsGroupBox.Controls.Add(this.continueToExternalInfo);
            this.optionsGroupBox.Controls.Add(this.DeleteAccountButton);
            this.optionsGroupBox.Controls.Add(this.optionsLabel);
            this.optionsGroupBox.Controls.Add(this.ContinueToExternalSiteButton);
            this.optionsGroupBox.Location = new System.Drawing.Point(16, 157);
            this.optionsGroupBox.Name = "optionsGroupBox";
            this.optionsGroupBox.Size = new System.Drawing.Size(470, 94);
            this.optionsGroupBox.TabIndex = 2;
            this.optionsGroupBox.TabStop = false;
            this.optionsGroupBox.Text = "Options";
            // 
            // continueToExternalInfo
            // 
            this.continueToExternalInfo.AutoSize = true;
            this.continueToExternalInfo.Location = new System.Drawing.Point(152, 37);
            this.continueToExternalInfo.Name = "continueToExternalInfo";
            this.continueToExternalInfo.Size = new System.Drawing.Size(271, 13);
            this.continueToExternalInfo.TabIndex = 3;
            this.continueToExternalInfo.Text = "(The website that redirected you here for authentication)";
            // 
            // DeleteAccountButton
            // 
            this.DeleteAccountButton.Location = new System.Drawing.Point(6, 61);
            this.DeleteAccountButton.Name = "DeleteAccountButton";
            this.DeleteAccountButton.Size = new System.Drawing.Size(140, 23);
            this.DeleteAccountButton.TabIndex = 1;
            this.DeleteAccountButton.Text = "Delete NemID account";
            this.DeleteAccountButton.UseVisualStyleBackColor = true;
            this.DeleteAccountButton.Click += new System.EventHandler(this.DeleteAccountButton_Click);
            // 
            // optionsLabel
            // 
            this.optionsLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.optionsLabel.AutoSize = true;
            this.optionsLabel.Location = new System.Drawing.Point(6, 16);
            this.optionsLabel.Name = "optionsLabel";
            this.optionsLabel.Size = new System.Drawing.Size(134, 13);
            this.optionsLabel.TabIndex = 0;
            this.optionsLabel.Text = "Your options from here are:";
            // 
            // ContinueToExternalSiteButton
            // 
            this.ContinueToExternalSiteButton.Location = new System.Drawing.Point(6, 32);
            this.ContinueToExternalSiteButton.Name = "ContinueToExternalSiteButton";
            this.ContinueToExternalSiteButton.Size = new System.Drawing.Size(140, 23);
            this.ContinueToExternalSiteButton.TabIndex = 0;
            this.ContinueToExternalSiteButton.Text = "Continue to external site";
            this.ContinueToExternalSiteButton.UseVisualStyleBackColor = true;
            this.ContinueToExternalSiteButton.Click += new System.EventHandler(this.ContinueToExternalSiteButton_Click);
            // 
            // NemIdLoggedIn
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "NemIdLoggedIn";
            this.Size = new System.Drawing.Size(500, 500);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.optionsGroupBox.ResumeLayout(false);
            this.optionsGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.GroupBox optionsGroupBox;
        private System.Windows.Forms.Label optionsLabel;
        private System.Windows.Forms.Button DeleteAccountButton;
        private System.Windows.Forms.Button ContinueToExternalSiteButton;
        private System.Windows.Forms.Label continueToExternalInfo;
        private System.Windows.Forms.Button LogOut;
        private System.Windows.Forms.Label label1;
    }
}
