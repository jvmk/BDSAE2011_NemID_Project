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
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Windows.Forms;

    using BDSA_Project_Communication;

    using BDSA_User_Creation;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class MainCreateUser
    {
        /// <summary>
        /// This enables us to print to a console like a regular console application
        /// Code found at:
        /// http://social.msdn.microsoft.com/Forums/en-US/csharpgeneral/thread/701e02cc-ad77-46b9-b4fc-410bb3ff7a0d/
        /// </summary>
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool FreeConsole();

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

            AllocConsole();

            StringData.filePath = args[0];

            Console.WriteLine("Loading the user creation screen");

            // Start up application.
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new UserCreation());
        }
    }
}

