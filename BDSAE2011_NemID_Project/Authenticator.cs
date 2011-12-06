﻿//-

namespace AuthenticatorComponent
{
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// The component that mimics DANID in the current NemId-solution.
    /// </summary>
    public class Authenticator
    {
        /// <summary>
        /// Represents the underlying database where user information is stored.
        /// </summary>
        private Dictionary<string, UserAccount> database = new Dictionary<string, UserAccount>(); // the string is the username for the associated useraccount

        /// <summary>
        /// Is there a user in the database with this username?
        /// </summary>
        /// <param name="username">Username to look up in database.</param>
        /// <returns>True if the username is registered in the database, false otherwise.</returns>
        [Pure]
        private bool IsUserInDatabase(string username)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(username));
            return this.database.ContainsKey(username);
        }

        /// <summary>
        /// Looks up in the database and compares if entetered username and password are valid.
        /// </summary>
        /// <param name="username">Entered username.</param>
        /// <param name="password">Entered password.</param>
        /// <returns>True if the parametres correspond to database values, false otherwise.</returns>
        [Pure]
        public bool IsLoginValid(string username, string password) //// TODO; Check visibility, mvh Simon, was private
        {
            Contract.Requires(this.IsUserInDatabase(username));
            Contract.Requires(!string.IsNullOrWhiteSpace(password)); // !string.IsNullOrWhiteSpace(username) checked in contract for IsUserInDatabase
            // decrypt ciphertext here...
            return this.database[username].Password.Equals(password);
        }

        /// <summary>
        /// What is the
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public string GetKeyIndex(string username) //// TODO: FIX VISIBILITY I just changed the visibility to private, since it was needed by the HTTPProcessor and I wanted it to compile, mvh Simon
        {
            //decrypt username here...
            return this.database[username].Keycard.GetKeyIndex().ToString("D4");
        }

        /// <summary>
        /// Checks if the entered hash value corresponds to the excpected one.
        /// </summary>
        /// <param name="submittedHash">The value the user submitted.</param>
        /// <param name="username">The user that the hash value corresponds to.</param>
        /// <returns></returns>
        private bool IsHashValueValid(uint submittedHash, string username)
        {
            Contract.Requires(this.IsUserInDatabase(username));
            Contract.Requires(!string.IsNullOrWhiteSpace(username));
            return database[username].VerifyKeyNumber(submittedHash);
        }

        private bool AddNewUser(string username, string password, string cprNumber)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password) && !string.IsNullOrWhiteSpace(cprNumber));
            Contract.Ensures(this.IsUserInDatabase(username));
            if (this.IsUserInDatabase(username)) return false;
            this.database.Add(username, new UserAccount(username, password, cprNumber));
            return true;
        }

        private void DeleteUser(string username)
        {
            Contract.Requires(this.IsUserInDatabase(username));
            Contract.Ensures(!this.IsUserInDatabase(username));
            database.Remove(username);
        }

        // TODO Should this really be here? Should this be a method?
        private void RecieveRedirectionFrom3rdParty()
        {

        }

        // TODO Implement method...
        private void SendTokenTo3rdPartyAndUser()
        {

        }

        // TODO Implement method
        private void SendKeyCardToUser(string username)
        {
            // call ToString on keycard and write to local file
        }

        // TODO Is this really needed? Alternatively make AddNewUser have return type bool.
        private void AcknowledgeAccountCreation()
        {
            // 
        }

        // TODO implement method
        private void AcknowledgeLoginRequest()
        {

        }

        // TODO Is this really needed?
        private void AcknowledgeSubmittedKeycardNumber()
        {

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