// EasyPost - demonstrate POST functionality

using System;
using System.IO;
using System.Net;
using System.Text;
using CurlSharp;

namespace EasyPost
{
    internal class Program
    {
        // Upload the provided PHP script (testpost.php) to webroot and modify the 
        // following URL accordingly
        private const string TEST_URL = "http://localhost/testpost.php";

        private static void Main(string[] args)
        {
            var bits = IntPtr.Size*8;
            Console.WriteLine("Test curl {0} bit", bits);
            Curl.GlobalInit(CurlInitFlag.All);
            Console.WriteLine("Curl Version: {0}\n", Curl.Version);

            const string postData = "parm1=12345&parm2=%22Hello+world%21%22";
            var postLength = postData.Length;

            Console.WriteLine("\n========== TEST 1 HttpWebRequest ============");

            var request = (HttpWebRequest)WebRequest.Create(TEST_URL);
            var data = Encoding.ASCII.GetBytes(postData);
            request.UserAgent = "HttpWebRequest";
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;

            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            var response = (HttpWebResponse)request.GetResponse();
            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

            Console.WriteLine(responseString);


            try
            {
                Console.WriteLine("\n========== TEST 2 CurlEasy PostFields ============");

                using (var easy = new CurlEasy())
                {
                    easy.WriteFunction = OnWriteData;
                    easy.WriteData = null;
                    easy.PostFields = postData;
                    easy.PostFieldSize = postLength;
                    easy.UserAgent = "CurlEasy PostFields";
                    easy.FollowLocation = true;
                    easy.Url = TEST_URL;
                    easy.Post = true;
                    var code = easy.Perform();
                }

                /*
                Console.WriteLine("\n========== TEST 3 CurlEasy HttpPost ============");

                var mf = new CurlHttpMultiPartForm();
                mf.AddSection(CurlFormOption.CopyName, "parm1", CurlFormOption.CopyContents, "value1",
                              CurlFormOption.End);
                mf.AddSection(CurlFormOption.CopyName, "parm2", CurlFormOption.CopyContents, "value2",
                              CurlFormOption.End);
                using (var easy = new CurlEasy())
                {
                    easy.WriteFunction = OnWriteData;
                    easy.WriteData = null;
                    easy.UserAgent = "CurlEasy HttpPost";
                    easy.FollowLocation = true;
                    easy.Url = TEST_URL;
                    easy.HttpPost = mf;
                    var code = easy.Perform();
                }
                */

                Curl.GlobalCleanup();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            Console.WriteLine("Press <ENTER> to exit...");
            Console.ReadLine();
        }

        public static Int32 OnWriteData(Byte[] buf, Int32 size, Int32 nmemb, Object extraData)
        {
            Console.Write(Encoding.UTF8.GetString(buf));
            return size*nmemb;
        }

        public static Int32 OnHeaderData(Byte[] buf, Int32 size, Int32 nmemb, Object extraData)
        {
            var userData = (string) extraData;
            Console.Write("->" + Encoding.UTF8.GetString(buf));
            return size*nmemb;
        }

        public static void OnDebug(CurlInfoType infoType, String msg, Object extraData)
        {
            // print out received data only
            if (infoType == CurlInfoType.DataIn)
                Console.WriteLine(msg);
        }
    }
}