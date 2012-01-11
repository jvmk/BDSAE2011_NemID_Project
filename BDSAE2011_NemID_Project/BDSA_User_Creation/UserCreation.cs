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
            // Set the file path
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                path = folderBrowserDialog1.SelectedPath;
                if (privateKey.Equals(null))
                {
                    //Generate the keys for the user
                    privateKey = Cryptograph.GenerateKeys(EmailTextBox.Text);
                }
            }

            // Save the key to the specified file path
            if (path == string.Empty)
            {
                Console.Write("The path was not set");
            }
            File.WriteAllBytes(path + usernameTextBox.Text + "privateKey", privateKey);

            //// Creates the auth proxy and creates an user
            AuthenticatorProxy proxy = new AuthenticatorProxy(StringData.AuthUri, EmailTextBox.Text, privateKey);
            bool creationSuccesfull = proxy.CreateUserAccount(
                usernameTextBox.Text, passwordTextBox.Text, cprTextBox.Text, EmailTextBox.Text);
            if (creationSuccesfull)
            {
                Console.Write("The user has successfully been created and the application is now closing");
                Application.Exit();
            }

            else
            {
                Console.WriteLine("Something unexpected went wrong");
            }
        }

        private void AbortButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
