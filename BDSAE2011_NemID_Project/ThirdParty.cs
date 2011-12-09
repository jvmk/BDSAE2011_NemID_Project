// -----------------------------------------------------------------------
// <copyright file="ThirdParty.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace ThirdPartyComponent
{
    using Communication;

    /// <summary>
    /// The 3rd party the user wants a secure and authenticated connection to.
    /// </summary>
    public class ThirdParty
    {
        /// <summary>
        /// Handles network communication to the third party.
        /// </summary>
        private AuthenticatorSocket server;

        /// <summary>
        /// The URI of the authenticator server.
        /// </summary>
        private static readonly string AUTH_URI = ""; // TODO update URI

        /// <summary>
        /// Indicates if the server is in service.
        /// </summary>
        private bool inService = false;

        /// <summary>
        /// Initializes a new instance of the ThirdParty class.
        /// This simulates the third party's server.
        /// </summary>
        /// <param name="port">The server port.</param>
        public ThirdParty(int port)
        {
            this.server = new AuthenticatorSocket(port);
        }
        
        /// <summary>
        /// Start listening for and process client requests.
        /// </summary>
        public void StartServer()
        {
            /*
            inService = true;
            while (this.inService)
            {
                server.ListenForRequests();
                string clientRequest = this.server.ReadMessage();
                switch (clientRequest)
                {
                    case "login":
                        this.server.SendMessage(AUTH_URI);
                        break;
                    case "token":
                        // Ask authenticator for token
                        // Read Token from authenticator
                        // Read Token from this client message
                        // Compare tokens
                        // Allow entry / deny entry
                    default:
                        // TODO do nothing..?
                }
            }
             */
        }

        /// <summary>
        /// Stop listening for client requests.
        /// </summary>
        public void ShutdownServer()
        {
            this.inService = false;
        }

        /// <summary>
        /// Are the client supplied and the authenticator supplied tokens equal?
        /// </summary>
        /// <param name="clientToken">The client supplied token.</param>
        /// <param name="authenticatorToken">The authenticator supplied token.</param>
        /// <returns>True if the two tokens are equal.</returns>
        private bool CompareTokens(int clientToken, int authenticatorToken)
        {
            return clientToken == authenticatorToken;
        }

    }
}
