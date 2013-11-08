// InfoDemo.cs - demonstrate CurlEasy.GetInfo
// usage: InfoDemo url, e.g. InfoDemo http://www.google.com

using System;
using System.Linq;
using CurlSharp;

namespace InfoDemo
{
    internal class InfoDemo
    {
        public static void Main(String[] args)
        {
            try
            {
                Curl.GlobalInit(CurlInitFlag.All);

                using (var easy = new CurlEasy())
                {
                    easy.Url = args.Count() > 1 ? args[0] : "http://www.amazon.com";
                    easy.Private = "Private string";
                    easy.Filetime = true;
                    easy.Perform();

                    // now, exercise the various GetInfo stuff
                    var d = easy.ConnectTime;
                    Console.WriteLine("Connect Time: {0}", d);

                    d = easy.ContentLengthDownload;
                    Console.WriteLine("Content Length (Download): {0}", d);

                    d = easy.ContentLengthUpload;
                    Console.WriteLine("Content Length (Upload): {0}", d);

                    var s = easy.ContentType;
                    Console.WriteLine("Content Type: {0}", s);

                    // TODO: !!!BUG!!! curl_shim_get_file_time()
                    var dt = easy.Filetime;
                    Console.WriteLine("File time: {0}", dt);

                    var n = easy.HeaderSize;
                    Console.WriteLine("Header Size: {0}", n);

                    n = easy.HttpAuthAvail;
                    Console.WriteLine("Authentication Bitmask: {0}", n);

                    n = easy.HttpConnectCode;
                    Console.WriteLine("HTTP Connect Code: {0}", n);

                    d = easy.NameLookupTime;
                    Console.WriteLine("Name Lookup Time: {0}", d);

                    n = easy.OsErrno;
                    Console.WriteLine("OS Errno: {0}", n);

                    d = easy.PreTransferTime;
                    Console.WriteLine("Pretransfer time: {0}", d);

                    var o = easy.Private;
                    Console.WriteLine("Private Data: {0}", o);

                    n = easy.ProxyAuthAvail;
                    Console.WriteLine("Proxy Authentication Schemes: {0}", n);

                    n = easy.RedirectCount;
                    Console.WriteLine("Redirect count: {0}", n);

                    d = easy.RedirectTime;
                    Console.WriteLine("Redirect time: {0}", d);

                    n = easy.RequestSize;
                    Console.WriteLine("Request size: {0}", n);

                    n = easy.ResponseCode;
                    Console.WriteLine("Response code: {0}", n);

                    d = easy.SizeDownload;
                    Console.WriteLine("Download size: {0}", d);

                    d = easy.SizeUpload;
                    Console.WriteLine("Upload size: {0}", d);

                    d = easy.SpeedDownload;
                    Console.WriteLine("Download speed: {0}", d);

                    d = easy.SpeedUpload;
                    Console.WriteLine("Upload speed: {0}", d);

                    n = easy.SslVerifyResult;
                    Console.WriteLine("Ssl verification result: {0}", n);

                    d = easy.StartTransferTime;
                    Console.WriteLine("Start transfer time: {0}", d);

                    d = easy.TotalTime;
                    Console.WriteLine("Total time: {0}", d);
                }

                Curl.GlobalCleanup();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}