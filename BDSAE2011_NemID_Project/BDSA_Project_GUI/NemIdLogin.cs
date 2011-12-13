namespace BDSA_Project_GUI
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    using BDSA_Project_Communication;

    using BDSA_Project_ThirdParty;

    public partial class NemIdLogin : UserControl
    {
        /// <summary>
        /// The AuthenticatorProxy instance used during the whole
        /// authentication session for communication between the client and
        /// the authenticator.
        /// </summary>
        private AuthenticatorProxy authenticatorProxy;

        /// <summary>
        /// The ThirdPartyHttpGenerator instance used during the 
        /// authentication session for communication between the client and
        /// the third party.
        /// </summary>
        private ThirdPartyHttpGenerator tp;

        /// <summary>
        /// The user name submitted at the third party login screen.
        /// </summary>
        private string thirdPartyUsername;

        /// <summary>
        /// Initializes a new instance of the NemIdLogin class.
        /// </summary>
        /// <param name="authenticatorProxy">
        /// The authenticatorProxy instance used by the client during 
        /// the whole auhtentication process.
        /// </param>
        /// <param name="thirdParty">
        /// The ThirdPartyHTTPGenerator instance user by the client
        /// during the whole authentication process.
        /// </param>
        /// <param name="thirdPartyUsername">
        /// The user name entered at the third party login page.
        /// </param>
        public NemIdLogin(
            AuthenticatorProxy authenticatorProxy, ThirdPartyHttpGenerator thirdParty, string thirdPartyUsername)
        {
            InitializeComponent();
            this.authenticatorProxy = authenticatorProxy;
            this.tp = thirdParty;
            this.thirdPartyUsername = thirdPartyUsername;
        }

        /// <summary>
        /// When the user clicks the "Submit"-button, this method
        /// is invoked.
        /// </summary>
        /// <param name="sender">
        /// 
        /// </param>
        /// <param name="e"></param>
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

            if (!thirdPartyUsername.Equals(username))
            {
                KeyPathLabel.Text = "The submitted username does not match the one entered at third party.";
                KeyPathLabel.ForeColor = Color.Red;
                return;
            }

            bool loginAccept = this.authenticatorProxy.LogIn(username, password);
            if (loginAccept)
            {
                string keyIndexLabelText = authenticatorProxy.GetKeyIndex();
                Application.OpenForms[0].Controls.Clear();
                Application.OpenForms[0].Controls.Add(new NemIdEnterKeyValue(authenticatorProxy, tp, keyIndexLabelText, username));
            }
            else
            {
                KeyPathLabel.Text = "Username/password combination mismatch.";
                KeyPathLabel.ForeColor = Color.Red;
            }
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.authenticatorProxy.Abort(this.thirdPartyUsername);
            Application.Exit();
        }
    }
}
