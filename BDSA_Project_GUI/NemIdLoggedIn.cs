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
    public partial class NemIdLoggedIn : UserControl
    {
        /// <summary>
        /// Username of the logged in user.
        /// </summary>
        private string username;

        public NemIdLoggedIn(string username)
        {
            InitializeComponent();
            this.username = username;
        }

        private void ContinueToExternalSiteButton_Click(object sender, EventArgs e)
        {
            // TODO how to handle this?
        }

        private void DeleteAccountButton_Click(object sender, EventArgs e)
        {

        }
    }
}
