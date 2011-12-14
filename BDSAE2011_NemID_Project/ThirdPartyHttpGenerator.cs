// -----------------------------------------------------------------------
// <copyright file="ThirdPartyHttpGenerator.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace BDSA_Project_ThirdParty
{
    using BDSA_Project_Communication;

    /// <summary>
    /// Class used to generate and send http messages to the third party server.
    /// <author>Janus Varmarken</author>
    /// </summary>
    public class ThirdPartyHttpGenerator
    {
        /// <summary>
        /// ClientSocket used to communicate with the the ThirdPartyServer.
        /// </summary>
        private readonly ClientSocket clientSocket;
        
        /// <summary>
        /// Initializes a new instance of the ThirdPartyHttpGenerator class.
        /// </summary>
        /// <param name="serverUri">The URI of the third party server.</param>
        /// <param name="clientPkiId">The PKI identifier of the client sending the messages.</param>
        /// <param name="clientPrivKey">The private key of the client sending the messages.</param>
        public ThirdPartyHttpGenerator(string serverUri, string clientPkiId, byte[] clientPrivKey)
        {
            this.clientSocket = new ClientSocket(serverUri, clientPkiId, clientPrivKey);
        }

        /// <summary>
        /// Submits a username to the ThirdParty loginpage.
        /// </summary>
        /// <param name="username">Username to submit.</param>
        /// <returns>True if the username is recognized by the third party, false otherwise.</returns>
        public bool SubmitUsername(string username)
        {
            this.clientSocket.SendMessage("loginpage", "username=" + username);
            Response r = this.clientSocket.ReadMessage();
            return r.Accepted;
        }

        /// <summary>
        /// Submits a user supplied token (nonce) to the ThirdParty in order to complete authentication.
        /// </summary>
        /// <param name="username">Username the token (nonce) is associated to.</param>
        /// <param name="userToken">The user's token (nonce) to submit</param>
        /// <returns>True if the token is accepted and authention is successful. False if the token is not valid.</returns>
        public bool SubmitUserToken(string username, string userToken)
        {
            this.clientSocket.SendMessage(
                "usertoken",
                "username=" + username + "&" + "usertoken=" + userToken);
            Response r = this.clientSocket.ReadMessage();
            return r.Accepted;
        }
    }
}
