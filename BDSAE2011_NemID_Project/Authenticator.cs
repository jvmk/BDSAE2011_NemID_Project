
namespace AuthenticatorComponent
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Security.Cryptography;

    using AuthenticationService;

    /// <summary>
    /// The component that mimics DANID in the current NemId-solution.
    /// </summary>
    public class Authenticator
    {
        /// <summary>
        /// Represents the underlying database where user information is stored.
        /// </summary>
        private Dictionary<string, UserAccount> database = new Dictionary<string, UserAccount>(); // the string is the username for the associated useraccount

        private RSAParameters authPrivKeyPath = new RSAParameters(); // TODO update this path // TODO find out what to do about the keytype

        public Authenticator()
        {
            // TODO load persisted data (database).
        }

        /// <summary>
        /// Is there a user in the database with this username?
        /// </summary>
        /// <param name="username">Username to look up in database (encrypted).</param>
        /// <returns>True if the username is registered in the database, false otherwise.</returns>
        [Pure]
        private bool IsUserInDatabase(string username)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(username)); // TODO all contracts in this class compromised since data recieved is encrypted data
            return this.database.ContainsKey(this.DecryptThisMessage(username));
        }

        /// <summary>
        /// Looks up in the database and compares if entetered username and password are valid.
        /// </summary>
        /// <param name="username">Entered username.</param>
        /// <param name="password">Entered password.</param>
        /// <returns>True if the parametres correspond to database values, false otherwise.</returns>
        [Pure]
        public bool IsLoginValid(string username, string password)
        {
            Contract.Requires(this.IsUserInDatabase(username));
            Contract.Requires(!string.IsNullOrWhiteSpace(password)); // !string.IsNullOrWhiteSpace(username) checked in contract for IsUserInDatabase
            return this.database[this.DecryptThisMessage(username)].Password.Equals(this.DecryptThisMessage(password));
        }

        /// <summary>
        /// What is the keyindex of the key the user has to enter?
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public string GetKeyIndex(string username)
        {
            Contract.Requires(this.IsUserInDatabase(username));
            return this.database[this.DecryptThisMessage(username)].Keycard.GetKeyIndex().ToString();
        }

        /// <summary>
        /// Checks if the entered hash value corresponds to the excpected one.
        /// </summary>
        /// <param name="submittedHash">The value the user submitted.</param>
        /// <param name="username">The user that the hash value corresponds to.</param>
        /// <returns></returns>
        public bool IsHashValueValid(string submittedHash, string username)
        {
            Contract.Requires(this.IsUserInDatabase(username));
            Contract.Requires(!string.IsNullOrWhiteSpace(username));

            uint parsedHash = uint.Parse(this.DecryptThisMessage(submittedHash));

            return database[this.DecryptThisMessage(username)].VerifyKeyNumber(parsedHash);
        }

        public bool AddNewUser(string username, string password, string cprNumber)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password) && !string.IsNullOrWhiteSpace(cprNumber));
            Contract.Ensures(this.IsUserInDatabase(username));
            if (this.IsUserInDatabase(username)) return false; // decprytion performed in IsUserInDatabase
            username = this.DecryptThisMessage(username); // decrypt all 3 parametres before inserting into database...
            password = this.DecryptThisMessage(password);
            cprNumber = this.DecryptThisMessage(cprNumber);
            this.database.Add(username, new UserAccount(username, password, cprNumber));
            return true;
        }

        public bool DeleteUser(string username)
        {
            Contract.Requires(this.IsUserInDatabase(username));
            Contract.Ensures(!this.IsUserInDatabase(username));
            database.Remove(this.DecryptThisMessage(username));
            return true;
        }

        // TODO Should this really be here? Should this be a method? Should be in AuthHttpProcessor
        private void RecieveRedirectionFrom3rdParty()
        {

        }

        // TODO Implement method... should also be in AuthHttpProcessor
        private void SendTokenTo3rdPartyAndUser()
        {

        }

        /// <summary>
        /// Writes the keycard to a local file.
        /// </summary>
        /// <param name="username">The encrypted username.</param>
        private void SendKeyCardToUser(string username)
        {
            Contract.Requires(this.IsUserInDatabase(username));
            String keycardPrint = this.database[this.DecryptThisMessage(username)].Keycard.ToString();
            File.WriteAllText(@"C:\" + username +
                this.database[this.DecryptThisMessage(username)].Keycard.GetKeyCardNumber() + 
                ".txt", keycardPrint);
        }

        private String DecryptThisMessage(string encryptedMessage)
        {
            // Decrypt first layer using own private key
            encryptedMessage = Cryptograph.Decrypt(encryptedMessage, authPrivKeyPath);
            // Decrypt second layer using senders public key
            String decryptedMessage = Cryptograph.Decrypt(encryptedMessage, authPrivKeyPath); // TODO How to obtain public key? We don't know who is the sender here... // TODO FIX THIS SIMON!
            return decryptedMessage;
        }

        [ContractInvariantMethod]
        private void AuthenticatorInvariant()
        {
            // It must hold that the username used to look up a user account is equal to the username registered with that useraccount
            Contract.Invariant(Contract.ForAll<string>(this.database.Keys, k => this.database[k].Username.Equals(k)));
            Contract.Invariant(Contract.ForAll<string>(this.database.Keys, k => !string.IsNullOrWhiteSpace(k)));
        }
    }
}