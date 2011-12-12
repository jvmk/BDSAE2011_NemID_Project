namespace BDSA_Project_GUI
{
    partial class NemIdEnterKeyValue
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
            this.CancelButton = new System.Windows.Forms.Button();
            this.KeyRequestBox = new System.Windows.Forms.GroupBox();
            this.KeyValueTextBox = new System.Windows.Forms.MaskedTextBox();
            this.KeyIndexLabel = new System.Windows.Forms.Label();
            this.SubmitKeyButton = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.KeyRequestBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // CancelButton
            // 
            this.CancelButton.Location = new System.Drawing.Point(147, 72);
            this.CancelButton.Name = "CancelButton";
            this.CancelButton.Size = new System.Drawing.Size(75, 23);
            this.CancelButton.TabIndex = 2;
            this.CancelButton.Text = "Cancel";
            this.CancelButton.UseVisualStyleBackColor = true;
            this.CancelButton.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // KeyRequestBox
            // 
            this.KeyRequestBox.Controls.Add(this.KeyValueTextBox);
            this.KeyRequestBox.Controls.Add(this.KeyIndexLabel);
            this.KeyRequestBox.Controls.Add(this.CancelButton);
            this.KeyRequestBox.Controls.Add(this.SubmitKeyButton);
            this.KeyRequestBox.Location = new System.Drawing.Point(119, 77);
            this.KeyRequestBox.Name = "KeyRequestBox";
            this.KeyRequestBox.Size = new System.Drawing.Size(249, 113);
            this.KeyRequestBox.TabIndex = 0;
            this.KeyRequestBox.TabStop = false;
            this.KeyRequestBox.Text = "Provide key from keycard";
            // 
            // KeyValueTextBox
            // 
            this.KeyValueTextBox.Location = new System.Drawing.Point(77, 46);
            this.KeyValueTextBox.Mask = "000000";
            this.KeyValueTextBox.Name = "KeyValueTextBox";
            this.KeyValueTextBox.Size = new System.Drawing.Size(100, 20);
            this.KeyValueTextBox.TabIndex = 4;
            this.KeyValueTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // KeyIndexLabel
            // 
            this.KeyIndexLabel.AutoSize = true;
            this.KeyIndexLabel.Location = new System.Drawing.Point(6, 16);
            this.KeyIndexLabel.Name = "KeyIndexLabel";
            this.KeyIndexLabel.Size = new System.Drawing.Size(206, 13);
            this.KeyIndexLabel.TabIndex = 3;
            this.KeyIndexLabel.Text = "Enter key corresponding to keycard index:";
            // 
            // SubmitKeyButton
            // 
            this.SubmitKeyButton.Location = new System.Drawing.Point(28, 72);
            this.SubmitKeyButton.Name = "SubmitKeyButton";
            this.SubmitKeyButton.Size = new System.Drawing.Size(75, 23);
            this.SubmitKeyButton.TabIndex = 1;
            this.SubmitKeyButton.Text = "Submit key";
            this.SubmitKeyButton.UseVisualStyleBackColor = true;
            this.SubmitKeyButton.Click += new System.EventHandler(this.SubmitKeyButton_Click);
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
            this.splitContainer1.Panel2.Controls.Add(this.KeyRequestBox);
            this.splitContainer1.Size = new System.Drawing.Size(500, 500);
            this.splitContainer1.SplitterDistance = 125;
            this.splitContainer1.TabIndex = 1;
            // 
            // NemIdEnterKeyValue
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "NemIdEnterKeyValue";
            this.Size = new System.Drawing.Size(500, 500);
            this.KeyRequestBox.ResumeLayout(false);
            this.KeyRequestBox.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button CancelButton;
        private System.Windows.Forms.GroupBox KeyRequestBox;
        private System.Windows.Forms.Button SubmitKeyButton;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.MaskedTextBox KeyValueTextBox;
        private System.Windows.Forms.Label KeyIndexLabel;
    }
}
