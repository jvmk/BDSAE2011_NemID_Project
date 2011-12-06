// -----------------------------------------------------------------------
// <copyright file="ClientSocket.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Communication
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Security;
    using System.Net.Sockets;
    using System.Security.Authentication;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class ClientSocket
    {
        /// <summary>
        /// Represents the non-validated connection to a server.
        /// </summary>
        private readonly TcpClient client;

        /// <summary>
        /// Represents the validated connection to a server.
        /// </summary>
        private readonly SslStream sslStream;

        /// <summary>
        /// Initializes a new instance of the ClientSocket class.
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
        public ClientSocket(string serverDomain, int port, string serverName)
        {
            this.client = new TcpClient(serverDomain, port);
            Console.WriteLine("Client connected");
            this.sslStream = this.getValidatedStream(serverName);
            Console.WriteLine("Validated connection established.");
        }

        /// <summary>
        /// Sends the specificed message over the socket.
        /// </summary>
        /// <param name="message">
        /// Message to be sent.
        /// </param>
        public void SendMessage(string message)
        {
            // TODO Encyption code of the message.
            Console.WriteLine("Client sending message.");
            byte[] messageBytes = Encoding.UTF8.GetBytes(message + "<EOF>");
            this.sslStream.Write(messageBytes);
            this.sslStream.Flush();
        }

        /// <summary>
        /// Reads a message from the stream.
        /// </summary>
        /// <returns>
        /// The message read from the stream.
        /// </returns>
        public string ReadMessage()
        {
            // Read the  message sent by the server.
            // The end of the message is signaled using the
            // "<EOF>" marker.
            byte[] buffer = new byte[2048];
            StringBuilder messageData = new StringBuilder();
            int bytes = -1;
            do
            {
                bytes = this.sslStream.Read(buffer, 0, buffer.Length);

                // Use Decoder class to convert from bytes to UTF8
                // in case a character spans two buffers.
                Decoder decoder = Encoding.UTF8.GetDecoder();
                char[] chars = new char[decoder.GetCharCount(buffer, 0, bytes)];
                decoder.GetChars(buffer, 0, bytes, chars, 0);
                messageData.Append(chars);

                // Check for EOF.
                if (messageData.ToString().IndexOf("<EOF>") != -1)
                {
                    break;
                }
            } while (bytes != 0);

            // TODO decryption of the message.

            Console.WriteLine("Client received message: " + messageData);

            return messageData.ToString();
        }

        /// <summary>
        /// Creates a validated SSL connection from the TcpClient connection.
        /// </summary>
        /// <param name="serverName">
        /// The name of the server as declared in its certificate.
        /// </param>
        /// <returns>
        /// A validated connection to the server.
        /// </returns>
        private SslStream getValidatedStream(string serverName)
        {
            SslStream sslStream = new SslStream(
                this.client.GetStream(),
                false,
                ValidateServerCertificate, // Delegate
                null); // TODO Last parameter might be superfluous.

            // The server name must match the name on the server certificate.
            try
            {
                Console.WriteLine("Client is authenticating server.");
                sslStream.AuthenticateAsClient(serverName);
                Console.WriteLine("Client authenticated server.");
                return sslStream;
            }
            catch (AuthenticationException e)
            {
                Console.WriteLine("Exception: {0}", e.Message);
                if (e.InnerException != null)
                {
                    Console.WriteLine("Inner exception: {0}", e.InnerException.Message);
                }
                Console.WriteLine("Authentication failed - closing the connection.");
                this.client.Close();

                // TODO better way to end.
                return null;
            }
        }

        /// <summary>
        /// Validates the server certificate.
        /// This method is invoked by the RemoteCertificateValidationDelegate used as
        /// a parameter for the SslStream-constructor.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="certificate"></param>
        /// <param name="chain"></param>
        /// <param name="sslPolicyErrors"></param>
        /// <returns></returns>
        private static bool ValidateServerCertificate(
              object sender,
              X509Certificate certificate,
              X509Chain chain,
              SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
            {
                return true;
            }

            Console.WriteLine("Certificate error: {0}", sslPolicyErrors);

            // Do not allow this client to communicate with unauthenticated servers.
            return false;
        }
    }
}
