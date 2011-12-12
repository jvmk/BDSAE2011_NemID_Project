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
    public partial class NemIdEnterKeyValue : UserControl
    {
        /// <summary>
        /// Username of the user that passed the username+password login step.
        /// </summary>
        private string username;
        public NemIdEnterKeyValue(string keyIndexLabelText, string username)
        {
            InitializeComponent();
            this.KeyIndexLabel.Text = keyIndexLabelText;
            this.username = username;
        }

        private void SubmitKeyButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(KeyValueTextBox.Text))
            {
                MessageBox.Show(
                    "Key value empty, please provide the key value corresponding to the specified keycard index.");
                return;
            }
            UsersBrowser browser = (UsersBrowser)this.ParentForm; // control is held in the UsersBrowser class that inherits from Form.
            bool keyValueValid = browser.AuthProxy.SubmitKey(Convert.ToInt32(KeyValueTextBox.Text), username); // Convert is safe since we are using a MaskedTextBox with a 6digit numeric mask
            if (keyValueValid)
            {
                // TODO Grant entry, redirect directly to ThirdParty or have in-the-middle screen with continue option (provide option to delete user here)?
                this.ParentForm.Controls.Clear();
                this.ParentForm.Controls.Add(new NemIdLoggedIn(username));
            }
            else
            {
                MessageBox.Show("Incorrect key value, please try again.");
            }
        }

        /// <summary>
        /// Cancel authentication.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void CancelButton_Click(object sender, EventArgs e)
        {
            UsersBrowser browser = (UsersBrowser)this.ParentForm; // control is held in the UsersBrowser class that inherits from Form.
            // browser.AuthProxy TODO: Use cancel login method in AuthenticatorProxy (when it is implemented in AuthenticatorProxy)
        }
    }
}
