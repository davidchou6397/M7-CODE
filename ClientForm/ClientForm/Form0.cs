using Microsoft.Win32;
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
    public partial class Form0 : Form
    {
        private string url;
        public void SetTaskManager(bool enable)
        {
            RegistryKey objRegistryKey = Registry.CurrentUser.CreateSubKey(
                @"Software\Microsoft\Windows\CurrentVersion\Policies\System");
            if (enable && objRegistryKey.GetValue("DisableTaskMgr") != null)
                objRegistryKey.DeleteValue("DisableTaskMgr");
            else
                objRegistryKey.SetValue("DisableTaskMgr", "1");
            objRegistryKey.Close();
        }
        public void ToggleTaskManager()
        {
            RegistryKey objRegistryKey = Registry.CurrentUser.CreateSubKey(
                @"Software\Microsoft\Windows\CurrentVersion\Policies\System");
            if (objRegistryKey.GetValue("DisableTaskMgr") == null)
                objRegistryKey.SetValue("DisableTaskMgr", "1");
            else
                objRegistryKey.DeleteValue("DisableTaskMgr");
            objRegistryKey.Close();
        }
        public Form0(string url)
        {
          //  SetTaskManager(false);
            // ToggleTaskManager();
            InitializeComponent();
            this.url = url;
            this.WindowState = FormWindowState.Maximized;
        }

    

        private void Form0_Load(object sender, EventArgs e)
        {
            webBrowser1.Navigate(url);
        }
        protected override void OnMouseMove(MouseEventArgs mouseEv)
        {
            var x=mouseEv.Location.X;

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if( Cursor.Position.X > this.Width / 2)
            {
               // this.Hide();
            }
           
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            var x = e.Location.X;
        }
    }
}
