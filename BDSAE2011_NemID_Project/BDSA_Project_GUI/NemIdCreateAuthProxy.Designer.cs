﻿namespace BDSA_Project_GUI
{
    partial class NemIdCreateAuthProxy
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
            this.continueBtn = new System.Windows.Forms.Button();
            this.KeyPathLabel = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.keyPkiIdTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.privKeyPathBtn = new System.Windows.Forms.Button();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
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
            this.splitContainer1.Panel2.Controls.Add(this.continueBtn);
            this.splitContainer1.Panel2.Controls.Add(this.KeyPathLabel);
            this.splitContainer1.Panel2.Controls.Add(this.label3);
            this.splitContainer1.Panel2.Controls.Add(this.keyPkiIdTextBox);
            this.splitContainer1.Panel2.Controls.Add(this.label1);
            this.splitContainer1.Panel2.Controls.Add(this.privKeyPathBtn);
            this.splitContainer1.Panel2.Controls.Add(this.richTextBox1);
            this.splitContainer1.Size = new System.Drawing.Size(667, 615);
            this.splitContainer1.SplitterDistance = 153;
            this.splitContainer1.SplitterWidth = 5;
            this.splitContainer1.TabIndex = 1;
            // 
            // continueBtn
            // 
            this.continueBtn.Location = new System.Drawing.Point(279, 331);
            this.continueBtn.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.continueBtn.Name = "continueBtn";
            this.continueBtn.Size = new System.Drawing.Size(100, 28);
            this.continueBtn.TabIndex = 15;
            this.continueBtn.Text = "Continue";
            this.continueBtn.UseVisualStyleBackColor = true;
            this.continueBtn.Click += new System.EventHandler(this.continueBtn_Click);
            // 
            // KeyPathLabel
            // 
            this.KeyPathLabel.AutoSize = true;
            this.KeyPathLabel.Location = new System.Drawing.Point(147, 286);
            this.KeyPathLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.KeyPathLabel.Name = "KeyPathLabel";
            this.KeyPathLabel.Size = new System.Drawing.Size(0, 17);
            this.KeyPathLabel.TabIndex = 14;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(76, 245);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(194, 17);
            this.label3.TabIndex = 13;
            this.label3.Text = "2. Set key\'s associated email:";
            // 
            // keyPkiIdTextBox
            // 
            this.keyPkiIdTextBox.Location = new System.Drawing.Point(279, 241);
            this.keyPkiIdTextBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.keyPkiIdTextBox.Name = "keyPkiIdTextBox";
            this.keyPkiIdTextBox.Size = new System.Drawing.Size(231, 22);
            this.keyPkiIdTextBox.TabIndex = 12;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(147, 212);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(122, 17);
            this.label1.TabIndex = 11;
            this.label1.Text = "1. Set private key:";
            // 
            // privKeyPathBtn
            // 
            this.privKeyPathBtn.Location = new System.Drawing.Point(332, 206);
            this.privKeyPathBtn.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.privKeyPathBtn.Name = "privKeyPathBtn";
            this.privKeyPathBtn.Size = new System.Drawing.Size(179, 28);
            this.privKeyPathBtn.TabIndex = 10;
            this.privKeyPathBtn.Text = "Set private key path";
            this.privKeyPathBtn.UseVisualStyleBackColor = true;
            this.privKeyPathBtn.Click += new System.EventHandler(this.privKeyPathBtn_Click);
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(151, 74);
            this.richTextBox1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(359, 77);
            this.richTextBox1.TabIndex = 0;
            this.richTextBox1.Text = "In order to communicate with the authenticator, the local program needs  you to p" +
    "rovide some information. Your private key will only be used on your local machin" +
    "e.";
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // NemIdCreateAuthProxy
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "NemIdCreateAuthProxy";
            this.Size = new System.Drawing.Size(667, 615);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox keyPkiIdTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button privKeyPathBtn;
        private System.Windows.Forms.Label KeyPathLabel;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button continueBtn;
    }
}
