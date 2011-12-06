// -----------------------------------------------------------------------
// <copyright file="PublicKeyInfrastructure.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace BDSAE2011_NemID_Project
{
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class PublicKeyInfrastructure
    {
        /// <summary>
        /// The collection to store the Xml strings and their ID
        /// </summary>
        private readonly Dictionary<string, string> keyCollection = new Dictionary<string, string>(); 

        /// <summary>
        /// Stores the specified public key in the PKI
        /// </summary>
        /// <param name="xmlKeyString">
        /// The xml Key String.
        /// </param>
        /// <param name="uniqueIdentifier">
        /// The unique Identifier. 
        /// </param>
        /// <returns>
        /// True if the key was succesfully stored, otherwise false
        /// </returns>
        public bool StoreKey(string xmlKeyString, string uniqueIdentifier)
        {
            Contract.Requires(xmlKeyString != null);
            if (!this.keyCollection.ContainsKey(xmlKeyString))
            {
                if (!this.keyCollection.ContainsValue(uniqueIdentifier))
                {
                    this.keyCollection.Add(uniqueIdentifier, xmlKeyString);
                }
            }

            return true;
        }

        /// <summary>
        /// Can I get the public key of this domain?
        /// </summary>
        /// <param name="uniqueIdentifier">The unique identifier for the domain</param>
        /// <returns>the corresponding key for the domain</returns>
        public string GetKey(string uniqueIdentifier)
        {
            Contract.Requires(this.keyCollection.ContainsKey(uniqueIdentifier));
            return this.keyCollection[uniqueIdentifier];
        }

        /// <summary>
        /// Removes the key from the PKI
        /// </summary>
        /// <param name="uniqueIdentifier"></param>
        /// <returns>True if the key is removed, otherwise false</returns>
        public bool RevokeKey(string uniqueIdentifier)
        {
            Contract.Requires(this.keyCollection.ContainsKey(uniqueIdentifier));
            Contract.Ensures(
                Contract.Result<bool>() == (this.keyCollection.Count == Contract.OldValue(this.keyCollection.Count) - 1));
            this.keyCollection.Remove(uniqueIdentifier);
        }

        /// <summary>
        /// Does this PKI contain a key from this domain?
        /// </summary>
        /// <param name="uniqueIdentifier">the unique ID for the domain</param>
        /// <returns>True if the key is present, otherwise false</returns>
        public bool ContainsKey(string uniqueIdentifier)
        {
            return this.keyCollection.ContainsKey(uniqueIdentifier);
        }

    }
}
