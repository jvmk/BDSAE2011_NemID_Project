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
            this.optionsGroupBox = new System.Windows.Forms.GroupBox();
            this.optionsLabel = new System.Windows.Forms.Label();
            this.DeleteAccountButton = new System.Windows.Forms.Button();
            this.ContinueToExternalSiteButton = new System.Windows.Forms.Button();
            this.continueToExternalInfo = new System.Windows.Forms.Label();
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
            this.splitContainer1.Panel2.Controls.Add(this.optionsGroupBox);
            this.splitContainer1.Size = new System.Drawing.Size(500, 500);
            this.splitContainer1.SplitterDistance = 125;
            this.splitContainer1.TabIndex = 1;
            // 
            // optionsGroupBox
            // 
            this.optionsGroupBox.Controls.Add(this.continueToExternalInfo);
            this.optionsGroupBox.Controls.Add(this.DeleteAccountButton);
            this.optionsGroupBox.Controls.Add(this.optionsLabel);
            this.optionsGroupBox.Controls.Add(this.ContinueToExternalSiteButton);
            this.optionsGroupBox.Location = new System.Drawing.Point(14, 15);
            this.optionsGroupBox.Name = "optionsGroupBox";
            this.optionsGroupBox.Size = new System.Drawing.Size(470, 94);
            this.optionsGroupBox.TabIndex = 2;
            this.optionsGroupBox.TabStop = false;
            this.optionsGroupBox.Text = "Options";
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
            // continueToExternalInfo
            // 
            this.continueToExternalInfo.AutoSize = true;
            this.continueToExternalInfo.Location = new System.Drawing.Point(152, 37);
            this.continueToExternalInfo.Name = "continueToExternalInfo";
            this.continueToExternalInfo.Size = new System.Drawing.Size(271, 13);
            this.continueToExternalInfo.TabIndex = 3;
            this.continueToExternalInfo.Text = "(The website that redirected you here for authentication)";
            // 
            // NemIdLoggedIn
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "NemIdLoggedIn";
            this.Size = new System.Drawing.Size(500, 500);
            this.splitContainer1.Panel2.ResumeLayout(false);
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
    }
}
