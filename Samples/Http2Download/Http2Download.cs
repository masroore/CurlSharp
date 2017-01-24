/*
 * Multiplexed HTTP/2 downloads over a single connection
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using CurlSharp;
using static System.Console;

namespace Http2Download
{
    internal class Program
    {
        private static readonly Dictionary<string, BinaryWriter> Writers = new Dictionary<string, BinaryWriter>();

        private static void Main(string[] args)
        {
            Curl.GlobalInit(CurlInitFlag.Default);
            var handles = new List<CurlEasy>();

            using (var multi = new CurlMulti())
            {
                foreach (var url in new[]
                {
                    "https://www.google.com",
                    "https://www.yahoo.com",
                    "http://edition.cnn.com",
                    "https://news.ycombinator.com",
                    "https://pages.github.com"
                })
                {
                    Writers.Add(url, new BinaryWriter(new FileStream($"dl-{HostName(url)}.html", FileMode.Create)));
                    var easy = CreateEasy(url);
                    multi.AddHandle(easy);
                    handles.Add(easy);
                }

                var stillRunning = 1;
                // call CurlMulti.Perform right away (note ref qualifier)
                multi.Perform(ref stillRunning);

                while (stillRunning != 0)
                {
                    var mc = multi.FdSet();
                    if (mc != CurlMultiCode.Ok)
                    {
                        WriteLine($"curl_multi_fdset() failed, code: {mc}");
                        break;
                    }

                    /* On success the value of maxfd is guaranteed to be >= -1. We call select(maxfd + 1, ...); specially in case of (maxfd == -1) there are no fds ready yet so we call select(0, ...) --or Sleep() on Windows-- to sleep 100ms, which is the minimum suggested value in the curl_multi_fdset() doc. */
                    var rc = 0;

                    if (multi.MaxFd == -1) Thread.Sleep(100);
                    else
                        rc = multi.Select(1000); // one second

                    switch (rc)
                    {
                        case -1: // select error
                            break;
                        default:
                        {
                            multi.Perform(ref stillRunning);
                            break;
                        }
                    }
                }
            }

            foreach (var w in Writers.Values) w.Close();

            foreach (var h in handles) h.Dispose();

            Curl.GlobalCleanup();
        }

        private static CurlEasy CreateEasy(string url)
        {
            return new CurlEasy
            {
                // HTTP/2 please
                HttpVersion = CurlHttpVersion.Http2_0,
                Url = url,
                WriteFunction = OnWriteData,
                WriteData = (string) url.Clone(),
                Encoding = "deflate, gzip",

                // set it verbose for max debuggaility
                Verbose = true,
                DebugFunction = OnDebug,

                // skip SSL verification during debugging
                SslVerifyPeer = false,
                SslVerifyhost = false
            };
        }

        public static void OnDebug(CurlInfoType infoType, string message, int size, object extraData)
        {
            switch (infoType)
            {
                case CurlInfoType.Text:
                    WriteLine($"== Info: {message.TrimEnd()}");
                    break;
                case CurlInfoType.HeaderOut:
                    WriteLine($"=> Send header");
                    break;
                case CurlInfoType.HeaderIn:
                    WriteLine($"<= Recv header");
                    break;
                case CurlInfoType.DataOut:
                    WriteLine($"=> Send data");
                    break;
                case CurlInfoType.DataIn:
                    WriteLine($"<= Recv data");
                    break;
                case CurlInfoType.SslDataOut:
                    WriteLine($"=> Send SSL data");
                    break;
                case CurlInfoType.SslDataIn:
                    WriteLine($"<= Recv SSL data");
                    break;
                case CurlInfoType.End:
                    WriteLine("== End");
                    break;
            }
        }

        private static string HostName(string url) => new Uri(url).Host.Replace('.', '_').ToLower();

        public static int OnWriteData(byte[] buf, int size, int nmemb, object extraData)
        {
            var nBytes = size*nmemb;
            Writers[(string) extraData].Write(buf);
            return nBytes;
        }
    }
}