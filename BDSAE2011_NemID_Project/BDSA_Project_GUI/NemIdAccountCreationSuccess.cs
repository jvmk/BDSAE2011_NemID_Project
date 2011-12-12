using System;
using System.Windows.Forms;

namespace BDSA_Project_GUI
{
    /// <summary>
    /// User control: shown when an account is successfully created.
    /// </summary>
    public partial class NemIdAccountCreationSuccess : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the NemIdAccountCreationSuccess class.
        /// </summary>
        public NemIdAccountCreationSuccess()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
