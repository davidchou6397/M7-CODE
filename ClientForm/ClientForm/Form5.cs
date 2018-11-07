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
	public partial class Form5 : Form
	{
		Timer timer1 = new Timer();
		Timer timer = new Timer();
		private int counter = 0;
		public Form5()
		{
			InitializeComponent();
			timer1.Interval = 1000;
			timer1.Enabled = true;
			this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
			counter = 60 * 21;
		}

		private void timer1_Tick(object sender, System.EventArgs e)
		{
			if (counter <= 20 * 60)
			{
				if (counter == 0)
				{
					// Exit loop code.
					timer1.Enabled = false;
					counter = 0;
					MessageBox.Show("alert");
				}
				else if (counter % (5 * 60) == 0)
				{
					Console.WriteLine(counter / 60);
					counter = counter - 1;
					this.Show();
					timer.Interval = 10000;
					timer.Tick += new EventHandler(timer_Tick);
					timer.Start();
				}
			}
			else
			{
				Console.WriteLine(counter / 60);
				// Run your procedure here.
				// Increment counter.
				counter = counter - 1;
			}
		}
		void timer_Tick(object sender, EventArgs e)
		{
			this.Hide();
			timer.Stop();
		}
	}

}
