// -----------------------------------------------------------------------
// <copyright file="PublicKeyInfrastructureTest.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace BDSA_Project_Cryptography
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using NUnit.Framework;

    /// <summary>
    /// a set of scenarios that aim to test most of the functionality of the PKI
    /// <author>Simon langhoff</author>
    /// </summary>
    [TestFixture]
    public class PublicKeyInfrastructureTest
    {
        /// <summary>
        /// store a key in the PKI and then check if the key is contained, then retrieve the key and check validity
        /// </summary>
        [Test]
        public void StoreGetAndValidify()
        {
            byte[] privateKey1 = Cryptograph.GenerateKeys("PublicKey1");
            Assert.That(PublicKeyInfrastructure.ContainsKey("PublicKey1"));
            Assert.That(PublicKeyInfrastructure.ValidPublicKeyBlob(PublicKeyInfrastructure.GetKey("PublicKey1")));

            //// Remove the key to create consistency
            Assert.That(PublicKeyInfrastructure.RevokeKey("PublicKey1"));
        }

        /// <summary>
        /// store a key and remove the same key again and check if it contains the key
        /// </summary>
        [Test]
        public void RemoveContainsTest()
        {
            byte[] privateKey2 = Cryptograph.GenerateKeys("PublicKey2");
            Assert.That(PublicKeyInfrastructure.RevokeKey("PublicKey2"));
        }

        /// <summary>
        /// Store a key, retrieve the key and check equality of the key retrieved from contains value
        /// </summary>
        [Test]
        public void CheckPersistency()
        {
            Cryptograph.GenerateKeys("PublicKey3");
            byte[] publicKey = PublicKeyInfrastructure.GetKey("PublicKey3");
            Assert.That(PublicKeyInfrastructure.RevokeKey("PublicKey3"));
        }
    }
}
