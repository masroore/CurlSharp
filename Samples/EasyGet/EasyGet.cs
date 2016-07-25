// EasyGet.cs - demonstrate trivial get capability
// usage: EasyGet url, e.g. EasyGet http://www.google.com

using System;
using System.Linq;
using System.Text;
using CurlSharp;

namespace EasyGet
{
    internal class EasyGet
    {
        public static void Main(String[] args)
        {
            try
            {
                Curl.GlobalInit(CurlInitFlag.All);

                using (var easy = new CurlEasy())
                {
                    easy.Url = args.Count() > 1 ? args[0] : "http://www.amazon.com/";
                    easy.WriteData = null;
                    easy.WriteFunction = OnWriteData;
                    //easy.ProgressFunction = OnProgressData;
                    easy.Perform();
                }

                Curl.GlobalCleanup();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.ReadLine();
            }
        }

        public static Int32 OnWriteData(Byte[] buf, Int32 size, Int32 nmemb, Object extraData)
        {
            //var userData = (string)extraData;
            Console.Write(Encoding.UTF8.GetString(buf));
            return size*nmemb;
        }

        public static int OnProgressData(Object extraData, double dlTotal, double dlNow, double ulTotal, double ulNow)
        {
            //Console.WriteLine("{0} / {1}", dlNow, dlTotal);
            return 0;
        }
    }
}