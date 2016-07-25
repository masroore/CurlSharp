// Upload.cs - demonstrate ftp upload capability
// usage: Upload srcFile destUrl username password
// e.g. Upload myFile.dat ftp://ftp.myftp.com me myPassword

using System;
using System.IO;
using CurlSharp;

namespace Upload
{
    internal class Upload
    {
        public static void Main(String[] args)
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
                Console.WriteLine(ex);
                Console.ReadLine();
            }
        }

        public static Int32 OnReadData(Byte[] buf, Int32 size, Int32 nmemb, Object extraData)
        {
            var fs = (FileStream) extraData;
            return fs.Read(buf, 0, size*nmemb);
        }
        
        public static void OnDebug(CurlInfoType infoType, String msg, Object extraData)
        {
            Console.WriteLine(msg);
        }
    
        public static Int32 OnProgress(Object extraData, Double dlTotal,
            Double dlNow, Double ulTotal, Double ulNow)
        {
            Console.WriteLine("Progress: {0} {1} {2} {3}",
                dlTotal, dlNow, ulTotal, ulNow);
            return 0; // standard return from PROGRESSFUNCTION
        }
    }
}