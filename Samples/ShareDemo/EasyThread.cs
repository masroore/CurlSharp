using System;
using CurlSharp;

namespace ShareDemo
{
    public class EasyThread
    {
        // state information
        private static readonly CurlWriteCallback wf;
        private readonly CurlShare _curlShare;
        private readonly String _url;

        // static class constructor to create static delegate
        static EasyThread()
        {
            Console.WriteLine("EasyThread class constructor");
            wf = OnWriteData;
        }

        // instance constructor for url
        public EasyThread(String s, CurlShare shr)
        {
            Console.WriteLine("EasyThread instance constructor: url={0}", s);
            _url = s;
            _curlShare = shr;
        }

        public void ThreadFunc()
        {
            using (var easy = new CurlEasy())
            {
                easy.BufferSize = 64*1024;
                easy.Url = _url;
                easy.WriteFunction = wf;
                easy.WriteData = _url;
                easy.Share = _curlShare;
                easy.Perform();
            }
        }

        public static Int32 OnWriteData(Byte[] buf, Int32 size, Int32 nmemb, Object extraData)
        {
            var nBytes = size*nmemb;
            Console.WriteLine("Obtained {0} bytes from {1}", nBytes, extraData);
            return nBytes;
        }
    }
}