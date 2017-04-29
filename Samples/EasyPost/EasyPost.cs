using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using CurlSharp;

namespace EasyPost
{
    internal class EasyPost
    {
        // Upload the provided PHP script (testpost.php) to webroot and modify the 
        // following URL accordingly
        // private const string TEST_URL = "http://localhost/testpost.php";
        private const string TEST_URL = "http://httpbin.org/post";

        private static void Main(string[] args)
        {
            var bits = IntPtr.Size * 8;
            Console.WriteLine("Test curl {0} bit", bits);
            Curl.GlobalInit(CurlInitFlag.All);
            Console.WriteLine("Curl Version: {0}\n", Curl.Version);

            const string postData = "parm1=12345&parm2=Hello+world%21";
            var postLength = postData.Length;

            Console.WriteLine("\n========== TEST 1 HttpClient ============");

            using (var httpClient = new HttpClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Post, TEST_URL);
                request.Headers.Add("User-Agent", "HttpClient");
                request.Content = new StringContent(postData);
                request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded");

                var response = httpClient.SendAsync(request).Result;
                var responseString = response.Content.ReadAsStringAsync().Result;

                Console.WriteLine(responseString);
            }

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

                Console.WriteLine("\n========== TEST 3 CurlEasy HttpPost ============");

                var mf = new CurlHttpMultiPartForm();
                mf.AddSection(CurlFormOption.CopyName, "parm1",
                    CurlFormOption.CopyContents, "12345",
                    CurlFormOption.End);
                mf.AddSection(CurlFormOption.CopyName, "parm2",
                    CurlFormOption.CopyContents, "Hello world!",
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

                Curl.GlobalCleanup();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            Console.WriteLine("\nPress <ENTER> to exit...");
            Console.ReadLine();
        }

        public static Int32 OnWriteData(Byte[] buf, Int32 size, Int32 nmemb, Object extraData)
        {
            Console.Write(Encoding.UTF8.GetString(buf));
            return size * nmemb;
        }

        public static Int32 OnHeaderData(Byte[] buf, Int32 size, Int32 nmemb, Object extraData)
        {
            var userData = (string) extraData;
            Console.Write("->" + Encoding.UTF8.GetString(buf));
            return size * nmemb;
        }

        public static void OnDebug(CurlInfoType infoType, String msg, Object extraData)
        {
            // print out received data only
            if (infoType == CurlInfoType.DataIn)
                Console.WriteLine(msg);
            }
        }
    }
