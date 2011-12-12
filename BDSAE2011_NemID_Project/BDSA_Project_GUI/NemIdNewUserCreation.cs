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
    public partial class NemIdNewUserCreation : UserControl
    {
        public NemIdNewUserCreation()
        {
            InitializeComponent();
        }

        private void CreateUserButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(usernameTextBox.Text))
            {
                MessageBox.Show("Please specify a username.");
                return;
            }
            if (string.IsNullOrEmpty(cprTextBox.Text))
            {
                MessageBox.Show("Please specify your CPR number.");
                return;
            }
            if (string.IsNullOrEmpty(passwordTextBox.Text) || string.IsNullOrEmpty(passwordConfirmTextBox.Text))
            {
                MessageBox.Show("A password field was left empty, please specify your password in both fields.");
                return;
            }
            if (!passwordTextBox.Text.Equals(passwordConfirmTextBox.Text))
            {
                MessageBox.Show("Supplied passwords differ, please supply the same password in both password fields.");
                return;
            }
            if (string.IsNullOrEmpty(EmailTextBox.Text))
            {
                MessageBox.Show(
                    "Email field was left empty, you need to enter a valid e-mail address in order to create an account");
                return;
            }
            if (!EmailTextBox.Text.Contains("@"))
            {
                MessageBox.Show("You need to enter a valid E-mail");
                return;
            }

            //// Specify path to save the private key
            string result = folderBrowserDialog1.SelectedPath;

            //// Todo write key to file
            //// todo: create user
            ///  todo: generate keys, write private key to file 
            if (succes)
            {
                this.ParentForm.Controls.Clear();
                this.ParentForm.Controls.Add(new NemIdAccountCreationSuccess());
            }
            UsersBrowser browser = (UsersBrowser)this.ParentForm;
            /*bool accountCreationSuccess = browser.AuthProxy.CreateUserAccount(usernameTextBox.Text, passwordTextBox.Text, cprTextBox.Text);
            if (accountCreationSuccess)
            {

            }
            else
            {
                // TODO Handle unsuccessful account creation attempt - need error code from authenticator in order to show proper MessageBox
            }*/
        }

        private void AbortButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
