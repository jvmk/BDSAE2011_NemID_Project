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
    using BDSA_Project_Communication;

    using BDSA_Project_ThirdParty;

    public partial class NemIdLoggedIn : UserControl
    {
        /// <summary>
        /// Username of the logged in user.
        /// </summary>
        private string username;

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
        private ThirdPartyHttpGenerator thirdPartyProxy;

        /// <summary>
        /// Initializes a new instance of the NemIdLoggedIn class.
        /// </summary>
        /// <param name="authenticatorProxy">
        /// The authenticatorProxy instance used by the client during 
        /// the whole auhtentication process.
        /// </param>
        /// <param name="thirdPartyProxy">
        /// The ThirdPartyHTTPGenerator instance user by the client
        /// during the whole authentication process.
        /// </param>
        /// <param name="username">
        /// The user name used of the autentication session.
        /// </param>
        public NemIdLoggedIn(
            AuthenticatorProxy authenticatorProxy, ThirdPartyHttpGenerator thirdPartyProxy, string username)
        {
            InitializeComponent();
            this.authenticatorProxy = authenticatorProxy;
            this.thirdPartyProxy = thirdPartyProxy;
            this.username = username;
        }

        private void ContinueToExternalSiteButton_Click(object sender, EventArgs e)
        {
            bool proceedOk = this.authenticatorProxy.Proceed(this.username);
            if (proceedOk)
            {
                string token = this.authenticatorProxy.GetToken();
                bool authWithTpOk = tp.SubmitUserToken(username, token);
                if (authWithTpOk)
                {
                    this.ParentForm.Controls.Clear();
                    this.ParentForm.Controls.Add(new AuthedWithTp(username));
                }
                else
                {
                    label1.Text = "An error has occured, you must start over your login attempt.";
                }
            }
        }

        private void DeleteAccountButton_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Delete user?", "Delete user", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                this.authenticatorProxy.RevokeUserAccount(this.username);
                Application.Exit();
            }
        }
    }
}
