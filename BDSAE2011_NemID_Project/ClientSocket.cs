// -----------------------------------------------------------------------
// <copyright file="ClientSocket.cs" company="">
// TODO: Update copyright text.
// </copyright>
// ----------------------------------------------------------------------

namespace Test
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
        private readonly string messageBody;

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
            this.messageBody = returnvalue;
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
        public string MessageBody
        {
            get
            {
                return this.messageBody;
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
        private readonly string clientDomain;

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
        public ClientSocket(string serverDomain, string clientDomain)
        {
            this.serverDomain = serverDomain;
            this.clientDomain = clientDomain;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="message"></param>
        public void SendMessage(string operation, string message)
        {
            // Console.WriteLine("Client sending message.");
            // byte[] messageBytes = Encoding.UTF8.GetBytes(message + "<EOF>");
            // this.sslStream.Write(messageBytes);
            // this.sslStream.Flush();

            this.clientRequest = (HttpWebRequest)WebRequest.Create(serverDomain + "/request=" + operation + "/");
            // request.Credentials = CredentialCache.DefaultCredentials;
            // ((HttpWebRequest)request).UserAgent = "AuthenticationSerivce";
            this.clientRequest.Method = "POST";

            string compiledMessageBody = this.CompileMessageBody(message);
            byte[] messageBytes = Encoding.UTF8.GetBytes(compiledMessageBody);

            this.clientRequest.ContentLength = messageBytes.Length;
            // HTTP version 1.1 have the KeepAlive-property set to default.
            this.clientRequest.ProtocolVersion = HttpVersion.Version11;

            Stream dataStream = this.clientRequest.GetRequestStream();
            dataStream.Write(messageBytes, 0, messageBytes.Length);

            dataStream.Close();
        }

        /// <summary>
        /// Reads a message from the stream.
        /// </summary>
        /// <returns>
        /// The message read from the stream.
        /// </returns>
        public Response ReadMessage()
        {
            HttpWebResponse response = (HttpWebResponse)clientRequest.GetResponse();

            // The HTTP status code indicates whether the request was 
            // accepted by the server.
            bool acceptedRequest = response.StatusCode == HttpStatusCode.OK;

            Stream responseStream = response.GetResponseStream();
            string responseMessage = this.ReadFrom(responseStream);
            string rawMessageBody = this.ReadFrom(responseStream);

            string responderDomain = this.GetResponderDomain(rawMessageBody);

            // If true we are certain the response came from the authenticator.
            bool originMatch = this.serverDomain.Equals(responderDomain);

            if (!(acceptedRequest && originMatch))
            {
                return new Response(false, string.Empty);
            }

            // If the request is accepted the message body will also
            // contain a return value.
            string returnValue = this.GetReturnValue(responseMessage);

            return new Response(true, returnValue);
        }

        private string CompileMessageBody(String message)
        {
            StringBuilder messageBody = new StringBuilder();

            // Domain signed in authenticator's public key.
            string encDomain = ""; // Cryptograh.Encyrpt(this.clientDomain, Cryptograph.GetPublicKey(authenticator));

            messageBody.Append("origin=" + encDomain + "&");

            //Encrypt in authenticator's public key. /TODO
            string encMessage = "";//Cryptograph.Encrypt()

            messageBody.Append(encMessage + "&");

            //Sign encMessage TODO
            string signedEncMessage = ""; // Cryptograph.Encrypt()

            messageBody.Append(signedEncMessage);

            return messageBody.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        private string ReadFrom(Stream stream)
        {
            byte[] buffer = new byte[2048];
            StringBuilder messageData = new StringBuilder();
            int bytes = -1;

            do
            {
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
        /// Gets the domain of the http-message received.
        /// </summary>
        /// <param name="rawMessageBody"></param>
        /// <returns>
        /// String representation of the domain specified
        /// in the raw message body.
        /// </returns>
        private string GetResponderDomain(string rawMessageBody)
        {
            int start = rawMessageBody.IndexOf("origin=") + "origin=".Length;

            int end = -1;

            // Determine the end-index of the responder domain
            if (rawMessageBody.Contains('&'))
            {
                end = rawMessageBody.IndexOf('&') - 1;
            }
            else
            {
                end = rawMessageBody.Length;
            }

            string encRequesterDomain = rawMessageBody.Substring(start, end - start);

            //TODO decrypt using client's private key.
            return ""; // Cryptograph.Decrypt(encRequesterDomain, /*Client's private key*/);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rawResponseMessage"></param>
        /// <returns></returns>
        private string GetReturnValue(string rawResponseMessage)
        {
            if (!rawResponseMessage.Contains('&'))
            {
                return "";
            }

            // The authenticator only sends back one return value.
            string decryptedMessageBody = this.ProcessRawMessageBody(rawResponseMessage);

            int start = decryptedMessageBody.IndexOf('=') + 1;
            int end = decryptedMessageBody.Length;

            return decryptedMessageBody.Substring(start, end - start);
        }

        private string ProcessRawMessageBody(string rawMessageBody)
        {
            string[] parts = rawMessageBody.Split('&');

            // encMessageBody is encrypted in the authenticator's public
            // key. This text represents the text that is signed by the
            // client.
            string encMessageBody = parts[1];
            string signedMessage = parts[2];

            //TODO
            bool verified = 1 == rawMessageBody.Length;// Cryptograph.Verify(encMasse, signedMessage, Cryptograph.GetPublicKey(this.serverDomain));

            if (verified)
            {
                //TODO
                return "";// Cryptograph.Decypt( /*authenticator's private key*/ );
            }

            // The message has been tangled with:
            throw new Exception();
        }
    }
}
