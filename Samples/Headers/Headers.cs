// $Id: Headers.cs,v 1.1 2005/02/17 22:47:24 jeffreyphillips Exp $
// Headers.cs - dump headers
// Compile with "csc /r:../bin//CurlSharp.dll /out:../bin/Headers.exe Headers.cs"

// usage: Headers url, e.g. Headers http://www.google.com

using System;
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
                Curl.GlobalInit((int) CurlInitFlag.All);

                using (var easy = new CurlEasy())
                {
                    easy.Url = args[0];
                    easy.HeaderFunction = OnHeaderData;
                    easy.Perform();
                }

                Curl.GlobalCleanup();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public static Int32 OnHeaderData(Byte[] buf, Int32 size, Int32 nmemb,
            Object extraData)
        {
            Console.Write(Encoding.UTF8.GetString(buf));
            return size*nmemb;
        }
    }
}