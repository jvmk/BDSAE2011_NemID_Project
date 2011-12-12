using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
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

        /// <summary>
        /// Go to NemID login screen after successfully creating an account.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void toLoginScreenButton_Click(object sender, EventArgs e)
        {
            this.ParentForm.Controls.Clear();
            this.ParentForm.Controls.Add(new NemIdLogin());
        }
    }
}
