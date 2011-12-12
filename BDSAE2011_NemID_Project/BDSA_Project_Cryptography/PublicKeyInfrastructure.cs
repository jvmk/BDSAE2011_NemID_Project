// -----------------------------------------------------------------------
// <copyright file="PublicKeyInfrastructure.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace BDSA_Project_Cryptography
{
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Security.Cryptography;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public static class PublicKeyInfrastructure
    {
        /// <summary>
        /// A collection to store a unique ID corresponding to a specific key.
        /// </summary>
        private static readonly Dictionary<string, byte[]> KeyCollection = new Dictionary<string, byte[]>();

        /// <summary>
        /// Stores the specified public key in the PKI
        /// </summary>
        /// <param name="publicKey">
        /// The public Key.
        /// </param>
        /// <param name="uniqueIdentifier">
        /// The unique Identifier. 
        /// </param>
        /// <returns>
        /// True if the key was succesfully stored, otherwise false
        /// </returns>
        public static bool StoreKey(byte[] publicKey, string uniqueIdentifier)
        {
            Contract.Requires(publicKey[0] == 0x06);
            Contract.Requires(publicKey != null);
            Contract.Requires(uniqueIdentifier != null);
            Contract.Requires(uniqueIdentifier != string.Empty);
            bool success = false;
            if (!KeyCollection.ContainsKey(uniqueIdentifier))
            {
                if (!KeyCollection.ContainsValue(publicKey))
                {
                    KeyCollection.Add(uniqueIdentifier, publicKey);
                    success = true;
                }
            }
            return success;
        }

        /// <summary>
        /// Can I get the public key of this domain?
        /// </summary>
        /// <param name="uniqueIdentifier">The unique identifier for the domain</param>
        /// <returns>the corresponding key for the domain</returns>
        [Pure]
        public static byte[] GetKey(string uniqueIdentifier)
        {
            Contract.Requires(uniqueIdentifier != null);
            Contract.Requires(uniqueIdentifier != string.Empty);
            Contract.Requires(ContainsKey(uniqueIdentifier));
            return KeyCollection[uniqueIdentifier];
        }

        /// <summary>
        /// Removes the key from the PKI
        /// </summary>
        /// <param name="uniqueIdentifier">The unique identifier for the domain</param>
        /// <returns>True if the key is removed, otherwise false</returns>
        public static bool RevokeKey(string uniqueIdentifier)
        {
            Contract.Requires(uniqueIdentifier != null);
            Contract.Requires(uniqueIdentifier != string.Empty);
            Contract.Requires(ContainsKey(uniqueIdentifier));
            Contract.Ensures(Contract.Result<bool>() == (KeyCollection.Count == Contract.OldValue(KeyCollection.Count) - 1));
            return KeyCollection.Remove(uniqueIdentifier);
        }

        /// <summary>
        /// Does this PKI contain a key from this domain?
        /// </summary>
        /// <param name="uniqueIdentifier">the unique ID for the domain</param>
        /// <returns>True if the key is present, otherwise false</returns>
        [Pure]
        public static bool ContainsKey(string uniqueIdentifier)
        {
            Contract.Requires(uniqueIdentifier != null);
            Contract.Requires(uniqueIdentifier != string.Empty);
            return KeyCollection.ContainsKey(uniqueIdentifier);
        }

        public static bool ContainsValue(byte[] publicKey)
        {
            Contract.Requires(publicKey != null);
            return KeyCollection.ContainsValue(publicKey);
        }

        /// <summary>
        /// Indicates if the passed byte[] represents a valid public key header
        /// </summary>
        /// <param name="keyBlob">The blob containing the key info</param>
        /// <returns>true if the blob has the signature of a public key blob</returns>
        public static bool ValidKeyBlob(IEnumerable<byte> keyBlob)
        {
            byte[] publicKeyTemplate = { 6, 2, 0, 0, 0, 164, 0, 0, 82, 83, 65, 49, 0, 16, 0, 0, 1, 0, 1, 0 };

            //// Compare the first 20 elements of the keyBlob collection to the template and see if they matches
            return publicKeyTemplate.Take(20).SequenceEqual(keyBlob.Take(20));
        }

        [ContractInvariantMethod]
        private static void PKIClassInvariant()
        {
            //// Each key must be unique
            //// each identifier must be unique
        }
    }
}
