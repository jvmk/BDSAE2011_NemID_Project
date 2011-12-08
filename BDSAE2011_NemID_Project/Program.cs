﻿namespace BDSAE2011_NemID_Project
{

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using System.IO;

    using AuthenticationService;

    using AuthenticatorComponent;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class Program
    {
        PublicKeyInfrastructure pki = new PublicKeyInfrastructure();

        public static void Main(string[] args)
        {
            PublicKeyInfrastructure pki = new PublicKeyInfrastructure();
            Cryptograph crypto = new Cryptograph();
            RSAParameters privateKey = crypto.GenerateKeys("simonlanghoff@gmail.com");

            const string PlainText = "hi";
            RSAParameters publicKey = crypto.GetPublicKey("simonlanghoff@gmail.com");

            string encryptedText = Cryptograph.Encrypt(PlainText, publicKey);

            Console.WriteLine("This is the encrypted Text:" + "\n " + encryptedText);

            string decryptedText = Cryptograph.Decrypt(encryptedText, privateKey);

            Console.WriteLine("This is the decrypted text: " + decryptedText);

        }
    }
}
