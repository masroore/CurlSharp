// $Id: BookPost.cs,v 1.1 2005/02/17 22:47:24 jeffreyphillips Exp $
// BookPost.cs - Look for Topology Books on Amazon
// Compile with "csc /r:..../bin/CurlSharp.dll /out:../bin/BookPost.exe BookPost.cs"

// usage: BookPost
// NOTE: you may have to tweak this, as Amazon's page changes from time-to-time

using System;
using System.Text;
using CurlSharp;

namespace BookPost
{
    internal class BookPost
    {
        public static void Main(String[] args)
        {
            try
            {
                Curl.GlobalInit((int) CurlInitFlag.All);

                using (var easy = new CurlEasy())
                {
                    easy.WriteFunction = OnWriteData;
                    // simple post - with a string
                    easy.PostFields = "url=index%3Dstripbooks&field-keywords=Topology&Go.x=10&Go.y=10";
                    easy.UserAgent = "Mozilla 4.0 (compatible; MSIE 6.0; Win32";
                    easy.FollowLocation = true;
                    easy.Url = "http://www.amazon.com/exec/obidos/search-handle-form/002-5928901-6229641";
                    easy.Post = true;

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