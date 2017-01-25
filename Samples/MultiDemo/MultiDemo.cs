// MultiDemo.cs - demonstrate multi capability
// usage: MultiDemo url1 url2, e.g. MultiDemo http://www.google.com http://www.yahoo.com

using System;
using System.Collections.Generic;
using CurlSharp;

namespace MultiDemo
{
    internal class MultiDemo
    {
        public static void Main(String[] args)
        {
            try
            {
                Curl.GlobalInit(CurlInitFlag.All);

                CurlWriteCallback wf = OnWriteData;

                var handles = new List<CurlEasy>();
                var urls = new[]
                {
                    "http://www.codeplex.com",
                    "http://www.yahoo.com",
                    "http://www.cnn.com",
                    "http://www.abc.com",
                    "http://www.bbc.co.uk"
                };

                using (var multi = new CurlMulti())
                {
                    foreach (var url in urls)
                    {
                        var easy = CreateEasy(url, wf);
                        multi.AddHandle(easy);
                        handles.Add(easy);
                    }

                    var runningObjects = 1;
                    // call CurlMulti.Perform right away (note ref qualifier)
                    multi.Perform(ref runningObjects);

                    while (runningObjects != 0)
                    {
                        var fdret = multi.FdSet();
                        //if (multi.MaxFd > -1)
                        {
                            var rc = multi.Select(1000); // one second
                            switch (rc)
                            {
                                case -1:
                                    Console.WriteLine("CurlMulti.Select() returned -1");
                                    //runningObjects = 0;
                                    break;
                                case 0: // time out
                                default:
                                {
                                    multi.Perform(ref runningObjects);
                                    break;
                                }
                            }
                        }
                    }
                }

                foreach (var easy in handles)
                {
                    easy.Dispose();
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

        private static CurlEasy CreateEasy(string url, CurlWriteCallback wf)
        {
            var easy = new CurlEasy();
            var data = (String) url.Clone();
            easy.Url = url;
            easy.WriteFunction = wf;
            easy.WriteData = data;
            easy.Encoding = "deflate, gzip";
            return easy;
        }

        public static Int32 OnWriteData(Byte[] buf, Int32 size, Int32 nmemb, Object extraData)
        {
            var nBytes = size*nmemb;
            Console.WriteLine("Obtained {0} bytes from {1}", nBytes, extraData);
            return nBytes;
        }
    }
}