// $Id: SSLGet.cs,v 1.1 2005/02/17 22:47:24 jeffreyphillips Exp $
// SSLGet.cs - demonstrate Ssl get capability
// Compile with "csc /r:../bin/CurlSharp.dll /out:../bin/SSLGet.exe SSLGet.cs"

// usage: SSLGet url, e.g. SSLGet https://sourceforge.net

using System;
using System.Text;
using CurlSharp;

internal class SSLGet
{
    public static void Main(String[] args)
    {
        try
        {
            Curl.GlobalInit((int) CurlInitFlag.All);

            using (var easy = new CurlEasy())
            {
                easy.WriteFunction = OnWriteData;
                easy.SslContextFunction = OnSslContext;
                easy.Url = args[0];
                easy.CaInfo = "ca-bundle.crt";

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

    public static CurlCode OnSslContext(CurlSslContext ctx, Object extraData)
    {
        // To do anything useful with the CurlSslContext object, you'll need
        // to call the OpenSSL native methods on your own. So for this
        // demo, we just return what cURL is expecting.
        return CurlCode.Ok;
    }
}