// -----------------------------------------------------------------------
// <copyright file="MainAuthenticator.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace BDSA_Project_Authenticator
{
    using System;

    using BDSA_Project_Communication;

    /// <summary>
    /// Entry point for setting up the authentication service.
    /// </summary>
    public class MainAuthenticator
    {
        /// <summary>
        /// Starts up the Authenticator's server.
        /// </summary>
        public static void Main()
        {

           // Generate public/private key pair of the authenticator.
            byte[] privateKey = BDSA_Project_Cryptography.Cryptograph.GenerateKeys(StringData.AuthUri);

            // Start up the server.
            AuthenticatorService server = new AuthenticatorService(StringData.AuthUri, privateKey);
            server.ServiceLoop();
        }

    }
}
