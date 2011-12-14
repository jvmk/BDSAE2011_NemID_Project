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
    /// <author>
    /// Kenneth Lundum Søhrmann
    /// </author>
    public class MainAuthenticator
    {
        /// <summary>
        /// Starts up the Authenticator's server.
        /// </summary>
        public static void Main(string[] args)
        {
            if (args == null)
            {
                return;
            }
            StringData.filePath = args[0];

            Console.WriteLine("\nGenerates public/private keypair for authenticator");

            // Generate public/private key pair of the authenticator.
            byte[] privateKey = BDSA_Project_Cryptography.Cryptograph.GenerateKeys(StringData.AuthUri);

            Console.WriteLine("Generation complete.");

            // Start up the server.
            AuthenticatorService server = new AuthenticatorService(StringData.AuthUri, privateKey);
            server.ServiceLoop();
        }

    }
}
