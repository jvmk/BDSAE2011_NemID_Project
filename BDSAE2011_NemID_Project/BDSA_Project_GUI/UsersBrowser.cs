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

        private AuthenticatorProxy authenticator;

        private static string ThirdPartyMainUri = "http://localhost:8082/";

        /// <summary>
        /// Initializes a new instance of the UsersBrowser class.
        /// This class simulates the user's browser window.
        /// </summary>
        public UsersBrowser()
        {
            InitializeComponent();
        }


        private void danskeBankLoginButton_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(UsernameTextbox.Text)) // Establish connection to authenticator by creating a new AuthenticatorProxy
            {
                // Get entered text
                string username = UsernameTextbox.Text;

                // Encrypt text
                string encryptedUsername = Cryptograph.Encrypt(username, PublicKeyInfrastructure.GetKey(ThirdPartyMainUri));

                // convert encrypted text to bytes
                byte[] buf = Encoding.UTF8.GetBytes(encryptedUsername);

                // Get a request
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ThirdPartyMainUri + "request=loginpage"); // TODO update to correct https URI
                request.Method = "POST";
                Stream output = request.GetRequestStream();

                // Write to 
                output.Write(buf, 0, buf.Length);

                // Send the request and get the response
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    // sucessfully redirected to authenticator login
                    this.Controls.Clear(); // reset window
                    this.Controls.Add(new NemIdCreateAuthProxy(username)); // go to nem id login screen
                }
                else
                {
                    errorMessage.Text = "Username not found.";
                }
            }
        }
    }
}
