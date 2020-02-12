using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DownloadManager
{
    public abstract class download
    {
        protected string url;
        protected string downloadPath;
        protected double contentLength=0;
        protected string securedAuth = "securedAuth";
        protected string anonymousAuth = "anonymousAuth";
        protected string username = string.Empty;
        protected string password = string.Empty;
        /**
         * Checks Syntax of URL and veryfify if url has file to Download
         * **/
        protected bool verifyurl(string url)
        {
            try
            {
                bool isUri = Uri.IsWellFormedUriString(url, UriKind.RelativeOrAbsolute);
                if (isUri)
                {
                    if (new Uri(url).AbsolutePath.Split('/').Last().Contains('.'))
                        return true;
                    else
                        return false;
                }
                else
                    return false;
            }
            catch(Exception e)
            {
                return false;
            }
           

        }

        /** 
         * Generic function to execute program in command prompt
         *  Input :2 Arguments 
         *  1st argument is program or executable to run
         *  2nd agument is command line agrs to be passed to executable
         * **/
        protected bool executeProcess(string programName,string commandArguments,string threadID)
        {
           
            

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = false;
            startInfo.FileName = programName;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.Arguments = commandArguments;
           
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
           

            try
            {
                // Start the process with the info we specified.
                // Call WaitForExit and then the using statement will close.
                using (Process exeProcess = Process.Start(startInfo))
                {
                 
                    if (contentLength == 0)
                    {
                        string oo =exeProcess.StandardOutput.ReadToEnd();
                        foreach (string line in oo.Split('\n'))
                        {
                            if (line.ToUpper().Contains("CONTENT-LENGTH:"))
                            {
                                contentLength = double.Parse(line.Split(':')[1].Trim());
                                break;
                            }
                        }
                        
                    }
                    string err = exeProcess.StandardError.ReadToEnd();
                   
                    exeProcess.WaitForExit();
                }
                return true;  
            }
            catch(InvalidOperationException e)
            {
                fileLogger.Instance().WriteToLog("   InvalidOperationException for URL"+url+"  ERROR:"+e.Message, threadID);
                return false;
            }
            catch(FileNotFoundException e)
            {
                fileLogger.Instance().WriteToLog("   FileNotFoundException for URL" + url + "  ERROR:" + e.Message, threadID);
                return false;
            }
            catch(IOException e)
            {
                fileLogger.Instance().WriteToLog("   IOException for URL" + url + "  ERROR:" + e.Message, threadID);
                return false;
            }
            catch (Exception e)
            {
                fileLogger.Instance().WriteToLog("   Exception for URL" + url + "  ERROR:" + e.Message, threadID);
                return false;
            }
        } 


        /**
         * Generic function which will be used by all classes (httpdownload,ftpdownload,sftpdownload)
         * to download files
         * **/
        protected bool startDownload(string commandArguments,string threadID)
        {
            try
            {
                string ExecutableLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                string CurlLocation = Path.Combine(ExecutableLocation, "curl.exe");
                if (File.Exists(CurlLocation))
                {
                    if (executeProcess(CurlLocation, commandArguments, threadID))
                        return true;
                    else
                        return false;
                }
                else
                {
                    fileLogger.Instance().WriteToLog("   Curl.exe not found...", threadID);
                    return false;
                }

            }
            catch(Exception e)
            {
                fileLogger.Instance().WriteToLog("   ERROR:" + e.Message,threadID);
                return false;
            }
        }


        /**
         * Generic function which get header of URL and finds length of file which is to be downloaded
         * **/
        protected double fileLength(string commandAruments,string threadID)
        {
            double headerContentLength =0;
            try
            {
               
                string ExecutableLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                string CurlLocation = Path.Combine(ExecutableLocation, "curl.exe");
                if (File.Exists(CurlLocation))
                {
                    executeProcess(CurlLocation, commandAruments, threadID);
                    headerContentLength = contentLength;
                    contentLength = 0;
                }
                else
                {
                    fileLogger.Instance().WriteToLog("   Curl.exe not found...", threadID);
                }
                //contentLength = int.MinValue;
                
            }
            catch(Exception e)
            {
                fileLogger.Instance().WriteToLog("   ERROR:" + e.Message, threadID);
            }
            return headerContentLength;
        }

      
        /**
         * Function to copy multiple chunks of files in to one file if file is downloaded chunk by chunk
         * Also deleted files partial and chunk files                 
         * **/
        protected bool executeCMD(string commandAruments,string threadID)
        {
            try
            {
                if (executeProcess("cmd.exe", commandAruments, threadID))
                    return true;
                else
                    return false;
            }
            catch(Exception e)
            {
                fileLogger.Instance().WriteToLog("   ERROR:" + e.Message, threadID);
                return false;
            }
        }


        protected double getDriveSize(string filepath,string threadID)
        {

            string DriveLetter = filepath.Split(':')[0];
            double freesize = 0;
            try
            {
                if (!string.IsNullOrEmpty(DriveLetter))
                {
                    DriveInfo Drive = new DriveInfo(DriveLetter);

                    if (Drive != null && Drive.IsReady)
                    {
                        freesize = Drive.AvailableFreeSpace;
                    }
                }

            }
            catch(Exception e)
            {
                fileLogger.Instance().WriteToLog("          > Error in getting drive info ", threadID);
            }
            return freesize;
        }


        /**
         * Function to download large file in multiple chunks and later combine it into one file.
         * It takes 2 arguments
         * 1st argument is filename to be downladed
         * 2nd argument is no of chunks in which file should be downloaded
         * **/
        protected bool downloadFile_inChunks(string filename, int chunkLength, string threadID,string authType)
        {
            try
            {

                string commandArguments = string.Empty;
                //int chunkLength = 5;
                double prev_chunk_size = 0;
                double current_chunk_size = contentLength / chunkLength;
                int i = 1;
                bool result = false;
                while (i < chunkLength)
                {
                    //Console.WriteLine("        Chunk " + i + " started");
                    double temp = current_chunk_size + prev_chunk_size;

                    if(authType==anonymousAuth)
                        commandArguments = "-k -L --range " + prev_chunk_size + "-" + temp + " " + url + " --output " + Path.Combine(downloadPath, filename.Split('.')[0] + ".part" + i);
                    else
                        commandArguments = "-k -L --range " + prev_chunk_size + "-" + " " + url + " --user " + username + ":" + password + " --output " + Path.Combine(downloadPath, filename.Split('.')[0] + ".part" + i);

                    result = startDownload(commandArguments, threadID);

                    prev_chunk_size = temp;

                    prev_chunk_size++;

                    //Console.WriteLine("        Chunk " + i + " complete");
                    if(result)
                        fileLogger.Instance().WriteToLog("          > Chunk " + i + " complete", threadID);
                    else
                        fileLogger.Instance().WriteToLog("          > Chunk " + i + " falied to downlaod", threadID);
                    i++;

                }
                if(authType==anonymousAuth)
                    commandArguments = "-k -L --range " + prev_chunk_size + "-" + " " + url + " --output " + Path.Combine(downloadPath, filename.Split('.')[0] + ".part" + i);
                else
                    commandArguments = "-k -L --range " + prev_chunk_size + "-" + " " + url + " --user "+username+":"+password+" --output " + Path.Combine(downloadPath, filename.Split('.')[0] + ".part" + i);

                result =startDownload(commandArguments, threadID);
               
                if(result)
                    fileLogger.Instance().WriteToLog("          > Chunk " + i + " complete", threadID);
                else
                    fileLogger.Instance().WriteToLog("          > Chunk " + i + " failed to download", threadID);

                string source = Path.Combine(downloadPath, filename.Split('.')[0].Trim() + ".part?");
                string dest = Path.Combine(Path.Combine(downloadPath, filename));

                commandArguments = "/C COPY /B " + source + " " + dest;

                result=executeCMD(commandArguments, threadID);
                if(result)
                    fileLogger.Instance().WriteToLog("          > Merged all chunks in to one file complete", threadID);
                else
                    fileLogger.Instance().WriteToLog("          > failed to merge Chunks into one file complete", threadID);


                commandArguments = "/C del " + source;
                result=executeCMD(commandArguments, threadID);
                if(result)
                    fileLogger.Instance().WriteToLog("          > deleting chunks successful", threadID);
                else
                    fileLogger.Instance().WriteToLog("          > deleting chunks failed ", threadID);

                return true;
            }
            catch(Exception e)
            {
                fileLogger.Instance().WriteToLog("   ERROR:" + e.Message, threadID);
                return false;
            }
       }
       
    }

  
}
