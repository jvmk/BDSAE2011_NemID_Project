using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BDSA_Project_GUI
{
    using System.IO;

    public partial class UserCreation : Form
    {
        public UserCreation()
        {
            InitializeComponent();
        }

        private void CreateUserButton_Click(object sender, EventArgs e)
        {
            string path = string.Empty;
            // Set the file path
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                path = folderBrowserDialog1.SelectedPath;
            }
            
            //Generate the keys for the user
            byte[] privateKey = BDSA_Project_Cryptography.Cryptograph.GenerateKeys(EmailTextBox.Text);

            // Save the key to the specified file path
            if (path == string.Empty)
            {
                Console.Write("The path was not set");
            }
            File.WriteAllBytes(path + usernameTextBox.Text + "privateKey", privateKey);
            

            //// TODO Send the information to the server and store user here
            
            Console.Write("The user has successfully been added and the application is now closing");
            Application.Exit();
            
        }

        private void AbortButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
