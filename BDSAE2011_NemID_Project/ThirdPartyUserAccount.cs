// -----------------------------------------------------------------------
// <copyright file="ThirdPartyUserAccount.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace ThirdPartyComponent
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    internal class ThirdPartyUserAccount
    {
        private string username;

        private string authToken;

        private DateTime authTokenUpdate = DateTime.Now;
        
        /// <summary>
        /// Initializes a new instance of the ThirdPartyUserAccount class.
        /// 
        /// </summary>
        /// <param name="username">The username of the new user (same as at the authenticator).</param>
        internal ThirdPartyUserAccount(string username)
        {
            this.username = username;
        }

        /// <summary>
        /// Sets the authenticator token and sets the instant in time when this updates was carried out.
        /// </summary>
        /// <param name="token"></param>
        internal void SetAuthToken(string token)
        {
            authToken = token;
            authTokenUpdate = DateTime.Now;
        }

        /// <summary>
        /// Compares a client token with the authenticator token.
        /// A check is performed to see if the auth token has been updated within the last minute.
        /// <param name="clientToken">The token supplied by the client.</param>
        /// <returns>True if the client token is equal to the registered authenticator token.</returns>
        /// </summary>
        internal bool CompareTokens(string clientToken)
        {
            if (this.authTokenUpdate.AddMinutes(1) < DateTime.Now)
            {
                return false;
            }

            if (clientToken.Equals(this.authToken))
            {
                return true;
            }

            return false;
        }
    }
}
