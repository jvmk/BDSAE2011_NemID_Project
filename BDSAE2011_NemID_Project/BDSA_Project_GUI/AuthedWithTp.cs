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
    public partial class AuthedWithTp : UserControl
    {

        public AuthedWithTp(string usernameToWelcome)
        {
            InitializeComponent();
            this.welcomeLabel.Text = welcomeLabel.Text + " " + usernameToWelcome;
        }
    }
}
