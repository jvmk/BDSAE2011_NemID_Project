// -----------------------------------------------------------------------
// <copyright file="MainCreateUser.cs" company="Hewlett-Packard">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace BDSA_Project_GUI
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Windows.Forms;

    using BDSA_Project_Communication;

    using BDSA_Project_Cryptography;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class MainCreateUser
    {
            using System.IO;
    using System.Runtime.InteropServices;

    using BDSA_Project_Communication;

    using BDSA_Project_Cryptography;

    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main(String[] args)
        {
            if (args == null)
            {
                return;
            }
            Console.WriteLine("Loading the user creation screen");

            // Start up application.
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new UserCreation());
        }
    }
}
}
