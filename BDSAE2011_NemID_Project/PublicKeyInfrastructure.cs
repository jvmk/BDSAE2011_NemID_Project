// -----------------------------------------------------------------------
// <copyright file="PublicKeyInfrastructure.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace BDSAE2011_NemID_Project
{
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Security.Cryptography;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class PublicKeyInfrastructure
    {
        /// <summary>
        /// The collection to store the Xml strings and their ID
        /// </summary>
        private Dictionary<string, RSAParameters> keyCollection = new Dictionary<string, RSAParameters>(); 

        /// <summary>
        /// Stores the specified public key in the PKI
        /// </summary>
        /// <param name="publicKeyParameters">
        /// The public Key Parameters.
        /// </param>
        /// <param name="uniqueIdentifier">
        /// The unique Identifier. 
        /// </param>
        /// <returns>
        /// True if the key was succesfully stored, otherwise false
        /// </returns>
        public bool StoreKey(RSAParameters publicKeyParameters, string uniqueIdentifier)
        {
            Contract.Requires(!publicKeyParameters.Equals(null));
            if (!this.keyCollection.ContainsKey(uniqueIdentifier))
            {
                if (!this.keyCollection.ContainsValue(publicKeyParameters))
                {
                    this.keyCollection.Add(uniqueIdentifier, publicKeyParameters);
                }
            }

            return true;
        }

        /// <summary>
        /// Can I get the public key of this domain?
        /// </summary>
        /// <param name="uniqueIdentifier">The unique identifier for the domain</param>
        /// <returns>the corresponding key for the domain</returns>
        public RSAParameters GetKey(string uniqueIdentifier)
        {
            Contract.Requires(this.keyCollection.ContainsKey(uniqueIdentifier));
            return this.keyCollection[uniqueIdentifier];
        }

        /// <summary>
        /// Removes the key from the PKI
        /// </summary>
        /// <param name="uniqueIdentifier">The unique identifier for the domain</param>
        /// <returns>True if the key is removed, otherwise false</returns>
        public bool RevokeKey(string uniqueIdentifier)
        {
            Contract.Requires(this.keyCollection.ContainsKey(uniqueIdentifier));
            Contract.Ensures(
                Contract.Result<bool>() == (this.keyCollection.Count == Contract.OldValue(this.keyCollection.Count) - 1));
            return this.keyCollection.Remove(uniqueIdentifier);
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
