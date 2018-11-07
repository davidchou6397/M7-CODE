using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;

namespace ServerForm
{
    public partial class Form1 : Form
    {
        ServerClass serverResponse = new ServerClass();
        private Socket server;
        private byte[] data = new byte[1024];
        private int size = 1024;
        private System.Collections.ArrayList m_workerSocketList =
                ArrayList.Synchronized(new System.Collections.ArrayList());
        private int m_clientCount = 0;
        public AsyncCallback pfnWorkerCallBack;
        public Form1()
        {
            try
            {
                InitializeComponent();
                this.WindowState = FormWindowState.Maximized;
                server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint iep = new IPEndPoint(IPAddress.Any, 9050);
                server.Bind(iep);
                server.Listen(5);
                server.BeginAccept(new AsyncCallback(AcceptConn), null);

            }
            catch (SocketException se)
            {
                Console.WriteLine(se);
            }
        }

        private void AcceptConn(IAsyncResult iar)
        {
            try
            {
                Socket workerSocket = server.EndAccept(iar);
                Interlocked.Increment(ref m_clientCount);
                m_workerSocketList.Add(workerSocket);
                string msg = "Connected to my server \n";
                SendMsgToClient(msg, m_clientCount);
                WaitForData(workerSocket, m_clientCount);
                server.BeginAccept(new AsyncCallback(OnClientConnect), null);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void OnClientConnect(IAsyncResult asyn)
        {
            try
            {
                // Here we complete/end the BeginAccept() asynchronous call
                // by calling EndAccept() - which returns the reference to
                // a new Socket object
                Socket workerSocket = server.EndAccept(asyn);

                // Now increment the client count for this client 
                // in a thread safe manner
                Interlocked.Increment(ref m_clientCount);

                // Add the workerSocket reference to our ArrayList
                m_workerSocketList.Add(workerSocket);

                // Send a welcome message to client
                string msg = "Welcome client " + m_clientCount + "\n";
                SendMsgToClient(msg, m_clientCount);



                // Let the worker Socket do the further processing for the 
                // just connected client
                WaitForData(workerSocket, m_clientCount);

                // Since the main Socket is now free, it can go back and wait for
                // other clients who are attempting to connect
                server.BeginAccept(new AsyncCallback(OnClientConnect), null);
            }
            catch (ObjectDisposedException)
            {
                System.Diagnostics.Debugger.Log(0, "1", "\n OnClientConnection: Socket has been closed\n");
            }
            catch (SocketException se)
            {
                Console.WriteLine(se.Message);
            }

        }

        public class SocketPacket
        {
            // Constructor which takes a Socket and a client number
            public SocketPacket(System.Net.Sockets.Socket socket, int clientNumber)
            {
                m_currentSocket = socket;
                m_clientNumber = clientNumber;
            }
            public System.Net.Sockets.Socket m_currentSocket;
            public int m_clientNumber;
            // Buffer to store the data sent by the client
            public byte[] dataBuffer = new byte[1024];
        }
        Form2 form2;
        public void OnDataReceived(IAsyncResult asyn)
        {
            SocketPacket socketData = (SocketPacket)asyn.AsyncState;
            try
            {
                // Complete the BeginReceive() asynchronous call by EndReceive() method
                // which will return the number of characters written to the stream 
                // by the client
                int iRx = socketData.m_currentSocket.EndReceive(asyn);
                char[] chars = new char[iRx + 1];
                // Extract the characters as a buffer
                System.Text.Decoder d = System.Text.Encoding.UTF8.GetDecoder();
                int charLen = d.GetChars(socketData.dataBuffer,
                    0, iRx, chars, 0);

                System.String szData = new System.String(chars);
                string msg = "" + socketData.m_clientNumber + "=";
                Console.WriteLine(msg + szData);

                // Send back the reply to the client
                string replyMsg = "Server Reply=" + szData.ToUpper();
                // Convert the reply to byte array
                byte[] byData = System.Text.Encoding.ASCII.GetBytes(replyMsg);

                Socket workerSocket = (Socket)socketData.m_currentSocket;
                workerSocket.Send(byData);

                
                // Continue the waiting for data on the Socket
                WaitForData(socketData.m_currentSocket, socketData.m_clientNumber);

            }
            catch (ObjectDisposedException)
            {
                System.Diagnostics.Debugger.Log(0, "1", "\nOnDataReceived: Socket has been closed\n");
            }
            catch (SocketException se)
            {
                if (se.ErrorCode == 10054) // Error code for Connection reset by peer
                {
                    string msg = "Client " + socketData.m_clientNumber + " Disconnected" + "\n";
                    Console.WriteLine(msg);

                    // Remove the reference to the worker socket of the closed client
                    // so that this object will get garbage collected
                    m_workerSocketList[socketData.m_clientNumber - 1] = null;
                }
                else
                {
                    Console.WriteLine(se.Message);
                }
            }
        }

        public void WaitForData(System.Net.Sockets.Socket soc, int clientNumber)
        {
            try
            {
                if (pfnWorkerCallBack == null)
                {
                    // Specify the call back function which is to be 
                    // invoked when there is any write activity by the 
                    // connected client
                    pfnWorkerCallBack = new AsyncCallback(OnDataReceived);
                }
                SocketPacket theSocPkt = new SocketPacket(soc, clientNumber);

                soc.BeginReceive(theSocPkt.dataBuffer, 0,
                    theSocPkt.dataBuffer.Length,
                    SocketFlags.None,
                    pfnWorkerCallBack,
                    theSocPkt);
            }
            catch (SocketException se)
            {
                Console.WriteLine(se.Message);
            }
        }

        void SendMsgToClient(string msg, int clientNumber)
        {
            // Convert the reply to byte array
            byte[] byData = System.Text.Encoding.ASCII.GetBytes(msg);

            Socket workerSocket = (Socket)m_workerSocketList[clientNumber - 1];
            workerSocket.Send(byData);
        }


        private void Form1_Load(object sender, EventArgs e)
        {

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

        private void buttonLogin_Click(object sender, EventArgs e)
        {
            /*if (ValidateLogin())
			{*/
            /*if (CheckUserPwd())
            {
                form2 = new Form2(m_workerSocketList, m_clientCount);
                form2.Show();

                this.Hide();
            }
            else
            {
                MessageBox.Show("帳號密碼錯誤");
            }
            */
            form2 = new Form2(m_workerSocketList, m_clientCount);
            form2.Show();

            this.Hide();

            /*}*/

        }
        // 當client 連入後
        private void timer1_Tick(object sender, EventArgs e)
        {
      /*       string msg = "USBState=" + form2.USBState.ToString();
             msg = msg + "\n";
             SendMsgToClient(msg);
            Thread.Sleep(200);

             msg = "";
             msg = "AccessTime=" + form2.textBox1.Text;
             msg = msg + "\n";
             SendMsgToClient(msg);
            Thread.Sleep(200);

            msg = "";
             msg = "URL1=" + form2.textBox2.Text;
             msg = msg + "\n";
             SendMsgToClient(msg);
            Thread.Sleep(200);

            msg = "";
             msg = "URL2=" + form2.textBox3.Text;
             msg = msg + "\n";
             SendMsgToClient(msg);
            Thread.Sleep(200);

            msg = "";
             msg = "URL3=" + form2.textBox6.Text;
             msg = msg + "\n";
             SendMsgToClient(msg);
            Thread.Sleep(200);
            timer1.Enabled = false;*/
             
        }
    }
}
