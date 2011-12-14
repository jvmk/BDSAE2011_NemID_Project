// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Cryptograph.cs" company="d">
//   
// </copyright>
// <summary>
//   The class for handling encryption and decryption of data
// </summary>
// -------------------------------------------------------------------------------------------------------------------

namespace BDSA_Project_Cryptography
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Security.Cryptography;
    using System.Text;

    /// <summary>
    /// The class for handling encryption and decryption of data
    /// </summary>
    public static class Cryptograph
    {
        /// <summary>
        /// Can I have the public key of this domain?
        /// </summary>
        /// <param name="uniqueIdentifier">
        /// The unique Identifier. this should either be a URI or something like CPRNumber
        /// </param>
        /// <returns>
        /// The public key from the PKI belonging to the unique domain
        /// </returns>
        [Pure]
        public static byte[] GetPublicKey(string uniqueIdentifier)
        {
            Contract.Requires(uniqueIdentifier != null);
            return PublicKeyInfrastructure.GetKey(uniqueIdentifier);
        }

        /// <summary>
        /// Does this key exist in the PKI?
        /// </summary>
        /// <param name="uniqueIdentifier">The unique ID of the public key</param>
        /// <returns>returns true if the PKI contains the key</returns>
        public static bool KeyExists(string uniqueIdentifier)
        {
            Contract.Requires(uniqueIdentifier != null);
            return PublicKeyInfrastructure.ContainsKey(uniqueIdentifier);
        }

        /// <summary>
        /// Encrypt this message using this public key
        /// </summary>
        /// <param name="messageToEncrypt">
        /// The data To Encrypt.
        /// </param>
        /// <param name="publicKeyInfo">
        /// The public Key Info.
        /// </param>
        /// <returns>
        /// The encrypted message. Returns null if cryptographic exception is caught
        /// </returns>
        public static string Encrypt(string messageToEncrypt, byte[] publicKeyInfo)
        {
            Contract.Requires(PublicKeyInfrastructure.ContainsValue(publicKeyInfo));
            Contract.Requires(publicKeyInfo != null);
            Contract.Requires(string.IsNullOrEmpty(messageToEncrypt));
            Contract.Ensures(!Contract.Result<string>().Equals(messageToEncrypt));
            

            //// Our bytearray to hold all of our data after the encryption
            byte[] encryptedBytes;
            using (var rsa = new RSACryptoServiceProvider())
            {
                byte[] encryptThis = Encoding.UTF8.GetBytes(messageToEncrypt);

                //// Importing the public key
                rsa.ImportCspBlob(publicKeyInfo);

                int blockSize = (rsa.KeySize / 8) - 32;

                var buffer = new byte[blockSize];

                //// Initializing our encryptedBytes array to a suitable size, depending on the size of data to be encrypted
                encryptedBytes = new byte[encryptThis.Length + blockSize - (encryptThis.Length % blockSize) + 32];

                try
                {
                    for (int i = 0; i < encryptThis.Length; i += blockSize)
                    {
                        //// If there is extra info to be parsed, but not enough to fill out a complete bytearray, fit array for last bit of data
                        if (2 * i > encryptThis.Length && ((encryptThis.Length - i) % blockSize != 0))
                        {
                            buffer = new byte[encryptThis.Length - i];
                            blockSize = encryptThis.Length - i;
                        }

                        //// If the amount of bytes we need to decrypt isn't enough to fill out a block, only decrypt part of it
                        if (encryptThis.Length < blockSize)
                        {
                            buffer = new byte[encryptThis.Length];
                            blockSize = encryptThis.Length;
                        }

                        //// encrypt the specified size of data, then add to final array.
                        Buffer.BlockCopy(encryptThis, i, buffer, 0, blockSize);
                        byte[] encryptedBuffer = rsa.Encrypt(buffer, false);
                        encryptedBuffer.CopyTo(encryptedBytes, i);
                    }
                }
                catch (CryptographicException e)
                {
                    Console.Write(e);
                    return null;
                }
                finally
                {
                    //// Clear the RSA key container, deleting generated keys.
                    rsa.PersistKeyInCsp = false;
                }
            }
            //// Convert the byteArray using Base64 and returns as an encrypted string
            return Convert.ToBase64String(encryptedBytes);
        }

        /// <summary>
        /// Decrypt this message using this private key
        /// </summary>
        /// <param name="dataToDecrypt">
        /// The data To decrypt.
        /// </param>
        /// <param name="privateKeyInfo">
        /// The private Key Info.
        /// </param>
        /// <returns>
        /// The decrypted data. Returns null if cryptographic exception is caught
        /// </returns>
        public static string Decrypt(string dataToDecrypt, byte[] privateKeyInfo)
        {
            Contract.Requires(dataToDecrypt != null);
            Contract.Requires(privateKeyInfo != null);
            Contract.Ensures(!Contract.Result<string>().Equals(dataToDecrypt));
            //// The bytearray to hold all of our data after decryption
            byte[] decryptedBytes;

            using (var rsa = new RSACryptoServiceProvider())
            {
                byte[] bytesToDecrypt = Convert.FromBase64String(dataToDecrypt);

                //// Import the private key info
                rsa.ImportCspBlob(privateKeyInfo);

                //// No need to subtract padding size when decrypting
                int blockSize = rsa.KeySize / 8;

                var buffer = new byte[blockSize];

                //// Initializes our array to make sure it can hold at least the amount needed to decrypt.
                decryptedBytes = new byte[dataToDecrypt.Length];

                try
                {
                    for (int i = 0; i < bytesToDecrypt.Length; i += blockSize)
                    {
                        if (2 * i > bytesToDecrypt.Length && ((bytesToDecrypt.Length - i) % blockSize != 0))
                        {
                            buffer = new byte[bytesToDecrypt.Length - i];
                            blockSize = bytesToDecrypt.Length - i;
                        }

                        //// If the amount of bytes we need to decrypt is not enough to fill out a block, only decrypt part of it
                        if (bytesToDecrypt.Length < blockSize)
                        {
                            buffer = new byte[bytesToDecrypt.Length];
                            blockSize = bytesToDecrypt.Length;
                        }

                        Buffer.BlockCopy(bytesToDecrypt, i, buffer, 0, blockSize);
                        byte[] decryptedBuffer = rsa.Decrypt(buffer, false);
                        decryptedBuffer.CopyTo(decryptedBytes, i);
                    }
                }
                catch (CryptographicException e)
                {
                    Console.Write(e.Message);
                    return null;
                }
                finally
                {
                    //// Clear the RSA key container, deleting generated keys.
                    rsa.PersistKeyInCsp = false;
                }
            }
            //// We encode each byte with UTF8 and then write to a string while trimming off the extra empty data created by the overhead.
            return Encoding.UTF8.GetString(decryptedBytes).TrimEnd(new[] { '\0' });
        }

        /// <summary>
        /// Sign this message, using this private key.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="privateKey">
        /// The private key of the signee
        /// </param>
        /// <returns>
        /// The signed message string
        /// </returns>
        public static string SignData(string message, byte[] privateKey)
        {
            Contract.Requires(message != string.Empty);
            Contract.Requires(privateKey != null);
            Contract.Ensures(!Contract.Result<string>().Equals(message));

            //// The array to store the signed message in bytes
            byte[] signedBytes;
            using (var rsa = new RSACryptoServiceProvider())
            {
                //// Write the message to a byte array using UTF8 as the encoding.
                var encoder = new UTF8Encoding();
                byte[] originalData = encoder.GetBytes(message);

                try
                {
                    //// Import the private key used for signing the message
                    rsa.ImportCspBlob(privateKey);

                    //// Sign the data, using SHA512 as the hashing algorithm 
                    signedBytes = rsa.SignData(originalData, CryptoConfig.MapNameToOID("SHA512"));
                }
                catch (CryptographicException e)
                {
                    return null;
                }
                finally
                {
                    //// Set the keycontainer to be cleared when rsa is garbage collected.
                    rsa.PersistKeyInCsp = false;
                }
            }

            return Convert.ToBase64String(signedBytes);
        }

        /// <summary>
        /// Verify the signature of this message using this key.
        /// </summary>
        /// <param name="originalMessage">
        /// The original message.
        /// </param>
        /// <param name="signedMessage">
        /// The signed message.
        /// </param>
        /// <param name="publicKey">
        /// The public key of the sender of the original message
        /// </param>
        /// <returns>
        /// Returns true if the message is authentic and sent by the holder of the specified public key. Returns false if cryptographic exception is caught
        /// </returns>
        public static bool VerifyData(string originalMessage, string signedMessage, byte[] publicKey)
        {
            Contract.Requires(originalMessage != null);
            Contract.Requires(signedMessage != null);
            Contract.Requires(publicKey != null);
            Contract.Requires(!originalMessage.Equals(signedMessage));
            Contract.Requires(PublicKeyInfrastructure.ValidPublicKeyBlob(publicKey));
            using (var rsa = new RSACryptoServiceProvider())
            {
                var encoder = new UTF8Encoding();
                byte[] bytesToVerify = encoder.GetBytes(originalMessage);
                byte[] signedBytes = Convert.FromBase64String(signedMessage);
                try
                {
                    rsa.ImportCspBlob(publicKey);

                    return rsa.VerifyData(bytesToVerify, CryptoConfig.MapNameToOID("SHA512"), signedBytes);
                }
                catch (CryptographicException e)
                {
                    return false;
                }
                finally
                {
                    rsa.PersistKeyInCsp = false;
                }
            }
        }
        
        /// <summary>
        /// Generate a hash based upon this string.
        /// </summary>
        /// <param name="uniqueId">A unique parameter to hash</param>
        /// <returns>The hashed value of the string</returns>
        public static string GenerateSHA2Hash(string uniqueId)
        {
            Contract.Requires(!string.IsNullOrEmpty(uniqueId));
            Contract.Ensures(!Contract.Result<string>().Equals(uniqueId));
            byte[] dataToHash = Convert.FromBase64String(uniqueId);

            var sha = new SHA512Managed();

            return Convert.ToBase64String(sha.ComputeHash(dataToHash));
        }

        /// <summary>
        /// Generate a public-private key pair.
        /// </summary>
        /// <param name="uniqueIdentifier">
        /// For a client this could be his/hers username or e-mail, for the server this could be its domain. Used to identify the public key.
        /// </param>
        /// <returns>
        /// The generated private key, the public key is automatically stored in the PKI
        /// </returns>
        public static byte[] GenerateKeys(string uniqueIdentifier)
        {
            Contract.Requires(uniqueIdentifier != string.Empty);
            byte[] privateKeyBlob;

            using (var rsa = new RSACryptoServiceProvider(4096))
            {
                try
                {
                    byte[] publicKey = rsa.ExportCspBlob(false);

                    privateKeyBlob = rsa.ExportCspBlob(true);

                    //// Store the key as an xml string along with the unique id in the PKI.
                    PublicKeyInfrastructure.StoreKey(publicKey, uniqueIdentifier);
                }
                catch (CryptographicException e)
                {
                    Console.Write(e.Message);
                    return null;
                }
                finally
                {
                    //// Clear the RSA key container, deleting generated keys.
                    rsa.PersistKeyInCsp = false;
                }
            }

            return privateKeyBlob;
        }

        /// <summary>
        /// Upload this public key to the public key repository.
        /// </summary>
        /// <param name="publicKey">
        /// The public Key.
        /// </param>
        /// <param name="uniqueIdentifier">
        /// The unique Identifier.
        /// </param>
        /// <returns>
        /// True if the key was succesfully added to the PKI
        /// </returns>
        public static bool PublishKey(byte[] publicKey, string uniqueIdentifier)
        {
            Contract.Requires(PublicKeyInfrastructure.ValidPublicKeyBlob(publicKey));
            Contract.Requires(publicKey != null || uniqueIdentifier != null);
            Contract.Ensures(Contract.Result<bool>() == PublicKeyInfrastructure.ContainsKey(uniqueIdentifier));
            return PublicKeyInfrastructure.StoreKey(publicKey, uniqueIdentifier);
        }

        /// <summary>
        /// This method tests if the private key info and the public key connected to the unique ID
        /// can be used to encrypt and decrypt text message and thus if the keys are part of the same keypair.
        /// </summary>
        /// <param name="privateKey">
        /// The privateKey
        /// </param>
        /// <param name="publicKeyIdentifier">
        /// The public Key Identifier.
        /// </param>
        /// <returns>
        /// Returns true if the keyValuePair matches, otherwise false
        /// </returns>
        [Pure]
        public static bool CheckConsistency(byte[] privateKey, string publicKeyIdentifier)
        {
            Contract.Requires(privateKey != null);
            Contract.Requires(publicKeyIdentifier != null);
            Contract.Requires(PublicKeyInfrastructure.ValidPublicKeyBlob(GetPublicKey(publicKeyIdentifier)));
            Contract.Requires(!PublicKeyInfrastructure.ValidPublicKeyBlob(privateKey));

            const string TestMessage = "encrypt and decrypt this";

            //// Encrypt and decrypt info, fetching the public key information from the PKI
            string result = Decrypt(Encrypt(TestMessage, GetPublicKey(publicKeyIdentifier)), privateKey);

            return result.Equals(TestMessage);
        }
    }
}
