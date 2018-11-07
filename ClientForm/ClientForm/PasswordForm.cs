using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GlobalVariables;

namespace ClientForm
{
    public partial class PasswordForm : Form
    {
        public string ReturnValue1 { get; set; }
        public string ReturnValue2 { get; set; }
        public PasswordForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var pass = Globals.ROOTPPassword; 
            if(this.textBox1.Text == pass) { ReturnValue1 = "pass"; }
            else ReturnValue1 = "fail";

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
