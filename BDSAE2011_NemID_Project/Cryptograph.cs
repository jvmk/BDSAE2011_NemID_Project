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
        /// Is this public key valid?
        /// </summary>
        /// <returns>
        /// True if the public key is deemed valid, otherwise false
        /// </returns>
        public bool IsValid()
        {
            return true;
        }

        /// <summary>
        /// Can I have the public key of this domain?
        /// </summary>
        /// <returns>True if a key is succesfully retrieved from a PKI</returns>
        public bool GetPublicKey()
        {
            return true;
        }

        /// <summary>
        /// Generate a symmetric key
        /// </summary>
        /// <param name="publicKeyPath">The key</param>
        public void GenerateSymmetricKey(string publicKeyPath)
        {
            using (var rijndael = new RijndaelManaged())
            {
                try
                {
                    var keyWriter = new FileStream("SymmetricKey.bin", FileMode.Create);
                    keyWriter.Write(rijndael.Key, 0, rijndael.Key.Length);
                    keyWriter.Close();
                }
                catch (IOException e)
                {
                    Console.Write(e);
                }
            }
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
        public string Encrypt(string keyPath, string dataToEncrypt)
        {
            string encryptedMessage;
            using (var rsa = new RSACryptoServiceProvider())
            {
                try
                {
                    //// Load the key from the specified path
                    var encryptKey = new XmlDocument();
                    encryptKey.Load(keyPath);
                    rsa.FromXmlString(encryptKey.OuterXml);

                    //// Conver the string message to a byte array for encryption
                    UTF8Encoding encoder = new UTF8Encoding();
                    var byteMessage = encoder.GetBytes(dataToEncrypt);

                    byte[] encryptedBytes = rsa.Encrypt(byteMessage, false);

                    //// Convert the byte array back to a string message
                    encryptedMessage = encoder.GetString(encryptedBytes);
                    File.WriteAllText(@"C:\Test\EncryptedStringMessage.txt", encryptedMessage);

                    //// Write the encrypted message to a file, for testing purposes
                    File.WriteAllBytes(@"C:\Test\encryptedBits.bin", encryptedBytes);

                }

                finally
                {
                    //// Clear the RSA key container, deleting generated keys.
                    rsa.PersistKeyInCsp = false;
                }
            }
            return encryptedMessage;
        }

        /// <summary>
        /// Decrypt this message using this key
        /// </summary>
        /// <param name="keyPath">
        /// The path to where the key is stored
        /// </param>
        /// <param name="dataToDecrypt">
        /// The data To Decrypt.
        /// </param>
        /// <returns>
        /// The decrypted data.
        /// </returns>
        public string Decrypt(string keyPath, string dataToDecrypt)
        {
            string decryptedText;
            using (var rsa = new RSACryptoServiceProvider())
            {
                try
                {
                    //// Loads the keyinfo into the rsa parameters from the keyfile
                    var privateKey = new XmlDocument();
                    privateKey.Load(keyPath);
                    rsa.FromXmlString(privateKey.OuterXml);

                    //// Convert the text from string to byte array for decryption
                    UTF8Encoding encoder = new UTF8Encoding();
                    var encryptedBytes = encoder.GetBytes(dataToDecrypt);

                    //// Create an aux array to store all the encrypted bytes
                    byte[] decryptedBytes = new byte[encryptedBytes.Length];

                    byte[] aux = new byte[512];

                    for (int j = 0; j <= dataToDecrypt.Length; j+=512)
                    {
                        encoder.GetBytes(dataToDecrypt, j, 512, aux, j);
                        byte[] auxDecrypted = rsa.Decrypt(aux, false);
                        auxDecrypted.CopyTo(decryptedBytes, j);
                    }

                    //// Decrypt the message
                    ////   byte[] decryptedBytes = rsa.Decrypt(encryptedBytes, false);

                    File.WriteAllBytes(@"C:\Test\decryptedBits.bin", decryptedBytes);
                    //// Convert from bytes to string
                    decryptedText = encoder.GetString(decryptedBytes);

                    //// Write the text to a file
                    File.WriteAllText(@"C:\Test\decryptedText.txt", decryptedText);

                }

                finally
                {
                    //// Clear the RSA key container, deleting generated keys.
                    rsa.PersistKeyInCsp = false;
                }
            }
            return decryptedText;
        }

        /// <summary>
        /// Generate public-private key pair.
        /// </summary>
        /// <param name="uniqueIdentifier">
        /// For a client this could be his/her CPRNumber, for the server this could be its domain. Used to identify the public key.
        /// TODO: Is the this right way to go about it?
        /// </param>
        public void GenerateKeys(string uniqueIdentifier)
        {
            var pki = new PublicKeyInfrastructure();
           using (var rsa = new RSACryptoServiceProvider(4096))
            {
                try
                {
                    string privateKey = rsa.ToXmlString(true);
                    string publicKey = rsa.ToXmlString(false);
                    var xdocPrivate = new XmlDocument();
                    var xdocPublic = new XmlDocument();
                    xdocPrivate.LoadXml(privateKey);
                    xdocPublic.LoadXml(publicKey);
                    xdocPrivate.Save(@"C:\Test\PrivateKeyInfo.xml");
                    xdocPublic.Save(@"C:\Test\PublicKeyInfo.xml");

                    //// Store the key as an xml string along with the unique id in the PKI.
                    this.PublishKey(publicKey, uniqueIdentifier);
                }
                finally
                {
                    //// Clear the RSA key container, deleting generated keys.
                    rsa.PersistKeyInCsp = false;
                }
            }
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
        public void PublishKey(string publicKey, string uniqueIdentifier)
        {
            PublicKeyInfrastructure PKI = new PublicKeyInfrastructure();
            if (!publicKey.Contains("<P>"))
            {
                PKI.StoreKey(publicKey, uniqueIdentifier);
            }
                
            
            ////Todo: make sure only public key info gets uploaded

        }
    }
}
