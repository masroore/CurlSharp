// Upload.cs - demonstrate ftp upload capability
// usage: Upload srcFile destUrl username password
// e.g. Upload myFile.dat ftp://ftp.myftp.com me myPassword

using System;
using System.IO;
using CurlSharp;
using static System.Console;

namespace Upload
{
    internal class Upload
    {
        public static void Main(string[] args)
        {
            try
            {
                Curl.GlobalInit(CurlInitFlag.All);

                using (var fs = new FileStream(args[0], FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    using (var easy = new CurlEasy())
                    {
                        easy.ReadFunction = OnReadData;
                        easy.ReadData = fs;
                        easy.DebugFunction = OnDebug;
                        easy.SetOpt(CurlOption.Verbose, true);
                        easy.ProgressFunction = OnProgress;
                        easy.Url = args[1];
                        easy.SetOpt(CurlOption.UserPwd, args[2] + ":" + args[3]);
                        easy.Upload = true;
                        easy.InfileSize = fs.Length;

                        easy.Perform();
                    }
                }

                Curl.GlobalCleanup();
            }
            catch (Exception ex)
            {
                WriteLine(ex);
                ReadLine();
            }
        }

        public static int OnReadData(byte[] buf, int size, int nmemb, object extraData)
        {
            var fs = (FileStream) extraData;
            return fs.Read(buf, 0, size*nmemb);
        }

        public static void OnDebug(CurlInfoType infoType, string msg, int size, object extraData) => WriteLine(msg);

        public static int OnProgress(object extraData, double dlTotal, double dlNow, double ulTotal, double ulNow)
        {
            WriteLine($"Progress: {dlTotal} {dlNow} {ulTotal} {ulNow}");
            return 0; // standard return from PROGRESSFUNCTION
        }
    }
}