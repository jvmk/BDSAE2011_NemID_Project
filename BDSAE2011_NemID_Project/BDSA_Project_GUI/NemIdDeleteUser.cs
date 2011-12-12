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
    public partial class NemIdDeleteUser : UserControl
    {
        /// <summary>
        /// The username of the user that has succesfully logged in.
        /// </summary>
        private string username;
        public NemIdDeleteUser(string username)
        {
            InitializeComponent();
            this.username = username;
            this.usernameLabel.Text = username;
            //TODO Delete CPR from displayed data?
        }

        private void ConfirmDeleteUserButton_Click(object sender, EventArgs e)
        {
            UsersBrowser browser = (UsersBrowser)this.ParentForm; // Control should only be added to a UsersBrowser!
            //bool deleteSuccess = browser.AuthProxy.RevokeUserAccount(username, "some1@email.com");
            //if (deleteSuccess)
            //{
                //this.ParentForm.Controls.Clear();
                //this.ParentForm.Controls.Add(new NemIdLogin());
                //return;
            //}
            //else
            //{
                //TODO something has gone wrong - how should we handle this?
            //}
        }
    }
}
