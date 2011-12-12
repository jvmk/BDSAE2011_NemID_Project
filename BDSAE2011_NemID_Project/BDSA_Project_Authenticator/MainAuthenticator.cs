﻿// -----------------------------------------------------------------------
// <copyright file="MainAuthenticator.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace BDSA_Project_Authenticator
{

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class MainAuthenticator
    {
        private const string serverUri = "http://localhost:8081/";

        public static void Main(string[] args)
        {
            byte[] privateKey = BDSA_Project_Cryptography.Cryptograph.GenerateKeys(serverUri);
            var server = new AuthenticatorService(serverUri, privateKey);
            server.ServiceLoop();
        }

    }
}