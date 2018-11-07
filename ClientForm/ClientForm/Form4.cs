using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;
using GlobalVariables;
using ServerForm;
using Newtonsoft.Json.Linq;
using System.Threading;
using System.Diagnostics;
using Microsoft.Win32;

namespace ClientForm
{
    public partial class Form4 : Form
    {
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);
        enum KeyModifier
        {
            None = 0,
            Alt = 1,
            Control = 2,
            Shift = 4,
            WinKey = 8
        }

        ServerClass serverResponse = new ServerClass();
        System.Windows.Forms.Timer timer1 = new System.Windows.Forms.Timer();
        private int counter = 0;
        System.Threading.Timer timer;
        public Form4()
        {
            int id = 0;     // The id of the hotkey. 
            RegisterHotKey(this.Handle, id, (int)KeyModifier.Control, Keys.F2.GetHashCode());

            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            this.ControlBox = false;


        }

        private void Form4_Load(object sender, EventArgs e)
        {
            this.FormClosing += Form4_FormClosing;
            this.Resize += new EventHandler(Form4_Resize);
            if(Globals.URL3!="" || Globals.URL3!=null) pictureBox1.Load(Globals.URL3);
        }

        private void Form4_Resize(object sender, EventArgs e)
        {
         /*   if (WindowState == FormWindowState.Minimized)
            {
                Form form3 = new Form3(this);
                form3.Show();
            }*/
        }

        private void Form4_FormClosing(object sender, FormClosingEventArgs e)
        {
            UnregisterHotKey(this.Handle, 0);
            e.Cancel = true;
        }
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
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x0312)
            {
                /* Note that the three lines below are not needed if you only want to register one hotkey.
                 * The below lines are useful in case you want to register multiple keys, which you can use a switch with the id as argument, or if you want to know which key/modifier was pressed for some particular reason. */

                Keys key = (Keys)(((int)m.LParam >> 16) & 0xFFFF);                  // The key of the hotkey that was pressed.
                KeyModifier modifier = (KeyModifier)((int)m.LParam & 0xFFFF);       // The modifier of the hotkey that was pressed.
                int id = m.WParam.ToInt32();                                        // The id of the hotkey that was pressed.



                var result = new PasswordForm();

                if (result.ShowDialog() == DialogResult.OK)
                {
                    string val = result.ReturnValue1;            //values preserved after close
                    if (val == "pass")
                    {
                        MessageBox.Show("System will quit");
                        SetTaskManager(true);
                        foreach (var process in Process.GetProcessesByName("ClientForm"))
                        {
                            process.Kill();
                        }

                    }
                }

                /*
                                RegistryKey regkey = default(RegistryKey);
                                string keyValueInt = "0";
                                //0x00000000 (0)
                                string subKey = "Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System";
                                try
                                {
                                    regkey = Registry.CurrentUser.CreateSubKey(subKey);
                                    regkey.SetValue("DisableTaskMgr", keyValueInt);
                                    regkey.Close();
                                }
                                catch (Exception ex)
                                {

                                }
                                // do something*/

            }
            base.WndProc(ref m);
        }
        //訪客登入
        private void buttonGuestLogin_Click(object sender, EventArgs e)
        {
            timer2.Enabled = false;
            string strURL = "http://220.130.59.159/webopac/PcWs.asmx/getInfo?value01=&value02=";
            System.Net.HttpWebRequest request;
            // 创建一个HTTP请求
            request = (System.Net.HttpWebRequest)WebRequest.Create(strURL);
            //request.Method="get";
            System.Net.HttpWebResponse response;
            response = (System.Net.HttpWebResponse)request.GetResponse();
            System.IO.StreamReader myreader = new System.IO.StreamReader(response.GetResponseStream(), Encoding.UTF8);
            string responseText = myreader.ReadToEnd();
            myreader.Close();
            JArray responseArr = JArray.Parse(responseText);
            foreach (JObject content in responseArr.Children<JObject>())
            {
                foreach (JProperty prop in content.Properties())
                {
                    string tempValue = (string)prop.Value;
                    System.Console.WriteLine(prop.Name);
                    System.Console.WriteLine(tempValue);
                    if (prop.Name == "result0")
                    {
                        serverResponse.setResult0(tempValue);
                    }
                    else if (prop.Name == "result1")
                    {
                        serverResponse.setResult1(tempValue);
                    }
                    else if (prop.Name == "result2")
                    {
                        serverResponse.setResult2(tempValue);
                    }
                    else if (prop.Name == "result3")
                    {
                        serverResponse.setResult3(tempValue);
                    }
                }
                //MessageBox.Show("訪客只可使用" + serverResponse.getResult2() + "分鐘");
            }
            MessageBox.Show("訪客只可使用" +  Globals.AccessTime1 + "分鐘");
            browser = new Browser(Globals.URL1,null);
            browser.Show();
            
            this.Hide();
         
            timer2.Tag =  Globals.AccessTime1; // 可使用時間 放tag
            timer2.Interval = 1000;
            timer2.Enabled = true;
        }
        Browser browser;
        //預約登入
        private void buttonLogin_Click(object sender, EventArgs e)
        {
            /*if (ValidateLogin())
            {
                if (CheckUserPwd())
                {

                    timer2.Interval = 1000 * 80;
                    timer2.Tag = Globals.AccessTime;
                    timer2.Enabled = true;
                    browser = new Browser(Globals.URL1);
                    browser.Show();
                    // this.WindowState = FormWindowState.Minimized;
                    this.Hide();


                }
                else
                {
                    MessageBox.Show("帳號密碼錯誤");
                }
                //System.Diagnostics.Process.Start(Globals.URL1);
            }*/
        /*    timer2.Interval = 1000 * 80;
            timer2.Tag = Globals.AccessTime;
            timer2.Enabled = true;*/
            browser = new Browser(Globals.URL1,null);
            browser.ShowDialog();
            // this.WindowState = FormWindowState.Minimized;
            this.Hide();

        }
        private void timer1_Tick(object sender, System.EventArgs e)
        {
            if (counter == 20 * 60)
            {
                counter = counter - 1;
                TimerCallback callback = new TimerCallback(_do);
                //1.function 2.開關  3.等多久再開始  4.隔多久反覆執行
                timer = new System.Threading.Timer(callback, null, 0, 1000 * 60 * 5);
            }
            else if (counter == 0)
            {
                timer1.Dispose();
            }
            else
            {
                // Run your procedure here.
                // Increment counter.
                counter = counter - 1;
            }
        }
        private void _do(object state)
        {
            this.BeginInvoke(new setLable2(setLabel2));
            //label2.Text = DateTime.Now.ToString();
            //Console.WriteLine(DateTime.Now.ToString());
        }
        delegate void setLable2();
        private void setLabel2()
        {
            MessageBox.Show("ending");
            if (counter == 0)
            {
                timer.Dispose();
            }
        }

        private bool ValidateLogin()
        {
            if (textBoxUsername.Text.Trim().Length == 0)
            {
                MessageBox.Show("帳號不可為空");
                return false;
            }
            if (textBoxPwd.Text.Trim().Length == 0)
            {
                MessageBox.Show("密碼不可為空");
                return false;
            }
            return true;
        }

        private bool CheckUserPwd()
        {
            //TODO
            string strURL = "http://220.130.59.159/webopac/PcWs.asmx/getInfo?value01=" + textBoxUsername.Text.Trim() +
                "&value02=" + textBoxPwd.Text.Trim();
            System.Net.HttpWebRequest request;
            // 创建一个HTTP请求
            request = (System.Net.HttpWebRequest)WebRequest.Create(strURL);
            //request.Method="get";
            System.Net.HttpWebResponse response;
            response = (System.Net.HttpWebResponse)request.GetResponse();
            System.IO.StreamReader myreader = new System.IO.StreamReader(response.GetResponseStream(), Encoding.UTF8);
            string responseText = myreader.ReadToEnd();
            myreader.Close();
            JArray responseArr = JArray.Parse(responseText);
            foreach (JObject content in responseArr.Children<JObject>())
            {
                foreach (JProperty prop in content.Properties())
                {
                    string tempValue = (string)prop.Value;
                    System.Console.WriteLine(prop.Name);
                    System.Console.WriteLine(tempValue);
                    if (prop.Name == "result0")
                    {
                        serverResponse.setResult0(tempValue);
                    }
                    else if (prop.Name == "result1")
                    {
                        serverResponse.setResult1(tempValue);
                    }
                    else if (prop.Name == "result2")
                    {
                        serverResponse.setResult2(tempValue);
                    }
                    else if (prop.Name == "result3")
                    {
                        serverResponse.setResult3(tempValue);
                    }
                }
            }
            if (serverResponse.getResult0() == "1")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        static int cnt = 0;
        // 使用的時間到期 
        private void timer2_Tick(object sender, EventArgs e)
        {
            cnt++;
            if (cnt >= (int)timer2.Tag*60)
            {
                this.Show();
                this.WindowState = FormWindowState.Maximized;
                browser.Close();
                browser = null;
                cnt = 0;
                timer2.Enabled = false;
            }

        }
    }
}
