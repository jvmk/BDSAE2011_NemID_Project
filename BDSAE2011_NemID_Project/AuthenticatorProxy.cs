// -----------------------------------------------------------------------
// <copyright file="AuthenticatorProxy.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace AuthenticationService
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Communication;

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
        private string currentServerResponse = string.Empty;

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
        public AuthenticatorProxy(string serverDomain, int port, string serverName)
        {
            this.serverDomain = serverDomain;
            this.socket = new ClientSocket(serverDomain, port, serverName);
        }

        // TODO request log-in screen?
        public void LogInScreen()
        {

        }

        // TODO
        public bool CreateUserAccount()
        {
            return true;
        }

        // TODO
        public bool RevokeUserAccount()
        {
            return true;
        }

        // TODO
        public bool RequestNewKeyCard()
        {
            return true;
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
            string request = this.CompileHttpRequest(
                "login",
                "userName=" + encUserName + ":" + "password=" + encPassword);

            this.socket.SendMessage(request);

            string rawServerResponse = this.socket.ReadMessage();

            this.currentServerResponse = this.ProcessHttpResponse(rawServerResponse);

            return this.currentServerResponse.Contains("true");
        }

        /// <summary>
        /// When the user submitted an accepted combination of userId
        /// and password to the authentication server, this method
        /// returns the keyIndex from the users key card which 
        /// corresponding key the authentication server is expecting.
        /// </summary>
        /// <returns>
        /// A string representation of the key index value.
        /// </returns>
        public string GetKeyIndex()
        {
            int lastIndex = this.currentServerResponse.Length - 1;
            return this.currentServerResponse.Substring(lastIndex - 3, lastIndex);
        }

        /// <summary>
        /// Submits the specified keyValue from the client's
        /// key card to the authentication server.
        /// </summary>
        /// <param name="encKeyValue">
        /// The encrypted key-value submitted by the user.
        /// </param>
        /// <returns>
        /// Returns true if the value of the submitted key is
        /// what the authentication server expected, false otherwise.
        /// </returns>
        public bool SubmitKey(int encKeyValue)
        {
            string request = this.CompileHttpRequest(
                "submitKey",
                "keyValue=" + encKeyValue);

            this.socket.SendMessage(request);

            string rawServerResponse = this.socket.ReadMessage();

            this.currentServerResponse = this.ProcessHttpResponse(rawServerResponse);

            return this.currentServerResponse.Contains("true");
        }

        /// <summary>
        /// Compile an HTTP-request method using the POST-method.
        /// </summary>
        /// <param name="operation">
        /// The method to be invoked on the server.
        /// </param>
        /// <param name="messageBody">
        /// The parameters of the method to be invoked.
        /// </param>
        /// <returns>
        /// A string representation of the http request message.
        /// </returns>
        private string CompileHttpRequest(string operation, string messageBody)
        {
            var requestString = new StringBuilder();

            requestString.Append("POST " + "/request=" + operation + " " + "HTTP/1.1" + "\n");
            requestString.Append("Host: " + this.serverDomain + "\n");
            requestString.Append("\n");
            requestString.Append(messageBody);

            return requestString.ToString();
        }

        /// <summary>
        /// Processes the specified string representation of a 
        /// http message.
        /// </summary>
        /// <param name="httpMessage">
        /// Raw string representation of the http response message.
        /// </param>
        /// <returns>
        /// A string constitution the return values of the authentication
        /// server.
        /// </returns>
        private string ProcessHttpResponse(string httpMessage)
        {
            string[] responseLines = httpMessage.Split('\n');

            string statusLine = responseLines[0];
            string messageBody = responseLines[responseLines.Count() - 1];

            if (messageBody.Contains("true"))
            {
                return "true" + ";" + messageBody;
            }

            return "false";
        }


        // Example request message:
        //
        // POST /request=login HTTP/1.1
        // Host: www.danid.dk 
        //
        // userName=3jd904kfh;passwork=29daflr03ja

        // Example response message:
        //
        // HTTP/1.1 200 OK
        // 
        // submissionValid=true;keyIndex=4063
    }
}
