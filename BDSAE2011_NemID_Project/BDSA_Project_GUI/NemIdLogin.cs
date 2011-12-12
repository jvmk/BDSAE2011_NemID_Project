using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BDSA_Project_GUI
{
    using System.IO;

    public partial class NemIdLogin : UserControl
    {
        public NemIdLogin()
        {
            InitializeComponent();
        }

        private void menuLoginButton_Click(object sender, EventArgs e)
        {
            // go to the nemid login screen - TODO simulate https here maybe not nessescary?
            this.ParentForm.Controls.Clear();
            this.ParentForm.Controls.Add(new NemIdLogin());
        }

        private void menuNewUserButton_Click(object sender, EventArgs e)
        {
            // go to new user page - TODO simulate https here maybe not nessescary?
            this.ParentForm.Controls.Clear();
            this.ParentForm.Controls.Add(new NemIdNewUserCreation());
        }

        private void authButton_Click_1(object sender, EventArgs e)
        {
            // Guard against empty input
            if (string.IsNullOrEmpty(this.usernameTextBox.Text))
            {
                MessageBox.Show("Username left blank, please enter your username.");
                return;
            }
            if (string.IsNullOrEmpty(this.passwordTextBox.Text))
            {
                MessageBox.Show("Password left empty, please enter your password.");
                return;
            }
            // Extract username and password
            string username = this.usernameTextBox.Text;
            string password = this.passwordTextBox.Text;

            //// what is the purpose of this? Getting access to the proxy? need to use something else than auth anyways
            // UsersBrowser browser = (UsersBrowser)this.ParentForm; // Only form in the program is a UsersBrowser, can safely cast.
            // bool loginSuccess = browser.AuthProxy.LogIn(username, password); // send login request
            // TODO: If correct create screen for entering key for specified keyindex value

            DialogResult result = openFileDialog1.ShowDialog();
            {
                if (result == DialogResult.OK)
                {
                    //// Will the keypath + filename be right?
                    if (openFileDialog1.CheckFileExists && openFileDialog1.CheckPathExists)
                    {
                        string privateKeyPath = openFileDialog1.InitialDirectory + openFileDialog1.FileName;

                        byte[] privateKey = File.ReadAllBytes(privateKeyPath);

                        KeyPathLabel.Text = privateKeyPath;

                        //// TODO: Do additional stuff here, use the keys and such.
                        //// Maybe check if valid private key signature? create new isValidPrivateKey in crypto? Then save as field? 
                    }
                    else
                    {
                        KeyPathLabel.Text = "No private key selected";
                        KeyPathLabel.ForeColor = Color.Red;
                    }
                }
            }

            /* 
            if (loginSuccess)
            {
                this.ParentForm.Controls.Clear();
                this.ParentForm.Controls.Add(new NemIdEnterKeyValue("Enter key corresponding to keycard index "
                    + browser.AuthProxy.GetKeyIndex() + ":", username));
            }
            else
            {
                MessageBox.Show("Incorrect username and password combination.");
            }
             */
        }
    }
}
