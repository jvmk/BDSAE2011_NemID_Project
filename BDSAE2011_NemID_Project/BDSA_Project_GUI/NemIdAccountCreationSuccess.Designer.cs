namespace BDSA_Project_GUI
{
    partial class NemIdAccountCreationSuccess
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
            this.successGroupBox = new System.Windows.Forms.GroupBox();
            this.toLoginScreenButton = new System.Windows.Forms.Button();
            this.keycardInfoLabel = new System.Windows.Forms.Label();
            this.accountCreatedLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.successGroupBox.SuspendLayout();
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
            this.splitContainer1.Panel2.Controls.Add(this.successGroupBox);
            this.splitContainer1.Size = new System.Drawing.Size(500, 500);
            this.splitContainer1.SplitterDistance = 125;
            this.splitContainer1.TabIndex = 1;
            // 
            // successGroupBox
            // 
            this.successGroupBox.Controls.Add(this.toLoginScreenButton);
            this.successGroupBox.Controls.Add(this.keycardInfoLabel);
            this.successGroupBox.Controls.Add(this.accountCreatedLabel);
            this.successGroupBox.Location = new System.Drawing.Point(14, 23);
            this.successGroupBox.Name = "successGroupBox";
            this.successGroupBox.Size = new System.Drawing.Size(456, 173);
            this.successGroupBox.TabIndex = 3;
            this.successGroupBox.TabStop = false;
            this.successGroupBox.Text = "Success!";
            // 
            // toLoginScreenButton
            // 
            this.toLoginScreenButton.Location = new System.Drawing.Point(164, 96);
            this.toLoginScreenButton.Name = "toLoginScreenButton";
            this.toLoginScreenButton.Size = new System.Drawing.Size(133, 23);
            this.toLoginScreenButton.TabIndex = 5;
            this.toLoginScreenButton.Text = "Go to NemID login";
            this.toLoginScreenButton.UseVisualStyleBackColor = true;
            this.toLoginScreenButton.Click += new System.EventHandler(this.toLoginScreenButton_Click);
            // 
            // keycardInfoLabel
            // 
            this.keycardInfoLabel.AutoSize = true;
            this.keycardInfoLabel.Location = new System.Drawing.Point(45, 71);
            this.keycardInfoLabel.Name = "keycardInfoLabel";
            this.keycardInfoLabel.Size = new System.Drawing.Size(367, 13);
            this.keycardInfoLabel.TabIndex = 4;
            this.keycardInfoLabel.Text = "You can start using NemID when you recieve your keycard in your snail mail.";
            // 
            // accountCreatedLabel
            // 
            this.accountCreatedLabel.AutoSize = true;
            this.accountCreatedLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.accountCreatedLabel.Location = new System.Drawing.Point(75, 54);
            this.accountCreatedLabel.Name = "accountCreatedLabel";
            this.accountCreatedLabel.Size = new System.Drawing.Size(311, 17);
            this.accountCreatedLabel.TabIndex = 3;
            this.accountCreatedLabel.Text = "NemID user account successfully created!";
            // 
            // NemIdAccountCreationSuccess
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "NemIdAccountCreationSuccess";
            this.Size = new System.Drawing.Size(500, 500);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.successGroupBox.ResumeLayout(false);
            this.successGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.GroupBox successGroupBox;
        private System.Windows.Forms.Button toLoginScreenButton;
        private System.Windows.Forms.Label keycardInfoLabel;
        private System.Windows.Forms.Label accountCreatedLabel;
    }
}
