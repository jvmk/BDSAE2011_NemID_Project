using System;
using System.Windows.Forms;

namespace BDSA_Project_GUI
{
    using System.Drawing;

    using BDSA_Project_Communication;

    using BDSA_Project_ThirdParty;

    public partial class NemIdEnterKeyValue : UserControl
    {
        /// <summary>
        /// Username of the user that passed the username+password login step.
        /// </summary>
        private readonly string username;

        private ThirdPartyHttpGenerator tp;

        private AuthenticatorProxy auth;

        public NemIdEnterKeyValue(AuthenticatorProxy auth, ThirdPartyHttpGenerator tp, string keyIndexLabelText, string username)
        {
            InitializeComponent();
            this.auth = auth;
            this.tp = tp;
            this.KeyIndex.Text = keyIndexLabelText;
            this.username = username;
        }

        /// <summary>
        /// The user submits the keyInfo
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SubmitKeyButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(KeyValueTextBox.Text))
            {
                errorLabel.Text = "Please enter a key value.";
                errorLabel.ForeColor = Color.Red;
                return;
            }

            bool validKey = this.auth.SubmitKey(this.KeyValueTextBox.Text, this.username);

            if (validKey)
            {
                this.ParentForm.Controls.Clear();
                this.ParentForm.Controls.Add(new NemIdLoggedIn(auth, tp, username));
            }
            else
            {
                errorLabel.Text = "Incorrect key value.";
            }
        }

        /// <summary>
        /// Cancel authentication.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.auth.Abort(this.username);
            Application.Exit();
        }
    }
}
