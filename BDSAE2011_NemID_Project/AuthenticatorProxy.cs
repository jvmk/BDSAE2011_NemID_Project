// -----------------------------------------------------------------------
// <copyright file="AuthenticatorProxy.cs" company="">
// TODO: Update copyright text.
// </copyright>
// ----------------------------------------------------------------------

namespace ClientComponent
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

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
        /// The server domain of the server the client is connected to.
        /// </summary>
        private readonly string serverDomain;

        /// <summary>
        /// string representation of the most recently received and
        /// processed server response.
        /// </summary>
        private Response currentServerResponse = default(Response);


        private byte[] clientPrivateKey;
        /// <summary>
        /// Initializes a new instance of the AuthenticatorProxy class.
        /// </summary>
        /// <param name="serverDomain">
        /// The domain of the authenticator.
        /// </param>
        /// <param name="port">
        /// The port the authenticator is listening to requests from.
        /// </param>
        /// <param name="serverName">
        /// The authenticator's name as it appears on it certificate.
        /// </param>
        //TODO servername must be removed.

        public AuthenticatorProxy(string serverDomain, string serverName, byte[] clientPrivateKey)
        {
            this.serverDomain = serverDomain;
            this.socket = new ClientSocket(serverDomain, serverName, clientPrivateKey);
        }

        /// <summary>
        /// Requests creation of new user account at the authentication
        /// service with the specified properties.
        /// </summary>
        /// <param name="encUserName">
        /// The suggested user name, encrypted.
        /// </param>
        /// <param name="encPassword">
        /// The suggested password, encrypted.
        /// </param>
        /// <param name="encCprNumber">
        /// The CPR-number of the resident requesting a new account.
        /// </param>
        /// <returns>
        /// True if the creation was successful at the authenticator,
        /// false otherwise.
        /// </returns>
        public bool CreateUserAccount(string encUserName, string encPassword, string encCprNumber)
        {
            this.socket.SendMessage(
                "createAccount",
                "username=" + encUserName + "&password=" + encPassword);
            this.currentServerResponse = this.socket.ReadMessage();
            return this.currentServerResponse.Accepted;
        }

        /// <summary>
        /// Submits log-in request to the authentication-server.
        /// </summary>
        /// <param name="encUserName">
        /// Client-submitted and encrypted user name.
        /// </param>
        /// <param name="encPassword">
        /// Client-submitted and encrypted user password.
        /// </param>
        /// <returns>
        /// True if the submitted combination of user name and
        /// user password was accepted by the server, false otherwise.
        /// </returns>
        public bool LogIn(string encUserName, string encPassword)
        {
            this.socket.SendMessage(
                "login",
                "userName=" + encUserName + ":" + "password=" + encPassword);
            this.currentServerResponse = this.socket.ReadMessage();
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
            return this.ProcessMessageBodyOf(this.currentServerResponse); // TODO process response.
        }

        /// <summary>
        /// Submits the specified keyValue from the client's
        /// key card to the authentication server.
        /// </summary>
        /// <param name="encKeyValue">
        /// The encrypted key-value submitted by the user.
        /// </param>
        /// <param name="encUserName">
        /// The encrypted user name of the user.
        /// </param> 
        /// <returns>
        /// Returns true if the value of the submitted key is
        /// what the authentication server expected, false otherwise.
        /// </returns>
        public bool SubmitKey(int encKeyValue, string encUserName)
        {
            this.socket.SendMessage(
                "submitKey",
                "keyValue=" + encKeyValue + "&" + "userName=" + encUserName);
            this.currentServerResponse = this.socket.ReadMessage();
            return this.currentServerResponse.Accepted;
        }

        public string GetToken()
        {
            return this.currentServerResponse.ReturnValue; //TODO process response.
        }

        /// <summary>
        /// Requests revocation of the user account specified
        /// by the user name. The client must have gone through the login
        /// procedure before this method can be invoked.
        /// </summary>
        /// <param name="encUserName">
        /// The user name of the account that is wished to be revoked,
        /// encrypted.
        /// </param>
        /// <returns>
        /// True if the revokation was succesful at the authenticator,
        /// false otherwise.
        /// </returns>
        public bool RevokeUserAccount(string encUserName, string pkiIdentifier)
        {
            this.socket.SendMessage(
                "revokeAccount",
                "userName=" + encUserName);
            this.currentServerResponse = this.socket.ReadMessage();
            return this.currentServerResponse.Accepted;
        }

        /// <summary>
        /// Processes the specified string representation of a 
        /// http message.
        /// </summary>
        /// <param name="messageBody">
        /// Raw string representation of the http response message.
        /// </param>
        /// <returns>
        /// A string constitution the return values of the authentication
        /// server.
        /// </returns>
        private string ProcessMessageBodyOf(Response response)
        {
            // Extract return value
            string encResponse = string.Empty;

            if (response.Accepted)
            {
                int start = response.ReturnValue.IndexOf('=') + 1;
                int end = response.ReturnValue.Length;

                return response.ReturnValue.Substring(start, end - start);
            }

            // The request wasn't accepted by the authenticator,
            // so the will be no return value.
            return string.Empty;
        }



        // Example request message:
        //
        // POST /request=login HTTP/1.1
        // Host: www.danid.dk
        //
        // userName=3jd904kfh&passwork=29daflr03ja

        // Example response message:
        //
        // HTTP/1.1 200 OK
        // 
        // keyIndex=4063 //TODO &-sign instead.
    }
}
