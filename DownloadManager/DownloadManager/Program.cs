using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DownloadManager
{
    public class Program
    {

        private static bool isclosing = false;
        private static string PARENT_THREAD = "PARENT_THREAD";
        private static bool ConsoleCtrlCheck(CtrlTypes ctrlType)

        {

            switch (ctrlType)
            {
                case CtrlTypes.CTRL_BREAK_EVENT:
                    Thread.Sleep(200);
                    isclosing = true;
                    commandWindowClosedClenUP();

                    Console.WriteLine("CTRL+BREAK received!");

                    break;

                case CtrlTypes.CTRL_CLOSE_EVENT:

                    Thread.Sleep(200);
                    isclosing = true;
                    commandWindowClosedClenUP();
                    break;

                case CtrlTypes.CTRL_LOGOFF_EVENT:

                case CtrlTypes.CTRL_SHUTDOWN_EVENT:

                    Thread.Sleep(200);
                    isclosing = true;
                    commandWindowClosedClenUP();
                     break;
            }

            return true;

        }
        private static string supportedFileExt = ".TXT";

        private static void commandWindowClosedClenUP()
        {
             Thread.Sleep(200);
            string DownloadPath = ConfigurationManager.AppSettings["DOWNLOAD_FILE_PATH"];
            DirectoryInfo di = new DirectoryInfo(DownloadPath);
            List<FileInfo> partialFilesToDelete = new List<FileInfo>();
            partialFilesToDelete = di.GetFiles("*.part*").ToList();

            foreach (FileInfo file in partialFilesToDelete)
            {
                try
                {
                    System.GC.Collect();
                    System.GC.WaitForPendingFinalizers();
                    //Process process = Process.GetProcessesByName("curl").First();
                    Process[] process = Process.GetProcessesByName("curl");
                    foreach (Process p in process)
                    {
                        if (!p.HasExited)
                        {
                            p.Kill();
                            fileLogger.Instance().WriteToLog("   Killing curl exe process", PARENT_THREAD);
                        }
                    }
                    File.Delete(file.FullName);
                    fileLogger.Instance().WriteToLog("   Delete Partial file:" + file.FullName, PARENT_THREAD);
                }
                catch (IOException ex)
                {
                    fileLogger.Instance().WriteToLog("   IOExeption in Deleting partial files:" + ex.Message, PARENT_THREAD);
                }
                catch (Exception genralException)
                {
                    fileLogger.Instance().WriteToLog("   Exeption in Deleting partial files:" + genralException.Message, PARENT_THREAD);
                }
                finally
                {
                    File.Delete(file.FullName);
                    fileLogger.Instance().WriteToLog("   Delete Partial file in Finally:" + file.FullName, PARENT_THREAD);
                }
            }
        }
        #region unmanaged

        // Declare the SetConsoleCtrlHandler function
        // as external and receiving a delegate.
        [DllImport("Kernel32")]
        public static extern bool SetConsoleCtrlHandler(HandlerRoutine Handler, bool Add);

        // A delegate type to be used as the handler routine
        // for SetConsoleCtrlHandler.

        public delegate bool HandlerRoutine(CtrlTypes CtrlType);

        // An enumerated type for the control messages
        // sent to the handler routine.

        public enum CtrlTypes

        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT,
            CTRL_CLOSE_EVENT,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT
        }
        #endregion


    

        public static void Main(string[] args)
        {
            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();
            SetConsoleCtrlHandler(new HandlerRoutine(ConsoleCtrlCheck), true);

            //if(!isclosing)
            //{
            Program obj = new Program();
            string filepath, DownloadPath = string.Empty;
            int threadCount = 1;
            if (args.Length == 0)
            {
                filepath = ConfigurationManager.AppSettings["URL_FILE_PATH"]; ;
                DownloadPath = ConfigurationManager.AppSettings["DOWNLOAD_FILE_PATH"];
                string temp = ConfigurationManager.AppSettings["THREAD_COUNT"];

                int.TryParse(temp, out threadCount);
                if (threadCount < 0 || threadCount > 10 || threadCount == 0)
                    threadCount = 1;

                if (obj.verifiyFilePath(filepath) && obj.veryfyFileExt(filepath) && obj.verifyDownloadPath(DownloadPath))
                {
                    fileLogger.Instance().WriteToLog("   ## Starting Run", PARENT_THREAD);
                    List<string> listOfurls = new List<string>();
                    listOfurls = obj.getUrlsFromFile(filepath);
                    if (listOfurls.Count > 0)
                    {
                        var cts = new CancellationTokenSource();
                        var options = new ParallelOptions()
                        {
                            MaxDegreeOfParallelism = threadCount
                        };

                        options.CancellationToken = cts.Token;
                        Parallel.ForEach(listOfurls, options, url =>
                        {
                            try
                            {

                                if (url.StartsWith("http"))
                                {
                                    download httpObj = new httpdownload(url, DownloadPath, listOfurls.IndexOf(url), "THREAD_ID_" + Thread.CurrentThread.ManagedThreadId.ToString() + "_URL_NO_" + listOfurls.IndexOf(url));
                                }
                                else if (url.StartsWith("ftp"))
                                {
                                    download ftpObj = new ftpdownload(url, DownloadPath, listOfurls.IndexOf(url), "THREAD_ID_" + Thread.CurrentThread.ManagedThreadId.ToString() + "_URL_NO_" + listOfurls.IndexOf(url));
                                }
                                else if (url.StartsWith("sftp"))
                                {
                                    download sftpObj = new sftpdownload(url, DownloadPath, listOfurls.IndexOf(url), "THREAD_ID_" + Thread.CurrentThread.ManagedThreadId.ToString() + "_URL_NO_" + listOfurls.IndexOf(url));
                                }
                                else
                                {
                                    fileLogger.Instance().WriteToLog("   Skipping URL:" + url + " as this program does not suppport it", Thread.CurrentThread.ManagedThreadId.ToString() + "_URL_NO_" + listOfurls.IndexOf(url));
                                }
                                options.CancellationToken.ThrowIfCancellationRequested();
                            }
                            catch (Exception e)
                            {
                                fileLogger.Instance().WriteToLog("   ERROR" + e.Message, PARENT_THREAD);
                            }
                        });
                        cts.Cancel();
                    }
                    else
                    {
                        Console.WriteLine("  No URLs to Process.");
                        fileLogger.Instance().WriteToLog("   No URLs to Process.", PARENT_THREAD);
                    }
                }
                else
                {
                    fileLogger.Instance().WriteToLog("   Invalid config..", PARENT_THREAD);
                }

            }
            else
            {
                Console.WriteLine("Invalid Syntax.");
                Console.WriteLine("Correct Syntax: DownloadManager.exe ");
                fileLogger.Instance().WriteToLog("   Invalid Syntax...      Correct Syntax: DownloadManager.exe ", PARENT_THREAD);

            }

            fileLogger.Instance().WriteToLog("   ## Ending Run", PARENT_THREAD);
            obj.cleanup(DownloadPath);

            watch.Stop();

            Console.WriteLine($"Execution Time: {watch.ElapsedMilliseconds} ms");
            Console.WriteLine("Done.");
        }


        /**  
         * This method is called before excution ends to clear all the semi downloaded files.
         * **/
        
        protected void cleanup(string DownloadPath)
        {
            //string DownloadPath = ConfigurationManager.AppSettings["DOWNLOAD_FILE_PATH"];
            if (!string.IsNullOrEmpty(DownloadPath))
            {
                DirectoryInfo di = new DirectoryInfo(DownloadPath);
                List<FileInfo> partialFilesToDelete = new List<FileInfo>();
                partialFilesToDelete = di.GetFiles("*.part*").ToList();

                foreach (FileInfo file in partialFilesToDelete)
                {
                    try
                    {
                        File.Delete(file.FullName);
                    }
                    catch (Exception e)
                    {
                        fileLogger.Instance().WriteToLog("   Deleting partial file " + file.FullName, PARENT_THREAD);
                    }

                }
            }
        }


        /**  
         * This method checks if file exists on given path.
         * **/
        protected bool verifiyFilePath(string filepath)
        {
            if (File.Exists(filepath))
                return true;
            else
                return false;
        }

        /**  
         * This method checks if download path is valid. If it is invalid it will create download path
         * **/
        protected bool verifyDownloadPath(string downloadPath) // For testing access is public
        {
            if (!string.IsNullOrEmpty(downloadPath) && !Directory.Exists(downloadPath))
            {
                //Console.WriteLine("Download folder not Present. Creating Download folder " + downloadPath);
                Directory.CreateDirectory(downloadPath);
            }
            else
            {
                //Console.WriteLine("Download folder present");
            }
            return true;
        }


        /**  
        * This method checks will read url file line by line and store url in list
        * **/
        protected List<string> getUrlsFromFile(string filepath) //For testing access is public
        {

            List<string> ListOfUrl = new List<string>();
            if (File.Exists(filepath))
            {
                string line;
                using (var fs = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var sr = new StreamReader(fs, Encoding.Default))
                {

                    while ((line = sr.ReadLine()) != null)
                    {
                        if (!string.IsNullOrEmpty(line))
                        {
                            ListOfUrl.Add(line.Trim());
                        }
                    }
                }
            }
            else
            {
                fileLogger.Instance().WriteToLog("   Invlid URL filename specified ", PARENT_THREAD);
            }

            return ListOfUrl;
        }


        /**  
        * This method checks will check extention of URL file
        * **/
        protected bool veryfyFileExt(string filepath)
        {
            if (File.Exists(filepath))
            {

                if (Path.GetExtension(filepath).ToUpper() == supportedFileExt)
                    return true;
                else
                    return false;
            }
            else
                return false;
        }
    }
}
