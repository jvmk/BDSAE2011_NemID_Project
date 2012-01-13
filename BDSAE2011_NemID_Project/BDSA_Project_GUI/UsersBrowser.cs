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
        /// Sets the AuthenticatorProxy-object to the specified one.
        /// </summary>
        /// <param name="ap">
        /// The AuthenticatorProxy-object to be assigned to this instance.
        /// </param>
        public void SetAuthenticatorProxy(AuthenticatorProxy ap)
        {
            this.authenticatorProxy = ap;
        }

        /// <summary>
        /// Called when the window closes.
        /// Aborts the current client session at the authenticator
        /// if the AuthenticatorProxy is set.
        /// </summary>
        /// <param name="e">
        /// Form closing event arguments.
        /// </param>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            if (e.CloseReason != CloseReason.UserClosing)
            {
                return;
            }

            if (ReferenceEquals(this.authenticatorProxy, null))
            {
                return;
            }

            this.authenticatorProxy.Abort(this.username);
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
        private void DanskeBankLoginButton_Click(object sender, EventArgs e)
        {
            // Establish connection to authenticator by creating a new AuthenticatorProxy
            if (!string.IsNullOrEmpty(UsernameTextbox.Text))
            {
                // Get entered user name
                username = UsernameTextbox.Text;

                Console.WriteLine("Client inserted user name: " + username);

                Console.WriteLine("Client encrypts the request message");

                // Encrypt user name in thirds party's public key.
                string encryptedUsername = Cryptograph.Encrypt(username, Cryptograph.GetPublicKey(StringData.ThirdUri));

                // Convert encrypted text to bytes.
                byte[] buf = Encoding.UTF8.GetBytes(encryptedUsername);

                // Create a request.
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(StringData.ThirdUri + "request=loginpage");

                request.Method = "POST";
                Stream output = request.GetRequestStream();

                Console.WriteLine("Client sends message to " + StringData.ThirdUri
                    + ", url: " + StringData.ThirdUri + "request=loginpage");

                // Write the encrypted user name to the request.
                output.Write(buf, 0, buf.Length);
                output.Flush();
                output.Close();

                HttpWebResponse response = null;

                try
                {
                    // Get the response.
                    response = (HttpWebResponse)request.GetResponse();
                }
                catch (WebException)
                {
                    errorMessage.ForeColor = Color.Red;
                    errorMessage.Text = "Username not valid.";
                    return;
                }

                Console.WriteLine("Client received response from " + response.ResponseUri);

                // The response was accepted (the third has the user name in it's database)...
                this.Controls.Clear(); // Reset window

                // Go to nem id login screen
                this.Controls.Add(new NemIdCreateAuthProxy(username));
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
