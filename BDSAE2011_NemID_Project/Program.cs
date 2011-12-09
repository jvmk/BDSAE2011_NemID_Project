namespace BDSAE2011_NemID_Project
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
        public static void Main(string[] args)
        {
            RSAParameters privateKey = Cryptograph.GenerateKeys("simlanghoff@gmail.com");

            const string PlainText = "This is really sent by me, really!";
 
            RSAParameters publicKey = Cryptograph.GetPublicKey("simlanghoff@gmail.com");

            string encryptedText = Cryptograph.Encrypt(PlainText, publicKey);

            Console.WriteLine("This is the encrypted Text:" + "\n " + encryptedText);

            string decryptedText = Cryptograph.Decrypt(encryptedText, privateKey);

            Console.WriteLine("This is the decrypted text: " + decryptedText);

            string messageToSign = encryptedText;

            string signedMessage = Cryptograph.SignData(messageToSign, privateKey);

            //// Is this message really, really, REALLY sent by me?
            bool success = Cryptograph.VerifyData(messageToSign, signedMessage, publicKey);

            Console.WriteLine("Is this message really, really, REALLY sent by me? " + success);
            
        }
    }
}
