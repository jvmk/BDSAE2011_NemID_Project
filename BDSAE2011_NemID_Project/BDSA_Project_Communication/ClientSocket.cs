// -----------------------------------------------------------------------
// <copyright file="ClientSocket.cs" company="">
// TODO: Update copyright text.
// </copyright>
// ----------------------------------------------------------------------

namespace BDSA_Project_Communication
{
    using System;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;

    using BDSA_Project_Cryptography;

    /// <summary>
    /// Data structure representing the properties of the 
    /// HTTP response message from the authenticator.
    /// </summary>
    /// <author>
    /// Kenneth Lundum Søhrmann
    /// </author>
    public struct Response
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
        /// Initializes a new instance of the Response struct.
        /// </summary>
        /// <param name="accepted">
        /// Indicates whether the request, the response message is a 
        /// response to, was accepted by the authenticator.
        /// </param>
        /// <param name="returnvalue">
        /// String representation of the return value of the requested
        /// authenticator operation.
        /// </param>
        public Response(bool accepted, string returnvalue)
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
        /// Gets a string representation of the message body returned
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
    /// The component that transform the client requests to HTTP request 
    /// messages and communicates with the authenticator server.”
    /// </summary>
    /// <author>
    /// Kenneth Lundum Søhrmann;
    /// </author>
    public class ClientSocket
    {
        /// <summary>
        /// Represents the connection to a server.
        /// </summary>
        private HttpWebRequest clientRequest = null;

        /// <summary>
        /// The url of the server this client socket has connected to.
        /// </summary>
        private readonly string serverDomain;

        /// <summary>
        /// The domain of the client that uses this socket.
        /// Used to set the "origin=" part of the messageBody
        /// of the HTTP-message to the server.
        /// </summary>
        private readonly string clientIdentifier;

        /// <summary>
        /// The private key of the client.
        /// </summary>
        private readonly byte[] clientPrivateKey;

        /// <summary>
        /// Indicated whether a message to the server has been sent.
        /// </summary>
        private bool haveSentMessage = false;

        /// <summary>
        /// Initializes a new instance of the ClientSocket class.
        /// </summary>
        /// <param name="serverDomain">
        /// The URL of the server wished to connect to.
        /// </param>
        /// <param name="clientIdentifier">
        /// The PKI-identifier of the client that uses the ClientSocket.
        /// </param>
        /// <param name="clientPrivateKey">
        /// The private key of the client that uses the ClientSocket.
        /// </param>
        public ClientSocket(string serverDomain, string clientIdentifier, byte[] clientPrivateKey)
        {
            Contract.Requires(MessageProcessingUtility.IsValidUrl(serverDomain));
            Contract.Requires(Cryptograph.KeyExists(clientIdentifier));
            Contract.Requires(Cryptograph.CheckConsistency(clientPrivateKey, clientIdentifier));

            this.serverDomain = serverDomain;
            this.clientIdentifier = clientIdentifier;
            this.clientPrivateKey = clientPrivateKey;
        }

        /// <summary>
        /// Sends an HTTP request message to the server requesting
        /// the specified operation and supplying the specified message.
        /// </summary>
        /// <param name="operation">
        /// The requested operation on the server.
        /// </param>
        /// <param name="message">
        /// The message of the request constituted of string
        /// representations of the parameters sent to the 
        /// specified operation.
        /// </param>
        public void SendMessage(string operation, string message)
        {
            Contract.Requires(!string.IsNullOrEmpty(operation));
            Contract.Requires(!string.IsNullOrEmpty(message));
            Contract.Requires(!this.HaveSentMessage());
            Contract.Ensures(this.HaveSentMessage());

            string requestUrl = this.serverDomain + "request=" + operation + "/";

            this.clientRequest = (HttpWebRequest)WebRequest.Create(requestUrl);

            Console.WriteLine("Client sends message to server, url: " + requestUrl);

            // request.Credentials = CredentialCache.DefaultCredentials; // TODO remove?
            // ((HttpWebRequest)request).UserAgent = "AuthenticationSerivce";

            // The client always have to sent data to the authenticator, so the
            // authenticator is able to verify the identity of the request message.
            // For this reason the client always must use the POST method.
            this.clientRequest.Method = "POST";

            // Compile the message body of the HTTP request.
            string compiledMessageBody = this.CompileMessageBody(message);

            Console.Write("HTTP request message body is: \n" + compiledMessageBody);

            byte[] messageBytes = Encoding.UTF8.GetBytes(compiledMessageBody);

            this.clientRequest.ContentLength = messageBytes.Length;

            // HTTP version 1.1 have the KeepAlive-property set to default.
            this.clientRequest.ProtocolVersion = HttpVersion.Version11;

            Stream dataStream = this.clientRequest.GetRequestStream();
            dataStream.Write(messageBytes, 0, messageBytes.Length);
            dataStream.Close();

            // Update the state of the socket.
            this.haveSentMessage = true;
        }

        /// <summary>
        /// Reads a message from the stream.
        /// </summary>
        /// <returns>
        /// A Response-struct representing properties of the 
        /// HTTP message received from the server.
        /// </returns>
        public Response ReadMessage()
        {
            Contract.Requires(this.HaveSentMessage());
            Contract.Ensures(!this.HaveSentMessage());

            HttpWebResponse response;
            bool acceptedRequest;

            try
            {
                // If the request has any other HTTP response code than 200,
                // the GetResponse-method will throw a WebException.
                response = response = (HttpWebResponse)this.clientRequest.GetResponse();
            }
            catch (WebException)
            {
                acceptedRequest = false;
                return new Response(acceptedRequest, null);
            }

            acceptedRequest = true;

            Console.WriteLine("Client received response from server.");
            Console.WriteLine("Processing response...");

            /*
            // The HTTP status code indicates whether the request was 
            // accepted by the server. If the codes is anything other than
            // 200 OK, the request wasn't accepted by the server.
            acceptedRequest = response.StatusCode == HttpStatusCode.OK;

            if (!acceptedRequest)
            {
                return new Response(false, string.Empty);
            }
             * */

            Stream responseStream = response.GetResponseStream();
            string rawMessageBody = MessageProcessingUtility.ReadFrom(responseStream);

            // Get the responder domain.
            string responderDomain = this.GetResponderDomain(rawMessageBody);

            // If true we are certain the response came from the authenticator. // TODO really?
            bool originMatch = this.serverDomain.Equals(responderDomain);

            // If the request is accepted the message body will also
            // contain a return value.
            string returnValue = this.GetResponderReturnValue(rawMessageBody);

            // If the returnValue is null, that message has been tangled with.
            if (returnValue == null)
            {
                return new Response(false, string.Empty);
            }

            // Update the state of the socket
            this.haveSentMessage = false;

            responseStream.Close();

            return new Response(acceptedRequest, returnValue);
        }

        /// <summary>
        /// Has a message been sent with this socket?
        /// When a message has been read, this method returns
        /// false, and returns true again, when a new message
        /// has been sent.
        /// </summary>
        /// <returns>
        /// True if a message has been sent, false otherwise.
        /// </returns>
        [Pure]
        public bool HaveSentMessage()
        {
            return this.haveSentMessage;
        }

        /// <summary>
        /// Compiles the message body of a HTTP message body 
        /// containing the specified message.
        /// </summary>
        /// <param name="message">
        /// The message to be compiled.
        /// </param>
        /// <returns>
        /// Encrypted and well formed HTTP message body.
        /// </returns>
        private string CompileMessageBody(string message)
        {
            Contract.Requires(message != null);
            Contract.Ensures(MessageProcessingUtility.IsRawMessageBodyWellFormed(Contract.Result<string>()));

            StringBuilder messageBody = new StringBuilder();

            // Domain encrypted in authenticator's public key.
            string encDomain = Cryptograph.Encrypt(
                this.clientIdentifier, Cryptograph.GetPublicKey(this.serverDomain));

            messageBody.Append("origin=" + encDomain + "&");

            // Encrypt message in authenticator's public key.
            string encMessage = Cryptograph.Encrypt(
                message, Cryptograph.GetPublicKey(this.serverDomain));

            messageBody.Append(encMessage + "&");

            // Sign encMessage in client's private key.
            string signedEncMessage = Cryptograph.SignData(encMessage, this.clientPrivateKey);

            messageBody.Append(signedEncMessage);

            return messageBody.ToString();
        }

        /// <summary>
        /// Gets the domain of the http-message received.
        /// </summary>
        /// <param name="rawMessageBody">
        /// The raw HTTP message body to be processed.
        /// </param>
        /// <returns>
        /// String representation of the domain specified
        /// in the raw message body.
        /// </returns>
        private string GetResponderDomain(string rawMessageBody)
        {
            Contract.Requires(MessageProcessingUtility.IsRawMessageBodyWellFormed(rawMessageBody));

            // Determine start index of the encrypted reponder domain.
            int start = rawMessageBody.IndexOf("origin=") + "origin=".Length;

            // Determine end index of the encrypted responder domain.
            int end = -1;
            if (rawMessageBody.Contains('&'))
            {
                end = rawMessageBody.IndexOf('&');
            }
            else
            {
                end = rawMessageBody.Length;
            }

            string encRequesterDomain = rawMessageBody.Substring(start, end - start);

            // Decrypt using client's private key.
            return Cryptograph.Decrypt(encRequesterDomain, this.clientPrivateKey); // TODO client private key.
        }

        /// <summary>
        /// Gets the return value form the raw response message.
        /// </summary>
        /// <param name="rawResponseMessageBody">
        /// The raw message body to be processed.
        /// </param>
        /// <returns>
        /// The return value of the reponse.
        /// Null if the processing of the raw HTTP message body failed, 
        /// indicating that the message has been tangled with.
        /// </returns>
        private string GetResponderReturnValue(string rawResponseMessageBody)
        {
            Contract.Requires(MessageProcessingUtility.IsRawMessageBodyWellFormed(rawResponseMessageBody));

            string[] parts = rawResponseMessageBody.Split('&');

            // encMessageBody is encrypted in the authenticator's public
            // key. This text represents the text that is signed by the
            // client.
            string encMessageBody = parts[1];
            string signedEncMessageBody = parts[2];

            bool verified = Cryptograph.VerifyData(
                encMessageBody, signedEncMessageBody, Cryptograph.GetPublicKey(this.serverDomain));

            if (verified)
            {
                return Cryptograph.Decrypt(
                    encMessageBody, clientPrivateKey);
            }

            return null;
        }
    }
}
