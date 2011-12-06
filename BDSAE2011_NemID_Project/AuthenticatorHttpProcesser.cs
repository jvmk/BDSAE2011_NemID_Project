// -----------------------------------------------------------------------
// <copyright file="AuthenticatorHttpProcesser.cs" company="">
// TODO: Update copyright text.
// </copyright>
// ----------------------------------------------------------------------

namespace AuthenticationService
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using AuthenticatorComponent;

    using Communication;

    /// <summary>
    /// Data structure that represents the properties of a client
    /// http message.
    /// </summary>
    internal struct HttpRequest
    {
        /// <summary>
        /// Represents the requested operation of the parameters for
        /// the operation.
        /// </summary>
        private readonly string requestedOperation;

        /// <summary>
        /// Holds string representations of the parameters that the
        /// requested operations takes.
        /// The parameters are ordered in the same way as they are
        /// declared in the requested operation.
        /// </summary>
        private readonly string[] parameters;

        /// <summary>
        /// Initializes a new instance of the HttpRequest struct.
        /// </summary>
        /// <param name="requestedOperation">
        /// String representation of the requested operation.
        /// </param>
        /// <param name="parameters">
        /// string representation of the requested operation's
        /// parameters.
        /// </param>
        public HttpRequest(string requestedOperation, string[] parameters)
        {
            this.requestedOperation = requestedOperation;
            this.parameters = parameters;
        }

        /// <summary>
        /// Gets the string representation of the requested
        /// operation of the http request.
        /// </summary>
        public string RequestedOperation
        {
            get
            {
                return this.requestedOperation;
            }
        }

        /// <summary>
        /// Gets the string representing of the parameters
        /// of the http request.
        /// </summary>
        public string[] Parameters
        {
            get
            {
                return this.parameters;
            }
        }
    }

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class AuthenticatorHttpProcessor
    {
        /// <summary>
        /// The authenticator service provider.
        /// </summary>
        private readonly Authenticator authenticator = new Authenticator();

        /// <summary>
        /// Represents the server socket of the authenticator.
        /// </summary>
        private readonly AuthenticatorSocket serverSocket;

        /// <summary>
        /// Indicates whether the service must keep running.
        /// </summary>
        private bool inService = true;

        /// <summary>
        /// Initializes a new instance of the AuthenticatorHttpProcessor class.
        /// </summary>
        /// <param name="authenticatorPort">
        /// The port the authenticator will be listening to.
        /// </param>
        public AuthenticatorHttpProcessor(int authenticatorPort)
        {
            this.serverSocket = new AuthenticatorSocket(authenticatorPort);
        }

        // TODO ready design for concurrency?

        /// <summary>
        /// Starts the authenticator service.
        /// </summary>
        public void ServiceLoop()
        {
            this.serverSocket.ListenForRequests();

            while (this.inService)
            {
                string clientRequest = this.serverSocket.ReadMessage();

                HttpRequest processedRequest = this.ProcessHttpRequest(clientRequest);

                string httpResponseMessageBody = string.Empty;
                switch (processedRequest.RequestedOperation)
                {
                    case "login":
                        string encUserName = processedRequest.Parameters[0];
                        string encPassword = processedRequest.Parameters[1];

                        bool validLogIn = this.authenticator.IsLoginValid(
                            encUserName, encPassword);
                        string keyIndex = string.Empty;

                        if (validLogIn)
                        {
                            // keyIndex is decrypted.
                            keyIndex = this.authenticator.GetKeyIndex(encUserName);
                        }

                        string reponseMessageBody = "submissionValid=" + validLogIn.ToString()
                                                    + (validLogIn ? "&" + "keyIndex=" + keyIndex : string.Empty);

                        httpResponseMessageBody = this.CompileHttpResponse(
                            validLogIn, reponseMessageBody);
                        goto default;
                    case "submitKey":
                        string encKey = processedRequest.Parameters[0];
                        string encUserName1 = processedRequest.Parameters[1];

                        bool validHash = this.authenticator.IsHashValueValid(encKey, encUserName1);

                        httpResponseMessageBody = this.CompileHttpResponse(validHash, string.Empty);
                        goto default;
                    case "createAccount":
                        string encUserName2 = processedRequest.Parameters[0];
                        string encPassword2 = processedRequest.Parameters[1];
                        string encCprNumber = processedRequest.Parameters[2];

                        bool validNewUser = this.authenticator.AddNewUser(
                            encUserName2, encPassword2, encCprNumber);

                        httpResponseMessageBody = this.CompileHttpResponse(validNewUser, string.Empty);
                        goto default;
                    case "revokeAccount":
                        // TODO How do we ensure, that the user has logged in before !!!!!!!!!!!!!!!!!!!!
                        // TODO this request is made? !!!!!!!!!!!!!!!!!!!

                        string encUserName3 = processedRequest.Parameters[0];

                        bool validRevoke = this.authenticator.DeleteUser(encUserName3);

                        httpResponseMessageBody = this.CompileHttpResponse(validRevoke, string.Empty);
                        goto default;

                    // TODO Is this needed? The user must contact danid to do this, as
                    // TODO it would not be possible to log in.
                    case "newKeyCard":
                        goto default;
                    default:
                        this.serverSocket.SendMessage(httpResponseMessageBody);
                        break;
                }
            }
        }

        /// <summary>
        /// Closes down the authentication service
        /// properly.
        /// </summary>
        public void CloseDown()
        {
            this.inService = false;
        }

        /// <summary>
        /// Processes the specified string representation of a 
        /// http message.
        /// </summary>
        /// <param name="httpMessage">
        /// The http message to be processed.
        /// </param>
        /// <returns>
        /// A HttpRequest struct that represents the semantics of 
        /// the specified http message.
        /// </returns>
        private HttpRequest ProcessHttpRequest(string httpMessage)
        {
            string[] requestParts = httpMessage.Split('\n');

            string requestLine = requestParts[0];
            string messageBody = requestParts[requestParts.Count() - 1];

            // Get string representation of requested operation invocation:
            int start = requestLine.IndexOf("request=") + "request=".Length;
            int end = requestLine.IndexOf(" ", start);

            string requestedOperation = requestLine.Substring(start, end);

            // Get string representation of the parameters to the requested operation
            // invocation sent in the message.
            int numberOfParameters = messageBody.Count(c => c.Equals('&')) + 1;

            string[] parameters = new string[numberOfParameters];

            int currentIndex = 0;
            for (int i = 0; i < numberOfParameters; i++)
            {
                int s = messageBody.IndexOf('=', currentIndex);
                int e = messageBody.IndexOf('&', currentIndex) - 1;

                parameters[i] = messageBody.Substring(s, e - s);

                currentIndex = e;
            }

            return new HttpRequest(requestedOperation, parameters);
        }

        /// <summary>
        /// Compiles a http response message with the specified
        /// messagebody.
        /// </summary>
        /// <param name="acceptedRequest">
        /// Indicates whether the request was accepted by the 
        /// authenticator.
        /// </param>
        /// <param name="messageBody">
        /// The message body containing the message to be sent
        /// back to the client.
        /// </param>
        /// <returns>
        /// A string representation of the http response message.
        /// </returns>
        private string CompileHttpResponse(bool acceptedRequest, string messageBody)
        {
            var httpResponse = new StringBuilder();

            if (acceptedRequest)
            {
                httpResponse.Append("HTTP/1.1 200 OK" + "\n");
                httpResponse.Append("\n");
                httpResponse.Append(messageBody);
            }
            else
            {
                httpResponse.Append("HTTP/1.1 400 Bad Request" + "\n");
            }

            return httpResponse.ToString();
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
