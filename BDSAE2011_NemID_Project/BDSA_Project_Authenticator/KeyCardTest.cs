// -----------------------------------------------------------------------
// <copyright file="KeyCardTest.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace BDSA_Project_Authenticator
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using NUnit.Framework;

    /// <summary>
    /// Testing several scenarios that aim to cover all the functionality of the key card class
    /// </summary>
    [TestFixture]
    public class KeyCardTest
    {
        /// <summary>
        /// Create a keycard and make sure that the card contains a unique ID along with other info
        /// </summary>
        [Test]
        public void CreateUniqueCard()
        {
            var userAccount1 = new UserAccount("Justesting", "password", "010101-1111", "justesting@test.com");
            var testCard = userAccount1.Keycard;
            Assert.True(!string.IsNullOrEmpty(testCard.GetUniqueId()));

            uint index = testCard.GetKeyIndex();
            Assert.That(index >= 0 && index <= 9999);

            Assert.That(testCard.GetKeyCardNumber() == 1);

            Assert.That(testCard.KeysLeft() > 0);

            // Assert that the length of the index as a string always will be 4 digits.
            Assert.That(testCard.GetKeyIndexAsString().Length == 4);

            string keyCard = testCard.ToString();
        }
    }
}
