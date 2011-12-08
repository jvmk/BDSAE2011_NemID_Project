// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Cryptograph.cs" company="d">
//   
// </copyright>
// <summary>
//   The class for handling encryption and decryption of data
// </summary>
// -------------------------------------------------------------------------------------------------------------------

namespace AuthenticationService
{
    using System;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;
    using System.Xml;

    using BDSAE2011_NemID_Project;

    /// <summary>
    /// The class for handling encryption and decryption of data
    /// </summary>
    public class Cryptograph
    {
        /// <summary>
        /// The public key infrastructure
        /// </summary>
        private readonly PublicKeyInfrastructure pki = new PublicKeyInfrastructure();

        /// <summary>
        /// Is this public key valid?
        /// </summary>
        /// <param name="uniqueIdentifier">
        /// The unique ID of the domain of the key
        /// </param>
        /// <returns>
        /// True if the public key is deemed valid, otherwise false
        /// </returns>
        public bool IsValid(string uniqueIdentifier)
        {
            return this.pki.ContainsKey(uniqueIdentifier);
        }

        /// <summary>
        /// Can I have the public key of this domain?
        /// </summary>
        /// <param name="uniqueIdentifier">
        /// The unique Identifier. this should either be a URI or something like CPRNumber
        /// </param>
        /// <returns>
        /// The public key from the PKI belonging to the unique domain
        /// </returns>
        public RSAParameters GetPublicKey(string uniqueIdentifier)
        {
            return this.pki.GetKey(uniqueIdentifier);
        }

        /// <summary>
        /// Encrypt this message using this key
        /// </summary>
        /// <param name="keyPath">
        /// The path to where the key is stored.
        /// </param>
        /// <param name="dataToEncrypt">
        /// The data To Encrypt.
        /// </param>
        /// <returns>
        /// The encrypted message.
        /// </returns>
        public static string Encrypt(string dataToEncrypt, RSAParameters publicKeyInfo)
        {

            //Create a new instance of RSACryptoServiceProvider.
            RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();

            UTF8Encoding encoder = new UTF8Encoding();

            byte[] encryptThis = encoder.GetBytes(dataToEncrypt);

            //// Importing the public key
            RSA.ImportParameters(publicKeyInfo);

            int blockSize = (RSA.KeySize / 8) - 32;

            //// buffer to write byte sequence of the given block_size
            byte[] buffer = new byte[blockSize];

            byte[] encryptedBuffer = new byte[blockSize];

            //// The bytearray to hold all of our data after encryption
            byte[] encryptedBytes = new byte[encryptThis.Length + blockSize - (encryptThis.Length % blockSize) + 32];

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
                encryptedBuffer = RSA.Encrypt(buffer, false);
                encryptedBuffer.CopyTo(encryptedBytes, i);

            }
            //// Convert the byteArray using Base64
            return Convert.ToBase64String(encryptedBytes);
        }

        /// <summary>
        /// Decrypt this message using this key
        /// </summary>
        /// <param name="dataToDecrypt">
        /// The data To decrypt.
        /// </param>
        /// <param name="privateKeyInfo">
        /// The private Key Info.
        /// </param>
        /// <returns>
        /// The decrypted data.
        /// </returns>
        public static string Decrypt(string dataToDecrypt, RSAParameters privateKeyInfo)
        {
            //Create a new instance of RSACryptoServiceProvider.
            RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();

            byte[] bytesToDecrypt = Convert.FromBase64String(dataToDecrypt);

            //// Import the private key info
            RSA.ImportParameters(privateKeyInfo);

            //// No need to subtract padding size when decrypting (OR do I?)
            int blockSize = RSA.KeySize / 8;

            //// buffer to write byte sequence of the given block_size
            byte[] buffer = new byte[blockSize];

            //// buffer containing decrypted information
            byte[] decryptedBuffer = new byte[blockSize];

            //// The bytearray to hold all of our data after decryption
            byte[] decryptedBytes = new byte[dataToDecrypt.Length];

            for (int i = 0; i < bytesToDecrypt.Length; i += blockSize)
            {
                if (2 * i > bytesToDecrypt.Length && ((bytesToDecrypt.Length - i) % blockSize != 0))
                {
                    buffer = new byte[bytesToDecrypt.Length - i];
                    blockSize = bytesToDecrypt.Length - i;
                }

                //// If the amount of bytes we need to decrypt isn't enough to fill out a block, only decrypt part of it
                if (bytesToDecrypt.Length < blockSize)
                {
                    buffer = new byte[bytesToDecrypt.Length];
                    blockSize = bytesToDecrypt.Length;
                }

                Buffer.BlockCopy(bytesToDecrypt, i, buffer, 0, blockSize);
                decryptedBuffer = RSA.Decrypt(buffer, false);
                decryptedBuffer.CopyTo(decryptedBytes, i);

            }

            var encoder = new UTF8Encoding();
            return encoder.GetString(decryptedBytes).TrimEnd(new[] { '\0' });

        }

        /// <summary>
        /// This method is used to facilitate splitting the messages up into chunks of data small enough to be decrypted and encrypted by the RSACryptoServiceProvider
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="encryptionMode"></param>
        /// <returns></returns>
        private byte[] blockCihper(byte[] bytes, bool encryptionMode)
        {
            //// Create 2 arrays, aux to be the buffer array, toReturn to hold the complete data
            byte[] aux = new byte[0];

            byte[] toReturn = new byte[0];

            //// We encrypt with 456 bytes long blocks, and decrypt with 512 bytes
            int length = (encryptionMode == true)? 456 : 512;

            byte[] buffer = new byte[length];

            for (int i = 0; i < bytes.Length; i++)
            {
                //// checks to see if the buffer is filled, then we can encrypt
                if ((i > 0) && (i % length == 0))
                {
                    // Do encryption/decryption

                }
            }

            return aux;
        }

        /// <summary>
        /// Generate public-private key pair.
        /// </summary>
        /// <param name="uniqueIdentifier">
        /// For a client this could be his/her CPRNumber, for the server this could be its domain. Used to identify the public key.
        /// TODO: Returning privateKey, no?
        /// </param>
        public RSAParameters GenerateKeys(string uniqueIdentifier)
        {
            RSAParameters privateKey;
           using (var rsa = new RSACryptoServiceProvider(4096))
            {
                try
                {
                    //// string privateKey = rsa.ToXmlString(true);
                    //// string publicKey = rsa.ToXmlString(false);
                    privateKey = rsa.ExportParameters(true);
                    RSAParameters publicKey = rsa.ExportParameters(false);
                    
                    //// Save the private key
                    //// var xdocPrivate = new XmlDocument();
                    //// xdocPrivate.LoadXml(privateKey);
                    //// xdocPrivate.Save(@"C:\Test\PrivateKeyInfo.xml");

                    //// Store the key as an xml string along with the unique id in the PKI.
                    this.pki.StoreKey(publicKey, uniqueIdentifier);
                }
                finally
                {
                    //// Clear the RSA key container, deleting generated keys.
                    rsa.PersistKeyInCsp = false;
                }
            }

            return privateKey;
        }

        /// <summary>
        /// Upload public key to public key repository.
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
        public bool PublishKey(RSAParameters publicKey, string uniqueIdentifier)
        {
            Contract.Requires(publicKey.P == null);
            return this.pki.StoreKey(publicKey, uniqueIdentifier);
        }
    }
}
