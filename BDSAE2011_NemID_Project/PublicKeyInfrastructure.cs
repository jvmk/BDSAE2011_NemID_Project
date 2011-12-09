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
    public static class PublicKeyInfrastructure
    {
        /// <summary>
        /// A collection to store a unique ID corresponding to a specific key.
        /// </summary>
        private static readonly Dictionary<string, RSAParameters> keyCollection = new Dictionary<string, RSAParameters>(); 

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
        public static bool StoreKey(RSAParameters publicKeyParameters, string uniqueIdentifier)
        {
            Contract.Requires(!publicKeyParameters.Equals(null));
            if (!keyCollection.ContainsKey(uniqueIdentifier))
            {
                if (!keyCollection.ContainsValue(publicKeyParameters))
                {
                    keyCollection.Add(uniqueIdentifier, publicKeyParameters);
                }
            }

            return true;
        }

        /// <summary>
        /// Can I get the public key of this domain?
        /// </summary>
        /// <param name="uniqueIdentifier">The unique identifier for the domain</param>
        /// <returns>the corresponding key for the domain</returns>
        public static RSAParameters GetKey(string uniqueIdentifier)
        {
            Contract.Requires(keyCollection.ContainsKey(uniqueIdentifier));
            return keyCollection[uniqueIdentifier];
        }

        /// <summary>
        /// Removes the key from the PKI
        /// </summary>
        /// <param name="uniqueIdentifier">The unique identifier for the domain</param>
        /// <returns>True if the key is removed, otherwise false</returns>
        public static bool RevokeKey(string uniqueIdentifier)
        {
            Contract.Requires(keyCollection.ContainsKey(uniqueIdentifier));
            Contract.Ensures(
                Contract.Result<bool>() == (keyCollection.Count == Contract.OldValue(keyCollection.Count) - 1));
            return keyCollection.Remove(uniqueIdentifier);
        }

        /// <summary>
        /// Does this PKI contain a key from this domain?
        /// </summary>
        /// <param name="uniqueIdentifier">the unique ID for the domain</param>
        /// <returns>True if the key is present, otherwise false</returns>
        public static bool ContainsKey(string uniqueIdentifier)
        {
            return keyCollection.ContainsKey(uniqueIdentifier);
        }

        [ContractInvariantMethod]
        private static void PKIClassInvariant()
        {
            //// Each key must be unique
            //// each identifier must be unique
        }
    }
}
