using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;
using GlobalVariables;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Runtime.InteropServices;
using System.Reflection;

namespace ClientForm
{
    public partial class Form1 : Form
    {
        private static bool USBState;

        private Socket m_clientSocket;
        public AsyncCallback m_pfnCallBack;
        IAsyncResult m_result;
        private byte[] data = new byte[1024];
        //private int size = 1024;

        // ManualResetEvent instances signal completion.  
        private static ManualResetEvent connectDone =
            new ManualResetEvent(false);
        private static ManualResetEvent receiveDone =
      new System.Threading.ManualResetEvent(false);


        private static String response = String.Empty;

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

        public class StateObject
        {
            public System.Net.Sockets.Socket workSocket;
            public byte[] buffer = new byte[1024];
            public const int BufferSize = 256;
            // Received data string.  
            public StringBuilder sb = new StringBuilder();
        }
        public static Form1  GForm1;
        public Form1()
        {
            InitializeComponent();

            int id = 0;     // The id of the hotkey. 
            RegisterHotKey(this.Handle, id, (int)KeyModifier.Control, Keys.F2.GetHashCode());

            this.WindowState = FormWindowState.Maximized;
            this.ControlBox = true;
            GForm1 = this;

        }
        // 設定parse
        private static void ReceiveCallback(IAsyncResult asyn)
        {
            try
            {
                StateObject theSockId = (StateObject)asyn.AsyncState;
                int iRx = theSockId.workSocket.EndReceive(asyn);
                char[] chars = new char[iRx + 1];
                System.Text.Decoder d = System.Text.Encoding.UTF8.GetDecoder();
                int charLen = d.GetChars(theSockId.buffer, 0, iRx, chars, 0);
                System.String szData = new System.String(chars);
                Console.WriteLine(szData);
                
                string []szData1=szData.Split('\n');
                for (int i = 0; i < szData1.Length; i++)
                {
                                                         
                      if (szData1[i].StartsWith("USBState="))
                    {
                        String USBStateStr = szData1[i].Split('=')[1];
                        if (USBStateStr.StartsWith("True"))
                        {
                            USBState = true;
                        }
                        else if (USBStateStr.StartsWith("False"))
                        {
                            USBState = false;
                        }
                        ChangeUSBState();
                    }
                    else if (szData1[i].StartsWith("AccessTime1"))
                    {
                        int t = Int32.Parse(szData1[i].Split('=')[1]);
                        Globals.SetAccessTime1(t);
                    }
                    else if (szData1[i].StartsWith("WarnTimeDownCount1"))
                    {
                        int t = Int32.Parse(szData1[i].Split('=')[1]);
                        Globals.SetWarnTimeDowunCount1(t);
                    }
                    else if (szData1[i].StartsWith("MSG1"))
                    {
                        string s = szData1[i].Split('=')[1] ;
                        Globals.SetMsg1 (s);
                    }
                    else if (szData1[i].StartsWith("AccessTime2"))
                    {
                        int t = Int32.Parse(szData1[i].Split('=')[1]);
                        Globals.SetAccessTime2(t);
                    }
                    else if (szData1[i].StartsWith("WarnTimeDownCount2"))
                    {
                        int t= Int32.Parse(szData1[i].Split('=')[1]);
                        Globals.SetWarnTimeDowunCount2(t);
                    }
                    else if (szData1[i].StartsWith("MSG2"))
                    {
                        string s = szData1[i].Split('=')[1];
                        Globals.SetMsg2(s);
                    }

                    else if (szData1[i].StartsWith("URL1="))
                    {
                        String s = szData1[i].Split('=')[1];
                        Globals.SetURL1(s);
                    }
                    else if (szData1[i].StartsWith("URL2="))
                    {
                        String s = szData1[i].Split('=')[1];
                        Globals.SetURL2(s);
                    }
                    else if (szData1[i].StartsWith("URL3="))
                    {
                        String s = szData1[i].Split('=')[1];
                        Globals.SetURL3(s);
                    }
                    else if (szData1[i].StartsWith("ROOT="))
                    {
                        String s = szData1[i].Split('=')[1];
                        Globals.SetROOT(s);
                    }
                    else if (szData1[i].StartsWith("URL4="))
                    {
                        String s = szData1[i].Split('=')[1];
                        Globals.SetURL4(s);
                    }
                    else if (szData1[i].StartsWith("TEXT1="))
                    {
                        String s = szData1[i].Split('=')[1].Replace('\t','\n');
                        Globals.SetText1(s);
                    }
                    else if (szData1[i].StartsWith("TEXT2="))
                    {
                        String s = szData1[i].Split('=')[1].Replace('\t', '\n');
                        Globals.SetText2(s);
                    }
                    
                }
                Receive(theSockId.workSocket);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            Globals.Active = true;
        }
        private static void Receive(Socket client)
        {
            try
            {
                // Create the state object.  
                StateObject state = new StateObject();
                state.workSocket = client;

                // Begin receiving the data from the remote device.  
                client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ReceiveCallback), state);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        private static void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.  
                Socket client = (Socket)ar.AsyncState;

                // Complete the connection.  
                client.EndConnect(ar);

                Console.WriteLine("Socket connected to {0}",
                    client.RemoteEndPoint.ToString());

                // Signal that the connection has been made.  
                connectDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            var MyIni = new IniFile("Settings.ini"); //讀取設定檔
            var host= MyIni.Read("Host");

            try
            {
                m_clientSocket = new Socket(AddressFamily.InterNetwork,
                                     SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint iep = new IPEndPoint(IPAddress.Parse(host), 9050);
                // Create a TCP/IP socket.  

                // Connect to the remote endpoint.  
                m_clientSocket.BeginConnect(iep,
                    new AsyncCallback(ConnectCallback), m_clientSocket);
                connectDone.WaitOne();
                Console.WriteLine("Response received : {0}", response);

                // Send test data to the remote device.  
                //Send(client, "This is a test<EOF>");
                //sendDone.WaitOne();

                // Receive the response from the remote device.  
                Receive(m_clientSocket);

                //receiveDone.WaitOne();

                // Write the response to the console.  
                Console.WriteLine("Response received : {0}", response);

                /* m_clientSocket.Connect(iep);
                 if (m_clientSocket.Connected)
                 {
                     //Wait for data asynchronously 
                     //WaitForData();
                     Receive(m_clientSocket);
                     //receiveDone.WaitOne();

                     // Write the response to the console.  
                     Console.WriteLine("Response received : {0}", response);

                 }*/
            }
            catch (SocketException se)
            {
                Console.WriteLine(se);
            }
            
            if (Globals.URL4 != "" && Globals.URL4 != null) pictureBox1.Load(Globals.URL4); // 初始畫面-logo
            richTextBox1.Text = Globals.TEXT1;


            this.Resize += new EventHandler(Form1_Resize);
            this.FormClosing += Form1_FormClosing;
            

        }
        /*
                public void WaitForData()
                {
                    try
                    {
                        if (m_pfnCallBack == null)
                        {
                            m_pfnCallBack = new AsyncCallback(OnDataReceived);
                        }
                        SocketPacket theSocPkt = new SocketPacket();
                        theSocPkt.thisSocket = m_clientSocket;
                        // Start listening to the data asynchronously
                        m_result = m_clientSocket.BeginReceive(theSocPkt.dataBuffer,
                                                                0, theSocPkt.dataBuffer.Length,
                                                                SocketFlags.None,
                                                                m_pfnCallBack,
                                                                theSocPkt);
                    }
                    catch (SocketException se)
                    {
                        Console.WriteLine(se.Message);
                    }

                }

                */



        private static void  ChangeUSBState()
        {
            int rValue;
            //USB open
            if (USBState)
            {
                rValue = 3;
                RegistryKey key =
       Registry.LocalMachine.OpenSubKey
          ("SYSTEM\\CurrentControlSet\\Services\\UsbStor", true);
                if (key != null)
                {
                    key.SetValue("Start", 3, RegistryValueKind.DWord);
                }
                key.Close();
            }
            else
            {
                //USB close
                rValue = 4;
                RegistryKey key =
        Registry.LocalMachine.OpenSubKey
           ("SYSTEM\\CurrentControlSet\\Services\\UsbStor", true);
                if (key != null)
                {
                    key.SetValue("Start", 4, RegistryValueKind.DWord);
                }
                key.Close();
            }

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                Form form3 = new Form3(this);
                form3.Show();
            }
            
        }
        Browser browser;
        private void reserveButton_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            /* Form2 form2 = new Form2();
             form2.ShowDialog();*/
            browser = new Browser(Globals.URL1,this);
            browser.Text = "預約FORM(URL1)";
            browser.Show();

            this.Hide();
            
        }

        class IniFile   // revision 11
        {
            string Path;
            string EXE = Assembly.GetExecutingAssembly().GetName().Name;

            [DllImport("kernel32", CharSet = CharSet.Unicode)]
            static extern long WritePrivateProfileString(string Section, string Key, string Value, string FilePath);

            [DllImport("kernel32", CharSet = CharSet.Unicode)]
            static extern int GetPrivateProfileString(string Section, string Key, string Default, StringBuilder RetVal, int Size, string FilePath);

            public IniFile(string IniPath = null)
            {
                Path = new FileInfo(IniPath ?? EXE + ".ini").FullName.ToString();
            }

            public string Read(string Key, string Section = null)
            {
                var RetVal = new StringBuilder(255);
                GetPrivateProfileString(Section ?? EXE, Key, "", RetVal, 255, Path);
                return RetVal.ToString();
            }

            public void Write(string Key, string Value, string Section = null)
            {
                WritePrivateProfileString(Section ?? EXE, Key, Value, Path);
            }

            public void DeleteKey(string Key, string Section = null)
            {
                Write(Key, null, Section ?? EXE);
            }

            public void DeleteSection(string Section = null)
            {
                Write(null, null, Section ?? EXE);
            }

            public bool KeyExists(string Key, string Section = null)
            {
                return Read(Key, Section).Length > 0;
            }
        }
        private bool CheckOpened(string name)
        {
            FormCollection fc = Application.OpenForms;

            foreach (Form frm in fc)
            {
                if (frm.Text == name)
                {
                    return true;
                }
            }
            return false;
        }
        public Form0 form0;
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (CheckOpened("新書介紹"))
            {   form0.WindowState= FormWindowState.Maximized;
                form0.Show();
                if (Globals.URL4 != "" && Globals.URL4 != null) pictureBox1.Load(Globals.URL4); // 初始畫面-logo
                richTextBox1.Text = Globals.TEXT1;
                return;
            }
            form0 = new Form0(Globals.URL2);
            form0.Show();
            timer1.Interval = 30000;
            
        }

        private void Form1_Activated(object sender, EventArgs e)
        {
          //  Thread.Sleep(2000);
            //  timer1.Enabled = true;        // 是否叫出"新書介紹"Form的偵測
            if (Globals.URL4 != "" && Globals.URL4 != null) pictureBox1.Load(Globals.URL4); // 初始畫面-logo
            richTextBox1.Text = Globals.TEXT1;

            
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            if (Globals.Active)
            {
                if (Globals.URL4 != "" && Globals.URL4 != null) pictureBox1.Load(Globals.URL4); // 初始畫面-logo
                richTextBox1.Text = Globals.TEXT1;
            }
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

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            Form2 f = new Form2();
            f.Text = "規則頁";
            f.Show();
        }
    }
}
