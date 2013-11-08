// $Id: EasyGet.cs,v 1.1 2005/02/17 22:47:24 jeffreyphillips Exp $
// EasyGet.cs - demonstrate trivial get capability
// Compile with "csc /r:../bin/CurlSharp.dll /out:../bin/EasyGet.exe EasyGet.cs"

// usage: EasyGet url, e.g. EasyGet http://www.google.com

using System;
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
                Curl.GlobalInit((int) CurlInitFlag.All);

                using (var easy = new CurlEasy())
                {
                    easy.Url = args[0];
                    easy.WriteFunction = OnWriteData;
                    easy.Perform();
                }

                Curl.GlobalCleanup();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public static Int32 OnWriteData(Byte[] buf, Int32 size, Int32 nmemb, Object extraData)
        {
            Console.Write(Encoding.UTF8.GetString(buf));
            return size*nmemb;
        }
    }
}