// -----------------------------------------------------------------------
// <copyright file="PublicKeyInfrastructure.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace BDSA_Project_Cryptography
{
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Linq;
    
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public static class PublicKeyInfrastructure
    {
        /// <summary>
        /// The path at which to store the collection of keys.
        /// </summary>
        private const string DatabasePath = @"C:\Test\PKIFile.bin";

        /// <summary>
        /// A collection to store a unique ID corresponding to a specific key.
        /// </summary>
        private static Dictionary<string, byte[]> keyCollection = new Dictionary<string, byte[]>();

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
            keyCollection = ReadFromFile(DatabasePath);
            return keyCollection[uniqueIdentifier];
        }

        /// <summary>
        /// Does the PKI contain a key from this domain?
        /// </summary>
        /// <param name="uniqueIdentifier">the unique ID for the domain</param>
        /// <returns>True if the key is present, otherwise false</returns>
        [Pure]
        public static bool ContainsKey(string uniqueIdentifier)
        {
            Contract.Requires(uniqueIdentifier != null);
            Contract.Requires(uniqueIdentifier != string.Empty);
            keyCollection = ReadFromFile(DatabasePath);
            return keyCollection.ContainsKey(uniqueIdentifier);
        }

        /// <summary>
        /// Does the PKI contain this key?
        /// </summary>
        /// <param name="publicKey">The key to check for</param>
        /// <returns>returns true if the value exists in the PKI</returns>
        [Pure]
        public static bool ContainsValue(byte[] publicKey)
        {
            Contract.Requires(publicKey != null);
            keyCollection = ReadFromFile(DatabasePath);
            return keyCollection.ContainsValue(publicKey);
        }

        /// <summary>
        /// Store this public key of this domain in the PKI.
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
            if (!keyCollection.ContainsKey(uniqueIdentifier))
            {
                if (!keyCollection.ContainsValue(publicKey))
                {
                    keyCollection.Add(uniqueIdentifier, publicKey);
                    success = true;

                    WriteToFile(keyCollection, DatabasePath);
                }
            }

            return success;
        }

        /// <summary>
        /// Remove the public key of this domain from the PKI.
        /// </summary>
        /// <param name="uniqueIdentifier">The unique identifier for the domain</param>
        /// <returns>True if the key is removed, otherwise false</returns>
        public static bool RevokeKey(string uniqueIdentifier)
        {
            Contract.Requires(uniqueIdentifier != null);
            Contract.Requires(uniqueIdentifier != string.Empty);
            Contract.Requires(ContainsKey(uniqueIdentifier));
            Contract.Ensures(
                Contract.Result<bool>() == (keyCollection.Count == Contract.OldValue(keyCollection.Count) - 1));
            if (keyCollection.Remove(uniqueIdentifier))
            {
                WriteToFile(keyCollection, DatabasePath);
                return true;
            }
            
            return false;
        }

        /// <summary>
        /// Does this key contain a valid public key header?
        /// </summary>
        /// <param name="keyBlob">The blob containing the key info</param>
        /// <returns>true if the blob has the signature of a public key blob</returns>
        public static bool ValidKeyBlob(IEnumerable<byte> keyBlob)
        {
            byte[] publicKeyTemplate = { 6, 2, 0, 0, 0, 164, 0, 0, 82, 83, 65, 49, 0, 16, 0, 0, 1, 0, 1, 0 };

            //// Compare the first 20 elements of the keyBlob collection to the template and see if they matches
            return publicKeyTemplate.Take(20).SequenceEqual(keyBlob.Take(20));
        }

        /// <summary>
        /// This method takes the dictionary and reads it to a path, this is used to ensure persistency 
        /// http://www.dotnetperls.com/dictionary-binary
        /// </summary>
        /// <param name="dictionary">The dictionary to write to path</param>
        /// <param name="path">The path to write the path to</param>
        private static void WriteToFile(Dictionary<string, byte[]> dictionary, string path)
        {
            Contract.Requires(dictionary != null);
            Contract.Requires(!string.IsNullOrEmpty(path));
            using (FileStream fs = File.OpenWrite(path))
            using (var writer = new BinaryWriter(fs))
            {
                // Put count.
                writer.Write(dictionary.Count);

                // Write pairs.
                foreach (var pair in dictionary)
                {
                    writer.Write(pair.Key);
                    writer.Write(pair.Value);
                }
            }
        }

        /// <summary>
        /// This is used to read a dictionary from a file
        /// http://www.dotnetperls.com/dictionary-binary
        /// </summary>
        /// <param name="path">The path to the file</param>
        /// <returns>The dictionary read from the file</returns>
        private static Dictionary<string, byte[]> ReadFromFile(string path)
        {
            Contract.Requires(!string.IsNullOrEmpty(path));
            var result = new Dictionary<string, byte[]>();
            using (FileStream fs = File.OpenRead(path))
            using (var reader = new BinaryReader(fs))
            {
                // Determine the amount of key value pairs to read.
                int count = reader.ReadInt32();

                // Read in all the pairs.
                for (int i = 0; i < count; i++)
                {
                    string key = reader.ReadString();
                    byte[] value = reader.ReadBytes(513);
                    result[key] = value;
                }
            }
            return result;
        }

        /// <summary>
        /// The class invariant for the PublicKeyInfrastructure class
        /// </summary>
        [ContractInvariantMethod]
        private static void PKIClassInvariant()
        {
            
            //// Each key must be unique
            //// each identifier must be unique
        }

        /// <summary>
        /// A helper method to 
        /// </summary>
        /// <returns></returns>
        private static bool InvarianHelper()
        {
            return true;
        }
    }
}
