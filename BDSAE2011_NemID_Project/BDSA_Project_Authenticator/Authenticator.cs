
namespace BDSA_Project_Authenticator
{
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.IO;

    /// <summary>
    /// The backend database of the authenticator. Stores user accounts.
    /// </summary>
    public class Authenticator
    {
        /// <summary>
        /// Represents the underlying database where user information is stored.
        /// The key is the user name and the UserAccount is the account associated
        /// with that user.
        /// </summary>
        private Dictionary<string, UserAccount> database =
            new Dictionary<string, UserAccount>();

        /// <summary>
        /// A set of URIs that this authenticator trusts.
        /// </summary>
        private HashSet<string> trustedThirdPartyURIs = new HashSet<string>();

        /// <summary>
        /// 
        /// </summary>
        private string fileLocation = @".\";

        /// <summary>
        /// Initializes a new instance of the Authenticator class.
        /// </summary>
        public Authenticator()
        {
            // Add a test user. For demonstration purposed only.
            this.AddNewUser("testUser", "password", "010101-0101", "testUser@nemId.dk");
        }

        /// <summary>
        /// Is there a user in the database with this username?
        /// </summary>
        /// <param name="username">Username to look up.</param>
        /// <returns>True if the username is registered in the database, false otherwise.</returns>
        [Pure]
        public bool IsUserInDatabase(string username)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(username));
            return this.database.ContainsKey(username);
        }

        /// <summary>
        /// Get all the user names of the users current
        /// registered in the authenticator's database.
        /// </summary>
        /// <returns>An array respresentation of all user names in the database.</returns>
        public string[] GetAllUsers()
        {
            string[] keysArray = new string[this.database.Count];
            this.database.Keys.CopyTo(keysArray, 0);
            return keysArray;
        }

        /// <summary>
        /// Check with the database whether the submitted combination of username and password is valid.
        /// </summary>
        /// <param name="username">Submitted username.</param>
        /// <param name="password">Submitted password.</param>
        /// <returns>True if the parametres correspond to database values, false otherwise.</returns>
        [Pure]
        public bool IsLoginValid(string username, string password)
        {
            //// Contract.Requires(this.IsUserInDatabase(username));
            //// Contract.Requires(!string.IsNullOrWhiteSpace(password)); // !string.IsNullOrWhiteSpace(username) checked in contract for IsUserInDatabase
            return this.database[username].Password.Equals(password);
        }

        /// <summary>
        /// What is the keyindex of the key the user has to enter?
        /// </summary>
        /// <param name="username">The username used to identify the user in the database.</param>
        /// <returns>The keyindex corresponding to the expected key value.</returns>
        public string GetKeyIndex(string username)
        {
            //// Contract.Requires(this.IsUserInDatabase(username));
            return this.database[username].Keycard.GetKeyIndex().ToString();
        }

        /// <summary>
        /// Checks if the entered key value corresponds to the excpected one.
        /// </summary>
        /// <param name="submittedKeycardValue">The key value the user submitted.</param>
        /// <param name="username">The user that the submitted key value corresponds to.</param>
        /// <returns>True if the key value equals the expected key value, false otherwise.</returns>
        public bool IsKeycardValueValid(string submittedKeycardValue, string username)
        {
            //// Contract.Requires(this.IsUserInDatabase(username));
            Contract.Requires(!string.IsNullOrWhiteSpace(username));
            uint parsedHash = uint.Parse(submittedKeycardValue);
            return this.database[username].VerifyKeyNumber(parsedHash);
        }

        /// <summary>
        /// Add a new user to the database.
        /// </summary>
        /// <param name="username">The username to associate with the new user account.</param>
        /// <param name="password">The password for the new user account.</param>
        /// <param name="cprNumber">The CPR number of this user.</param>
        /// <returns>True if the user is successfully added to the database. False if the username is already taken.</returns>
        public bool AddNewUser(string username, string password, string cprNumber, string email)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password) && !string.IsNullOrWhiteSpace(cprNumber));
            Contract.Ensures(this.IsUserInDatabase(username));
            if (this.IsUserInDatabase(username))
            {
                return false;
            }

            this.database.Add(username, new UserAccount(username, password, cprNumber, email));
            this.SendKeyCardToUser(username); // Simulate that the keycard is send by snail mail
            return true;
        }

        /// <summary>
        /// Delete the useraccount with the provided username.
        /// </summary>
        /// <param name="username">The username that determines the account to delete.</param>
        /// <returns>True if the username exists in the database and the corresponding account is successfully removed.
        /// False if the username is not found in the database.</returns>
        public bool DeleteUser(string username)
        {
            Contract.Ensures(!this.IsUserInDatabase(username));
            return this.database.Remove(username); // return false if username was not found
        }

        /// <summary>
        /// Is this third party URI an authenticator-trusted third party?
        /// </summary>
        /// <param name="thirdPartyUri">URI in question</param>
        /// <returns>True if the thirdPartyUri is trusted, false otherwise.</returns>
        public bool IsThisThirdPartyTrusted(string thirdPartyUri)
        {
            return this.trustedThirdPartyURIs.Contains(thirdPartyUri);
        }

        /// <summary>
        /// Writes the keycard to a local file.
        /// </summary>
        /// <param name="username">The encrypted username.</param>
        private void SendKeyCardToUser(string username)
        {
            Contract.Requires(this.IsUserInDatabase(username));
            string keycardPrint = this.database[username].Keycard.ToString();
            if (!Directory.Exists(this.fileLocation))
            {
                Directory.CreateDirectory(this.fileLocation);
            }
            File.WriteAllText(
                fileLocation + username +
                this.database[username].Keycard.GetKeyCardNumber() +
                ".txt",
                keycardPrint);
        }

        [ContractInvariantMethod]
        private void AuthenticatorInvariant()
        {
            // It must hold that the username used to look up a user account is equal to the username registered in that useraccount
            Contract.Invariant(Contract.ForAll<string>(this.database.Keys, k => this.database[k].Username.Equals(k)));
            Contract.Invariant(Contract.ForAll<string>(this.database.Keys, k => !string.IsNullOrWhiteSpace(k)));
        }
    }
}