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
    public partial class NemIdNewUserCreation : UserControl
    {
        public NemIdNewUserCreation()
        {
            InitializeComponent();
        }

        private void MenuLoginButton_Click(object sender, EventArgs e)
        {
            this.ParentForm.Controls.Clear();
            this.ParentForm.Controls.Add(new NemIdLogin());
        }

        private void CreateUserButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(usernameTextBox.Text))
            {
                MessageBox.Show("Please specify a username.");
                return;
            }
            if (string.IsNullOrEmpty(cprTextBox.Text))
            {
                MessageBox.Show("Please specify your CPR number.");
                return;
            }
            if (string.IsNullOrEmpty(passwordTextBox.Text) || string.IsNullOrEmpty(passwordConfirmTextBox.Text))
            {
                MessageBox.Show("A password field was left empty, please specify your password in both fields.");
                return;
            }
            if (!passwordTextBox.Text.Equals(passwordConfirmTextBox.Text))
            {
                MessageBox.Show("Supplied passwords differ, please supply the same password in both password fields.");
                return;
            }

            UsersBrowser browser = (UsersBrowser)this.ParentForm;
            bool accountCreationSuccess = browser.AuthProxy.CreateUserAccount(usernameTextBox.Text, passwordTextBox.Text, cprTextBox.Text);
            if (accountCreationSuccess)
            {

            }
            else
            {
                // TODO Handle unsuccessful account creation attempt - need error code from authenticator in order to show proper MessageBox
            }
        }
    }
}
