using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
/*
 * api url 設定沒寫
 * 
 * */

/* time control textbox number
 * 訪客 10 11 1
 * 預約 12 9 8
                    textBox10.Text = (string)obj["AccessTime1"];
                    textBox11.Text = (string)obj["WarnTimeDownCount1"];
                    textBox1.Text = (string)obj["MSG1"];

                    textBox12.Text = (string)obj["AccessTime2"];
                    textBox9.Text = (string)obj["WarnTimeDownCount2"];
                    textBox8.Text = (string)obj["MSG2"];
 * 
 * */
namespace ServerForm
{

    public partial class Form2 : Form
    {
        public bool USBState;
        public int AccessTime;

        private System.Collections.ArrayList m_workerSocketList =
                ArrayList.Synchronized(new System.Collections.ArrayList());
        private int m_clientCount = 0;

        public Form2(ArrayList socketList, int count)
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            radioButtonClose.Checked = true;

            m_workerSocketList = socketList; // client 列表array
            m_clientCount = count;

         
            string EXE = Assembly.GetExecutingAssembly().GetName().Name;
            string Path = new FileInfo("settings" ?? EXE).FullName.ToString();

            using (FileStream fs = new FileStream(Path, FileMode.Open, FileAccess.Read))
            using (StreamReader sr = new StreamReader(fs))
            using (JsonTextReader reader = new JsonTextReader(sr))
            {
                while (reader.Read())
                {
                    if (reader.TokenType == JsonToken.StartObject)
                    {
                        // Load each object from the stream and do something with it
                        JObject obj = JObject.Load(reader);
                        USBState = ((string)obj["USBState"] == "true") ? true : false;
                        
                        textBox2.Text = (string)obj["URL1"];
                        textBox3.Text = (string)obj["URL2"];
                        textBox6.Text = (string)obj["URL3"];
                        textBox7.Text = (string)obj["URL4"];
                        textBox5.Text = (string)obj["URL5"]; //api url
                        richTextBox1.Text = (string)obj["TEXT1"];
                        richTextBox2.Text = (string)obj["TEXT2"];
                        
                        /*
                        riter.WritePropertyName("AccessTime1");
                writer.WriteValue(textBox10.Text);
                writer.WritePropertyName("WarnTimeDownCount1");
                writer.WriteValue(textBox11.Text);
                writer.WritePropertyName("MSG1");
                writer.WriteValue(textBox1.Text);

                writer.WritePropertyName("AccessTime2");
                writer.WriteValue(textBox12.Text);
                writer.WritePropertyName("WarnTimeDownCount2");
                writer.WriteValue(textBox11.Text);
                writer.WritePropertyName("MSG2");
                writer.WriteValue(textBox1.Text);
                */

                        textBox10.Text = (string)obj["AccessTime1"];
                        textBox11.Text = (string)obj["WarnTimeDownCount1"];
                        textBox1.Text = (string)obj["MSG1"];

                        textBox12.Text = (string)obj["AccessTime2"];
                        textBox9.Text = (string)obj["WarnTimeDownCount2"];
                        textBox8.Text = (string)obj["MSG2"];
                     

                        textBox4.Text = (string)obj["ROOT"];

                        //Console.WriteLine(obj["id"] + " - " + obj["name"]);
                    }
                }
            }

        }

        private void SendMsgToClient(string msg)
        {
            try
            {
                byte[] byData = System.Text.Encoding.ASCII.GetBytes(msg);
                Socket workerSocket = null;
                for (int i = 0; i < m_workerSocketList.Count; i++)
                {
                    workerSocket = (Socket)m_workerSocketList[i];
                    if (workerSocket != null)
                    {
                        if (workerSocket.Connected)
                        {
                            workerSocket.Send(byData);
                        }
                    }
                }
            }
            catch (SocketException se)
            {
                Console.WriteLine(se.Message);
            }
        }
        //function list
        private void buttonUSB_Click(object sender, EventArgs e)
        {
            if (radioButtonClose.Checked)
            {
                USBState = false;
            }
            if (radioButtonOpen.Checked)
            {
                USBState = true;
            }
            string msg = "USBState=" + USBState.ToString();
            msg = msg + "\n";
            SendMsgToClient(msg);
        }

       
        //invoke browser to access url
        private void button2_Click(object sender, EventArgs e)
        {
            string msg = "URL1=" + textBox2.Text;
            msg = msg + "\n";
            SendMsgToClient(msg);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string msg = "URL2=" + textBox3.Text;
            msg = msg + "\n";
            SendMsgToClient(msg);
        }


        // 底圖set
        private void button6_Click(object sender, EventArgs e)
        {
            string msg = "URL3=" + textBox6.Text;
            msg = msg + "\n";
            SendMsgToClient(msg);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {

         

                        StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);

            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                writer.Formatting = Formatting.Indented;

                writer.WriteStartObject();
                writer.WritePropertyName("USBState");
                writer.WriteValue(USBState.ToString());
               
                writer.WritePropertyName("URL1");
                writer.WriteValue(textBox2.Text);
                writer.WritePropertyName("URL2");
                writer.WriteValue(textBox3.Text);
                writer.WritePropertyName("URL3");
                writer.WriteValue(textBox6.Text);
                writer.WritePropertyName("URL4");
                writer.WriteValue(textBox7.Text);

                writer.WritePropertyName("URL5");
                writer.WriteValue(textBox5.Text);

                writer.WritePropertyName("TEXT1");
                writer.WriteValue(richTextBox1.Text);

                writer.WritePropertyName("TEXT2");
                writer.WriteValue(richTextBox2.Text);

                /*
                 * 
                 * 
                  textBox10.Text = (string)obj["AccessTime"];
                        textBox11.Text=(string)obj["WarnTimeDownCount1"];
                        textBox1.Text = (string)obj["MSG1"];
                        */
                writer.WritePropertyName("AccessTime1");
                writer.WriteValue(textBox10.Text);
                writer.WritePropertyName("WarnTimeDownCount1");
                writer.WriteValue(textBox11.Text);
                writer.WritePropertyName("MSG1");
                writer.WriteValue(textBox1.Text);

                writer.WritePropertyName("AccessTime2");
                writer.WriteValue(textBox12.Text);
                writer.WritePropertyName("WarnTimeDownCount2");
                writer.WriteValue(textBox9.Text);
                writer.WritePropertyName("MSG2");
                writer.WriteValue(textBox8.Text);


                writer.WritePropertyName("ROOT");
                writer.WriteValue(textBox4.Text);


                writer.WriteEndObject();
            }
            string EXE = Assembly.GetExecutingAssembly().GetName().Name;
            string Path = new FileInfo("settings" ?? EXE).FullName.ToString();
            System.IO.StreamWriter file = new System.IO.StreamWriter(Path);
            file.WriteLine(sb.ToString()); // "sb" is the StringBuilder
            file.Close();

            timer1.Interval = 3000;

            if (m_workerSocketList.Count != m_clientCount)
            {
                string msg;
                msg = "USBState=" + USBState.ToString();
                msg = msg + "\n";
                SendMsgToClient(msg);



                msg = "";
                msg = "URL1=" + textBox2.Text;
                msg = msg + "\n";
                SendMsgToClient(msg);


                msg = "";
                msg = "URL2=" + textBox3.Text;
                msg = msg + "\n";
                SendMsgToClient(msg);

                msg = "";
                msg = "URL3=" + textBox6.Text;
                msg = msg + "\n";
                SendMsgToClient(msg);

                msg = "";
                msg = "URL4=" + textBox7.Text;
                msg = msg + "\n";
                SendMsgToClient(msg);

                msg = "";
                msg = "TEXT1=" + richTextBox1.Text.Replace('\n', '\t');
                msg = msg + "\n";
                SendMsgToClient(msg);

                msg = "";
                msg = "TEXT2=" + richTextBox2.Text.Replace('\n', '\t');
                msg = msg + "\n";
                SendMsgToClient(msg);
                /*
                                * 
                                * 
                                 textBox10.Text = (string)obj["AccessTime"];
                                       textBox11.Text=(string)obj["WarnTimeDownCount1"];
                                       textBox1.Text = (string)obj["MSG1"];
                                       */
                                       /*
                writer.WritePropertyName("AccessTime1");
                writer.WriteValue(textBox10.Text);
                writer.WritePropertyName("WarnTimeDownCount1");
                writer.WriteValue(textBox11.Text);
                writer.WritePropertyName("MSG1");
                writer.WriteValue(textBox1.Text);

                writer.WritePropertyName("AccessTime2");
                writer.WriteValue(textBox12.Text);
                writer.WritePropertyName("WarnTimeDownCount2");
                writer.WriteValue(textBox11.Text);
                writer.WritePropertyName("MSG2");
                writer.WriteValue(textBox1.Text);
                */
                msg = "";
                msg = "AccessTime1=" + textBox10.Text;
                msg = msg + "\n";
                SendMsgToClient(msg);

                msg = "WarnTimeDownCount1=" + textBox11.Text;
                msg = msg + "\n";
                SendMsgToClient(msg);

                msg = "MSG1=" + textBox1.Text;
                msg = msg + "\n";
                SendMsgToClient(msg);

                msg = "";
                msg = "AccessTime2=" + textBox12.Text;
                msg = msg + "\n";
                SendMsgToClient(msg);

                msg = "WarnTimeDownCount2=" + textBox9.Text;
                msg = msg + "\n";
                SendMsgToClient(msg);

                msg = "MSG2=" + textBox8.Text;
                msg = msg + "\n";
                SendMsgToClient(msg);



                msg = "ROOT=" + textBox4.Text;
                msg = msg + "\n";
                SendMsgToClient(msg);

                m_clientCount = m_workerSocketList.Count;
            }

        }

        private void timer4_Tick(object sender, EventArgs e)
        {

        }

        private void Form2_Activated(object sender, EventArgs e)
        {
            timer1.Enabled = true;
        }
        // hotkey password setting
        private void button4_Click(object sender, EventArgs e)
        {
            string msg = "ROOT=" + textBox4.Text;
            msg = msg + "\n";
            SendMsgToClient(msg);
        }
        // logo url
        private void button7_Click(object sender, EventArgs e)
        {
            string msg = "URL4=" + textBox7.Text;
            msg = msg + "\n";
            SendMsgToClient(msg);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            string msg = "TEXT1=" + richTextBox1.Text.Replace('\n', '\t');
            msg = msg + "\n";
            SendMsgToClient(msg);
            richTextBox1.Text.Replace('\t', '\n');
        }

        private void button9_Click(object sender, EventArgs e)
        {
            string msg = "TEXT2=" + richTextBox2.Text.Replace('\n', '\t');
            msg = msg + "\n";
            SendMsgToClient(msg);
        }
        //api url
        private void button5_Click(object sender, EventArgs e)
        {

        }
        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        //使用時間到期管理 
        //訪客
        /*
          textBox10.Text = (string)obj["AccessTime"];
                       textBox11.Text=(string)obj["WarnTimeDownCount1"];
                       textBox1.Text = (string)obj["MSG1"];
                       */

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox10.Text != "") // 訪客可使用的分鐘數 string
            {
                string msg = "AccessTime1=" + textBox10.Text;
                msg = msg + "\n";
                SendMsgToClient(msg);
            }
            else
            {
                MessageBox.Show("訪客可使用的分鐘數不可為空");
            }

            if (textBox11.Text != "")//倒數時間
            {

                string msg = "WarnTimeDownCount1=" + textBox11;
                msg = msg + "\n";
                SendMsgToClient(msg);


            }
            else MessageBox.Show("請輸入倒數時間");

            if (textBox1.Text != "") //警語
            {

                string msg = "MSG1=" + textBox1;
                msg = msg + "\n";
                SendMsgToClient(msg);


            }
            else MessageBox.Show("請輸入倒數時間");
        }

        //預約者
        private void button10_Click(object sender, EventArgs e)
        {
            /*string msg = "TEXT3=" + textBox8.Text + "=" + textBox9.Text;
            msg = msg + "\n";
            SendMsgToClient(msg);*/
            if (textBox12.Text != "") // 預約者可使用的分鐘數 string
            {                               
                string msg = "AccessTime2=" + textBox12.Text;
                msg = msg + "\n";
                SendMsgToClient(msg);
            }
            else
            {
                MessageBox.Show("訪客可使用的分鐘數不可為空");
            }

            if (textBox9.Text != "")//倒數時間
            {

                string msg = "WarnTimeDownCount2=" + textBox9;
                msg = msg + "\n";
                SendMsgToClient(msg);


            }
            else MessageBox.Show("請輸入倒數時間");

            if (textBox8.Text != "") //警語
            {

                string msg = "MSG2=" + textBox8;
                msg = msg + "\n";
                SendMsgToClient(msg);


            }
            else MessageBox.Show("請輸入倒數時間");


        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
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
}
