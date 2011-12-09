// -----------------------------------------------------------------------
// <copyright file="AuthenticatorSocket.cs" company="">
// TODO: Update copyright text.
// </copyright>
// ----------------------------------------------------------------------

namespace AuthenticatorComponent
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Security;
    using System.Net.Sockets;
    using System.Security.Authentication;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;
    using System.Threading;

    /// <summary>
    /// Data structure that represents the properties of a client
    /// http message.
    /// </summary>
    public struct Request
    {

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
        public Request(string requesterDomain, string requestedOperation, string[] parameters)
        {
            this.requesterDomain = requesterDomain;
            this.requestedOperation = requestedOperation;
            this.parameters = parameters;
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
        private readonly string certificatePath = @"C:\Users\Kenneth88\Desktop\danIDCertificate.cer";

        /// <summary>
        /// 
        /// </summary>
        private HttpListenerContext currentListenerContext = default(HttpListenerContext);

        /// <summary>
        /// Is "http://localhost:8080" for this application.
        /// </summary>
        private string authenticatorDomain;


        /// <summary>
        /// Initializes a new instance of the AuthenticatorSocket class.
        /// </summary>
        /// <param name="port">
        /// The port with the socket listens to request from.
        /// </param>
        public AuthenticatorSocket(string authenticatorDomain)
        {
            // TODO GENERATE CERTIFICATE.
            this.authenticatorDomain = authenticatorDomain;
            this.server = new HttpListener();
            this.server.Prefixes.Add(authenticatorDomain + "/");
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
        /// Read a message from the speciefied SSL stream.
        /// </summary>
        /// <returns>
        /// The message sent by the client.
        /// </returns>
        public Request ReadMessage()
        {
            this.currentListenerContext = this.server.GetContext();
            Console.WriteLine("Server received client request.");
            HttpListenerRequest request = this.currentListenerContext.Request;

            // Get the raw messageBody of the HTTP request message.
            Stream requestDataStream = request.InputStream;
            string rawMessageBody = this.ReadFrom(requestDataStream);

            // Get requester's domain
            string requesterDomain = this.GetRequesterDomain(rawMessageBody);

            // Get the requested operation.
            string url = request.Url.OriginalString;
            string requestedOperation = this.GetRequestedOperation(url);

            // Get requester's parameters.
            string[] parameters = this.GetParameters(rawMessageBody, requesterDomain);

            return new Request(requesterDomain, requestedOperation, parameters);
        }

        /// <summary>
        /// Sends the specified message as a response to the
        /// request received by the ReadMessage-call.
        /// </summary>
        /// <param name="reqeust">
        /// The request this new message is a response to.
        /// </param>
        /// <param name="accepted"></param>
        /// <param name="message"></param>
        public void SendMessage(Request reqeust, bool accepted, string message)
        {
            // Obtain a response object.
            HttpListenerResponse responseMessage = this.currentListenerContext.Response;

            // Stream used to write the response HTTP-message.
            Stream output = null;

            // Encrypted in requester's public key
            string encOrigin = ""; // Cyrptograph.Encrypt(this.authenticatorDomain, Cryptograph.GetPublicKey(request.Domain))

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

            // HTTP status code 200: OK
            responseMessage.StatusCode = 200;

            // Encrypt message in requester's public key.
            string encMessage = ""; // Cryptograph.Encrypt(message, Cryptograph.GetPublicKey(request.Domain));

            // Sign the encrypted message with the authenticator's private key.
            string signedEncMessage = ""; //Cryptograph.Sign(ensMessage, /*authenticator's private key*/)

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
        }

        private string GetRequestedOperation(string url)
        {
            int start = url.IndexOf("request=") + "request=".Length;
            int end = url.IndexOf('/', start);

            return url.Substring(start, end - start);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        private string ReadFrom(Stream stream)
        {
            // Read the message sent by the client.
            byte[] buffer = new byte[2048];
            StringBuilder messageData = new StringBuilder();
            int bytes = -1;

            do
            {
                // Read the client's test message.
                bytes = stream.Read(buffer, 0, buffer.Length);

                // Use Decoder class to convert from bytes to UTF8
                // in case a character spans two buffers.
                Decoder decoder = Encoding.UTF8.GetDecoder();
                char[] chars = new char[decoder.GetCharCount(buffer, 0, bytes)];
                decoder.GetChars(buffer, 0, bytes, chars, 0);
                messageData.Append(chars);
            }
            while (bytes != 0);

            return messageData.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rawMessageBody"></param>
        /// <returns>
        /// Is encrypted
        /// </returns>
        private string GetRequesterDomain(string rawMessageBody)
        {
            int start = rawMessageBody.IndexOf("origin=") + "origin=".Length;
            // Get the index of the first occurence of '&':
            int end = rawMessageBody.IndexOf('&') - 1;

            string encRequesterDomain = rawMessageBody.Substring(start, end - start);

            //TODO decrypt using authenticator's private key.
            return ""; // Cryptograph.Decrypt(encRequesterDomain, /*Authenticator's private key*/);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rawMessageBody"></param>
        /// <param name="clientDomain">
        /// The domain of the requester, who sent the original HTTP message.
        /// Used for verification of the signed message body part.
        /// </param>
        /// <returns></returns>
        private string[] GetParameters(string rawMessageBody, string clientDomain)
        {
            // Get string representation of the parameters to the requested operation
            // invocation sent in the message.

            Console.WriteLine("MessageBody: " + rawMessageBody);
            string decryptedMessage = this.ProcessRawMessageBody(rawMessageBody, clientDomain);

            int numberOfParameters = decryptedMessage.Count(c => c.Equals('&')) + 1;
            string[] parameters = new string[numberOfParameters];

            int currentIndex = 0;

            for (int i = 1; i < numberOfParameters; i++)
            {
                int s = decryptedMessage.IndexOf('=', currentIndex) + 1;
                Console.WriteLine(s);

                int e = -1;

                // If true, no more '&'-signs are present and the
                // last parameter of the requester message has been
                // reached.
                if (i + 1 == numberOfParameters)
                {
                    e = decryptedMessage.Length;
                }
                else
                {
                    e = decryptedMessage.IndexOf('&', currentIndex);
                }

                parameters[i] = decryptedMessage.Substring(s, e - s);

                currentIndex = e;
            }

            return parameters;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rawMessageBody"></param>
        /// <param name="clientDomain"></param>
        /// <returns>
        /// The decrypted message of the messagebody.
        /// </returns>
        private string ProcessRawMessageBody(string rawMessageBody, string clientDomain)
        {
            string[] parts = rawMessageBody.Split('&');

            // encMessageBody is encrypted in the authenticator's public
            // key. This text represents the text that is signed by the
            // client.
            string encMessageBody = parts[1];
            string signedMessage = parts[2];

            //TODO
            bool verified = 1 == rawMessageBody.Length;// Cryptograph.Verify(encMasse, signedMessage, Cryptograph.GetPublicKey(clientDomain));

            if (verified)
            {
                //TODO
                return "";// Cryptograph.Decypt( /*authenticator's private key*/ );
            }

            // The message has been tangled with:
            throw new Exception();
        }

        // TODO closing of stream and sockets.
        // TODO Is message well-formed?
    }
}
