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
    using System.Drawing;
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Windows.Forms;

    using BDSA_Project_Communication;

    using BDSA_Project_Cryptography;

    /// <summary>
    /// The controller for the UserBrowser class (MVC)
    /// </summary>
    public partial class UsersBrowser : Form
    {

        private AuthenticatorProxy authenticatorProxy;

        private string username = "";

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
                username = UsernameTextbox.Text;

                Console.WriteLine("inserted user name: " + username);

                // Encrypt user name in thirds party's public key.
                string encryptedUsername = Cryptograph.Encrypt(username, Cryptograph.GetPublicKey(StringData.ThirdUri));

                // Convert encrypted text to bytes.
                byte[] buf = Encoding.UTF8.GetBytes(encryptedUsername);

                // Create a request.
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(StringData.ThirdUri + "request=loginpage"); // TODO update to correct https URI

                request.Method = "POST";
                Stream output = request.GetRequestStream();

                // Write the encrypted user name to the request.
                output.Write(buf, 0, buf.Length);
                output.Flush();
                output.Close();

                // Send the request and get the response.
                HttpWebResponse response = null;

                try
                {
                    response = (HttpWebResponse)request.GetResponse();
                }
                catch (WebException)
                {
                    errorMessage.ForeColor = Color.Red;
                    errorMessage.Text = "Username not valid.";
                    return;
                }

                // The response was accepted (the third has the user name in it's database)...
                this.Controls.Clear(); // reset window
                this.Controls.Add(new NemIdCreateAuthProxy(username)); // go to nem id login screen
            }
            // If the user input was not valid...
            else
            {
                // ...print an error message.
                errorMessage.Text = "No value inserted.";
            }
        }

        public void setAuthenticatorProxy(AuthenticatorProxy ap)
        {
            this.authenticatorProxy = ap;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            if (ReferenceEquals(this.authenticatorProxy, null)) return;

            this.authenticatorProxy.Abort(username);
        }
    }
}
