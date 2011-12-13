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
            // Test user, for demonstration purposes only.
            // User name: testUser, password: password, cprNumber: 010101-0101, email: testUser@nemId.dk
            byte[] testUserPrivateKey = Cryptograph.GenerateKeys("testUser@nemId.dk");

            // Write the private key to the execution path.
            File.WriteAllBytes(@".\testUserPrivateKey.bin", testUserPrivateKey);

            // Start up application.
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new UsersBrowser());
        }
    }
}
