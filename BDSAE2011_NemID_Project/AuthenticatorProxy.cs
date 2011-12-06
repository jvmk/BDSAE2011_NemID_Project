// -----------------------------------------------------------------------
// <copyright file="AuthenticatorProxy.cs" company="">
// TODO: Update copyright text.
// </copyright>
// ----------------------------------------------------------------------

namespace AuthenticationService
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Communication;

    /// <summary>
    /// Data structure representing the properties of the 
    /// http response message from the authenticator.
    /// </summary>
    internal struct HttpResponse
    {
        /// <summary>
        /// Indicated whether the request that this reponse it a
        /// reponse to was accepted by the authenticator.
        /// </summary>
        private readonly bool accepted;

        /// <summary>
        /// The string representation of the return value of the 
        /// requested operation
        /// </summary>
        private readonly string returnValue;

        /// <summary>
        /// Initializes a new instance of the HttpResponse struct.
        /// </summary>
        /// <param name="accepted">
        /// Indicates whether the request, the response message is a 
        /// response to, was accepted by the authenticator.
        /// </param>
        /// <param name="returnvalue">
        /// String representation of the return value of the requested
        /// authenticator operation.
        /// </param>
        public HttpResponse(bool accepted, string returnvalue)
        {
            this.accepted = accepted;
            this.returnValue = returnvalue;
        }

        /// <summary>
        /// Gets a value indicating whether the request was accepted by the
        /// authenticator.
        /// </summary>
        public bool Accepted
        {
            get
            {
                return this.accepted;
            }
        }

        /// <summary>
        /// Gets a string representation of the return value returned
        /// by the authenticator operation.
        /// Will be null if the authenticator didn't accept the requested
        /// operation.
        /// </summary>
        public string ReturnValue
        {
            get
            {
                return this.returnValue;
            }
        }
    }

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
        private HttpResponse currentServerResponse = default(HttpResponse);

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
            string request = this.CompileHttpRequest(
                "createAccount",
                "userName=" + encUserName + "&" +
                "password=" + encPassword + "&" +
                "cprNumber=" + encCprNumber);

            this.socket.SendMessage(request);
            this.ReceiveResponse();

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
            string request = this.CompileHttpRequest(
                "login",
                "userName=" + encUserName + ":" + "password=" + encPassword);

            this.socket.SendMessage(request);
            this.ReceiveResponse();

            return this.currentServerResponse.Accepted;
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
            return this.currentServerResponse.ReturnValue;
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
            string request = this.CompileHttpRequest(
                "submitKey",
                "keyValue=" + encKeyValue + "&" + "userName=" + encUserName);

            this.socket.SendMessage(request);
            this.ReceiveResponse();

            return this.currentServerResponse.Accepted;
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
        public bool RevokeUserAccount(string encUserName)
        {
            string request = this.CompileHttpRequest(
                "revokeAccount",
                "userName=" + encUserName);

            this.socket.SendMessage(request);
            this.ReceiveResponse();

            return this.currentServerResponse.Accepted;
        }

        // TODO Should not be a part of the program
        public bool RequestNewKeyCard()
        {
            return true;
        }

        /// <summary>
        /// Helper method; when a response from the authentication 
        /// server is received, the raw HttpMessage is processed
        /// and assigned to the currentServerReponse-field.
        /// </summary>
        private void ReceiveResponse()
        {
            string rawServerResponse = this.socket.ReadMessage();
            this.currentServerResponse = this.ProcessHttpResponse(rawServerResponse);
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
        private HttpResponse ProcessHttpResponse(string httpMessage)
        {
            string[] responseLines = httpMessage.Split('\n');

            string statusLine = responseLines[0];
            string messageBody = responseLines[responseLines.Count() - 1];

            // Extract HTTP reponse code
            int start = statusLine.IndexOf(" ");
            int end = statusLine.IndexOf(" ", start + 1);

            int httpResponseCode = int.Parse(statusLine.Substring(start, end - start));

            // Extract return value
            string encResponse = null;

            if (httpResponseCode == 200 && !messageBody.Equals(string.Empty))
            {
                int s = messageBody.IndexOf('=');
                int e = messageBody.Length;

                encResponse = messageBody.Substring(s, e - s);
            }

            HttpResponse response = default(HttpResponse);

            switch (httpResponseCode)
            {
                case 200:
                    response = new HttpResponse(true, encResponse);
                    break;
                case 400:
                    response = new HttpResponse(false, encResponse);
                    break;
            }

            return response;
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
