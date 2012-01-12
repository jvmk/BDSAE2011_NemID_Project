using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BDSA_User_Creation
{
    using System.IO;
    using System.Threading;

    using BDSA_Project_Communication;

    using BDSA_Project_Cryptography;

    public partial class UserCreation : Form
    {
        public UserCreation()
        {
            InitializeComponent();
        }

        private void CreateUserButton_Click(object sender, EventArgs e)
        {
            string path = string.Empty;
            byte[] privateKey = null;
            string userName = usernameTextBox.Text;
            string cprno = cprTextBox.Text;
            string password1 = passwordTextBox.Text;
            string password2 = passwordConfirmTextBox.Text;
            string email = EmailTextBox.Text;

            // Check if there is inserted a user name.
            if (string.IsNullOrEmpty(userName))
            {
                ErrorMessageLabel.ForeColor = Color.Red;
                ErrorMessageLabel.Text = "No user name inserted";
                return;
            }

            if (cprno.Length != "112233-1122".Length)
            {
                ErrorMessageLabel.ForeColor = Color.Red;
                ErrorMessageLabel.Text = "Format of cpr. no. is not correct";
                return;
            }

            // Check if there is inserted characters in the two password fields.
            if (string.IsNullOrEmpty(password1) || string.IsNullOrEmpty(password2))
            {
                ErrorMessageLabel.ForeColor = Color.Red;
                ErrorMessageLabel.Text = "One or both of the password fields are empty.";
                return;
            }

            // Check if the two passwords are identical.
            if (!password1.Equals(password2))
            {
                ErrorMessageLabel.ForeColor = Color.Red;
                ErrorMessageLabel.Text = "The passwords are not identical.";
                return;
            }

            // Check if an email field is empty
            if (string.IsNullOrEmpty(email))
            {
                ErrorMessageLabel.ForeColor = Color.Red;
                ErrorMessageLabel.Text = "The email address field is empty.";
                return;
            }

            // Check if the email address already has been added
            /*if (Cryptograph.KeyExists(email))
            {
                ErrorMessageLabel.ForeColor = Color.Red;
                ErrorMessageLabel.Text = "A public key is already registered with the email";
            }
            */

            // Set the file path
            // TODO: Add an explanation.. Specify the location you want to save your private key file to
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                if (Cryptograph.KeyExists(email))
                {
                    ErrorMessageLabel.ForeColor = Color.Red;
                    ErrorMessageLabel.Text = "A public key is already registered with the email";
                    return;
                }
                path = folderBrowserDialog1.SelectedPath;
                if (ReferenceEquals(privateKey, null))
                {
                    // Generate the keys for the user
                    ErrorMessageLabel.ForeColor = Color.Green;
                    ErrorMessageLabel.Text = "Generating keys, please wait a moment...";
                    ErrorMessageLabel.Refresh();
                    privateKey = Cryptograph.GenerateKeys(email);
                }
            }

            // Save the key to the specified file path
            File.WriteAllBytes(path + "/" + userName + "privateKey.bin", privateKey);

            //// Creates the auth proxy and creates an user
            AuthenticatorProxy proxy = new AuthenticatorProxy(StringData.AuthUri, email, privateKey);
            bool creationSuccesfull = proxy.CreateUserAccount(
                userName, password1, cprno, email);
            if (creationSuccesfull)
            {
                Application.OpenForms[0].Controls.Clear();
                Application.OpenForms[0].Controls.Add(new UserCreationSuccess());
            }
            else
            {
                ErrorMessageLabel.ForeColor = Color.Red;
                ErrorMessageLabel.Text = "Creation of account failed at the authenticator.\n" +
                    "This might be that the requested user name already exists.";
                return;
            }
        }

        private void AbortButton_Click_1(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
