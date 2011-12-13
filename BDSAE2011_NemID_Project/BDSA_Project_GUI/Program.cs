using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace BDSA_Project_GUI
{
    using System.IO;

    using BDSA_Project_Cryptography;

    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Console.WriteLine("Client browser starting, generating a test user.");

            // Test user, for demonstration purposes only.
            // User name: testUser, password: password, cprNumber: 010101-0101, email: testUser@nemId.dk
            byte[] testUserPrivateKey = Cryptograph.GenerateKeys("testUser@nemId.dk");

            // Write the private key to the execution path.
            File.WriteAllBytes(@".\testUserPrivateKey.bin", testUserPrivateKey);

            Console.WriteLine("Test user generated, launching application.");

            // Start up application.
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new UsersBrowser());
        }
    }
}
