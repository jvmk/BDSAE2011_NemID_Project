// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UsersBrowser.cs" company="">
//   // TODO Update header
// </copyright>
// <summary>
//   Defines the UsersBrowser type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BDSA_Project_GUI
{
    using System;
    using System.Windows.Forms;

    /// <summary>
    /// The controller for the UserBrowser class (MVC)
    /// </summary>
    public partial class UsersBrowser : Form
    {
    



        /// <summary>
        /// Start the authenticator server! TODO Should not be instantiated here!
        /// </summary>
        /// private AuthenticatorSocket authenticator = new AuthenticatorSocket("https://localhost:8081", privateKey); // TODO agree on ip

        /// <summary>
        /// Initializes a new instance of the UsersBrowser class.
        /// This class simulates the user's browser window.
        /// </summary>
        public UsersBrowser()
        {
            
            InitializeComponent();
            
            // TODO Establish connection to Authenticator here?
        }


        private void danskeBankLoginButton_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(UsernameTextbox.Text)) // Establish connection to authenticator by creating a new AuthenticatorProxy
                // TODO: Query the third party for login, recieve response containing redirection info from the ThirdParty
            // TODO: Use redirection info to create the AuthenticatorProxy (arguments).

            this.Controls.Clear(); // reset window
            //// TODO only go to nem id login screen, if everything is alright.
            
            this.Controls.Add(new NemIdLogin()); // go to nem id login screen
        }
    }
}
