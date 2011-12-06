// -----------------------------------------------------------------------
// <copyright file="PublicKeyInfrastructure.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace BDSAE2011_NemID_Project
{
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class PublicKeyInfrastructure
    {
        /// <summary>
        /// The collection to store the Xml strings and their ID
        /// </summary>
        private readonly Dictionary<string, string> keyCollection = new Dictionary<string, string>(); 

        /// <summary>
        /// Stores the specified public key in the PKI
        /// </summary>
        /// <param name="xmlKeyString">
        /// The xml Key String.
        /// </param>
        /// <param name="uniqueIdentifier">
        /// The unique Identifier. 
        /// </param>
        /// <returns>
        /// True if the key was succesfully stored, otherwise false
        /// </returns>
        public bool StoreKey(string xmlKeyString, string uniqueIdentifier)
        {
            if (!this.keyCollection.ContainsKey(xmlKeyString))
            {
                if (!this.keyCollection.ContainsValue(uniqueIdentifier))
                {
                    this.keyCollection.Add(xmlKeyString, uniqueIdentifier);
                }
            }

            return true;
        }

        public string GetKey(string uniqueIdentifier)
        {
            return "";
        }
    }
}
