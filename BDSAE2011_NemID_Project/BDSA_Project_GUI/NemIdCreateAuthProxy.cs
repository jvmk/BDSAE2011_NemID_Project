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

    using BDSA_Project_Cryptography;

    using BDSA_Project_ThirdParty;

    public partial class NemIdCreateAuthProxy : UserControl
    {
        private byte[] privateKey;

        private AuthenticatorProxy auth;

        private ThirdPartyHttpGenerator tp;

        private string username;

        public NemIdCreateAuthProxy(string username)
        {
            InitializeComponent();
            this.username = username;
        }

        private void privKeyPathBtn_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog1.ShowDialog();
            {
                if (result == DialogResult.OK)
                {
                    //// Will the keypath + filename be right?
                    if (openFileDialog1.CheckFileExists && openFileDialog1.CheckPathExists)
                    {
                        string privateKeyPath = openFileDialog1.InitialDirectory + openFileDialog1.FileName;

                        privateKey = File.ReadAllBytes(privateKeyPath);

                        KeyPathLabel.Text = privateKeyPath;

                        KeyPathLabel.ForeColor = Color.Green;

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
        }

        private void continueBtn_Click(object sender, EventArgs e)
        {
            if (!ReferenceEquals(privateKey, null))
            {
                if (Cryptograph.CheckConsistency(privateKey, keyPkiIdTextBox.Text))
                {
                    this.auth = new AuthenticatorProxy(StringData.AuthUri, keyPkiIdTextBox.Text, privateKey);
                    this.tp = new ThirdPartyHttpGenerator(StringData.ThirdUri, keyPkiIdTextBox.Text, privateKey);
                    this.ParentForm.Controls.Clear();
                    this.ParentForm.Controls.Add(new NemIdLogin(this.auth, this.tp, username));
                }
                else
                {
                    KeyPathLabel.Text = "Mismatch between key and key email identifier.";
                }
            }
            else
            {
                KeyPathLabel.Text = "Private key not selected.";
                KeyPathLabel.ForeColor = Color.Red;
            }
        }
    }
}
