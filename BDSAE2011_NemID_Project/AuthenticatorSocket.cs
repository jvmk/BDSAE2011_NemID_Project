// -----------------------------------------------------------------------
// <copyright file="AuthenticatorSocket.cs" company="">
// TODO: Update copyright text.
// </copyright>
// ----------------------------------------------------------------------

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
    using System.Threading;

    /// <summary>
    /// TODO: Update summary.
    /// THE CLASS IS CALLED 'HTTPINTERPRETER' IN THE BON-SPECIFICATION.
    /// </summary>
    public class AuthenticatorSocket
    {
        /// <summary>
        /// http://msdn.microsoft.com/en-us/library/ms143998(v=VS.80).aspx
        /// Ip-address of the local host.
        /// </summary>
        private readonly IPAddress ipAddress = Dns.GetHostEntry("localhost").AddressList[0];

        /// <summary>
        /// Represents the server instance.
        /// </summary>
        private readonly TcpListener server;

        /// <summary>
        /// The certificate of the server
        /// </summary>
        private readonly X509Certificate serverCertificate;

        /// <summary>
        /// The path to the server certificate.
        /// </summary>
        private readonly string certificatePath = @"C:\Users\Kenneth88\Desktop\danIDCertificate.cer";

        /// <summary>
        /// Represents the validated connection between the authenticator
        /// and the client.
        /// </summary>
        private SslStream sslStream;

        /// <summary>
        /// Initializes a new instance of the AuthenticatorSocket class.
        /// </summary>
        /// <param name="port">
        /// The port with the socket listens to request from.
        /// </param>
        public AuthenticatorSocket(int port)
        {
            // TODO GENERATE CERTIFICATE.
            this.serverCertificate = X509Certificate.CreateFromCertFile(this.certificatePath);
            this.server = new TcpListener(this.ipAddress, port);
            this.server.Start();
            Console.WriteLine("Server started.");
        }

        /// <summary>
        /// Makes the socket listen for client requests.
        /// Blocks until a client connects and a ssl-connection
        /// between the parties has been established.
        /// </summary>
        public void ListenForRequests()
        {
            this.sslStream = this.GetClientStream();
            Console.WriteLine("Client connected, validated connection established.");
        }

        /// <summary>
        /// Listens for client requests
        /// </summary>
        /// <returns>
        /// Returns a secure SslStream between the authenticator and the client.
        /// </returns>
        private SslStream GetClientStream()
        {
            Console.WriteLine("Server waiting for clients to connect.");

            // Returns when a client connects
            TcpClient client = this.server.AcceptTcpClient();

            Console.WriteLine("Client connected, initiate ssl establishment.");

            SslStream sslStream = new SslStream(client.GetStream(), false);
            this.AuthenticateAsServer(sslStream);

            return sslStream;
        }


        /// <summary>
        /// Authenticates the SslStream.
        /// </summary>
        /// <param name="stream">
        /// The SSL stream to be authenticated.
        /// </param>
        private void AuthenticateAsServer(SslStream stream)
        {
            try
            {
                Thread.Sleep(5000);
                stream.AuthenticateAsServer(this.serverCertificate);
            }
            catch (AuthenticationException)
            {
                Console.WriteLine("Server authentication failed.");
            }
        }


        /// <summary>
        /// Read a message from the speciefied SSL stream.
        /// </summary>
        /// <returns>
        /// The message sent by the client.
        /// </returns>
        public string ReadMessage()
        {
            // Read the message sent by the client.
            byte[] buffer = new byte[2048];
            StringBuilder messageData = new StringBuilder();
            int bytes = -1;

            do
            {
                // Read the client's test message.
                bytes = this.sslStream.Read(buffer, 0, buffer.Length);

                // Use Decoder class to convert from bytes to UTF8
                // in case a character spans two buffers.
                Decoder decoder = Encoding.UTF8.GetDecoder();
                char[] chars = new char[decoder.GetCharCount(buffer, 0, bytes)];
                decoder.GetChars(buffer, 0, bytes, chars, 0);
                messageData.Append(chars);

                // Check for EOF or an empty message.
                if (messageData.ToString().IndexOf("<EOF>") != -1)
                {
                    break;
                }
            }
            while (bytes != 0);

            Console.WriteLine("Server received message: " + messageData);

            return messageData.ToString();
        }

        /// <summary>
        /// Sends the specified message over the specified stream
        /// </summary>
        /// <param name="message">
        /// Message to be wrote.
        /// </param>
        public void SendMessage(string message)
        {
            byte[] messageBytes = Encoding.UTF8.GetBytes(message + "<EOF>");
            Console.WriteLine("Server responding to client message.");
            this.sslStream.Write(messageBytes);
            this.sslStream.Flush();
        }

        // TODO closing of stream and sockets.

        /*
        public static void Main()
        {
            //System.Diagnostics.ProcessStartInfo procStartInfo =
            // new System.Diagnostics.ProcessStartInfo("cmd", "/c " + "makecert testCert.cer");
        }
         */

    }
}
