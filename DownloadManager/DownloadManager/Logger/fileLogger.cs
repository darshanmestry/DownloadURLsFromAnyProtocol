using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownloadManager
{
    class fileLogger
    {
        private static fileLogger instance = null;
        public static string sLogFilePath;
        string sLogFilename = string.Empty;
        protected fileLogger()
        {
            // path for the log file is the path from which it is run 
            sLogFilePath = ConfigurationManager.AppSettings["LOG_FILE_PATH"];

            if(!Directory.Exists(sLogFilePath))
            {
                try
                {
                    Directory.CreateDirectory(sLogFilePath);
                }
               catch(Exception e)
                {
                    Console.WriteLine("Error:" + e.Message);
                }
            }
            //string temp = System.Reflection.Assembly.GetEntryAssembly().ToString();
            //sLogFilename = sLogFilePath + "\\" + temp.Substring(0, Math.Max(temp.IndexOf(','), 0)).ToUpper() + "_" + DateTime.Now.ToString("dd_MMM_yyyy_HH_mm_ss") + ".log";
            
        }

        public static fileLogger Instance()
        {
            if (instance == null)
            {
                instance = new fileLogger();
            }
            return instance;
        }

        public void WriteToLog(string information,string threadID)
        {
            if(threadID=="-1")
            {
                threadID = "Parent_Thread";
            }
            //string temp = System.Reflection.Assembly.GetEntryAssembly().ToString();
            string temp = "DownloadManager";
            // sLogFilename = sLogFilePath + "\\" +threadID.ToString()+"_"+ temp.Substring(0, Math.Max(temp.IndexOf(','), 0)).ToUpper() + "_" + DateTime.Now.ToString("dd_MMM_yyyy") + ".log";
            sLogFilename = sLogFilePath + "\\" + threadID.ToString() + "_" + temp + "_" + DateTime.Now.ToString("dd_MMM_yyyy") + ".log";

            DateTime dtNow = DateTime.Now;
            StreamWriter myStream = null;
           

            using (myStream = new StreamWriter(sLogFilename, true))
            {
                myStream.WriteLine();
                myStream.WriteLine("{0} >> {1}", DateTime.Now, information);
            }
        }
    }
}
