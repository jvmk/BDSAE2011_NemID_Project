namespace BDSA_User_Creation
{
    partial class UserCreation
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
            this.cprTextBox = new System.Windows.Forms.MaskedTextBox();
            this.passwordConfirmTextBox = new System.Windows.Forms.TextBox();
            this.passwordTextBox = new System.Windows.Forms.TextBox();
            this.cprLabel = new System.Windows.Forms.Label();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.panel1 = new System.Windows.Forms.Panel();
            this.AbortButton = new System.Windows.Forms.Button();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.usernameLabel = new System.Windows.Forms.Label();
            this.passwordLabel1 = new System.Windows.Forms.Label();
            this.passwordLabel2 = new System.Windows.Forms.Label();
            this.usernameTextBox = new System.Windows.Forms.TextBox();
            this.EmailLabel = new System.Windows.Forms.Label();
            this.EmailTextBox = new System.Windows.Forms.TextBox();
            this.CreateUserButton = new System.Windows.Forms.Button();
            this.CreateUserLabel = new System.Windows.Forms.Label();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // cprTextBox
            // 
            this.cprTextBox.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.cprTextBox.Location = new System.Drawing.Point(147, 68);
            this.cprTextBox.Margin = new System.Windows.Forms.Padding(4);
            this.cprTextBox.Mask = "000000-0000";
            this.cprTextBox.Name = "cprTextBox";
            this.cprTextBox.Size = new System.Drawing.Size(313, 22);
            this.cprTextBox.TabIndex = 4;
            this.cprTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // passwordConfirmTextBox
            // 
            this.passwordConfirmTextBox.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.passwordConfirmTextBox.Location = new System.Drawing.Point(147, 174);
            this.passwordConfirmTextBox.Margin = new System.Windows.Forms.Padding(4);
            this.passwordConfirmTextBox.Name = "passwordConfirmTextBox";
            this.passwordConfirmTextBox.Size = new System.Drawing.Size(313, 22);
            this.passwordConfirmTextBox.TabIndex = 7;
            this.passwordConfirmTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.passwordConfirmTextBox.UseSystemPasswordChar = true;
            // 
            // passwordTextBox
            // 
            this.passwordTextBox.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.passwordTextBox.Location = new System.Drawing.Point(147, 121);
            this.passwordTextBox.Margin = new System.Windows.Forms.Padding(4);
            this.passwordTextBox.Name = "passwordTextBox";
            this.passwordTextBox.Size = new System.Drawing.Size(313, 22);
            this.passwordTextBox.TabIndex = 6;
            this.passwordTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.passwordTextBox.UseSystemPasswordChar = true;
            // 
            // cprLabel
            // 
            this.cprLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.cprLabel.AutoSize = true;
            this.cprLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cprLabel.Location = new System.Drawing.Point(47, 70);
            this.cprLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.cprLabel.Name = "cprLabel";
            this.cprLabel.Size = new System.Drawing.Size(48, 18);
            this.cprLabel.TabIndex = 1;
            this.cprLabel.Text = "CPR:";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(4);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.panel1);
            this.splitContainer1.Size = new System.Drawing.Size(671, 571);
            this.splitContainer1.SplitterDistance = 141;
            this.splitContainer1.SplitterWidth = 5;
            this.splitContainer1.TabIndex = 2;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.AbortButton);
            this.panel1.Controls.Add(this.tableLayoutPanel2);
            this.panel1.Controls.Add(this.CreateUserButton);
            this.panel1.Controls.Add(this.CreateUserLabel);
            this.panel1.Location = new System.Drawing.Point(96, 47);
            this.panel1.Margin = new System.Windows.Forms.Padding(4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(477, 366);
            this.panel1.TabIndex = 4;
            // 
            // AbortButton
            // 
            this.AbortButton.Location = new System.Drawing.Point(8, 314);
            this.AbortButton.Margin = new System.Windows.Forms.Padding(4);
            this.AbortButton.Name = "AbortButton";
            this.AbortButton.Size = new System.Drawing.Size(163, 28);
            this.AbortButton.TabIndex = 3;
            this.AbortButton.Text = "ABANDON SHIP!";
            this.AbortButton.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30.65903F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 69.34097F));
            this.tableLayoutPanel2.Controls.Add(this.cprTextBox, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.passwordConfirmTextBox, 1, 3);
            this.tableLayoutPanel2.Controls.Add(this.passwordTextBox, 1, 2);
            this.tableLayoutPanel2.Controls.Add(this.cprLabel, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.usernameLabel, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.passwordLabel1, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.passwordLabel2, 0, 3);
            this.tableLayoutPanel2.Controls.Add(this.usernameTextBox, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.EmailLabel, 0, 4);
            this.tableLayoutPanel2.Controls.Add(this.EmailTextBox, 1, 4);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(8, 25);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(4);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 5;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 37F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(465, 250);
            this.tableLayoutPanel2.TabIndex = 2;
            // 
            // usernameLabel
            // 
            this.usernameLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.usernameLabel.AutoSize = true;
            this.usernameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.usernameLabel.Location = new System.Drawing.Point(26, 17);
            this.usernameLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.usernameLabel.Name = "usernameLabel";
            this.usernameLabel.Size = new System.Drawing.Size(90, 18);
            this.usernameLabel.TabIndex = 0;
            this.usernameLabel.Text = "Username:";
            // 
            // passwordLabel1
            // 
            this.passwordLabel1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.passwordLabel1.AutoSize = true;
            this.passwordLabel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.passwordLabel1.Location = new System.Drawing.Point(27, 123);
            this.passwordLabel1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.passwordLabel1.Name = "passwordLabel1";
            this.passwordLabel1.Size = new System.Drawing.Size(88, 18);
            this.passwordLabel1.TabIndex = 2;
            this.passwordLabel1.Text = "Password:";
            // 
            // passwordLabel2
            // 
            this.passwordLabel2.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.passwordLabel2.AutoSize = true;
            this.passwordLabel2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.passwordLabel2.Location = new System.Drawing.Point(28, 167);
            this.passwordLabel2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.passwordLabel2.Name = "passwordLabel2";
            this.passwordLabel2.Size = new System.Drawing.Size(86, 36);
            this.passwordLabel2.TabIndex = 3;
            this.passwordLabel2.Text = "Confirm password:";
            // 
            // usernameTextBox
            // 
            this.usernameTextBox.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.usernameTextBox.Location = new System.Drawing.Point(147, 15);
            this.usernameTextBox.Margin = new System.Windows.Forms.Padding(4);
            this.usernameTextBox.Name = "usernameTextBox";
            this.usernameTextBox.Size = new System.Drawing.Size(313, 22);
            this.usernameTextBox.TabIndex = 4;
            this.usernameTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // EmailLabel
            // 
            this.EmailLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.EmailLabel.AutoSize = true;
            this.EmailLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.EmailLabel.Location = new System.Drawing.Point(15, 222);
            this.EmailLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.EmailLabel.Name = "EmailLabel";
            this.EmailLabel.Size = new System.Drawing.Size(111, 17);
            this.EmailLabel.TabIndex = 8;
            this.EmailLabel.Text = "Email Address";
            this.EmailLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // EmailTextBox
            // 
            this.EmailTextBox.Location = new System.Drawing.Point(146, 216);
            this.EmailTextBox.Margin = new System.Windows.Forms.Padding(4);
            this.EmailTextBox.Name = "EmailTextBox";
            this.EmailTextBox.Size = new System.Drawing.Size(313, 22);
            this.EmailTextBox.TabIndex = 9;
            // 
            // CreateUserButton
            // 
            this.CreateUserButton.Location = new System.Drawing.Point(331, 314);
            this.CreateUserButton.Margin = new System.Windows.Forms.Padding(4);
            this.CreateUserButton.Name = "CreateUserButton";
            this.CreateUserButton.Size = new System.Drawing.Size(139, 28);
            this.CreateUserButton.TabIndex = 0;
            this.CreateUserButton.Text = "CreateUser";
            this.CreateUserButton.UseVisualStyleBackColor = true;
            this.CreateUserButton.Click += new System.EventHandler(this.CreateUserButton_Click);
            // 
            // CreateUserLabel
            // 
            this.CreateUserLabel.AutoSize = true;
            this.CreateUserLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CreateUserLabel.Location = new System.Drawing.Point(115, 2);
            this.CreateUserLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.CreateUserLabel.Name = "CreateUserLabel";
            this.CreateUserLabel.Size = new System.Drawing.Size(222, 18);
            this.CreateUserLabel.TabIndex = 1;
            this.CreateUserLabel.Text = "Create NemID user account:";
            this.CreateUserLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // UserCreation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(671, 571);
            this.Controls.Add(this.splitContainer1);
            this.Name = "UserCreation";
            this.Text = "UserCreation";
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.MaskedTextBox cprTextBox;
        private System.Windows.Forms.TextBox passwordConfirmTextBox;
        private System.Windows.Forms.TextBox passwordTextBox;
        private System.Windows.Forms.Label cprLabel;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button AbortButton;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label usernameLabel;
        private System.Windows.Forms.Label passwordLabel1;
        private System.Windows.Forms.Label passwordLabel2;
        private System.Windows.Forms.TextBox usernameTextBox;
        private System.Windows.Forms.Label EmailLabel;
        private System.Windows.Forms.TextBox EmailTextBox;
        private System.Windows.Forms.Button CreateUserButton;
        private System.Windows.Forms.Label CreateUserLabel;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
    }
}