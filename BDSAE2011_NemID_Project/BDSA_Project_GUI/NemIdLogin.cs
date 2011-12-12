namespace BDSA_Project_GUI
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    using BDSA_Project_Communication;

    using BDSA_Project_ThirdParty;

    public partial class NemIdLogin : UserControl
    {
        private AuthenticatorProxy auth;

        private ThirdPartyHttpGenerator tp;

        private string thirdPartyUsername;
        
        public NemIdLogin(AuthenticatorProxy auth, ThirdPartyHttpGenerator tp, string thirdPartyUsername)
        {
            InitializeComponent();
            this.auth = auth;
            this.tp = tp;
            this.thirdPartyUsername = thirdPartyUsername;
        }

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

            bool loginAccept = this.auth.LogIn(username, password);
            if (loginAccept)
            {
                string keyIndexLabelText = auth.GetKeyIndex();
                this.ParentForm.Controls.Clear();
                this.ParentForm.Controls.Add(new NemIdEnterKeyValue(auth, tp, keyIndexLabelText, username));
            }
            else
            {
                KeyPathLabel.Text = "Username/password combination mismatch.";
                KeyPathLabel.ForeColor = Color.Red;
            }

            //// what is the purpose of this? Getting access to the proxy? need to use something else than auth anyways
            // UsersBrowser browser = (UsersBrowser)this.ParentForm; // Only form in the program is a UsersBrowser, can safely cast.
            // bool loginSuccess = browser.AuthProxy.LogIn(username, password); // send login request
            // TODO: If correct create screen for entering key for specified keyindex value
          
            /* 
            if (loginSuccess)
            {
                this.ParentForm.Controls.Clear();
                this.ParentForm.Controls.Add(new NemIdEnterKeyValue("Enter key corresponding to keycard index "
                    + browser.AuthProxy.GetKeyIndex() + ":", username));
            }
            else
            {
                MessageBox.Show("Incorrect username and password combination.");
            }
             */
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.auth.Abort(this.thirdPartyUsername);
            Application.Exit();
        }
    }
}
