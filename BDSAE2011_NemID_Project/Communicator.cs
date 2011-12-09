// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Communicator.cs" company="NemID project group">
//   Open Source
// </copyright>
// <summary>
//   Defines the Communicator type.
// </summary>
// -------------------------------------------------------------------------------------------------------------------

//TODO OBSOLETE?

namespace ExamProject_COMMUNICATOR
{
    using System.Diagnostics.Contracts;
    using System.Net;
    using System.Text;

    /// <summary>
    /// Handles the authenticator’s communication with external components.
    /// </summary>
    internal delegate bool Hello(int i, double d);

    public class Communicator
    {
        /// <summary>
        ///  The HttpListener used to listen for requests
        /// </summary>
        private readonly HttpListener server = new HttpListener();

        /// <summary>
        /// TODO: Start the server from here?
        /// </summary>
        /// <param name="args">TODO: any params? </param>
        //public static void Main(string[] args)
        //{
            //Communicator authServer = new Communicator("localhost");
            //authServer.StartListening();
        //}

        public Communicator(string hostAndDomain)
        {
            Contract.Requires(!string.IsNullOrEmpty(hostAndDomain));
            this.PrepareServer(hostAndDomain);
        }

        private void StartListening()
        {
            this.server.Start();
            HttpListenerContext hlc = this.server.GetContext(); // Blocks while waiting for a request
            // Use HttpListenerContext.Request to analyze the request here...
            string responseStr = "<html><body>Hello, world!</body></html>";
            byte[] buf = Encoding.UTF8.GetBytes(responseStr);
            hlc.Response.OutputStream.Write(buf, 0, buf.Length);
            hlc.Response.OutputStream.Close();
            this.server.Stop(); // Stop the server
        }

        /// <summary>
        /// Adds a URI the server should listen to.
        /// </summary>
        /// <param name="uri">The host and domain name. Leave out protocol (http / https).</param>
        private void PrepareServer(string uri)
        {
            this.server.Prefixes.Add("https://" + uri + ":443/");
        }
    }
}
