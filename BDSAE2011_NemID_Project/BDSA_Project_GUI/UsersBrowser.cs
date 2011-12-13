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
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Windows.Forms;

    using BDSA_Project_Communication;

    using BDSA_Project_Cryptography;

    using BDSA_Project_ThirdParty;

    /// <summary>
    /// The controller for the UserBrowser class (MVC)
    /// </summary>
    public partial class UsersBrowser : Form
    {

        // private AuthenticatorProxy authenticator; // TODO add this?

        private static string ThirdPartyMainUri = "http://localhost:8082/";

        /// <summary>
        /// Initializes a new instance of the UsersBrowser class.
        /// This class simulates the user's browser window.
        /// </summary>
        public UsersBrowser()
        {
            InitializeComponent();
        }

        /// <summary>
        /// This method is called, when the login button on the third party
        /// login screen is pressed.
        /// </summary>
        /// <param name="sender">
        /// The sender object
        /// </param>
        /// <param name="e">
        /// Arguments associated with the event.
        /// </param>
        private void danskeBankLoginButton_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(UsernameTextbox.Text)) // Establish connection to authenticator by creating a new AuthenticatorProxy
            {
                // Get entered user name
                string username = UsernameTextbox.Text;

                // Encrypt user name in thirds party's public key.
                string encryptedUsername = Cryptograph.Encrypt(username, PublicKeyInfrastructure.GetKey(ThirdPartyMainUri));

                // Convert encrypted text to bytes.
                byte[] buf = Encoding.UTF8.GetBytes(encryptedUsername);

                // Create a request.
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ThirdPartyMainUri + "request=loginpage"); // TODO update to correct https URI
                request.Method = "POST";
                Stream output = request.GetRequestStream();

                // Write the encrypted user name to the request.
                output.Write(buf, 0, buf.Length);

                // Send the request and get the response.
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                // If the response was accepted (the third has the user name in it's database)...
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    // ...redirect client to authenticator login screen.
                    this.Controls.Clear(); // reset window
                    this.Controls.Add(new NemIdCreateAuthProxy(username)); // go to nem id login screen
                }
                // If the request was not successful...
                else
                {
                    // ...print an error message.
                    errorMessage.Text = "Username not found.";
                }
            }
            // If the user input was not valid...
            else
            {
                // ...print an error message.
                errorMessage.Text = "No value inserted.";
            }
        }
    }
}
