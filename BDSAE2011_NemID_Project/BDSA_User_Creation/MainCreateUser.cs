// -----------------------------------------------------------------------
// <copyright file="MainCreateUser.cs" company="Hewlett-Packard">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace BDSA_Project_User_Creation
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Windows.Forms;

    using BDSA_User_Creation;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class MainCreateUser
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

