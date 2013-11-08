// FileUpload.cs - demonstrate RFC 1867 file upload capability
// usage: FileUpload url fileName
// e.g. FileUpload http://mybox/cgi-bin/myscript.cgi myFile.dat
// NOTE: you'll need to tweak this as per the specific form you're sending

using System;
using CurlSharp;

namespace FileUpload
{
    internal class FileUpload
    {
        public static void Main(String[] args)
        {
            try
            {
                Curl.GlobalInit(CurlInitFlag.All);

                // <form action="http://mybox/cgi-bin/myscript.cgi
                //  method="post" enctype="multipart/form-data">
                using (var mf = new CurlHttpMultiPartForm())
                {
                    mf.AddSection(CurlFormOption.CopyName, "frmUsername",
                        CurlFormOption.CopyContents, "testtcc",
                        CurlFormOption.End);

                    // <input name="frmPassword">
                    mf.AddSection(CurlFormOption.CopyName, "frmPassword",
                        CurlFormOption.CopyContents, "tcc",
                        CurlFormOption.End);

                    // <input name="frmFileOrigPath">
                    mf.AddSection(CurlFormOption.CopyName, "frmFileOrigPath",
                        CurlFormOption.CopyContents, args[1],
                        CurlFormOption.End);

                    // <input name="frmFileDate">
                    mf.AddSection(CurlFormOption.CopyName, "frmFileDate",
                        CurlFormOption.CopyContents, "08/01/2004",
                        CurlFormOption.End);

                    // <input type="File" name="f1">
                    mf.AddSection(CurlFormOption.CopyName, "f1",
                        CurlFormOption.File, args[1],
                        CurlFormOption.ContentType, "application/binary",
                        CurlFormOption.End);

                    using (var easy = new CurlEasy())
                    {
                        easy.DebugFunction = OnDebug;
                        easy.Verbose = true;
                        easy.ProgressFunction = OnProgress;
                        easy.Url = args[0];
                        easy.HttpPost = mf;

                        easy.Perform();
                    }
                }

                Curl.GlobalCleanup();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public static void OnDebug(CurlInfoType infoType, String msg, Object extraData)
        {
            // print out received data only
            if (infoType == CurlInfoType.DataIn)
                Console.WriteLine(msg);
        }


        public static Int32 OnProgress(Object extraData, Double dlTotal, Double dlNow, Double ulTotal, Double ulNow)
        {
            Console.WriteLine("Progress: {0} {1} {2} {3}", dlTotal, dlNow, ulTotal, ulNow);
            return 0; // standard return from PROGRESSFUNCTION
        }
    }
}