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
using System.Threading;

namespace ClientForm
{
    public partial class Browser : Form
    {
        private string url;
        Form1 form1;
        public Browser(string url,Form1 form1)
        {
            InitializeComponent();
            this.url = url;
            this.WindowState = FormWindowState.Maximized;
            form1 = this.form1;
        }

        private void Browser_LocationChanged(object sender, EventArgs e)
        {

        }

        private void Browser_Load(object sender, EventArgs e)
        {
            webBrowser1.Navigate(url);
                              //可用時間              //倒數 n 分鐘
            timer1.Interval = (Globals.AccessTime2 - Globals.WarnTimeDowunCount2); // 運行 interval 分後 trig.
            timer1.Enabled = true;
        }

        // 使用的時間到期 show 警語
        private void timer1_Tick(object sender, EventArgs e)
        {  
            label1.Text = Globals.Msg2; // 警語文字
            Thread.Sleep(5000); //5sec wait            
        }

        private void Browser_FormClosed(object sender, FormClosedEventArgs e)
        {
            Form1.GForm1.Show();
        }
    }
}
