// -----------------------------------------------------------------------
// <copyright file="AuthenticatorSocket.cs" company="">
// TODO: Update copyright text.
// </copyright>
// ----------------------------------------------------------------------

namespace BDSA_Project_Authenticator
{
    using System;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Net;
    using System.Text;

    using BDSA_Project_Communication;
    using BDSA_Project_Cryptography;

    /// <summary>
    /// Data structure that represents the properties of a client
    /// http message.
    /// </summary>
    /// <author>
    /// Kenneth Lundum Søhrmann.
    /// </author>
    public struct Request
    {
        /// <summary>
        /// The raw URL that this request originated from.
        /// </summary>
        private string rawUrl;

        /// <summary>
        /// PKI-identifier of the client.
        /// </summary>
        private string requesterIdentifier;

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
        public Request(string rawUrl, string requesterIdentifier, string requestedOperation, string[] parameters)
        {
            this.rawUrl = rawUrl;
            this.requesterIdentifier = requesterIdentifier;
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
                return this.requesterIdentifier;
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

        /// <summary>
        /// Determines if all the values of the Request have been
        /// set. If they are not set, decryption and verification of
        /// the message failed indicating that the message has
        /// been tangled with.
        /// </summary>
        /// <returns>
        /// True if the values of the Request-struct is set, false otherwise.
        /// </returns>
        public bool IsComplete()
        {
            bool isComplete = !ReferenceEquals(this.rawUrl, null);
            isComplete = isComplete && !ReferenceEquals(this.requesterIdentifier, null);
            isComplete = isComplete && !ReferenceEquals(this.parameters, null);
            return isComplete;
        }
    }

    /// <summary>
    /// Network socket that receives request from clients, send responses and
    /// processes the HTTP-message exchanged during these operations.	
    /// </summary>
    /// <author>
    /// Kenneth Lundum Søhrmann
    /// </author>
    public class AuthenticatorServer
    {
        /// <summary>
        /// The domian of the authencator server.
        /// </summary>
        private readonly string authenticatorDomain;

        /// <summary>
        /// Represents the server instance.
        /// </summary>
        private readonly HttpListener server;

        /// <summary>
        /// The current HttpListenerContext. If a ReadMessage has been performed, the 
        /// SendMessage will repond using the same HttpListenerObject.
        /// </summary>
        private HttpListenerContext currentListenerContext = default(HttpListenerContext);

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
        /// Initializes a new instance of the AuthenticatorServer class.
        /// </summary>
        /// <param name="authenticatorDomain">
        /// The domain of the authenticator server.
        /// </param>
        /// <param name="authenticatorPrivateKey">
        /// The private key of the authenticator.
        /// </param>
        public AuthenticatorServer(string authenticatorDomain, byte[] authenticatorPrivateKey)
        {
            Contract.Requires(MessageProcessingUtility.IsValidUrl(authenticatorDomain));
            Contract.Requires(authenticatorPrivateKey != null);

            this.authenticatorDomain = authenticatorDomain;
            this.authenticatorPrivateKey = authenticatorPrivateKey;
            this.server = new HttpListener();
            this.server.Prefixes.Add(authenticatorDomain);

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
            Console.WriteLine("Authenticator server is listening for requests at " + this.authenticatorDomain);
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
            HttpListenerRequest request = this.currentListenerContext.Request;

            // Get the raw url of the requst:
            string rawUrl = request.RawUrl;

            Console.WriteLine("\nAuthenticator received request, URL: " + request.Url);

            // Get the raw messageBody of the HTTP request message.
            Stream requestDataStream = request.InputStream;
            string rawMessageBody = MessageProcessingUtility.ReadFrom(requestDataStream);

            // If the raw message body is not well formed...
            if (!MessageProcessingUtility.IsRawMessageBodyWellFormed(rawMessageBody))
            {
                //... we return a Request with the properties known.
                return new Request(rawUrl, null, MessageProcessingUtility.GetRequesterOperation(rawUrl), null);
            }

            // Get requester's domain.
            string requesterDomain = MessageProcessingUtility.GetRequesterDomain(
                rawMessageBody, this.authenticatorPrivateKey);

            // Get the requested operation.
            string requestedOperation = MessageProcessingUtility.GetRequesterOperation(rawUrl);

            // Get requester's parameters.
            string[] parameters = MessageProcessingUtility.GetRequesterParameters(
                rawMessageBody, requesterDomain, this.authenticatorPrivateKey);

            Console.WriteLine("Requester message was successfully veryfied and decrypted: " +
                !(ReferenceEquals(requesterDomain, null) && !ReferenceEquals(requestedOperation, null) &&
                !ReferenceEquals(parameters, null)));

            // Update the state of the socket.
            this.hasReadHappened = true;

            requestDataStream.Close();

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
        /// <param name="accepted">
        /// Indicates if the request, this message is a response, was
        /// accepted by the authenticator.
        /// </param>
        /// <param name="message">
        /// The return value to be sent back to the requester.
        /// If the request was not accepted, then message will just be
        /// an empty string.
        /// </param>
        public void SendMessage(Request request, bool accepted, string message)
        {
            Contract.Requires(this.HasReadHappened());
            Contract.Ensures(!this.HasReadHappened());

            // Obtain a response object.
            HttpListenerResponse responseMessage = this.currentListenerContext.Response; // TODO use response in request objekt instead.

            // Stream used to write the response HTTP-message.
            Stream output = null;

            // If the request was not succesfully completed...
            if (!accepted)
            {
                // ... the HTTP status code is set to 403 Forbidden.
                responseMessage.StatusCode = 403;

                output = responseMessage.OutputStream;
                output.Close();
                return;
            }

            // The request was accepted; HTTP status code 200 OK
            responseMessage.StatusCode = 200;

            Console.WriteLine("Authenticator encrypting in public key of: " + request.RequesterDomain);

            string encOrigin = Cryptograph.Encrypt(
                 this.authenticatorDomain, Cryptograph.GetPublicKey(request.RequesterDomain));

            // Encrypt message in requester's public key.
            string encMessage = Cryptograph.Encrypt(
                message, Cryptograph.GetPublicKey(request.RequesterDomain));

            Console.WriteLine("Authenticator signing encrypted message.");

            // Sign the encrypted message with the authenticator's private key.
            string signedEncMessage = Cryptograph.SignData(
                encMessage, this.authenticatorPrivateKey);

            Console.WriteLine("Authenticator compiling response message.");

            // Finally assemble the message body that the requester will
            // receive.
            string compiledMessage =
                "origin=" + encOrigin + "&" +
                encMessage + "&" +
                signedEncMessage;

            byte[] compiledMessageBytes = Encoding.UTF8.GetBytes(compiledMessage);

            output = responseMessage.OutputStream;
            output.Write(compiledMessageBytes, 0, compiledMessageBytes.Length);

            Console.WriteLine("Authenticator responding to request with the url: " + request.RawUrl);

            output.Flush();
            output.Close();

            responseMessage.Close();

            // Update the state of the socket.
            this.hasReadHappened = false;
        }

        /// <summary>
        /// Used for redirection requests.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="accepted"></param>
        public void SendMessage(Request request, bool accepted)
        {
            Contract.Requires(this.HasReadHappened());
            Contract.Ensures(!this.HasReadHappened());

            // Obtain a response object.
            HttpListenerResponse responseMessage = this.currentListenerContext.Response;

            // Stream used to write the response HTTP-message.
            Stream output = null;

            // If the request was not succesfully completed...
            if (!accepted)
            {
                // ... the HTTP status code is set to 403 Forbidden.
                responseMessage.StatusCode = 403;

                output = responseMessage.OutputStream;
                Console.WriteLine("Authenticator sent response to client request: " + request.RequesterDomain +
                    "\nResponding to request with the url: " + request.RawUrl);
                output.Close();
                return;
            }

            // The request was accepted; HTTP status code 200 OK
            responseMessage.StatusCode = 200;

            string message = "validRirect=true";

            // Sign the message with the authenticator's private key.
            string signedMessage = Cryptograph.SignData(
                message, this.authenticatorPrivateKey);

            byte[] compiledMessageBytes = Encoding.UTF8.GetBytes(signedMessage);

            output = responseMessage.OutputStream;
            output.Write(compiledMessageBytes, 0, compiledMessageBytes.Length);

            Console.WriteLine("Authenticator sent response to redirect." +
                "\nResponding to request with the url: " + request.RawUrl);

            output.Flush();
            output.Close();

            responseMessage.Close();

            // Update the state of the socket.
            this.hasReadHappened = false;
        }
    }
}
