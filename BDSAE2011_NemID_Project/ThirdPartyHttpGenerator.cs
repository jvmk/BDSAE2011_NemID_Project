// -----------------------------------------------------------------------
// <copyright file="ThirdPartyHttpGenerator.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace ThirdPartyComponent
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using ClientComponent;

    /// <summary>
    /// Class used to generate and send http messages to the third party server.
    /// </summary>
    public class ThirdPartyHttpGenerator
    {
        /// <summary>
        /// ClientSocket used to communicate with the the ThirdPartyServer.
        /// </summary>
        private ClientSocket clientSocket;
        
        /// <summary>
        /// Initializes a new instance of the ThirdPartyHttpGenerator class.
        /// </summary>
        /// <param name="serverUri">The URI of the third party server.</param>
        /// <param name="clientPkiId">The PKI identifier of the client sending the messages.</param>
        public ThirdPartyHttpGenerator(string serverUri, string clientPkiId)
        {
            this.clientSocket = new ClientSocket(serverUri, clientPkiId);
        }

        public bool SubmitUsername(string username)
        {
            this.clientSocket.SendMessage("loginpage", 
                "username=" + username);
            Response r = this.clientSocket.ReadMessage();
            return r.Accepted;
        }

        public bool SubmitUserToken(string username, int userToken)
        {
            this.clientSocket.SendMessage("usertoken", "username=" + username + "&"
               + "usertoken=" + userToken);
            Response r = this.clientSocket.ReadMessage();
            return r.Accepted;
        }
    }
}
