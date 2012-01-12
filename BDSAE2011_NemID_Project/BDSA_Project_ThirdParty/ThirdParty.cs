// -----------------------------------------------------------------------
// <copyright file="ThirdParty.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace BDSA_Project_ThirdParty
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Represents the backend database of the third party server.
    /// <author>Janus Varmarken</author>
    /// </summary>
    internal class ThirdParty
    {

        /// <summary>
        /// The users in this third party's database.
        /// </summary>
        private readonly Dictionary<string, ThirdPartyUserAccount> users = new Dictionary<string, ThirdPartyUserAccount>();

        /// <summary>
        /// Initializes a new instance of the ThirdParty class.
        /// </summary>
        internal ThirdParty()
        {
            
        }

        /// <summary>
        /// Add a new user to the database.
        /// </summary>
        /// <param name="username">The new users username.</param>
        /// <param name="pkiIdEmail">The PKI id to associate with this account.</param>
        internal bool AddUserAccount(string username, string pkiIdEmail)
        {
            Contract.Requires(!ReferenceEquals(username, null));
            Contract.Ensures(this.users.ContainsKey(username));
            if (this.users.ContainsKey(username))
            {
                return false;
            }

            this.users.Add(username, new ThirdPartyUserAccount(username, pkiIdEmail));
            return true;
        }

        /// <summary>
        /// Remove a user from the Third Party user database.
        /// </summary>
        /// <param name="username">The username of the user that is to be removed.</param>
        /// <returns>True if the username was in the database and successfully removed, false if the username was not found in the database.</returns>
        internal bool DeleteUserAccount(string username)
        {
            Contract.Ensures(!users.ContainsKey(username));
            return this.users.Remove(username);

        }

        /// <summary>
        /// Is this username registered in the database?
        /// </summary>
        /// <param name="username">Username to look for in the database.</param>
        /// <returns>True if the username is already in the database, false otherwise.</returns>
        internal bool ContainsUsername(string username)
        {
            Contract.Requires(!ReferenceEquals(username, null));

            return this.users.ContainsKey(username);
        }

        /// <summary>
        /// Submit a client token to the database.
        /// </summary>
        /// <param name="clientToken">The client token (nonce).</param>
        /// <param name="username">The username to look up in the database.</param>
        /// <returns>True if the username is in the database, the supplied token equals the expected token and the token has not timed out. False otherwise.</returns>
        internal bool CompareTokens(string clientToken, string username)
        {
            Contract.Requires(!ReferenceEquals(clientToken, null));
            Contract.Requires(!ReferenceEquals(username, null));
            if (!this.users.ContainsKey(username))
            {
                return false;
            }

            return this.users[username].CompareTokens(clientToken);
        }

        /// <summary>
        /// Retrieve the PKI identifier email for the specified username.
        /// </summary>
        /// <param name="username">The username for which the associated PKI identifier email is to be retrieved.</param>
        /// <returns>The PKI identifier email for the specified username.</returns>
        internal string PkiIdForAccount(string username)
        {
            Contract.Requires(this.users.ContainsKey(username));
            return this.users[username].PkiIdForAccount;
        }

        /// <summary>
        /// Update the authenticator token for a specific account.
        /// </summary>
        /// <param name="username">username to identify the target account.</param>
        /// <param name="authToken">The value to update the authenticator token with.</param>
        internal bool SetAuthTokenForAccount(string username, string authToken)
        {
            Contract.Requires(!ReferenceEquals(username, null));
            Contract.Requires(!ReferenceEquals(authToken, null));
            if (!this.users.ContainsKey(username))
            {
                return false;
            }

            this.users[username].SetAuthToken(authToken);
            return true;
        }

        [ContractInvariantMethod]
        private void ThirdPartyContract()
        {
            // It must hold that the username used to look up a user account is equal to the username registered in that useraccount
            Contract.Invariant(Contract.ForAll<string>(this.users.Keys, k => this.users[k].Username.Equals(k)));
            Contract.Invariant(Contract.ForAll<string>(this.users.Keys, k => !string.IsNullOrWhiteSpace(k)));
        }
    }
}
