// -----------------------------------------------------------------------
// <copyright file="AuthenticatorSocket.cs" company="">
// TODO: Update copyright text.
// </copyright>
// ----------------------------------------------------------------------

namespace AuthenticatorComponent
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;

    using Miscellaneoues;

    /// <summary>
    /// Data structure that represents the properties of a client
    /// http message.
    /// </summary>
    public struct Request
    {
        private string rawUrl;

        /// <summary>
        /// PKI-identifier of the client.
        /// </summary>
        private string requesterDomain;

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
        public Request(string rawUrl, string requesterDomain, string requestedOperation, string[] parameters)
        {
            this.rawUrl = rawUrl;
            this.requesterDomain = requesterDomain;
            this.requestedOperation = requestedOperation;
            this.parameters = parameters;
        }

        public string RawUrl
        {
            get
            {
                return this.rawUrl;
            }
        }

        public string RequesterDomain
        {
            get
            {
                return this.requesterDomain;
            }
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
    /// THE CLASS IS CALLED 'HTTPINTERPRETER' IN THE BON-SPECIFICATION.
    /// </summary>
    public class AuthenticatorSocket
    {
        /// <summary>
        /// Represents the server instance.
        /// </summary>
        private readonly HttpListener server;

        /// <summary>
        /// The certificate of the server
        /// </summary>
        private readonly X509Certificate serverCertificate;

        /// <summary>
        /// The path to the server certificate.
        /// </summary>
        private readonly string certificatePath = @"C:\Users\Kenneth88\Desktop\danIDCertificate.cer"; // TODO delete this?

        /// <summary>
        /// 
        /// </summary>
        private HttpListenerContext currentListenerContext = default(HttpListenerContext);

        /// <summary>
        /// Is "http://localhost:8080" for this application.
        /// </summary>
        private string authenticatorDomain;

        /// <summary>
        /// Used for contracts; determines if a read has occured. Is necessary
        /// to determine, because a send can't happen before a read has been
        /// executed.
        /// </summary>
        private bool hasReadHappened = false;

        /// <summary>
        /// The private key of the authenticator.
        /// </summary>
        private byte[] authenticatorPrivateKey;

        /// <summary>
        /// Initializes a new instance of the AuthenticatorSocket class.
        /// </summary>
        /// <param name="port">
        /// The port with the socket listens to request from.
        /// </param>
        public AuthenticatorSocket(string authenticatorDomain)
        {
            Contract.Requires(IsValidURL(authenticatorDomain));

            this.authenticatorDomain = authenticatorDomain;
            this.server = new HttpListener();
            this.server.Prefixes.Add(authenticatorDomain + "/");

            // Generate a public/private key pair
            // this.authenticatorPrivateKey = Cryptograph.GenerateKeys(this.authenticatorDomain);
        }

        /// <summary>
        /// Makes the socket listen for client requests.
        /// Blocks until a client connects and a ssl-connection
        /// between the parties has been established.
        /// </summary>
        public void Start()
        {
            this.server.Start();
            Console.WriteLine("Server is listening");
        }

        /// <summary>
        /// Source: http://stackoverflow.com/questions/7578857/how-to-check-whether-a-string-is-a-valid-http-url
        /// </summary>
        /// <param name="URL">
        /// Stirng representation of the URL.
        /// </param>
        /// <returns>
        /// True if it is a valid URL, false otherwise.
        /// </returns>
        [Pure]
        public static bool IsValidURL(string URL)
        {
            Uri uri = new Uri(URL);
            return Uri.TryCreate(URL, UriKind.Absolute, out uri) && uri.Scheme == Uri.UriSchemeHttp;
        }

        /// <summary>
        /// Determines if a read has been executed.
        /// </summary>
        /// <returns>
        /// True if a read has happened, false otherwise.
        /// </returns>
        [Pure]
        public bool HasReadHappened()
        {
            return this.hasReadHappened;
        }

        /// <summary>
        /// Read a message from the speciefied SSL stream.
        /// </summary>
        /// <returns>
        /// The message sent by the client.
        /// </returns>
        public Request ReadMessage()
        {
            Contract.Requires(!this.HasReadHappened());
            Contract.Ensures(this.HasReadHappened());

            this.currentListenerContext = this.server.GetContext();
            Console.WriteLine("Server received client request.");
            HttpListenerRequest request = this.currentListenerContext.Request;

            // Get the raw url of the requst:
            string rawUrl = request.RawUrl;

            // Get the raw messageBody of the HTTP request message.
            Stream requestDataStream = request.InputStream;
            string rawMessageBody = MessageProcessingUtility.ReadFrom(requestDataStream);

            // Get requester's domain.
            string requesterDomain = MessageProcessingUtility.GetRequesterDomain(rawMessageBody);

            // Get the requested operation.
            string url = request.Url.OriginalString;
            string requestedOperation = MessageProcessingUtility.GetRequesterOperation(url);

            // Get requester's parameters.
            string[] parameters = MessageProcessingUtility.GetRequesterParameters(rawMessageBody, requesterDomain);

            // Update the state of the socket.
            this.hasReadHappened = true;

            // Return a Request struct containing the properties just
            // achieved.
            return new Request(rawUrl, requesterDomain, requestedOperation, parameters);
        }

        /// <summary>
        /// Sends the specified message as a response to the
        /// request received by the ReadMessage-call.
        /// </summary>
        /// <param name="request">
        /// The request this new message is a response to.
        /// </param>
        /// <param name="accepted"></param>
        /// <param name="message"></param>
        public void SendMessage(Request request, bool accepted, string message)
        {
            Contract.Requires(this.HasReadHappened());
            Contract.Requires(string.IsNullOrEmpty(message));
            Contract.Ensures(!this.HasReadHappened());

            // Obtain a response object.
            HttpListenerResponse responseMessage = this.currentListenerContext.Response; // TODO use response in request objekt instead.

            // Stream used to write the response HTTP-message.
            Stream output = null;

            // Encrypted in requester's public key
            string encOrigin = Cryptograph.Encrypt(
                this.authenticatorDomain, Cryptograph.GetPublicKey(request.RequesterDomain));

            // If the request was not succesfully completed.
            if (!accepted)
            {
                // HTTP status code  403: Forbidden.
                responseMessage.StatusCode = 403;

                output = responseMessage.OutputStream;
                byte[] messageBodyBytes = Encoding.UTF8.GetBytes("origin=" + encOrigin);
                Console.WriteLine("Server responding to client request.");
                output.Write(messageBodyBytes, 0, messageBodyBytes.Length);

                output.Close();
                return;
            }

            // The request was accepted; HTTP status code 200 OK
            responseMessage.StatusCode = 200;

            // Encrypt message in requester's public key.
            string encMessage = Cryptograph.Encrypt(
                message, Cryptograph.GetPublicKey(request.RequesterDomain));

            // Sign the encrypted message with the authenticator's private key.
            string signedEncMessage = Cryptograph.SignData(
                encMessage, Cryptograph.GetPublicKey(request.RequesterDomain));

            // Finally assemble the message body that the requester will
            // receive.
            string compiledMessage =
                "origin=" + encOrigin + "&" +
                encMessage + "&" +
                signedEncMessage;

            byte[] compiledMessageBytes = Encoding.UTF8.GetBytes(compiledMessage);

            Console.WriteLine("Server responding to client request.");
            output = responseMessage.OutputStream;
            output.Write(compiledMessageBytes, 0, compiledMessageBytes.Length);

            output.Close();

            // Update the state of the socket.
            this.hasReadHappened = false;
        }


        // TODO closing of stream and sockets.
    }
}
