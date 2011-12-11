// -----------------------------------------------------------------------
// <copyright file="ThirdParty.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace ThirdPartyComponent
{
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Represents the backend database of the third party server.
    /// </summary>
    internal class ThirdParty
    {

        private Dictionary<string, ThirdPartyUserAccount> users = new Dictionary<string, ThirdPartyUserAccount>(); 
        
        /// <summary>
        /// Initializes a new instance of the ThirdParty class.
        /// </summary>
        internal ThirdParty()
        {
            
        }

        internal void AddUserAccount(string username)
        {
            Contract.Requires(!users.ContainsKey(username));
            Contract.Ensures(users.ContainsKey(username));
            users.Add(username, new ThirdPartyUserAccount(username));
        }

        /// <summary>
        /// Submit a client token to the database.
        /// </summary>
        /// <param name="clientToken">The client token (nonce).</param>
        /// <param name="username">The username to look up in the database.</param>
        /// <returns>True if the username is in the database, the supplied token equals the expected token and the token has not timed out. False otherwise.</returns>
        internal bool CompareTokens(int clientToken, string username)
        {
            if (!this.users.ContainsKey(username))
            {
                return false;
            }
            return this.users[username].CompareTokens(clientToken);
        }

        /// <summary>
        /// Update the authenticator token for a specific account.
        /// </summary>
        /// <param name="username">username to identify the target account.</param>
        /// <param name="authToken">The value to update the authenticator token with.</param>
        internal void SetAuthTokenForAccount(string username, int authToken)
        {
            if (!this.users.ContainsKey(username))
            {
                return;
            }
            this.users[username].SetAuthToken(authToken);
        }
    }
}
