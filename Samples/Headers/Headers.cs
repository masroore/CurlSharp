// Headers.cs - dump headers
// usage: Headers url, e.g. Headers http://www.google.com

using System;
using System.Linq;
using System.Text;
using CurlSharp;

namespace Headers
{
    internal class Headers
    {
        public static void Main(String[] args)
        {
            try
            {
                Curl.GlobalInit(CurlInitFlag.All);

                using (var easy = new CurlEasy())
                {
                    easy.Url = args.Count() > 1 ? args[0] : "http://www.amazon.com";
                    easy.HeaderData = "headerContext";
                    easy.HeaderFunction = OnHeaderData;
                    easy.Perform();
                }

                Curl.GlobalCleanup();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            Console.ReadLine();
        }

        public static Int32 OnHeaderData(Byte[] buf, Int32 size, Int32 nmemb, Object extraData)
        {
            var userData = (string) extraData;
            Console.Write(Encoding.UTF8.GetString(buf));
            return size*nmemb;
        }
    }
}