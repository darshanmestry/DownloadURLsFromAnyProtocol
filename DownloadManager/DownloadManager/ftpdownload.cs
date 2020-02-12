using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownloadManager
{
    class ftpdownload : download
    {
        public ftpdownload(string url, string downloadPath, int url_no, string threadID)
        {
            try
            {
                this.url = url;
                this.downloadPath = downloadPath;
                bool isUrlValid = verifyurl(url);
                

                if (isUrlValid)
                {

                    string commandArguments = "-I -k -L " + url;

                    fileLogger.Instance().WriteToLog("   Url_no:" + url_no + "   " + url, threadID);
                    fileLogger.Instance().WriteToLog("      - Getting length of file for " + url, threadID);
                    double headerContentLength = fileLength(commandArguments, threadID);


                    string filename = new Uri(url).AbsolutePath.Split('/').Last();

                    if (headerContentLength > 0 && headerContentLength <= getDriveSize(downloadPath, threadID))
                    {


                        if (headerContentLength >= 1073741824) //File >1GB will be downloaded in chunk
                        {

                            fileLogger.Instance().WriteToLog("      - File Will be downloaded in chunks " + url, threadID);
                            if (downloadFile_inChunks(filename, 5, threadID, anonymousAuth))
                                fileLogger.Instance().WriteToLog("      - Download compelete.", threadID);
                            else
                                fileLogger.Instance().WriteToLog("      - Download failed.", threadID);
                        }
                        else
                        {
                            fileLogger.Instance().WriteToLog("      - File Will be downloaded in one go " + url, threadID);
                            if (downloadFile_inChunks(filename, 1, threadID, anonymousAuth))
                                fileLogger.Instance().WriteToLog("      - Download compelete", threadID);
                            else
                                fileLogger.Instance().WriteToLog("      - Download failed", threadID);
                        }

                    }
                    else
                    {
                        fileLogger.Instance().WriteToLog("      - Skikkping " + url + " as file is empty or no drive space", threadID);
                    }
                }
                else
                {

                    fileLogger.Instance().WriteToLog("   Url_no:" + url_no + "   " + url, threadID);
                    fileLogger.Instance().WriteToLog("      - Skilling  " + url + " as it is invalid of does not have file.", threadID);
                }
            }
            catch(Exception e)
            {
                fileLogger.Instance().WriteToLog("   ERROR:" + e.Message, threadID);
            }  
        }
    }
}
