namespace BDSA_User_Creation
{
    partial class UserCreationSuccess
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
            this.button1 = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.successGroupBox = new System.Windows.Forms.GroupBox();
            this.keycardInfoLabel = new System.Windows.Forms.Label();
            this.accountCreatedLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.successGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(240, 148);
            this.button1.Margin = new System.Windows.Forms.Padding(4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(121, 28);
            this.button1.TabIndex = 5;
            this.button1.Text = "Close browser";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Location = new System.Drawing.Point(8, 8);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(4);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.BackgroundImage = global::BDSA_User_Creation.Properties.Resources.nemidlogo;
            this.splitContainer1.Panel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.successGroupBox);
            this.splitContainer1.Size = new System.Drawing.Size(667, 615);
            this.splitContainer1.SplitterDistance = 153;
            this.splitContainer1.SplitterWidth = 5;
            this.splitContainer1.TabIndex = 2;
            // 
            // successGroupBox
            // 
            this.successGroupBox.Controls.Add(this.button1);
            this.successGroupBox.Controls.Add(this.keycardInfoLabel);
            this.successGroupBox.Controls.Add(this.accountCreatedLabel);
            this.successGroupBox.Location = new System.Drawing.Point(19, 28);
            this.successGroupBox.Margin = new System.Windows.Forms.Padding(4);
            this.successGroupBox.Name = "successGroupBox";
            this.successGroupBox.Padding = new System.Windows.Forms.Padding(4);
            this.successGroupBox.Size = new System.Drawing.Size(608, 213);
            this.successGroupBox.TabIndex = 3;
            this.successGroupBox.TabStop = false;
            this.successGroupBox.Text = "Success!";
            // 
            // keycardInfoLabel
            // 
            this.keycardInfoLabel.AutoSize = true;
            this.keycardInfoLabel.Location = new System.Drawing.Point(60, 87);
            this.keycardInfoLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.keycardInfoLabel.Name = "keycardInfoLabel";
            this.keycardInfoLabel.Size = new System.Drawing.Size(489, 17);
            this.keycardInfoLabel.TabIndex = 4;
            this.keycardInfoLabel.Text = "You can start using NemID when you recieve your keycard in your snail mail.";
            // 
            // accountCreatedLabel
            // 
            this.accountCreatedLabel.AutoSize = true;
            this.accountCreatedLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.accountCreatedLabel.Location = new System.Drawing.Point(100, 66);
            this.accountCreatedLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.accountCreatedLabel.Name = "accountCreatedLabel";
            this.accountCreatedLabel.Size = new System.Drawing.Size(367, 20);
            this.accountCreatedLabel.TabIndex = 3;
            this.accountCreatedLabel.Text = "NemID user account successfully created!";
            // 
            // UserCreationSuccess
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "UserCreationSuccess";
            this.Size = new System.Drawing.Size(689, 616);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.successGroupBox.ResumeLayout(false);
            this.successGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.GroupBox successGroupBox;
        private System.Windows.Forms.Label keycardInfoLabel;
        private System.Windows.Forms.Label accountCreatedLabel;


    }
}
