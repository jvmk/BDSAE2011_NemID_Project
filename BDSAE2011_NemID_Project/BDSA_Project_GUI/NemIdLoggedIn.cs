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

        private AuthenticatorProxy auth;

        private ThirdPartyHttpGenerator tp;

        public NemIdLoggedIn(AuthenticatorProxy auth, ThirdPartyHttpGenerator tp, string username)
        {
            InitializeComponent();
            this.auth = auth;
            this.tp = tp;
            this.username = username;
        }

        private void ContinueToExternalSiteButton_Click(object sender, EventArgs e)
        {
            bool proceedOk = this.auth.Proceed(this.username);
            if (proceedOk)
            {
                string token = this.auth.GetToken();
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
                this.auth.RevokeUserAccount(this.username);
                Application.Exit();
            }
        }
    }
}
