// BookPost.cs - Look for Topology Books on Amazon
// usage: BookPost
// NOTE: you may have to tweak this, as Amazon's page changes from time-to-time

using System;
using System.Text;
using CurlSharp;

namespace BookPost
{
    internal class BookPost
    {
        public static void Main(String[] args)
        {
            try
            {
                Curl.GlobalInit(CurlInitFlag.All);

                using (var easy = new CurlEasy())
                {
                    const string postData = "url=index%3Dstripbooks&field-keywords=Topology&Go.x=10&Go.y=10";
                    easy.WriteFunction = OnWriteData;
                    easy.WriteData = null;
                    easy.PostFields = postData;
                    easy.PostFieldSize = postData.Length;
                    easy.UserAgent = "Mozilla 4.0 (compatible; MSIE 6.0; Win32";
                    easy.FollowLocation = true;
                    easy.Url = "http://www.amazon.com/exec/obidos/search-handle-form/002-5928901-6229641";
                    easy.Post = true;

                    var code = easy.Perform();
                }

                Curl.GlobalCleanup();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.ReadLine();
            }
            Console.ReadLine();
        }

        public static Int32 OnWriteData(Byte[] buf, Int32 size, Int32 nmemb, Object extraData)
        {
            Console.Write(Encoding.UTF8.GetString(buf));
            return size*nmemb;
        }
    }
}