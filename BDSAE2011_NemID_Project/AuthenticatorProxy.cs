// -----------------------------------------------------------------------
// <copyright file="AuthenticatorProxy.cs" company="">
// TODO: Update copyright text.
// </copyright>
// ----------------------------------------------------------------------

namespace BDSA_Project_Communication
{
    using System;
    using System.Diagnostics.Contracts;

    using BDSA_Project_Cryptography;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class AuthenticatorProxy
    {
        /// <summary>
        /// Socket representing a connection between the client
        /// and the authenticator.
        /// </summary>
        private readonly ClientSocket socket;

        /// <summary>
        /// string representation of the most recently received and
        /// processed server response.
        /// </summary>
        private Response currentServerResponse = default(Response);

        /// <summary>
        /// Initializes a new instance of the AuthenticatorProxy class.
        /// </summary>
        /// <param name="serverDomain">
        /// The domain of the authenticator.
        /// </param>
        /// <param name="clientIdentifier">
        /// The PKI-identifier of the client using the proxy.
        /// </param>
        /// <param name="clientPrivateKey">
        /// The private key of the client.
        /// </param>
        public AuthenticatorProxy(string serverDomain, string clientIdentifier, byte[] clientPrivateKey)
        {
            Contract.Requires(MessageProcessingUtility.IsValidUrl(serverDomain));
            Contract.Requires(Cryptograph.KeyExists(clientIdentifier));
            Contract.Requires(Cryptograph.CheckConsistency(clientPrivateKey, clientIdentifier));

            this.socket = new ClientSocket(serverDomain, clientIdentifier, clientPrivateKey);
        }

        /// <summary>
        /// Requests creation of new user account at the authentication
        /// service with the specified properties.
        /// </summary>
        /// <param name="userName">
        /// The suggested user name.
        /// </param>
        /// <param name="password">
        /// The suggested password.
        /// </param>
        /// <param name="cprNumber">
        /// The CPR-number of the resident requesting a new account.
        /// </param>
        /// <returns>
        /// True if the creation was successful at the authenticator,
        /// false otherwise.
        /// </returns>
        public bool CreateUserAccount(
            string userName, string password, string cprNumber, string email)
        {
            Contract.Requires(userName != null);
            Contract.Requires(password != null);
            Contract.Requires(cprNumber != null);
            Contract.Requires(email != null);

            Console.WriteLine("Client initiates createUserAccount-request to authenticator.");

            this.socket.SendMessage(
                "createAccount",
                "username=" + userName + "&password=" + password +
                "&cprnumber=" + cprNumber + "&email=" + email);
            this.currentServerResponse = this.socket.ReadMessage();

            Console.WriteLine("The createUserAccount-request at the authenticator was accepted: " +
                currentServerResponse.Accepted);

            return this.currentServerResponse.Accepted;
        }

        /// <summary>
        /// Submits log-in request to the authentication-server.
        /// </summary>
        /// <param name="userName">
        /// Client-submitted and encrypted user name.
        /// </param>
        /// <param name="password">
        /// Client-submitted and encrypted user password.
        /// </param>
        /// <returns>
        /// True if the submitted combination of user name and
        /// user password was accepted by the server, false otherwise.
        /// </returns>
        public bool LogIn(string userName, string password)
        {
            Contract.Requires(userName != null);
            Contract.Requires(password != null);

            Console.WriteLine("Client initiates login-request to authenticator.");

            this.socket.SendMessage(
                "login",
                "userName=" + userName + ":" + "password=" + password);
            this.currentServerResponse = this.socket.ReadMessage();

            Console.WriteLine("The log-in request at the authenticator was accepted: " +
                currentServerResponse.Accepted);

            return this.currentServerResponse.Accepted;
        }

        /// <summary>
        /// When the user submitted an accepted combination of userId
        /// and password to the authentication server, this method
        /// returns the keyIndex from the users key card which 
        /// corresponding key the authentication server is expecting.
        /// </summary>
        /// <returns>
        /// A string representation of the key index value, encrypted.
        /// </returns>
        public string GetKeyIndex()
        {
            return this.GetReturnValueOf(this.currentServerResponse);
        }

        /// <summary>
        /// Submits the specified keyValue from the client's
        /// key card to the authentication server.
        /// </summary>
        /// <param name="keyValue">
        /// The encrypted key-value submitted by the user.
        /// </param>
        /// <param name="userName">
        /// The encrypted user name of the user.
        /// </param> 
        /// <returns>
        /// Returns true if the value of the submitted key is
        /// what the authentication server expected, false otherwise.
        /// </returns>
        public bool SubmitKey(string keyValue, string userName)
        {
            Contract.Requires(keyValue != null);
            Contract.Requires(userName != null);

            Console.WriteLine("Client initiates subitKey-request to authenticator.");

            this.socket.SendMessage(
                "submitKey",
                "keyValue=" + keyValue + "&" + "userName=" + userName);
            this.currentServerResponse = this.socket.ReadMessage();

            Console.WriteLine("The submitKey-request at the authenticator was accepted: " +
                currentServerResponse.Accepted);

            return this.currentServerResponse.Accepted;
        }

        /// <summary>
        /// Submits to the server, that the user wishes to proceed
        /// to the third party after having logged in and submitted
        /// a key value.
        /// </summary>
        /// <param name="userName">
        /// The client's user name that wants to proceed to third
        /// party.
        /// </param>
        /// <returns>
        /// True if the client can proceed, false otherwise.
        /// </returns>
        public bool Proceed(string userName)
        {
            Contract.Requires(userName != null);

            this.socket.SendMessage(
                "proceed",
                "userName=" + userName);
            this.currentServerResponse = this.socket.ReadMessage();
            return this.currentServerResponse.Accepted;
        }

        /// <summary>
        /// Gets the the shared secret between the client and the third
        /// party, for which the client is being authenticated.
        /// When the user has proceeded via Proceed, this method can be
        /// called.
        /// </summary>
        /// <returns>
        /// A string representation of the shared secret.
        /// </returns>
        public string GetToken()
        {
            return this.GetReturnValueOf(this.currentServerResponse);
        }

        /// <summary>
        /// Aborts the current authentication session at the authenticator.
        /// </summary>
        /// <param name="userName">
        /// The user name of the client that wish to abort the session.
        /// Only the user name of the the client using this proxy can
        /// be passed.
        /// </param>
        /// <returns>
        /// True if the abortion was accepted, false otherwise.
        /// </returns>
        public bool Abort(string userName)
        {
            Contract.Requires(userName != null);

            this.socket.SendMessage(
                "abort",
                "userName=" + userName);
            this.currentServerResponse = this.socket.ReadMessage();
            return this.currentServerResponse.Accepted;
        }

        /// <summary>
        /// Requests revocation of the user account specified
        /// by the user name. The client must have gone through the login
        /// procedure before this method can be invoked.
        /// </summary>
        /// <param name="userName">
        /// The user name of the account that is wished to be revoked.
        /// </param>
        /// <returns>
        /// True if the revokation was succesful at the authenticator,
        /// false otherwise.
        /// </returns>
        public bool RevokeUserAccount(string userName)
        {
            Contract.Requires(userName != null);

            this.socket.SendMessage(
                "revokeAccount",
                "userName=" + userName);
            this.currentServerResponse = this.socket.ReadMessage();
            return this.currentServerResponse.Accepted;
        }

        /// <summary>
        /// Processes the specified string representation of a 
        /// http message.
        /// </summary>
        /// <param name="response">
        /// The response containing a return value.
        /// </param>
        /// <returns>
        /// A string constituting the return values of the authentication
        /// server.
        /// </returns>
        private string GetReturnValueOf(Response response)
        {
            Contract.Requires(response.Accepted);

            // Extract return value
            int start = response.ReturnValue.IndexOf('=') + 1;
            int end = response.ReturnValue.Length;

            return response.ReturnValue.Substring(start, end - start);
        }
    }
}
