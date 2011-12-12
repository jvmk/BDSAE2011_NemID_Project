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
    using System.Threading;
    using System.Windows.Forms;

    using AuthenticatorComponent;

    using BDSA_Project_GUI;

    using ClientComponent;

    /// <summary>
    /// The controller for the UserBrowser class (MVC)
    /// </summary>
    public partial class UsersBrowser : Form
    {
    
        /// <summary>
        /// The proxy object that is used when the user starts communicating with the authenticator.
        /// Serves as the link to the underlying Model (MVC).
        /// </summary>
        private AuthenticatorProxy authProxy;

        /// <summary>
        /// Start the authenticator server! TODO Should not be instantiated here!
        /// </summary>
        private AuthenticatorSocket authenticator = new AuthenticatorSocket("https://localhost:8081"); // TODO agree on ip

        /// <summary>
        /// Initializes a new instance of the UsersBrowser class.
        /// This class simulates the user's browser window.
        /// </summary>
        public UsersBrowser()
        {
            //Thread t = new Thread(StartServer);
            //t.Start();
            InitializeComponent();
            authenticator.Start();
            // TODO Establish connection to ThirdParty here!!!
        }

        /// <summary>
        /// Allow internal user controls to access the authenticator proxy instance of this form.
        /// </summary>
        internal AuthenticatorProxy AuthProxy
        {
            get { return authProxy; }
        }

        private void danskeBankLoginButton_Click(object sender, EventArgs e)
        {
            // Establish connection to authenticator by creating a new AuthenticatorProxy
            // TODO: Query the third party for login, recieve response containing redirection info from the ThirdParty
            // TODO: Use redirection info to create the AuthenticatorProxy (arguments).
            this.authProxy = new AuthenticatorProxy("https://localhost:8081/", "dummy"); // TODO: Update serverName argument when SSL is established.
            Console.WriteLine("user created:" + authProxy.CreateUserAccount("dummyUser", "hello", "909090-9090"));
            Console.WriteLine("authProxy created!");
            this.Controls.Clear(); // reset window
            this.Controls.Add(new NemIdLogin()); // go to nem id login screen
        }
    }
}
