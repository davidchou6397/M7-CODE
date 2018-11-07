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
	public partial class Form2 : Form
	{
		public Form2()
		{
			InitializeComponent();
			this.WindowState = FormWindowState.Maximized;
			this.ControlBox = false;
		}

		private void Form2_Load(object sender, EventArgs e)
		{
            richTextBox1.Text = Globals.TEXT2;
            this.FormClosing += Form2_FormClosing;
		//	this.Resize += new EventHandler(Form2_Resize);
		}

		private void Form2_Resize(object sender, EventArgs e)
		{
			if (WindowState == FormWindowState.Minimized)
			{
				Form form3 = new Form3(this);
				form3.Show();
			}
		}

		private void Form2_FormClosing(object sender, FormClosingEventArgs e)
		{
			e.Cancel = true;
		}

		private void AgreeCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			/*if (AgreeCheckBox.Checked)
			{
				AgreeButton.Enabled = true;
			}
			else
			{
				AgreeButton.Enabled = false;
			}*/
		}
        public static Form4 form4;
		private void AgreeButton_Click(object sender, EventArgs e)
		{
			
		}

		private void richTextBox1_TextChanged(object sender, EventArgs e)
		{

		}

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            form4 = new Form4();
            form4.Show();
            this.Hide();
        }
    }
}
