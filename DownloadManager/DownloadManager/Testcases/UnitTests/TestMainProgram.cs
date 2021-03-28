using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DownloadManager;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DownloadManager
{
    [TestClass]
    public class TestMainProgram:Program
    {
        [TestMethod]
        public void Invalid_FilePath()
        {
            TestMainProgram obj = new TestMainProgram();

            string filepath = "";

           // bool res = obj.verifiyFilePath(filepath);

            Assert.AreEqual(obj.verifiyFilePath(filepath), false);
        }

        [TestMethod]
        public void Valid_FilePath()
        {
            TestMainProgram obj = new TestMainProgram();

            string filepath = @"C:\Users\dmestry\Documents\Curl\TESTING\URLLIST.txt";

            //bool res = obj.verifiyFilePath(filepath);

           
            Assert.AreEqual(obj.verifiyFilePath(filepath), true);
        }


        [TestMethod]
        public void Empty_downlaod_path()
        {
            TestMainProgram obj = new TestMainProgram();

            string filepath = "";

            //bool res = obj.verifyDownloadPath(filepath);

            Assert.AreEqual(obj.verifyDownloadPath(filepath), true);
        }

        [TestMethod]
        public void Valid_downlaod_path()
        {
            TestMainProgram obj = new TestMainProgram();

            string filepath = @"C:\Users\dmestry\Documents\Curl\TESTING";

            //bool res = obj.verifyDownloadPath(filepath);

            Assert.AreEqual(obj.verifyDownloadPath(filepath), true);
        }

        [TestMethod]
        public void get_urls_from_nonempty_file()
        {
            TestMainProgram obj = new TestMainProgram();

            string filepath = @"C:\Users\dmestry\Documents\Curl\TESTING\URLLIST.txt";

            List<string> urls = obj.getUrlsFromFile(filepath);
            bool res = false;
            //if(urls.Count>0)
            //    Assert.AreEqual(true, true);
            //else
            //    Assert.AreEqual(false, false);
            if (urls.Count > 0)
                res = true;

            Assert.AreEqual(res, true);

        }

        [TestMethod]
        public void get_urls_from_empty_file()
        {
            TestMainProgram obj = new TestMainProgram();

            string filepath = @"C:\Users\dmestry\Documents\Curl\TESTING\URLLISTEMPTY.txt";

            List<string> urls = obj.getUrlsFromFile(filepath);

            bool res = false;
           

            if (urls.Count > 0)
                res = true;

            Assert.AreEqual(res, false);

        }


        [TestMethod]
        public void test_cleanup_method()
        {
            TestMainProgram obj = new TestMainProgram();
            string downlaodpath = @"C:\Users\dmestry\Documents\Curl\TESTING\";

            obj.cleanup(downlaodpath);

            
            DirectoryInfo di = new DirectoryInfo(downlaodpath);
            List<FileInfo> partialFilesToDelete = new List<FileInfo>();
            partialFilesToDelete = di.GetFiles("*.part*").ToList();
            bool res = false;
            if (partialFilesToDelete.Count == 0)
                res = true;

             Assert.AreEqual(res, true);
            
        }

        [TestMethod]
        public void valid_file_ext()
        {
            TestMainProgram obj = new TestMainProgram();
            string filepath = @"C:\Users\dmestry\Documents\Curl\TESTING\URLLIST.txt";

            //if(obj.veryfyFileExt(filepath))
            //    Assert.AreEqual(true, true);
            //else
            //    Assert.AreEqual(false, true);

            Assert.AreEqual(obj.veryfyFileExt(filepath), true);
        }
    }


   


}
