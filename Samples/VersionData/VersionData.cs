// $Id: VersionData.cs,v 1.1 2005/02/17 22:47:24 jeffreyphillips Exp $
// VersionData.cs - dump cURL version data
// Compile with "csc /r:../bin/CurlSharp.dll /out:../bin/VersionData.exe VersionData.cs"

// usage: VersionData

using System;
using CurlSharp;

internal class VersionData
{
    public static void Main(String[] args)
    {
        try
        {
            Curl.GlobalInit((int) CurlInitFlag.All);

            var vd = Curl.GetVersionInfo(CurlVersion.Now);
            Console.WriteLine("           Age: {0}", vd.Age);
            Console.WriteLine("Version String: {0}", vd.Version);
            Console.WriteLine("Version Number: {0}", vd.VersionNum);
            Console.WriteLine("   Host System: {0}", vd.Host);
            Console.WriteLine("Feature Bitmap: {0}", vd.Features);
            Console.WriteLine("   Ssl Version: {0}", vd.SslVersion);
            Console.WriteLine("Ssl VersionNum: {0}", vd.SSLVersionNum);
            Console.WriteLine("  LibZ Version: {0}", vd.LibZVersion);
            Console.WriteLine("  ARES Version: {0}", vd.ARes);
            Console.WriteLine("  ARES Ver Num: {0}", vd.AResNum);
            Console.WriteLine("LibIdn Version: {0}", vd.LibIdn);
            Console.WriteLine();
            Console.WriteLine("Protocols:");
            var protocols = vd.Protocols;
            foreach (var prot in protocols)
                Console.WriteLine("  {0}", prot);

            Curl.GlobalCleanup();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }
}