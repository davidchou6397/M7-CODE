using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;

namespace ClientForm
{
	static class Program
	{
		/// <summary>
		/// 應用程式的主要進入點。
		/// </summary>
		[STAThread]
		static void Main()
		{
			DisableTaskManager();
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new Form1());
		}
       
		private static void DisableTaskManager()
		{
			RegistryKey regkey = default(RegistryKey);
			string keyValueInt = "1";
			string subKey = "Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System";
			try
			{
				regkey = Registry.CurrentUser.CreateSubKey(subKey);
				regkey.SetValue("DisableTaskMgr", keyValueInt);
				regkey.Close();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}

		}
	}
}

namespace GlobalVariables
{
	public static class Globals
	{
		static Globals()
		{
			int AccessTime = 10;
		}
        public static bool Active = false;
        public static string ROOTPPassword { get; private set; }
        
        public static string URL1 { get; private set; }
        
        public static string URL2 { get; private set; }
        public static string URL3 { get; private set; }
        public static string URL4{ get; private set; }

        public static string TEXT1 { get; private set; }
        public static string TEXT2 { get; private set; }
     
        public static int AccessTime1 { get; private set; }
        public static int WarnTimeDowunCount1 { get; private set; }
        public static string Msg1 { get; private set; } //警語1 訪客使用者

        public static int AccessTime2 { get; private set; }
        public static int WarnTimeDowunCount2 { get; private set; }
        public static string Msg2 { get; private set; } //警語1 訪客使用者

        public static void SetAccessTime1(int newTime)
		{
			AccessTime1 = newTime;
		}
        /*
         * 
         * else if (szData1[i].StartsWith("AccessTime1"))
                    {
                        int AccessTime = Int32.Parse(szData1[i].Split('=')[1]);
                        Globals.SetAccessTime1(AccessTime);
                    }
                    else if (szData1[i].StartsWith("WarnTimeDownCount1"))
                    {
                        int AccessTime = Int32.Parse(szData1[i].Split('=')[1]);
                        Globals.SetWarnTimeDowunCount1(AccessTime);
                    }
                    else if (szData1[i].StartsWith("MSG1"))
                    {
                        int AccessTime = Int32.Parse(szData1[i].Split('=')[1]);
                        Globals.SetMsg1 (AccessTime);
                    }

    */

        public static void SetWarnTimeDowunCount1(int Time)
        {
            WarnTimeDowunCount1 = Time;
        }
        public static void SetMsg1(string s)
        {
            Msg1= s;
        }
        public static void SetAccessTime2(int newTime)
        {
            AccessTime2 = newTime;
        }
       
        public static void SetWarnTimeDowunCount2(int Time)
        {
            WarnTimeDowunCount2 = Time;
        }
        public static void SetMsg2(string s)
        {
            Msg2 = s;
        }

        public static void SetURL1(string s)
        {
            URL1=s;
        }
        public static void SetURL2(string s)
        {
            URL2 = s;

        }
        public static void SetURL3(string s)
        {
            URL3 = s;

        }
        public static void SetURL4(string s)
        {
            URL4 = s;

        }
        public static void SetText1(string s)
        {
            TEXT1 = s;

        }
        public static void SetText2(string s)
        {
            TEXT2 = s;

        }
      
        public static void SetROOT(string s)
        {
            ROOTPPassword = s;

        }
    }
}
