// -----------------------------------------------------------------------
// <copyright file="ThirdPartyUserAccount.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace BDSA_Project_ThirdParty
{
    using System;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Represents a user in the ThirdParty database.
    /// </summary>
    internal class ThirdPartyUserAccount
    {
        /// <summary>
        /// Username corresponding to username at the authenticator.
        /// </summary>
        private string username;

        /// <summary>
        /// The current authenticator supplied token (nonce) for this account.
        /// </summary>
        private string authToken;

        /// <summary>
        /// The last update of the authenticator token (nonce).
        /// </summary>
        private DateTime authTokenUpdate = DateTime.MinValue; // initialize to smallest value so that a CompareToken call at system start cannot grant access through knowledge of initial authToken value.

        /// <summary>
        /// Number of times the CompareTokens method has been called for the current authenticator token.
        /// Used to prevent tokenspam attacks.
        /// </summary>
        private uint tokenCompares = 0;

        /// <summary>
        /// Initializes a new instance of the ThirdPartyUserAccount class.
        /// </summary>
        /// <param name="username">The username of the new user (same as at the authenticator).</param>
        internal ThirdPartyUserAccount(string username)
        {
            Contract.Requires(!ReferenceEquals(username, null));
            this.username = username;
        }

        /// <summary>
        /// Gets the username of this user.
        /// </summary>
        internal string Username
        {
            get
            {
                return this.username;
            }
        }

        /// <summary>
        /// Sets the authenticator token (nonce) and sets the time of this update.
        /// </summary>
        /// <param name="token">The new value of the authenticator token (nonce).</param>
        internal void SetAuthToken(string token)
        {
            Contract.Requires(!ReferenceEquals(token, null));
            this.authToken = token;
            this.tokenCompares = 0; // Reset tokenCompares since a new token was provided.
            this.authTokenUpdate = DateTime.Now;
        }

        /// <summary>
        /// Compares a client token with the authenticator token.
        /// A check is performed to see if the auth token has been updated within the last minute.
        /// </summary>
        /// <param name="clientToken">The token (nonce) supplied by the client.</param>
        /// <returns>True if the client token is equal to the registered authenticator token.</returns>
        internal bool CompareTokens(string clientToken)
        {
            Contract.Requires(!ReferenceEquals(clientToken, null));
            this.tokenCompares++;
            return clientToken.Equals(this.authToken) && this.tokenCompares == 1
                    && DateTime.Now < this.authTokenUpdate.AddMinutes(1);
        }
    }
}
