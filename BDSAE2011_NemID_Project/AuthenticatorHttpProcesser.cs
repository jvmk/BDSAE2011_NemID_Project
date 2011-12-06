// -----------------------------------------------------------------------
// <copyright file="AuthenticatorHttpProcesser.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace AuthenticationService
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using AuthenticatorComponent;

    using Communication;

    internal struct HttpRequest
    {
        /// <summary>
        /// Represents the requested operation of the parameters for
        /// the operation.
        /// </summary>
        private readonly string requestedOperation, parametersString;

        /// <summary>
        /// Initializes a new instance of the HttpRequest struct.
        /// </summary>
        /// <param name="requestedOperation">
        /// String representation of the requested operation.
        /// </param>
        /// <param name="parameterString">
        /// string representation of the requested operation's
        /// parameters.
        /// </param>
        public HttpRequest(string requestedOperation, string parameterString)
        {
            this.requestedOperation = requestedOperation;
            this.parametersString = parameterString;
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
        public string ParametersString
        {
            get
            {
                return this.parametersString;
            }
        }
    }

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class AuthenticatorHttpProcesser
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

        public AuthenticatorHttpProcesser(int authenticatorPort)
        {
            this.serverSocket = new AuthenticatorSocket(authenticatorPort);
        }

        // TODO ready design for concurrency?

        /// <summary>
        /// Starts the authenticator service.
        /// </summary>
        public void serviceLoop()
        {
            this.serverSocket.ListenForRequests();

            while (this.inService)
            {
                string clientRequest = this.serverSocket.ReadMessage();

                HttpRequest processedRequest = this.ProcessHttpRequest(clientRequest);

                string[] methodParameters = processedRequest.ParametersString.Split(';');

                string httpResponseMessageBody = string.Empty;
                switch (processedRequest.RequestedOperation)
                {
                    case "login":
                        // TODO the authenticator encrypts the message?
                        bool validLogIn = this.authenticator.IsLoginValid(
                            methodParameters[0], methodParameters[1]);
                        string keyIndex = string.Empty;
                        if (validLogIn)
                        {
                            keyIndex = this.authenticator.GetKeyIndex(methodParameters[0]);
                        }

                        httpResponseMessageBody = this.CompileHttpResponse(
                            "submissionValid=" + validLogIn.ToString() +
                            (validLogIn ? ";" + "keyIndex=" + keyIndex : string.Empty));
                        goto default;
                    case "submitKey":
                        // TODO
                        goto default;
                    case "createAccount":
                        // TODO
                        goto default;
                    case "revokeAccount":
                        // TODO
                        goto default;
                    case "newKeyCard":
                        // TODO
                        goto default;
                    default:
                        this.serverSocket.SendMessage(httpResponseMessageBody);
                        break;
                }
            }
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
            // TODO
            return new HttpRequest("login", "huhs;dkso");
        }

        /// <summary>
        /// Compiles a http response message with the specified
        /// messagebody.
        /// </summary>
        /// <param name="messageBody">
        /// The message body containing the message to be sent
        /// back to the client.
        /// </param>
        /// <returns>
        /// A string representation of the http response message.
        /// </returns>
        private string CompileHttpResponse(string messageBody)
        {
            var httpResponse = new StringBuilder();

            httpResponse.Append("HTTP/1.1 200 OK" + "\n");
            httpResponse.Append("\n");
            httpResponse.Append(messageBody);

            return httpResponse.ToString();
        }

        /// <summary>
        /// Closes down the authentication service
        /// properly.
        /// </summary>
        public void CloseDown()
        {
            this.inService = false;
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
