// -----------------------------------------------------------------------
// <copyright file="ClientSocket.cs" company="">
// TODO: Update copyright text.
// </copyright>
// ----------------------------------------------------------------------

namespace ClientComponent
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;

    using Miscellaneoues;

    /// <summary>
    /// Data structure representing the properties of the 
    /// http response message from the authenticator.
    /// </summary>
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
    /// TODO: Update summary.
    /// </summary>
    public class ClientSocket
    {
        /// <summary>
        /// Represents the non-validated connection to a server.
        /// </summary>
        private HttpWebRequest clientRequest = default(HttpWebRequest);

        private readonly string serverDomain;

        /// <summary>
        /// The domain of the client that uses this socket.
        /// Used to set the "origin=" part of the messageBody
        /// of the HTTP-message to the server.
        /// </summary>
        private readonly string clientIdentifier;

        private bool haveSentMessage = false;

        private byte[] clientPrivateKey;

        /// <summary>
        /// Initializes a new instance of the ClientSocket class.
        /// http://stackoverflow.com/questions/749030/keep-a-http-connection-alive-in-c
        /// </summary>
        /// <param name="serverDomain">
        /// The domain of the server wished to connect to.
        /// </param>
        /// <param name="port">
        /// The port that is wished to connect to.
        /// </param>
        /// <param name="serverName">
        /// The name of the server as declared in its certificate.
        /// </param>
        public ClientSocket(string serverDomain, string clientIdentifier)
        {
            Contract.Requires(IsValidURL(serverDomain));

            this.serverDomain = serverDomain;
            this.clientIdentifier = clientIdentifier;
        }

        /// <summary>
        /// Sends an HTTP request message to the server domian requesting
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
            Contract.Requires(!this.HaveSentMessage());
            Contract.Ensures(this.HaveSentMessage());

            Console.WriteLine("Client sending message.");

            this.clientRequest = (HttpWebRequest)WebRequest.Create(this.serverDomain + "request=" + operation + "/");

            // request.Credentials = CredentialCache.DefaultCredentials; // TODO remove?
            // ((HttpWebRequest)request).UserAgent = "AuthenticationSerivce";

            // The client always have to sent data to the authenticator, so the
            // authenticator is able to verify the identity of the request message.
            // For this reason the client always must use the POST method.
            this.clientRequest.Method = "POST";

            string compiledMessageBody = this.CompileMessageBody(message);
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
        /// The message read from the stream.
        /// </returns>
        public Response ReadMessage()
        {
            Contract.Requires(this.HaveSentMessage());
            Contract.Ensures(!this.HaveSentMessage());

            HttpWebResponse response = (HttpWebResponse)this.clientRequest.GetResponse();

            // The HTTP status code indicates whether the request was 
            // accepted by the server. If the codes is anything other than
            // 200 OK, the request wasn't accepted by the server.
            bool acceptedRequest = response.StatusCode == HttpStatusCode.OK;

            Stream responseStream = response.GetResponseStream();
            string rawMessageBody = MessageProcessingUtility.ReadFrom(responseStream);

            // Get the responder domain.
            string responderDomain = this.GetResponderDomain(rawMessageBody);

            // If true we are certain the response came from the authenticator. // TODO really?
            bool originMatch = this.serverDomain.Equals(responderDomain);

            if (!(acceptedRequest && originMatch))
            {
                return new Response(false, string.Empty);
            }

            // If the request is accepted the message body will also
            // contain a return value.
            string returnValue = this.GetResponderReturnValue(rawMessageBody);

            // Update the state of the socket
            this.haveSentMessage = false;

            return new Response(true, returnValue);
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

        [Pure]
        public bool HaveSentMessage()
        {
            return this.haveSentMessage;
        }

        private string CompileMessageBody(String message)
        {
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
            string signedEncMessage = Cryptograph.SignData(encMessage, clientPrivateKey); // TODO client private key

            messageBody.Append(signedEncMessage);

            return messageBody.ToString();
        }

        /// <summary>
        /// Gets the domain of the http-message received.
        /// </summary>
        /// <param name="rawMessageBody"></param>
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
                end = rawMessageBody.IndexOf('&') - 1;
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
        /// <param name="rawResponseMessage"></param>
        /// <returns></returns>
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
                    encMessageBody, Cryptograph.GetPublicKey(this.serverDomain));
            }

            // TODO 
            // The message has been tangled with:
            throw new Exception();
        }
    }
}
