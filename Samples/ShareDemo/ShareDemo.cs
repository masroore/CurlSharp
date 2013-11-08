// $Id: ShareDemo.cs,v 1.1 2005/02/17 22:47:24 jeffreyphillips Exp $
// ShareDemo.cs - demonstrate share capability
// Compile with "csc /r:../bin/CurlSharp.dll /out:../bin/ShareDemo.exe ShareDemo.cs"

// usage: ShareDemo url1 url2, e.g. ShareDemo http://www.google.com http://www.yahoo.com

using System;
using System.Collections.Generic;
using System.Threading;
using CurlSharp;

namespace ShareDemo
{
    internal class ShareDemo
    {
        // synchronization objects for Dns and Cookies
        private static object dnsLock, cookieLock;

        public static void Main(String[] args)
        {
            try
            {
                Curl.GlobalInit((int) CurlInitFlag.All);

                dnsLock = new Object();
                cookieLock = new Object();

                var urls = new[]
                {
                    "http://www.codeplex.com",
                    "http://www.yahoo.com",
                    "http://www.cnn.com",
                    "http://www.abc.com",
                    "http://www.bbc.co.uk"
                };

                using (var share = new CurlShare())
                {
                    share.LockFunction = OnLock;
                    share.UnlockFunction = OnUnlock;
                    share.Share = CurlLockData.Cookie;
                    share.Share = CurlLockData.Dns;

                    var workers = new List<Thread>();
                    foreach (var url in urls)
                    {
                        var et = new EasyThread(url, share);
                        var thread = new Thread(et.ThreadFunc);
                        workers.Add(thread);
                        thread.Start();
                    }

                    foreach (var thread in workers)
                    {
                        thread.Join();
                    }
                }

                Curl.GlobalCleanup();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public static void OnLock(CurlLockData data, CurlLockAccess access, Object extraData)
        {
            //Console.WriteLine("OnLock({0}, {1})", data, access);
            switch (data)
            {
                case CurlLockData.Dns:
                    Monitor.Enter(dnsLock);
                    break;
                case CurlLockData.Cookie:
                    Monitor.Enter(cookieLock);
                    break;
            }
        }

        public static void OnUnlock(CurlLockData data, Object extraData)
        {
            //Console.WriteLine("OnUnlock({0})", data);
            switch (data)
            {
                case CurlLockData.Dns:
                    Monitor.Exit(dnsLock);
                    break;
                case CurlLockData.Cookie:
                    Monitor.Exit(cookieLock);
                    break;
            }
        }
    }
}