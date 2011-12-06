//-

namespace AuthenticatorComponent
{
    /// <summary>
    /// Represents the account each user has at the authenticator.
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
        /// The user's current keycard
        /// </summary>
        private KeyCard keycard;

        /// <summary>
        /// Constructor for the userAccount class
        /// </summary>
        /// <param name="username">The username</param>
        /// <param name="password">The password</param>
        /// <param name="cprNumber">The CPR-number</param>
        public UserAccount(string username, string password, string cprNumber)
        {
            this.username = username;
            this.password = password;
            this.cprNumber = cprNumber;
            this.keycard = new KeyCard();
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