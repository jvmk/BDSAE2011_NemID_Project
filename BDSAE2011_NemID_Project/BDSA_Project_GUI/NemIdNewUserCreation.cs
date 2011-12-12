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
    using BDSA_Project_Communication;

    public partial class NemIdNewUserCreation : UserControl
    {
        public NemIdNewUserCreation()
        {
            InitializeComponent();
        }

        private void CreateUserButton_Click(object sender, EventArgs e)
        {
            // Check for valid user inputs.
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

            // Specify path to save the private key.
            string privateKeyPath = folderBrowserDialog1.SelectedPath;

            // Generate the public/private key pair for the user.
            byte[] generatedPrivateKey = BDSA_Project_Cryptography.Cryptograph.GenerateKeys(EmailTextBox.Text);

            // Write the generated private key to the specified path.
            File.WriteAllBytes(privateKeyPath, generatedPrivateKey);

            // Get a new authenticatorProxy...
            AuthenticatorProxy authenticatorProxy =
                new AuthenticatorProxy(StringData.AuthUri, EmailTextBox.Text, generatedPrivateKey);

            // ...and an create the new user at the authenticator.
            bool createdNewAccount = authenticatorProxy.CreateUserAccount(
                usernameTextBox.Text, passwordTextBox.Text, cprTextBox.Text, EmailTextBox.Text);

            if (createdNewAccount)
            {
                this.ParentForm.Controls.Clear();
                this.ParentForm.Controls.Add(new NemIdAccountCreationSuccess());
            }
            else
            {
                // TODO print a text stating unsuccessful account creation.
            }
        }

        private void AbortButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
