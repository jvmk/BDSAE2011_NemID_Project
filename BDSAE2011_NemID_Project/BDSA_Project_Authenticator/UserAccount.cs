// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserAccount.cs" company="">
//   
// </copyright>
// <summary>
//   Represents the account each user has at the authenticator.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BDSA_Project_Authenticator
{
    /// <summary>
    /// Represents the account each user has at the authenticator.
    /// <author>Janus Varmarken</author>
    /// </summary>
    internal class UserAccount
    {
        /// <summary>
        /// The username of the user
        /// </summary>
        private string username;

        /// <summary>
        /// The password of the user
        /// </summary>
        private string password;

        /// <summary>
        /// The user's CPR-Number, social security number
        /// </summary>
        private string cprNumber;

        /// <summary>
        /// The user's email address.
        /// </summary>
        private string email;

        /// <summary>
        /// The user's current keycard
        /// </summary>
        private KeyCard keycard;

        /// <summary>
        /// Constructor for the userAccount class
        /// </summary>
        /// <param name="username">The username</param>
        /// <param name="password">The password</param>
        /// <param name="cprNumber">The CPR-number</param>
        /// <param name="email">The email address</param>
        public UserAccount(string username, string password, string cprNumber, string email)
        {
            this.username = username;
            this.password = password;
            this.cprNumber = cprNumber;
            this.email = email;
            this.keycard = new KeyCard(username, password, cprNumber, email);
        }

        /// <summary>
        /// Gets the username of the user
        /// </summary>
        public string Username
        {
            get
            {
                return this.username;
            }
        }

        /// <summary>
        /// Gets (TODO sets as well?) the user's password.
        /// </summary>
        public string Password
        {
            get
            {
                return this.password;
            }
        }

        /// <summary>
        /// Gets the user's email address..
        /// </summary>
        public string Email
        {
            get
            {
                return this.email;
            }
        }

        public KeyCard Keycard
        {
            get
            {
                return keycard;
            }
            set
            {
                keycard = value;
            }
        }

        public bool VerifyKeyNumber(uint keyNumber)
        {
            return this.keycard.VerifyEnteredKey(keyNumber);
        }
    }
}