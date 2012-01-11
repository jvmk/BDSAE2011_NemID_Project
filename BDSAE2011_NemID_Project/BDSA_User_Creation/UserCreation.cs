﻿using System;
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
            string email = EmailTextBox.Text;

            // Set the file path
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                path = folderBrowserDialog1.SelectedPath;
                if (ReferenceEquals(privateKey, null))
                {
                    //Generate the keys for the user
                    privateKey = Cryptograph.GenerateKeys(email);
                    //Thread.Sleep(1000);
                }
            }

            // Save the key to the specified file path
            File.WriteAllBytes(path + "/" + usernameTextBox.Text + "privateKey", privateKey);

            //// Creates the auth proxy and creates an user
            AuthenticatorProxy proxy = new AuthenticatorProxy(StringData.AuthUri, email, privateKey);
            bool creationSuccesfull = proxy.CreateUserAccount(
                usernameTextBox.Text, passwordTextBox.Text, cprTextBox.Text, email);
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
