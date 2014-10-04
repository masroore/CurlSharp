CurlSharp
=========

CurlSharp is a .Net binding and object-oriented wrapper for [libcurl](http://curl.haxx.se/libcurl/).

libcurl is a web-client library that can provide cross-platform .Net applications with an easy way to implement such things as:

- HTTP ( GET / HEAD / PUT / POST / multi-part / form-data )
- FTP ( upload / download / list / 3rd-party )
- HTTPS, FTPS, SSL, TLS  ( via OpenSSL or GnuTLS )
- Proxies, proxy tunneling, cookies, user+password authentication.
- File transfer resume, byte ranges, multiple asynchronous transfers.
- and much more...

CurlSharp provides simple get/set properties for libcurl's options and information functions, event-based hooks to libcurl's I/O, status, and progress callbacks, and wraps the c-style file I/O behind simple filename properties. The CurlEasy class contains has more than 100 different properties and methods to handle a wide variety of URL transfer requirements. While this may seem overwhelming at first glance, the good news is you will probably need only a tiny subset of these for most situations.

The CurlSharp library consists of these parts:

- Pure C# P/Invoke bindings to the libcurl API.
- Optional libcurlshim helper DLL [WIN32].
- The `CurlEasy` class which provides a wrapper around a curl_easy session.
- The `CurlMulti` class, which serves as a container for multiple CurlEasy objects, and provides a wrapper around a curl_multi session.
- The `CurlShare` class which provides an infrastructure for serializing access to data shared by multiple `CurlEasy` objects, including cookie data and DNS hosts. It implements the `curl_share_xxx` API. 
- The `CurlHttpMultiPartForm` to easily construct multi-part forms.
- The `CurlSlist` class which wraps a linked list of strings used in cURL.

CurlSharp is available for these platforms:

- [Stable] Windows 32-bit
- [Experimental] Win64 port
- [Experimental] Mono Linux & OS X support

#### Examples ####
A simple HTTP download program...

    using System;
    using CurlSharp;

    internal class EasyGet
    {
        public static void Main(String[] args)
        {
            Curl.GlobalInit(CurlInitFlag.All);
			
			try
			{
	            using (var easy = new CurlEasy())
	            {
	                easy.Url = "http://www.google.com/";
	            	easy.WriteFunction = OnWriteData;
		            easy.Perform();
	    	    }
			}
			finally
			{
    	    	Curl.GlobalCleanup();
			}	
	    }
	
	    public static Int32 OnWriteData(Byte[] buf, Int32 size, Int32 nmemb, Object extraData)
	    {
        	Console.Write(Encoding.UTF8.GetString(buf));
	        return size*nmemb;
	    }
	}
	
Simple HTTP Post example:

  using (var easy = new CurlEasy())
  {
      easy.Url = "http://hostname/testpost.php";
      easy.Post = true;
      var postData = "parm1=12345&parm2=Hello+world%21";
      easy.PostFields = postData;
      easy.PostFieldSize = postLength;
      easy.Perform();
  }

Several more samples are included in the Samples folder.

#### Credits ####
CurlSharp Written by Dr. Masroor Ehsan.

CurlSharp is based on original code by Jeff Phillips [libcurl.NET](http://sourceforge.net/projects/libcurl-net/). Original code has been modified and greatly extended.

----------

CurlSharp Copyright © 2013 Dr. Masroor Ehsan