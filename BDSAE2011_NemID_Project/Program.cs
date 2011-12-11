namespace BDSAE2011_NemID_Project
{

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using System.IO;

    using AuthenticatorComponent;

    using Miscellaneoues;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class Program
    {
        public static void Main(string[] args)
        {
            byte[] privateKey = Cryptograph.GenerateKeys("simlanghoff@gmail.com");

            File.WriteAllBytes(@"C:\Test\privateKey.bin", privateKey);

            byte[] privateImportedKeyFile = File.ReadAllBytes(@"C:\Test\privateKey.bin");

            const string PlainText = "This is really sent by me, really!";

            byte[] publicKey = Cryptograph.GetPublicKey("simlanghoff@gmail.com");

            Console.Write(PublicKeyInfrastructure.ValidKeyBlob(publicKey));

            Console.Write(PublicKeyInfrastructure.ValidKeyBlob(privateKey));

            string encryptedText = Cryptograph.Encrypt(PlainText, publicKey);

            Console.WriteLine("This is the encrypted Text:" + "\n " + encryptedText);

            string decryptedText = Cryptograph.Decrypt(encryptedText, privateImportedKeyFile);

            Console.WriteLine("This is the decrypted text: " + decryptedText);

            string messageToSign = encryptedText;

            string signedMessage = Cryptograph.SignData(messageToSign, privateImportedKeyFile);

            //// Is this message really, really, REALLY sent by me?
            bool success = Cryptograph.VerifyData(messageToSign, signedMessage, privateImportedKeyFile);

            Console.WriteLine("Is this message really, really, REALLY sent by me? " + success);

            UserAccount user1 = new UserAccount("user", "passy", "0101012949");

            uint keyIndex = user1.Keycard.GetKeyIndex();

            uint keyToEnter = 0;

            bool correctkey = user1.Keycard.VerifyEnteredKey(keyToEnter);

            Console.Write(correctkey);
        }
    }
}
