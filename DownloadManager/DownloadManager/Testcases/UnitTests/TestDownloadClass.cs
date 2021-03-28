using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownloadManager
{
    [TestClass]
    public class TestDownloadClass : download
    {
        [TestMethod]
        public void verify_url_syntax_valid()
        {
            TestDownloadClass obj = new TestDownloadClass();
            obj.url = "ftp://someinvalidpath.net/5MB.zip";

            //if(obj.verifyurl(obj.url))
            //    Assert.AreEqual(true, true);
            //else
            //    Assert.AreEqual(false, true);

            Assert.AreEqual(obj.verifyurl(obj.url), true);

        }

        [TestMethod]
        public void verify_url_syntax_Invalid()
        {
            TestDownloadClass obj = new TestDownloadClass();
            obj.url = "ThisISSurelyNOTURL";

            //if (!obj.verifyurl(obj.url))
            //    Assert.AreEqual(false, false);
            //else
            //    Assert.AreEqual(true, false);

            Assert.AreEqual(obj.verifyurl(obj.url), false);
        }


        [TestMethod]
        public void Invalid_URLPath_Length()
        {
            TestDownloadClass obj = new TestDownloadClass();


            bool res;
            string url = "ftp://someinvalidpath.net/5MB.zip";
            string commandArguments = "-I -k -L " + url;
            double headerContentLength = int.MinValue;
            headerContentLength = obj.fileLength(commandArguments, "INVALID_TESTCASE_FILE_LEN");
            if (headerContentLength == 0)
                res = true;
            else
                res = false;

            Assert.AreEqual(res, true);
        }

        [TestMethod]
        public void Valid_URLPath_URL_Length()
        {
            TestDownloadClass obj = new TestDownloadClass();

            bool res;
            string url = "ftp://speedtest.tele2.net/5MB.zip";
            string commandArguments = "-I -k -L " + url;
            double headerContentLength = int.MinValue;
            headerContentLength = obj.fileLength(commandArguments, "VALID_TESTCASE_FILE_LEN");
            if (headerContentLength != int.MinValue)
                res = true;
            else
                res = false;

            Assert.AreEqual(res, true);
        }

        [TestMethod]
        public void execute_proccess_valid_program_name_and_args()
        {
            TestDownloadClass obj = new TestDownloadClass();
            obj.contentLength = 1;
            string prg_name = "cmd.exe";
            string args = "/C del somefile.txt"; ;
            
            // if (obj.executeProcess(prg_name, args, "veryify_execute_proccess_valid_program_name"))
            //     res = true;

            //Assert.AreEqual(res, true);

            Assert.AreEqual(obj.executeProcess(prg_name, args, "veryify_execute_proccess_valid_program_name"), true);
        }


        [TestMethod]
        public void execute_proccess_Invalid_program_name_and_args()
        {
            TestDownloadClass obj = new TestDownloadClass();

            string prg_name = "invalid.exe";
            string args = "This is invalid args";
            //bool res = false;
            //if (obj.executeProcess(prg_name, args, "veryify_execute_proccess_valid_program_name"))
            //{
            //    res = true;
            //}

            //Assert.AreEqual(res, false

            Assert.AreEqual(obj.executeProcess(prg_name, args, "veryify_execute_proccess_valid_program_name"), false);
        }


        [TestMethod]
        public void start_download_with_correct_command_Args()
        {
            TestDownloadClass obj = new TestDownloadClass();
            string downloadPath = "";
            int prev_chunk_size = 0;
            string filename="5mb.zip";
            int i = 0;
            obj.url = "ftp://speedtest.tele2.net/5MB.zip";
            string commandArguments = "-k -L --range " + prev_chunk_size + "-" + " " + url + " --output " + Path.Combine(downloadPath, filename.Split('.')[0] + ".part" + i);

            //if(obj.startDownload(commandArguments, "veryify_execute_proccess_Invalid_program_name_and_args"))
            //    Assert.AreEqual(true, true);
            //else
            //    Assert.AreEqual(false, true);

            Assert.AreEqual(obj.startDownload(commandArguments, "veryify_execute_proccess_Invalid_program_name_and_args"), true);
        }

        [TestMethod]
        public void execute_cmd_with_valid_args()
        {
            TestDownloadClass obj = new TestDownloadClass();
            string commandArguments = "/C del somefile.txt";
            obj.contentLength = 1111;

            //if (obj.executeCMD(commandArguments, "verify_execute_cmd_with_valid_args"))
            //    Assert.AreEqual(true, true);
            //else
            //    Assert.AreEqual(false, true);

            Assert.AreEqual(obj.executeCMD(commandArguments, "verify_execute_cmd_with_valid_args"), true);
        }

        [TestMethod]
        public void valid_drive_size()
        {
            string drive = "C:";
            TestDownloadClass obj = new TestDownloadClass();
            double size = double.MinValue;
            size= obj.getDriveSize(drive, "verify_valid_drive_size");
            bool res = false;
            if (size != double.MinValue)
                res = true;
          
             Assert.AreEqual(res, true);


        }


        [TestMethod]
        public void invalid_drive_size()
        {
            string drive = "E";
            TestDownloadClass obj = new TestDownloadClass();
            double size = double.MinValue;
            size = obj.getDriveSize(drive, "verify_valid_drive_size");

            bool res = false;
            if (size == 0)
                res = true;
          
            Assert.AreEqual(res, true);


        }

        [TestMethod]
        public void download_HTTP_file_in_chunks_valid_args()
        {
            TestDownloadClass obj = new TestDownloadClass();
            //int chunkLength = 1048576;
            obj.url = "http://speedtest.tele2.net/1MB.zip";
            obj.downloadPath = @"C:\Users\dmestry\Documents\Curl\TESTING";

            //if (obj.downloadFile_inChunks("1MB.zip",1, "verify_download_file_in_chunks_valid_args",anonymousAuth))
            //    Assert.AreEqual(true, true);
            //else
            //    Assert.AreEqual(false, true);

            Assert.AreEqual(obj.downloadFile_inChunks("1MB.zip", 1, "verify_download_file_in_chunks_valid_args", anonymousAuth), true);
        }


        [TestMethod]
        public void download_HTTPS_file_in_chunks_valid_args()
        {
            TestDownloadClass obj = new TestDownloadClass();
            //int chunkLength = 1048576;
            obj.url = "https://www.dropbox.com/s/jf4wr6m075sevy5/curl_747_1_ssl.zip?dl=0";
            obj.downloadPath = @"C:\Users\dmestry\Documents\Curl\TESTING";

            //if (obj.downloadFile_inChunks("1MB.zip",1, "verify_download_file_in_chunks_valid_args",anonymousAuth))
            //    Assert.AreEqual(true, true);
            //else
            //    Assert.AreEqual(false, true);

            Assert.AreEqual(obj.downloadFile_inChunks("curl_747_1_ssl.zip", 1, "verify_download_file_in_chunks_valid_args", anonymousAuth), true);
        }


        [TestMethod]
        public void download_FTP_file_in_chunks_valid_args()
        {
            TestDownloadClass obj = new TestDownloadClass();
            //int chunkLength = 1048576;
            obj.url = "ftp://speedtest.tele2.net/5MB.zip";
            obj.downloadPath = @"C:\Users\dmestry\Documents\Curl\TESTING";

            //if (obj.downloadFile_inChunks("1MB.zip",1, "verify_download_file_in_chunks_valid_args",anonymousAuth))
            //    Assert.AreEqual(true, true);
            //else
            //    Assert.AreEqual(false, true);

            Assert.AreEqual(obj.downloadFile_inChunks("5MB.zip", 1, "verify_download_file_in_chunks_valid_args", anonymousAuth), true);
        }

        [TestMethod]
        public void download_SFTP_file_in_chunks_valid_args()
        {
            TestDownloadClass obj = new TestDownloadClass();
            //int chunkLength = 1048576;
            obj.url = "sftp://192.168.29.221/testfile.txt";
            obj.downloadPath = @"C:\Users\dmestry\Documents\Curl\TESTING";

            username = ConfigurationManager.AppSettings["USER_SFTP"];
            password = ConfigurationManager.AppSettings["PASSWORD_SFTP"];

            //if (obj.downloadFile_inChunks("1MB.zip",1, "verify_download_file_in_chunks_valid_args",anonymousAuth))
            //    Assert.AreEqual(true, true);
            //else
            //    Assert.AreEqual(false, true);

            Assert.AreEqual(obj.downloadFile_inChunks("testfile.txt", 1, "verify_download_file_in_chunks_valid_args", securedAuth), true);
        }


    }


}
