﻿// -----------------------------------------------------------------------
// <copyright file="KeyCard.cs" company="">
// TODO: Update copyright text.
// </copyright>
// ----------------------------------------------------------------------

namespace AuthenticatorComponent
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;

    /// <summary>
    /// Represents instances of the clients’ key cards.
    /// </summary>
    internal class KeyCard
    {
        /// <summary>
        /// A set of unique keys and key indexes, sorted by the index of the key.
        /// </summary>
        private readonly SortedDictionary<uint, uint> keyCollection = new SortedDictionary<uint, uint>();

        /// <summary>
        /// Contains the index for the next key that has to be entered by the user
        /// </summary>
        private uint currentIndex;

        /// <summary>
        /// The number of cards the user has had so far.
        /// </summary>
        private uint cardNumber;



        /// <summary>
        /// Initializes a new instance of the <see cref="KeyCard"/> class. 
        /// Constructor for the keycard class
        /// </summary>
        public KeyCard()
        {
            this.GenerateCard();
            this.SetNextKeyIndex();
        }

        /// <summary>
        /// Pseudo-randomly determines the next index to be stored as the next reference value for the key.
        /// </summary>
        /// <returns>Returns the value of the next keyIndex</returns>
        private void SetNextKeyIndex()
        {
            //// Generate a number between 0 and the amount of keys.
            uint nextIndex = GenerateRandomNumber((byte)this.keyCollection.Count);

            //// Copy the keys to an array
            var aux = new uint[this.keyCollection.Count];
            this.keyCollection.Keys.CopyTo(aux, 0);

            //// return the key value associated with the keyIndex at the location specified by the random number
            this.currentIndex = aux[nextIndex];
        }

        /// <summary>
        /// Can I get the key card’s number?
        /// </summary>
        /// <returns>
        /// The key card number.
        /// </returns>
        public uint GetKeyCardNumber()
        {
            return this.cardNumber;
        }

        /// <summary>
        /// How many keys are left?
        /// </summary>
        /// <returns>The amount of keys left on the card</returns>
        public int KeysLeft()
        {
            ////Contract.Requires(this.keyCollection != null);
            return this.keyCollection.Count;
        }

        /// <summary>
        /// Can I get a text-representation of the key card?
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var element in this.keyCollection)
            {
                sb.AppendLine("Index = " + element.Key.ToString("D4") + "  " + "key = " + element.Value.ToString("D6"));
            }
            this.WriteToFile(sb.ToString() + this.KeyIndex());
            return sb.ToString();
        }

        /// <summary>
        /// Writes the keycard to a local file
        /// </summary>
        /// <param name="textToWrite">The string to write to a file</param>
        private void WriteToFile(string textToWrite)
        {
            File.WriteAllText(@"C:\test\testFile.txt", textToWrite);
        }

        /// <summary>
        /// Can I get the key index for the key I need to enter?
        /// </summary>
        /// <returns>Returns the key number the user has to enter to login</returns>
        public uint KeyIndex()
        {
            uint currentKey = this.keyCollection[this.currentIndex];
            //// TODO: Set a new key if timeout or?
            return currentKey;
        }

        /// <summary>
        /// Is this the correct key for the current key index?
        /// </summary>
        /// <param name="key">
        /// The key to validify
        /// </param>
        /// <returns>
        /// Returns true if the keyNumber is correct for the current sequence number
        /// </returns>
        public bool VerifyEnteredKey(uint enteredKey)
        {
            ////Contract.Requires(this.currentKey != default(uint));
            uint keyToBeEntered = this.currentIndex;
            this.RemoveKeyPair(this.currentIndex);
            this.SetNextKeyIndex();
            return keyToBeEntered.Equals(enteredKey);
        }

        /// <summary>
        /// Generate key card.
        /// </summary>
        public void GenerateCard()
        {
            //// Clears the collection, removing all the entries.
            this.keyCollection.Clear();

            this.cardNumber = this.cardNumber + 1;

            //// Keep on adding key value pairs to the set until 100 elements have been stored succesfully.
            for (var i = this.keyCollection.Count; i <= 100; i++)
            {
                //// Creates 2 bytearrays, one for storing the key and the other for the index
                var randomKeyIndex = new uint[4];
                var randomKey = new uint[6];

                for (int j = 0; j < 4; j++)
                {
                    randomKeyIndex[j] = GenerateRandomNumber(10);
                }

                for (int k = 0; k < 6; k++)
                {
                    randomKey[k] = GenerateRandomNumber(10);
                }

                uint index =
                    uint.Parse(randomKeyIndex[0].ToString() + randomKeyIndex[1] + randomKeyIndex[2] + randomKeyIndex[3]);

                uint key =
                    uint.Parse(randomKey[0].ToString() + randomKey[1] + randomKey[2] + randomKey[3] + randomKey[4] + randomKey[5]);

                //// Careful only to add if the index is not already added.
                if (!this.keyCollection.ContainsKey(index))
                {
                    //// Checks to see if the key is not identical to another key already in the list
                    if (!this.keyCollection.ContainsValue(key))
                    {
                        this.keyCollection.Add(index, key);
                    }
                }
            }
        }

        /// <summary>
        /// Remove keypair.
        /// </summary>
        /// <param name="keyIndex">
        /// The key Index.
        /// </param>
        /// <returns>
        /// True if the value was succesfully removed
        /// </returns>
        private bool RemoveKeyPair(uint keyIndex)
        {
            Contract.Requires(this.keyCollection.Count > 0);
            Contract.Ensures(
                Contract.Result<bool>() == (this.keyCollection.Count == Contract.OldValue(this.keyCollection.Count) - 1));
            return this.keyCollection.Remove(keyIndex);
        }

        /// <summary>
        /// This is a private helper method used to decide if a value generated is valid
        /// A value is valid if it is within the range of 0-highest value.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="highestValue">
        /// The highest value to be allowed
        /// </param>
        /// <returns>
        /// True if the value is valid
        /// </returns>
        private static bool IsValidNumber(byte value, uint highestValue)
        {
            //// In order to not skew the results of the random number generation, 
            //// We need to specify the set of valid values, which is the maximum of values divded by the amount of accepted values (0-9)
            //// This is then the amount of valid sets.
            uint fullSetOfValues = byte.MaxValue / highestValue;

            return value < highestValue * fullSetOfValues;
        }

        /// <summary>
        /// This method generates a pseudo-random number between 0 and the userdefined numberOfUniques - 1
        /// Thus, wanting to create a random value between 0-9, the number of unique numbers will be 10.
        /// </summary>
        /// <param name="numberOfUniques">
        /// The amount of unique numbers
        /// </param>
        /// <returns>
        /// A value between 0 and maximum
        /// </returns>
        private static uint GenerateRandomNumber(byte numberOfUniques)
        {
            var generator = new RNGCryptoServiceProvider();

            var randomNumber = new byte[1];
            do
            {
                generator.GetNonZeroBytes(randomNumber);
            }
            while (!IsValidNumber(randomNumber[0], numberOfUniques));

            return (uint)(randomNumber[0] % numberOfUniques);
        }

        /// <summary>
        /// The invariant method for the keycard class.
        /// </summary>
        [ContractInvariantMethod]
        private void KeyCardInvariantMethod()
        {
            Contract.Invariant(this.keyCollection.Count > 0 && this.keyCollection.Count < 255);
            //// For all values in keyCollection, none must be null, and must be unique.
        }
    }
}
