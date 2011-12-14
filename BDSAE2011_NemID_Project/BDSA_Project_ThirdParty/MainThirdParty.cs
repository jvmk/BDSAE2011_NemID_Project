// -----------------------------------------------------------------------
// <copyright file="MainThirdParty.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace BDSA_Project_ThirdParty
{
    using System;

    using BDSA_Project_Communication;

    /// <summary>
    /// Starts the ThirdParty server.
    /// </summary>
    public class MainThirdParty
    {
        private static ThirdPartyServer server;

        private static byte[] privateKey;

        /// <summary>
        /// Starts the ThirdParty server.
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            Console.WriteLine("Generates public/private keypair for third party");

            privateKey = BDSA_Project_Cryptography.Cryptograph.GenerateKeys(StringData.ThirdUri);

            Console.WriteLine("Generation complete");

            server = new ThirdPartyServer(StringData.ThirdUri, privateKey);
            server.RunServer();
        }
    }
}
