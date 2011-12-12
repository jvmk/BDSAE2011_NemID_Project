// -----------------------------------------------------------------------
// <copyright file="MainThirdParty.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace BDSA_Project_ThirdParty
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Starts the ThirdParty server.
    /// </summary>
    public class MainThirdParty
    {
        private static string serverUri = "http://localhost:8082/";

        private static ThirdPartyServer server;

        private static byte[] privateKey;

        /// <summary>
        /// Starts the ThirdParty server.
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            privateKey = BDSA_Project_Cryptography.Cryptograph.GenerateKeys(serverUri);
            server = new ThirdPartyServer(serverUri, privateKey);
            server.RunServer();
        }
    }
}
