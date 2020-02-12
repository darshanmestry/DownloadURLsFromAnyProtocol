using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownloadManager
{
    class sftpdownload:download
    {
        public sftpdownload(string url,string downloadpath,int url_no,string threadID)
        {
            try
            {
                this.url = url;
                this.downloadPath = downloadpath;
                bool isUrlValid = verifyurl(url);
               

                if (isUrlValid)
                {

                    string commandArguments = "-I -k -L " + url;
                    fileLogger.Instance().WriteToLog("   Url_no:" + url_no + "   " + url, threadID);
                    fileLogger.Instance().WriteToLog("      - Getting length of file for " + url, threadID);
                    double headerContentLength=fileLength(commandArguments, threadID);
                    username= ConfigurationManager.AppSettings["USER_SFTP"];
                    password= ConfigurationManager.AppSettings["PASSWORD_SFTP"];

                    string filename = new Uri(url).AbsolutePath.Split('/').Last();

                    if (/*headerContentLength > 0 &&*/ headerContentLength < getDriveSize(downloadPath, threadID))
                    {

                        if (headerContentLength >= 1073741824) //File >1GB will be downloaded in chunk
                        {
                            fileLogger.Instance().WriteToLog("      - File Will be downloaded in chunks " + url, threadID);
                            if (downloadFile_inChunks(filename, 5, threadID, securedAuth))
                                fileLogger.Instance().WriteToLog("      - Download compelete.", threadID);
                            else
                                fileLogger.Instance().WriteToLog("      - Download failed.", threadID);
                        }
                        else
                        {


                            fileLogger.Instance().WriteToLog("      - File Will be downloaded in one go " + url, threadID);
                            if (downloadFile_inChunks(filename, 1, threadID, securedAuth))
                                fileLogger.Instance().WriteToLog("      - Download compelete", threadID);
                            else
                                fileLogger.Instance().WriteToLog("      - Download failed", threadID);
                        }

                    }
                    else
                    {
                        fileLogger.Instance().WriteToLog("      - Skipping " + url + " as file is empty or no drive space", threadID);
                    }

                }
                else
                {

                    fileLogger.Instance().WriteToLog("   Url_no:" + url_no + "   " + url, threadID);
                    fileLogger.Instance().WriteToLog("      - Skipping  " + url + " as it is invalid of does not have file.", threadID);
                }
            }
            catch (Exception e)
            {
                fileLogger.Instance().WriteToLog("   ERROR:" + e.Message, threadID);
            }  
        }
    }
}
