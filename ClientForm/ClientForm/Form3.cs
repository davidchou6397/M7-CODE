using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClientForm
{
	public partial class Form3 : Form
	{
		public Form activeForm;
		public Form3(Form form)
		{
			InitializeComponent();
			activeForm = form;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.FormBorderStyle = FormBorderStyle.FixedSingle;
		}

		private void Form3_Load(object sender, EventArgs e)
		{
			this.FormClosing += Form3_FormClosing;
		}

		private void Form3_FormClosing(object sender, FormClosingEventArgs e)
		{
			activeForm.WindowState = FormWindowState.Maximized;
		}
	}
}
