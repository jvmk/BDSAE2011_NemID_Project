using System;
using System.Windows.Forms;

namespace BDSA_Project_GUI
{
    using System.IO;
    using System.Runtime.InteropServices;

    using BDSA_Project_Communication;

    using BDSA_Project_Cryptography;

    static class Program
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
        static void Main(String[] args)
        {
            AllocConsole();
            if (args == null)
            {
                return;
            }
            StringData.filePath = args[0];

            // Start up application.
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new UsersBrowser());
        }
    }
}
