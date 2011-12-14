// -----------------------------------------------------------------------
// <copyright file="CryptographTest.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace BDSA_Project_Cryptography
{
    using NUnit.Framework;

    /// <summary>
    /// Several scenarios that tests the cryptographic functions of the cryptograph
    /// </summary>
    [TestFixture]
    public class CryptographTest
    {
        /// <summary>
        /// A simple string used for testing
        /// </summary>
        private const string TestMessage = "The is a test message";

        /// <summary>
        /// Private test key A
        /// </summary>
        private readonly byte[] privateKeyA = Cryptograph.GenerateKeys("publicKeyA");

        /// <summary>
        /// Private test key B
        /// </summary>
        private readonly byte[] privateKeyB = Cryptograph.GenerateKeys("publicKeyB");

        /// <summary>
        /// Create a public/private keypair, then encrypt/decrypt the message and compare the two strings for equality.
        /// </summary>
        [Test]
        public void ValidKeyEncryptionTest()
        {
            byte[] privateTestKey = Cryptograph.GenerateKeys("PublicTestKey");
            byte[] publicKey = PublicKeyInfrastructure.GetKey("PublicTestKey");

            string encryptedMessage = Cryptograph.Encrypt(TestMessage, publicKey);
            string decryptedMessage = Cryptograph.Decrypt(encryptedMessage, privateTestKey);

            Assert.True(string.Equals(TestMessage, decryptedMessage));
            Assert.True(!string.Equals(encryptedMessage, decryptedMessage));
        }

        /// <summary>
        /// Using a public and private key not part of the same pair to encrypt and decrypt messages
        /// </summary>
        [Test]
        public void InvalidKeyEncryptionTest()
        {
            string encryptedMessage = Cryptograph.Encrypt(TestMessage, PublicKeyInfrastructure.GetKey("publicKeyA"));

            // Decrypting the message encrypted with public key A with private key B
            string decryptedMessage = Cryptograph.Decrypt(encryptedMessage, this.privateKeyB);

            // method should return null if cryptographic exception, like wrong keys passed as arguements
            Assert.True(string.IsNullOrEmpty(decryptedMessage));
        }

        /// <summary>
        /// Use a private key to encrypt and a public key to decrypt a message
        /// </summary>
        [Test]
        public void WrongKeyTypeTest()
        {
            Assert.Null(Cryptograph.Encrypt(TestMessage, this.privateKeyA));

            string correctCipherText = Cryptograph.Encrypt(TestMessage, PublicKeyInfrastructure.GetKey("publicKeyA"));
            Assert.Null(Cryptograph.Decrypt(correctCipherText, PublicKeyInfrastructure.GetKey("publicKeyA")));
        }

        /// <summary>
        /// Using a private/public pair of keys, sign a message using the private key, then verify using the public key
        /// </summary>
        [Test]
        public void SignAndVerify()
        {
            string signedMessage = Cryptograph.SignData(TestMessage, this.privateKeyB);
            
            Assert.True(Cryptograph.VerifyData(TestMessage, signedMessage, PublicKeyInfrastructure.GetKey("publicKeyB")));
        }

        /// <summary>
        /// Using a non-matching private/public keypair, verify and sign a message
        /// </summary>
        [Test]
        public void SignAndVerifyWrongKeyPair()
        {
            string signedMessage = Cryptograph.SignData(TestMessage, this.privateKeyB);

            Assert.False(Cryptograph.VerifyData(TestMessage, signedMessage, PublicKeyInfrastructure.GetKey("publicKeyA")));
        }

        /// <summary>
        /// Create two different keypairs and compare the private keys with the public key from the other pair
        /// </summary>
        [Test]
        public void CheckConsistency()
        {
            Assert.True(Cryptograph.CheckConsistency(this.privateKeyA, "publicKeyA"));
            Assert.True(Cryptograph.CheckConsistency(this.privateKeyB, "publicKeyB"));
            Assert.False(Cryptograph.CheckConsistency(this.privateKeyA, "publicKeyB"));
            Assert.False(Cryptograph.CheckConsistency(this.privateKeyB, "publicKeyA"));
        }

        /// <summary>
        /// Create two messages equal to eachother, hash each of them and then compare the output
        /// </summary>
        [Test]
        public void HashSame()
        {
            Assert.True(string.Equals(Cryptograph.GenerateSHA2Hash(TestMessage), Cryptograph.GenerateSHA2Hash(TestMessage)));
        }

        /// <summary>
        /// Hash two different strings and compare the output
        /// </summary>
        [Test]
        public void HashDifferent()
        {
            const string TestMessage2 = "this string is not the same as the test string to be compared with";

            Assert.False(string.Equals(Cryptograph.GenerateSHA2Hash(TestMessage), Cryptograph.GenerateSHA2Hash(TestMessage2)));
        }
    }
}
