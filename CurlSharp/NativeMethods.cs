/***************************************************************************
 *
 * CurlS#arp
 *
 * Copyright (c) 2014 Dr. Masroor Ehsan (masroore@gmail.com)
 * Portions copyright (c) 2004, 2005 Jeff Phillips (jeff@jeffp.net)
 *
 * This software is licensed as described in the file LICENSE, which you
 * should have received as part of this distribution.
 *
 * You may opt to use, copy, modify, merge, publish, distribute and/or sell
 * copies of this Software, and permit persons to whom the Software is
 * furnished to do so, under the terms of the LICENSE file.
 *
 * This software is distributed on an "AS IS" basis, WITHOUT WARRANTY OF
 * ANY KIND, either express or implied.
 *
 **************************************************************************/

//#define USE_LIBCURLSHIM

using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace CurlSharp
{
    /// <summary>
    ///     P/Invoke signatures.
    /// </summary>
    internal static unsafe class NativeMethods
    {
        private const string CURL_LIB_X64 = "libcurl.dll"; // should be in amd64 directory
        private const string CURL_LIB_X86 = "libcurl.dll"; // should be in i386 directory

        private const string CURL_LIB_LINUX = "libcurl";
        private const string CURLSHIM_LIB_X64 = "libcurlshim64.dll";
        private const string CURLSHIM_LIB_X86 = "libcurlshim.dll";

        private const string WINSOCK_LIB_LINUX = "libc";
        private const string WINSOCK_LIB_X86 = "ws2_32.dll";
        private const string LIBX64DIR = "amd64";
        private const string LIBX86DIR = "i386";

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool SetDllDirectory(string lpPathName);

        internal enum NETPlatformType
        {
            Unknown,
            Mono,
            WinX64,
            WinX86
        }
        private static NETPlatformType platformType = NETPlatformType.Unknown;

        internal static NETPlatformType PlatformType
        {
            get
            {
                return platformType;
            }

            set
            {
                platformType = value;
            }
        }
        internal static NETPlatformType ProcessPlatformType()
        {
            NETPlatformType type;
            string dllSubFolder = string.Empty;
            type = GetPlatformType();
            string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + Path.DirectorySeparatorChar;
                 
            switch (type)
            {
                case NETPlatformType.Mono:
                break;
                case NETPlatformType.WinX64:
                dllSubFolder = LIBX64DIR;
                 
                break;
                case NETPlatformType.WinX86:
                dllSubFolder = LIBX86DIR;
                 
                break;
                case NETPlatformType.Unknown:
                break;
                default:
                    break;
            }
        
            if (type  == NETPlatformType.WinX86 || type == NETPlatformType.WinX64)
            {
                path += dllSubFolder;
                SetDllDirectory(path);
            }
         
            return type;

        }
        internal static NETPlatformType GetPlatformType()        
        {
            NETPlatformType type = NETPlatformType.Unknown;

            if (Type.GetType("Mono.Runtime") != null && Environment.OSVersion.Platform == PlatformID.Unix)
            {
                // Mono on Linux or OSX
                type = NETPlatformType.Mono;
            }
            else
            {
                
              
                // this is Windows NET
                if (IntPtr.Size == 8)
                {
                    // this is x64 process    
                    type = NETPlatformType.WinX64;
                
                }       
                else if (IntPtr.Size == 4)
                {
                   
                    type = NETPlatformType.WinX86;
                    
                }

            }


            return type;
        }

        // internal delegates from cURL
        
        // libcurl imports
        [DllImport(CURL_LIB_LINUX, EntryPoint = "curl_global_init", CallingConvention = CallingConvention.Cdecl)]
        private static extern CurlCode curl_global_init_linux(int flags);
        [DllImport(CURL_LIB_X64, EntryPoint = "curl_global_init", CallingConvention = CallingConvention.Cdecl)]
        private static extern CurlCode curl_global_init_x64(int flags);
        [DllImport(CURL_LIB_X86, EntryPoint = "curl_global_init", CallingConvention = CallingConvention.Cdecl)]
        private static extern CurlCode curl_global_init_x86(int flags);



        internal static CurlCode curl_global_init(int flags)
        {
            CurlCode result;
            if (PlatformType == NETPlatformType.Unknown)
            {
                PlatformType = ProcessPlatformType();
            }


            switch (PlatformType)
            {
                case NETPlatformType.Mono:
                    result = curl_global_init_linux(flags);
                    break;
                case NETPlatformType.WinX86:
                    result = curl_global_init_x86(flags);
                    break;
                case NETPlatformType.WinX64:
                    result = curl_global_init_x64(flags);
                    break;
                default:
                    throw new InvalidOperationException("Can not determine type of NET platform");
                    
            }
            return result;
        }
    



        [DllImport(CURL_LIB_LINUX, EntryPoint = "curl_global_cleanup", CallingConvention = CallingConvention.Cdecl)]
        private static extern void curl_global_cleanup_linux();
        [DllImport(CURL_LIB_X64, EntryPoint = "curl_global_cleanup", CallingConvention = CallingConvention.Cdecl)]
        private static extern void curl_global_cleanup_x64();
        [DllImport(CURL_LIB_X86, EntryPoint = "curl_global_cleanup", CallingConvention = CallingConvention.Cdecl)]
        private static extern void curl_global_cleanup_x86();
        
        internal static void curl_global_cleanup()
        {
            if (PlatformType == NETPlatformType.Unknown)
            {
                PlatformType = ProcessPlatformType();
            }


            switch (PlatformType)
            {
                case NETPlatformType.Mono:
                    curl_global_cleanup_linux();
                    break;
                case NETPlatformType.WinX86:
                    curl_global_cleanup_x86();
                    break;
                case NETPlatformType.WinX64:
                    curl_global_cleanup_x64();
                    break;
                default:
                    throw new InvalidOperationException("Can not determine type of NET platform");

            }
        }
    


        [DllImport(CURL_LIB_LINUX, EntryPoint = "curl_escape", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr curl_escape_linux(String url, int length);
        [DllImport(CURL_LIB_X64, EntryPoint = "curl_escape", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr curl_escape_x64(String url, int length);
        [DllImport(CURL_LIB_X86, EntryPoint = "curl_escape", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr curl_escape_x86(String url, int length);
        internal static IntPtr curl_escape(String url, int length)
        {
            IntPtr result;
            if (PlatformType == NETPlatformType.Unknown)
            {
                PlatformType = ProcessPlatformType();
            }


            switch (PlatformType)
            {
                case NETPlatformType.Mono:
                    result = curl_escape_linux(url,length);
                    break;
                case NETPlatformType.WinX86:
                    result = curl_escape_x86(url,length);
                    break;
                case NETPlatformType.WinX64:
                    result = curl_escape_x64(url, length);
                    break;
                default:
                    throw new InvalidOperationException("Can not determine type of NET platform");

            }
            return result;
        }
    



        [DllImport(CURL_LIB_LINUX, EntryPoint = "curl_unescape", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr curl_unescape_linux(String url, int length);
        [DllImport(CURL_LIB_X64, EntryPoint = "curl_unescape", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr curl_unescape_x64(String url, int length);
        [DllImport(CURL_LIB_X86, EntryPoint = "curl_unescape", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr curl_unescape_x86(String url, int length);


        internal static IntPtr curl_unescape(String url, int length)
        {
            IntPtr result;
            if (PlatformType == NETPlatformType.Unknown)
            {
                PlatformType = ProcessPlatformType();
            }


            switch (PlatformType)
            {
                case NETPlatformType.Mono:
                    result = curl_unescape_linux(url, length);
                    break;
                case NETPlatformType.WinX86:
                    result = curl_unescape_x86(url, length);
                    break;
                case NETPlatformType.WinX64:
                    result = curl_unescape_x64(url, length);
                    break;
                default:
                    throw new InvalidOperationException("Can not determine type of NET platform");

            }
            return result;
        }


        [DllImport(CURL_LIB_LINUX, EntryPoint = "curl_free", CallingConvention = CallingConvention.Cdecl)]
        private static extern void curl_free_linux(IntPtr p);
        [DllImport(CURL_LIB_X64, EntryPoint = "curl_free", CallingConvention = CallingConvention.Cdecl)]
        private static extern void curl_free_x64(IntPtr p);
        [DllImport(CURL_LIB_X86, EntryPoint = "curl_free", CallingConvention = CallingConvention.Cdecl)]
        private static extern void curl_free_x86(IntPtr p);

        internal static void curl_free(IntPtr p)
        {
           
            if (PlatformType == NETPlatformType.Unknown)
            {
                PlatformType = ProcessPlatformType();
            }


            switch (PlatformType)
            {
                case NETPlatformType.Mono:
                    curl_free_linux(p);
                    break;
                case NETPlatformType.WinX86:
                    curl_free_x86(p);
                    break;
                case NETPlatformType.WinX64:
                    curl_free_x64(p);
                    break;
                default:
                    throw new InvalidOperationException("Can not determine type of NET platform");

            }
        }


        [DllImport(CURL_LIB_LINUX, EntryPoint = "curl_version", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr curl_version_linux();
        [DllImport(CURL_LIB_X64, EntryPoint = "curl_version", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr curl_version_x64();
        [DllImport(CURL_LIB_X86, EntryPoint = "curl_version", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr curl_version_x86();
        internal static IntPtr curl_version()
        {
            IntPtr result;
            if (PlatformType == NETPlatformType.Unknown)
            {
                PlatformType = ProcessPlatformType();
            }


            switch (PlatformType)
            {
                case NETPlatformType.Mono:
                    result = curl_version_linux();
                    break;
                case NETPlatformType.WinX86:
                    result = curl_version_x86();
                    break;
                case NETPlatformType.WinX64:
                    result = curl_version_x64();
                    break;
                default:
                    throw new InvalidOperationException("Can not determine type of NET platform");

            }
            return result;
        }


        [DllImport(CURL_LIB_LINUX, EntryPoint = "curl_easy_init", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr curl_easy_init_linux();
        [DllImport(CURL_LIB_X64, EntryPoint = "curl_easy_init", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr curl_easy_init_x64();
        [DllImport(CURL_LIB_X86, EntryPoint = "curl_easy_init", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr curl_easy_init_x86();
        internal static IntPtr curl_easy_init()
        {
            IntPtr result;
            if (PlatformType == NETPlatformType.Unknown)
            {
                PlatformType = ProcessPlatformType();
            }


            switch (PlatformType)
            {
                case NETPlatformType.Mono:
                    result = curl_easy_init_linux();
                    break;
                case NETPlatformType.WinX86:
                    result = curl_easy_init_x86();
                    break;
                case NETPlatformType.WinX64:
                    result = curl_easy_init_x64();
                    break;
                default:
                    throw new InvalidOperationException("Can not determine type of NET platform");

            }
            return result;
        }

        [DllImport(CURL_LIB_LINUX, EntryPoint = "curl_easy_cleanup", CallingConvention = CallingConvention.Cdecl)]
        private static extern void curl_easy_cleanup_linux(IntPtr pCurl);
        [DllImport(CURL_LIB_X64, EntryPoint = "curl_easy_cleanup", CallingConvention = CallingConvention.Cdecl)]
        private static extern void curl_easy_cleanup_x64(IntPtr pCurl);
        [DllImport(CURL_LIB_X86, EntryPoint = "curl_easy_cleanup", CallingConvention = CallingConvention.Cdecl)]
        private static extern void curl_easy_cleanup_x86(IntPtr pCurl);
        internal static void curl_easy_cleanup(IntPtr pCurl)
        {
            if (PlatformType == NETPlatformType.Unknown)
            {
                PlatformType = ProcessPlatformType();
            }


            switch (PlatformType)
            {
                case NETPlatformType.Mono:
                    curl_easy_cleanup_linux(pCurl);
                    break;
                case NETPlatformType.WinX86:
                    curl_easy_cleanup_x86(pCurl);
                    break;
                case NETPlatformType.WinX64:
                    curl_easy_cleanup_x64(pCurl);
                    break;
                default:
                    throw new InvalidOperationException("Can not determine type of NET platform");

            }
        }
     
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate int _CurlGenericCallback(IntPtr ptr, int sz, int nmemb, IntPtr userdata);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate int _CurlProgressCallback(
            IntPtr extraData, double dlTotal, double dlNow, double ulTotal, double ulNow);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate int _CurlDebugCallback(
            IntPtr ptrCurl, CurlInfoType infoType, string message, int size, IntPtr ptrUserData);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate int _CurlSslCtxCallback(IntPtr ctx, IntPtr parm);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate CurlIoError _CurlIoctlCallback(CurlIoCommand cmd, IntPtr parm);

        
        // curl_easy_setopt() overloads
        [DllImport(CURL_LIB_LINUX, EntryPoint = "curl_easy_setopt", CallingConvention = CallingConvention.Cdecl)]
        private static extern CurlCode curl_easy_setopt_linux(IntPtr pCurl, CurlOption opt, IntPtr parm);
        [DllImport(CURL_LIB_X64, EntryPoint = "curl_easy_setopt", CallingConvention = CallingConvention.Cdecl)]
        private static extern CurlCode curl_easy_setopt_x64(IntPtr pCurl, CurlOption opt, IntPtr parm);
        [DllImport(CURL_LIB_X86, EntryPoint = "curl_easy_setopt", CallingConvention = CallingConvention.Cdecl)]
        private static extern CurlCode curl_easy_setopt_x86(IntPtr pCurl, CurlOption opt, IntPtr parm);

        internal static CurlCode curl_easy_setopt(IntPtr pCurl, CurlOption opt, IntPtr parm)
        {
            CurlCode result;
            if (PlatformType == NETPlatformType.Unknown)
            {
                PlatformType = ProcessPlatformType();
            }


            switch (PlatformType)
            {
                case NETPlatformType.Mono:
                    result = curl_easy_setopt_linux(pCurl,opt,parm);
                    break;
                case NETPlatformType.WinX86:
                    result = curl_easy_setopt_x86(pCurl, opt, parm);
                    break;
                case NETPlatformType.WinX64:
                    result = curl_easy_setopt_x64(pCurl, opt, parm);
                    break;
                default:
                    throw new InvalidOperationException("Can not determine type of NET platform");

            }
            return result;
        }


        [DllImport(CURL_LIB_LINUX, EntryPoint = "curl_easy_setopt", CallingConvention = CallingConvention.Cdecl)]
        private static extern CurlCode curl_easy_setopt_linux(IntPtr pCurl, CurlOption opt, string parm);
        [DllImport(CURL_LIB_X64, EntryPoint = "curl_easy_setopt", CallingConvention = CallingConvention.Cdecl)]
        private static extern CurlCode curl_easy_setopt_x64(IntPtr pCurl, CurlOption opt, string parm);
        [DllImport(CURL_LIB_X86, EntryPoint = "curl_easy_setopt", CallingConvention = CallingConvention.Cdecl)]
        private static extern CurlCode curl_easy_setopt_x86(IntPtr pCurl, CurlOption opt, string parm);

        internal static CurlCode curl_easy_setopt(IntPtr pCurl, CurlOption opt, string parm)
        {
            CurlCode result;
            if (PlatformType == NETPlatformType.Unknown)
            {
                PlatformType = ProcessPlatformType();
            }


            switch (PlatformType)
            {
                case NETPlatformType.Mono:
                    result = curl_easy_setopt_linux(pCurl, opt, parm);
                    break;
                case NETPlatformType.WinX86:
                    result = curl_easy_setopt_x86(pCurl, opt, parm);
                    break;
                case NETPlatformType.WinX64:
                    result = curl_easy_setopt_x64(pCurl, opt, parm);
                    break;
                default:
                    throw new InvalidOperationException("Can not determine type of NET platform");

            }
            return result;
        }

        [DllImport(CURL_LIB_LINUX, EntryPoint = "curl_easy_setopt", CallingConvention = CallingConvention.Cdecl)]
        private static extern CurlCode curl_easy_setopt_linux(IntPtr pCurl, CurlOption opt, byte[] parm);
        [DllImport(CURL_LIB_X64, EntryPoint = "curl_easy_setopt", CallingConvention = CallingConvention.Cdecl)]
        private static extern CurlCode curl_easy_setopt_x64(IntPtr pCurl, CurlOption opt, byte[] parm);
        [DllImport(CURL_LIB_X86, EntryPoint = "curl_easy_setopt", CallingConvention = CallingConvention.Cdecl)]
        private static extern CurlCode curl_easy_setopt_x86(IntPtr pCurl, CurlOption opt, byte[] parm);

        internal static CurlCode curl_easy_setopt(IntPtr pCurl, CurlOption opt, byte[] parm)
        {
            CurlCode result;
            if (PlatformType == NETPlatformType.Unknown)
            {
                PlatformType = ProcessPlatformType();
            }


            switch (PlatformType)
            {
                case NETPlatformType.Mono:
                    result = curl_easy_setopt_linux(pCurl, opt, parm);
                    break;
                case NETPlatformType.WinX86:
                    result = curl_easy_setopt_x86(pCurl, opt, parm);
                    break;
                case NETPlatformType.WinX64:
                    result = curl_easy_setopt_x64(pCurl, opt, parm);
                    break;
                default:
                    throw new InvalidOperationException("Can not determine type of NET platform");

            }
            return result;
        }


        [DllImport(CURL_LIB_LINUX, EntryPoint = "curl_easy_setopt", CallingConvention = CallingConvention.Cdecl)]
        private static extern CurlCode curl_easy_setopt_linux(IntPtr pCurl, CurlOption opt, long parm);
        [DllImport(CURL_LIB_X64, EntryPoint = "curl_easy_setopt", CallingConvention = CallingConvention.Cdecl)]
        private static extern CurlCode curl_easy_setopt_x64(IntPtr pCurl, CurlOption opt, long parm);
        [DllImport(CURL_LIB_X86, EntryPoint = "curl_easy_setopt", CallingConvention = CallingConvention.Cdecl)]
        private static extern CurlCode curl_easy_setopt_x86(IntPtr pCurl, CurlOption opt, long parm);
        internal static CurlCode curl_easy_setopt(IntPtr pCurl, CurlOption opt, long parm)
        {
            CurlCode result;
            if (PlatformType == NETPlatformType.Unknown)
            {
                PlatformType = ProcessPlatformType();
            }


            switch (PlatformType)
            {
                case NETPlatformType.Mono:
                    result = curl_easy_setopt_linux(pCurl, opt, parm);
                    break;
                case NETPlatformType.WinX86:
                    result = curl_easy_setopt_x86(pCurl, opt, parm);
                    break;
                case NETPlatformType.WinX64:
                    result = curl_easy_setopt_x64(pCurl, opt, parm);
                    break;
                default:
                    throw new InvalidOperationException("Can not determine type of NET platform");

            }
            return result;
        }


        [DllImport(CURL_LIB_LINUX, EntryPoint = "curl_easy_setopt", CallingConvention = CallingConvention.Cdecl)]
        private static extern CurlCode curl_easy_setopt_linux(IntPtr pCurl, CurlOption opt, bool parm);
        [DllImport(CURL_LIB_X64, EntryPoint = "curl_easy_setopt", CallingConvention = CallingConvention.Cdecl)]
        private static extern CurlCode curl_easy_setopt_x64(IntPtr pCurl, CurlOption opt, bool parm);
        [DllImport(CURL_LIB_X86, EntryPoint = "curl_easy_setopt", CallingConvention = CallingConvention.Cdecl)]
        private static extern CurlCode curl_easy_setopt_x86(IntPtr pCurl, CurlOption opt, bool parm);
        internal static CurlCode curl_easy_setopt(IntPtr pCurl, CurlOption opt, bool parm)
        {
            CurlCode result;
            if (PlatformType == NETPlatformType.Unknown)
            {
                PlatformType = ProcessPlatformType();
            }


            switch (PlatformType)
            {
                case NETPlatformType.Mono:
                    result = curl_easy_setopt_linux(pCurl, opt, parm);
                    break;
                case NETPlatformType.WinX86:
                    result = curl_easy_setopt_x86(pCurl, opt, parm);
                    break;
                case NETPlatformType.WinX64:
                    result = curl_easy_setopt_x64(pCurl, opt, parm);
                    break;
                default:
                    throw new InvalidOperationException("Can not determine type of NET platform");

            }
            return result;
        }



        [DllImport(CURL_LIB_LINUX, EntryPoint = "curl_easy_setopt", CallingConvention = CallingConvention.Cdecl)]
        private static extern CurlCode curl_easy_setopt_cb_linux(IntPtr pCurl, CurlOption opt, _CurlGenericCallback parm);
        [DllImport(CURL_LIB_X64, EntryPoint = "curl_easy_setopt", CallingConvention = CallingConvention.Cdecl)]
        private static extern CurlCode curl_easy_setopt_cb_x64(IntPtr pCurl, CurlOption opt, _CurlGenericCallback parm);
        [DllImport(CURL_LIB_X86, EntryPoint = "curl_easy_setopt", CallingConvention = CallingConvention.Cdecl)]
        private static extern CurlCode curl_easy_setopt_cb_x86(IntPtr pCurl, CurlOption opt, _CurlGenericCallback parm);
        internal static CurlCode curl_easy_setopt_cb(IntPtr pCurl, CurlOption opt, _CurlGenericCallback parm)
        {
            CurlCode result;
            if (PlatformType == NETPlatformType.Unknown)
            {
                PlatformType = ProcessPlatformType();
            }


            switch (PlatformType)
            {
                case NETPlatformType.Mono:
                    result = curl_easy_setopt_cb_linux(pCurl, opt, parm);
                    break;
                case NETPlatformType.WinX86:
                    result = curl_easy_setopt_cb_x86(pCurl, opt, parm);
                    break;
                case NETPlatformType.WinX64:
                    result = curl_easy_setopt_cb_x64(pCurl, opt, parm);
                    break;
                default:
                    throw new InvalidOperationException("Can not determine type of NET platform");

            }
            return result;
        }

        [DllImport(CURL_LIB_LINUX, EntryPoint = "curl_easy_setopt", CallingConvention = CallingConvention.Cdecl)]
        private static extern CurlCode curl_easy_setopt_cb_linux(IntPtr pCurl, CurlOption opt, _CurlProgressCallback parm);
        [DllImport(CURL_LIB_X64, EntryPoint = "curl_easy_setopt", CallingConvention = CallingConvention.Cdecl)]
        private static extern CurlCode curl_easy_setopt_cb_x64(IntPtr pCurl, CurlOption opt, _CurlProgressCallback parm);
        [DllImport(CURL_LIB_X86, EntryPoint = "curl_easy_setopt", CallingConvention = CallingConvention.Cdecl)]
        private static extern CurlCode curl_easy_setopt_cb_x86(IntPtr pCurl, CurlOption opt, _CurlProgressCallback parm);
        internal static CurlCode curl_easy_setopt_cb(IntPtr pCurl, CurlOption opt, _CurlProgressCallback parm)
        {
            CurlCode result;
            if (PlatformType == NETPlatformType.Unknown)
            {
                PlatformType = ProcessPlatformType();
            }


            switch (PlatformType)
            {
                case NETPlatformType.Mono:
                    result = curl_easy_setopt_cb_linux(pCurl, opt, parm);
                    break;
                case NETPlatformType.WinX86:
                    result = curl_easy_setopt_cb_x86(pCurl, opt, parm);
                    break;
                case NETPlatformType.WinX64:
                    result = curl_easy_setopt_cb_x64(pCurl, opt, parm);
                    break;
                default:
                    throw new InvalidOperationException("Can not determine type of NET platform");

            }
            return result;
        }

        [DllImport(CURL_LIB_LINUX, EntryPoint = "curl_easy_setopt", CallingConvention = CallingConvention.Cdecl)]
        private static extern CurlCode curl_easy_setopt_cb_linux(IntPtr pCurl, CurlOption opt, _CurlDebugCallback parm);
        [DllImport(CURL_LIB_X64, EntryPoint = "curl_easy_setopt", CallingConvention = CallingConvention.Cdecl)]
        private static extern CurlCode curl_easy_setopt_cb_x64(IntPtr pCurl, CurlOption opt, _CurlDebugCallback parm);
        [DllImport(CURL_LIB_X86, EntryPoint = "curl_easy_setopt", CallingConvention = CallingConvention.Cdecl)]
        private static extern CurlCode curl_easy_setopt_cb_x86(IntPtr pCurl, CurlOption opt, _CurlDebugCallback parm);
        internal static CurlCode curl_easy_setopt_cb(IntPtr pCurl, CurlOption opt, _CurlDebugCallback parm)
        {
            CurlCode result;
            if (PlatformType == NETPlatformType.Unknown)
            {
                PlatformType = ProcessPlatformType();
            }


            switch (PlatformType)
            {
                case NETPlatformType.Mono:
                    result = curl_easy_setopt_cb_linux(pCurl, opt, parm);
                    break;
                case NETPlatformType.WinX86:
                    result = curl_easy_setopt_cb_x86(pCurl, opt, parm);
                    break;
                case NETPlatformType.WinX64:
                    result = curl_easy_setopt_cb_x64(pCurl, opt, parm);
                    break;
                default:
                    throw new InvalidOperationException("Can not determine type of NET platform");

            }
            return result;
        }

        [DllImport(CURL_LIB_LINUX, EntryPoint = "curl_easy_setopt", CallingConvention = CallingConvention.Cdecl)]
        private static extern CurlCode curl_easy_setopt_cb_linux(IntPtr pCurl, CurlOption opt, _CurlSslCtxCallback parm);
        [DllImport(CURL_LIB_X64, EntryPoint = "curl_easy_setopt", CallingConvention = CallingConvention.Cdecl)]
        private static extern CurlCode curl_easy_setopt_cb_x64(IntPtr pCurl, CurlOption opt, _CurlSslCtxCallback parm);
        [DllImport(CURL_LIB_X86, EntryPoint = "curl_easy_setopt", CallingConvention = CallingConvention.Cdecl)]
        private static extern CurlCode curl_easy_setopt_cb_x86(IntPtr pCurl, CurlOption opt, _CurlSslCtxCallback parm);
        internal static CurlCode curl_easy_setopt_cb(IntPtr pCurl, CurlOption opt, _CurlSslCtxCallback parm)
        {
            CurlCode result;
            if (PlatformType == NETPlatformType.Unknown)
            {
                PlatformType = ProcessPlatformType();
            }


            switch (PlatformType)
            {
                case NETPlatformType.Mono:
                    result = curl_easy_setopt_cb_linux(pCurl, opt, parm);
                    break;
                case NETPlatformType.WinX86:
                    result = curl_easy_setopt_cb_x86(pCurl, opt, parm);
                    break;
                case NETPlatformType.WinX64:
                    result = curl_easy_setopt_cb_x64(pCurl, opt, parm);
                    break;
                default:
                    throw new InvalidOperationException("Can not determine type of NET platform");

            }
            return result;
        }

        [DllImport(CURL_LIB_LINUX, EntryPoint = "curl_easy_setopt", CallingConvention = CallingConvention.Cdecl)]
        private static extern CurlCode curl_easy_setopt_cb_linux(IntPtr pCurl, CurlOption opt, _CurlIoctlCallback parm);
        [DllImport(CURL_LIB_X64, EntryPoint = "curl_easy_setopt", CallingConvention = CallingConvention.Cdecl)]
        private static extern CurlCode curl_easy_setopt_cb_x64(IntPtr pCurl, CurlOption opt, _CurlIoctlCallback parm);
        [DllImport(CURL_LIB_X86, EntryPoint = "curl_easy_setopt", CallingConvention = CallingConvention.Cdecl)]
        private static extern CurlCode curl_easy_setopt_cb_x86(IntPtr pCurl, CurlOption opt, _CurlIoctlCallback parm);
        internal static CurlCode curl_easy_setopt_cb(IntPtr pCurl, CurlOption opt, _CurlIoctlCallback parm)
        {
            CurlCode result;
            if (PlatformType == NETPlatformType.Unknown)
            {
                PlatformType = ProcessPlatformType();
            }


            switch (PlatformType)
            {
                case NETPlatformType.Mono:
                    result = curl_easy_setopt_cb_linux(pCurl, opt, parm);
                    break;
                case NETPlatformType.WinX86:
                    result = curl_easy_setopt_cb_x86(pCurl, opt, parm);
                    break;
                case NETPlatformType.WinX64:
                    result = curl_easy_setopt_cb_x64(pCurl, opt, parm);
                    break;
                default:
                    throw new InvalidOperationException("Can not determine type of NET platform");

            }
            return result;
        }

      
#if !USE_LIBCURLSHIM
        [DllImport(CURL_LIB_LINUX, EntryPoint = "curl_multi_fdset", CallingConvention = CallingConvention.Cdecl)]
        private static extern CurlMultiCode curl_multi_fdset_linux(IntPtr pmulti,
                                                              [In, Out] ref fd_set read_fd_set,
                                                              [In, Out] ref fd_set write_fd_set,
                                                              [In, Out] ref fd_set exc_fd_set,
                                                              [In, Out] ref int max_fd);
        [DllImport(CURL_LIB_X64, EntryPoint = "curl_multi_fdset", CallingConvention = CallingConvention.Cdecl)]
        private static extern CurlMultiCode curl_multi_fdset_x64(IntPtr pmulti,
                                                              [In, Out] ref fd_set read_fd_set,
                                                              [In, Out] ref fd_set write_fd_set,
                                                              [In, Out] ref fd_set exc_fd_set,
                                                              [In, Out] ref int max_fd);

        [DllImport(CURL_LIB_X86, EntryPoint = "curl_multi_fdset", CallingConvention = CallingConvention.Cdecl)]
        private static extern CurlMultiCode curl_multi_fdset_x86(IntPtr pmulti,
                                                              [In, Out] ref fd_set read_fd_set,
                                                              [In, Out] ref fd_set write_fd_set,
                                                              [In, Out] ref fd_set exc_fd_set,
                                                              [In, Out] ref int max_fd);

        internal static CurlMultiCode curl_multi_fdset(IntPtr pmulti,
                                                              [In, Out] ref fd_set read_fd_set,
                                                              [In, Out] ref fd_set write_fd_set,
                                                              [In, Out] ref fd_set exc_fd_set,
                                                              [In, Out] ref int max_fd)
        {
            CurlMultiCode result;
            if (PlatformType == NETPlatformType.Unknown)
            {
                PlatformType = ProcessPlatformType();
            }


            switch (PlatformType)
            {
                case NETPlatformType.Mono:
                    result = curl_multi_fdset_linux(pmulti, ref read_fd_set, 
                        ref write_fd_set, ref exc_fd_set,ref  max_fd);
                    break;
                case NETPlatformType.WinX86:
                    result = curl_multi_fdset_x86(pmulti, ref read_fd_set,
                        ref write_fd_set, ref exc_fd_set, ref  max_fd);
                    break;
                case NETPlatformType.WinX64:
                    result = curl_multi_fdset_x64(pmulti, ref read_fd_set,
                        ref write_fd_set, ref exc_fd_set, ref  max_fd);
                    break;
                default:
                    throw new InvalidOperationException("Can not determine type of NET platform");

            }
            return result;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct fd_set
        {
            internal uint fd_count;
            //[MarshalAs(UnmanagedType.ByValArray, SizeConst = FD_SETSIZE)] internal IntPtr[] fd_array;
            internal fixed uint fd_array[FD_SETSIZE];

            internal const int FD_SETSIZE = 64;

            internal void Cleanup()
            {
                //fd_array = null;
            }

            internal static fd_set Create()
            {
                return new fd_set
                       {
                           //fd_array = new IntPtr[FD_SETSIZE],
                           fd_count = 0
                       };
            }

            internal static fd_set Create(IntPtr socket)
            {
                var handle = Create();
                handle.fd_count = 1;
                handle.fd_array[0] = (uint)socket;
                return handle;
            }
        }

        internal static void FD_ZERO(fd_set fds)
        {
            for (var i = 0; i < fd_set.FD_SETSIZE; i++)
            {
                //fds.fd_array[i] = (IntPtr) 0;
                fds.fd_array[i] = 0;
            }
            fds.fd_count = 0;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct timeval
        {
            /// <summary>
            ///     Time interval, in seconds.
            /// </summary>
            internal int tv_sec;

            /// <summary>
            ///     Time interval, in microseconds.
            /// </summary>
            internal int tv_usec;

            internal static timeval Create(int milliseconds)
            {
                return new timeval
                       {
                           tv_sec = milliseconds / 1000,
                           tv_usec = (milliseconds % 1000) * 1000
                       };
            }
        };

        [DllImport(WINSOCK_LIB_LINUX, EntryPoint = "select")]
        private static extern int select_linux(
            int nfds, // number of sockets, (ignored in winsock)
            [In, Out] ref fd_set readfds, // read sockets to watch
            [In, Out] ref fd_set writefds, // write sockets to watch
            [In, Out] ref fd_set exceptfds, // error sockets to watch
            ref timeval timeout);



        [DllImport(WINSOCK_LIB_X86, EntryPoint = "select")]
        private static extern int select_x86(
            int nfds, // number of sockets, (ignored in winsock)
            [In, Out] ref fd_set readfds, // read sockets to watch
            [In, Out] ref fd_set writefds, // write sockets to watch
            [In, Out] ref fd_set exceptfds, // error sockets to watch
            ref timeval timeout);


        internal static int select(
            int nfds, // number of sockets, (ignored in winsock)
            [In, Out] ref fd_set readfds, // read sockets to watch
            [In, Out] ref fd_set writefds, // write sockets to watch
            [In, Out] ref fd_set exceptfds, // error sockets to watch
            ref timeval timeout)
        {
            int result;
            if (PlatformType == NETPlatformType.Unknown)
            {
                PlatformType = ProcessPlatformType();
            }


            switch (PlatformType)
            {
                case NETPlatformType.Mono:
                    result = select_linux(
                                            nfds, // number of sockets, (ignored in winsock)
                                            ref readfds, // read sockets to watch
                                            ref writefds, // write sockets to watch
                                            ref exceptfds, // error sockets to watch
                                            ref timeout);
                    break;
                case NETPlatformType.WinX86:
                    result = select_x86(
                                        nfds, // number of sockets, (ignored in winsock)
                                        ref readfds, // read sockets to watch
                                        ref writefds, // write sockets to watch
                                        ref exceptfds, // error sockets to watch
                                        ref timeout);
                  break;
               
               /* case NETPlatformType.WinX64:
                    result = curl_multi_fdset_x64(pmulti, ref read_fd_set,
                        ref write_fd_set, ref exc_fd_set, ref  max_fd);
                    break;
                */
                default:
                    throw new InvalidOperationException("Can not determine type of NET platform");

            }
            return result;
        }
        //[DllImport(WINSOCK_LIB, EntryPoint = "select")]
        //internal static int select(int ndfs, fd_set* readfds, fd_set* writefds, fd_set* exceptfds, timeval* timeout);
#endif
        
        [DllImport(CURL_LIB_LINUX, EntryPoint = "curl_easy_perform", CallingConvention = CallingConvention.Cdecl)]
        private static extern CurlCode curl_easy_perform_linux(IntPtr pCurl);
        [DllImport(CURL_LIB_X64, EntryPoint = "curl_easy_perform", CallingConvention = CallingConvention.Cdecl)]
        private static extern CurlCode curl_easy_perform_x64(IntPtr pCurl);
        [DllImport(CURL_LIB_X86, EntryPoint = "curl_easy_perform", CallingConvention = CallingConvention.Cdecl)]
        private static extern CurlCode curl_easy_perform_x86(IntPtr pCurl);

        internal static CurlCode curl_easy_perform(IntPtr pCurl)
        {
            CurlCode result;
            if (PlatformType == NETPlatformType.Unknown)
            {
                PlatformType = ProcessPlatformType();
            }


            switch (PlatformType)
            {
                case NETPlatformType.Mono:
                    result = curl_easy_perform_linux(pCurl);
                    break;
                case NETPlatformType.WinX86:
                    result = curl_easy_perform_x86(pCurl);
                    break;
                case NETPlatformType.WinX64:
                    result = curl_easy_perform_x64(pCurl);
                    break;
                default:
                    throw new InvalidOperationException("Can not determine type of NET platform");

            }
            return result;
        }



        [DllImport(CURL_LIB_LINUX, EntryPoint = "curl_easy_duphandle", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr curl_easy_duphandle_linux(IntPtr pCurl);
        [DllImport(CURL_LIB_X64, EntryPoint = "curl_easy_duphandle", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr curl_easy_duphandle_x64(IntPtr pCurl);
        [DllImport(CURL_LIB_X86, EntryPoint = "curl_easy_duphandle", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr curl_easy_duphandle_x86(IntPtr pCurl);
        internal static IntPtr curl_easy_duphandle(IntPtr pCurl)
        {
            IntPtr result;
            if (PlatformType == NETPlatformType.Unknown)
            {
                PlatformType = ProcessPlatformType();
            }


            switch (PlatformType)
            {
                case NETPlatformType.Mono:
                    result = curl_easy_duphandle_linux(pCurl);
                    break;
                case NETPlatformType.WinX86:
                    result = curl_easy_duphandle_x86(pCurl);
                    break;
                case NETPlatformType.WinX64:
                    result = curl_easy_duphandle_x64(pCurl);
                    break;
                default:
                    throw new InvalidOperationException("Can not determine type of NET platform");

            }
            return result;
        }

        [DllImport(CURL_LIB_LINUX, EntryPoint = "curl_easy_strerror", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr curl_easy_strerror_linux(CurlCode err);
        [DllImport(CURL_LIB_X64, EntryPoint = "curl_easy_strerror", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr curl_easy_strerror_x64(CurlCode err);
        [DllImport(CURL_LIB_X86, EntryPoint = "curl_easy_strerror", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr curl_easy_strerror_x86(CurlCode err);

        internal static IntPtr curl_easy_strerror(CurlCode err)
        {
            IntPtr result;
            if (PlatformType == NETPlatformType.Unknown)
            {
                PlatformType = ProcessPlatformType();
            }


            switch (PlatformType)
            {
                case NETPlatformType.Mono:
                    result = curl_easy_strerror_linux(err);
                    break;
                case NETPlatformType.WinX86:
                    result = curl_easy_strerror_x86(err);
                    break;
                case NETPlatformType.WinX64:
                    result = curl_easy_strerror_x64(err);
                    break;
                default:
                    throw new InvalidOperationException("Can not determine type of NET platform");

            }
            return result;
        }



        [DllImport(CURL_LIB_LINUX, EntryPoint = "curl_easy_getinfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern CurlCode curl_easy_getinfo_linux(IntPtr pCurl, CurlInfo info, ref IntPtr pInfo);
        [DllImport(CURL_LIB_X64, EntryPoint = "curl_easy_getinfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern CurlCode curl_easy_getinfo_x86(IntPtr pCurl, CurlInfo info, ref IntPtr pInfo);
        [DllImport(CURL_LIB_X86, EntryPoint = "curl_easy_getinfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern CurlCode curl_easy_getinfo_x64(IntPtr pCurl, CurlInfo info, ref IntPtr pInfo);

        internal static CurlCode curl_easy_getinfo(IntPtr pCurl,CurlInfo info, ref IntPtr pInfo)
        {
            CurlCode result;
            if (PlatformType == NETPlatformType.Unknown)
            {
                PlatformType = ProcessPlatformType();
            }


            switch (PlatformType)
            {
                case NETPlatformType.Mono:
                    result = curl_easy_getinfo_linux(pCurl,info, ref pInfo);
                    break;
                case NETPlatformType.WinX86:
                    result = curl_easy_getinfo_x86(pCurl,info, ref pInfo);
                    break;
                case NETPlatformType.WinX64:
                    result = curl_easy_getinfo_x64(pCurl,info, ref pInfo);
                    break;
                default:
                    throw new InvalidOperationException("Can not determine type of NET platform");

            }
            return result;
        }



        [DllImport(CURL_LIB_LINUX, EntryPoint = "curl_easy_getinfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern CurlCode curl_easy_getinfo_linux(IntPtr pCurl, CurlInfo info, ref double dblVal);
        [DllImport(CURL_LIB_X64, EntryPoint = "curl_easy_getinfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern CurlCode curl_easy_getinfo_x64(IntPtr pCurl, CurlInfo info, ref double dblVal);
        [DllImport(CURL_LIB_X86, EntryPoint = "curl_easy_getinfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern CurlCode curl_easy_getinfo_x86(IntPtr pCurl, CurlInfo info, ref double dblVal);

        internal static CurlCode curl_easy_getinfo(IntPtr pCurl, CurlInfo info, ref double pInfo)
        {
            CurlCode result;
            if (PlatformType == NETPlatformType.Unknown)
            {
                PlatformType = ProcessPlatformType();
            }


            switch (PlatformType)
            {
                case NETPlatformType.Mono:
                    result = curl_easy_getinfo_linux(pCurl, info, ref pInfo);
                    break;
                case NETPlatformType.WinX86:
                    result = curl_easy_getinfo_x86(pCurl, info, ref pInfo);
                    break;
                case NETPlatformType.WinX64:
                    result = curl_easy_getinfo_x64(pCurl, info, ref pInfo);
                    break;
                default:
                    throw new InvalidOperationException("Can not determine type of NET platform");

            }
            return result;
        }


        [DllImport(CURL_LIB_LINUX, EntryPoint = "curl_easy_reset", CallingConvention = CallingConvention.Cdecl)]
        private static extern void curl_easy_reset_linux(IntPtr pCurl);
        [DllImport(CURL_LIB_X64, EntryPoint = "curl_easy_reset", CallingConvention = CallingConvention.Cdecl)]
        private static extern void curl_easy_reset_x64(IntPtr pCurl);
        [DllImport(CURL_LIB_X86, EntryPoint = "curl_easy_reset", CallingConvention = CallingConvention.Cdecl)]
        private static extern void curl_easy_reset_x86(IntPtr pCurl);

        internal static void curl_easy_reset(IntPtr pCurl)
        {
        
            if (PlatformType == NETPlatformType.Unknown)
            {
                PlatformType = ProcessPlatformType();
            }


            switch (PlatformType)
            {
                case NETPlatformType.Mono:
                    curl_easy_reset_linux(pCurl);
                    break;
                case NETPlatformType.WinX86:
                    curl_easy_reset_x86(pCurl);
                    break;
                case NETPlatformType.WinX64:
                    curl_easy_reset_x64(pCurl);
                    break;
                default:
                    throw new InvalidOperationException("Can not determine type of NET platform");

            }
         
        }


        [DllImport(CURL_LIB_LINUX, EntryPoint = "curl_multi_init", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr curl_multi_init_linux();
        [DllImport(CURL_LIB_X64, EntryPoint = "curl_multi_init", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr curl_multi_init_x64();
        [DllImport(CURL_LIB_X86, EntryPoint = "curl_multi_init", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr curl_multi_init_x86();

        internal static IntPtr curl_multi_init()
        {
            IntPtr result;
            if (PlatformType == NETPlatformType.Unknown)
            {
                PlatformType = ProcessPlatformType();
            }


            switch (PlatformType)
            {
                case NETPlatformType.Mono:
                    result = curl_multi_init_linux();
                    break;
                case NETPlatformType.WinX86:
                    result = curl_multi_init_x86();
                    break;
                case NETPlatformType.WinX64:
                    result = curl_multi_init_x64();
                    break;
                default:
                    throw new InvalidOperationException("Can not determine type of NET platform");

            }
            return result;
        }


        [DllImport(CURL_LIB_LINUX, EntryPoint = "curl_multi_cleanup", CallingConvention = CallingConvention.Cdecl)]
        private static extern CurlMultiCode curl_multi_cleanup_linux(IntPtr pmulti);
        [DllImport(CURL_LIB_X64, EntryPoint = "curl_multi_cleanup", CallingConvention = CallingConvention.Cdecl)]
        private static extern CurlMultiCode curl_multi_cleanup_x86(IntPtr pmulti);
        [DllImport(CURL_LIB_X86, EntryPoint = "curl_multi_cleanup", CallingConvention = CallingConvention.Cdecl)]
        private static extern CurlMultiCode curl_multi_cleanup_x64(IntPtr pmulti);


        internal static CurlMultiCode curl_multi_cleanup(IntPtr pmulti)
        {
            CurlMultiCode result;
            if (PlatformType == NETPlatformType.Unknown)
            {
                PlatformType = ProcessPlatformType();
            }


            switch (PlatformType)
            {
                case NETPlatformType.Mono:
                    result = curl_multi_cleanup_linux(pmulti);
                    break;
                case NETPlatformType.WinX86:
                    result = curl_multi_cleanup_x86(pmulti);
                    break;
                case NETPlatformType.WinX64:
                    result = curl_multi_cleanup_x64(pmulti);
                    break;
                default:
                    throw new InvalidOperationException("Can not determine type of NET platform");

            }
            return result;
        }


        [DllImport(CURL_LIB_LINUX, EntryPoint = "curl_multi_add_handle", CallingConvention = CallingConvention.Cdecl)]
        private static extern CurlMultiCode curl_multi_add_handle_linux(IntPtr pmulti, IntPtr peasy);
        [DllImport(CURL_LIB_X64, EntryPoint = "curl_multi_add_handle", CallingConvention = CallingConvention.Cdecl)]
        private static extern CurlMultiCode curl_multi_add_handle_x64(IntPtr pmulti, IntPtr peasy);
        [DllImport(CURL_LIB_X86, EntryPoint = "curl_multi_add_handle", CallingConvention = CallingConvention.Cdecl)]
        private static extern CurlMultiCode curl_multi_add_handle_x86(IntPtr pmulti, IntPtr peasy);
        internal static CurlMultiCode curl_multi_add_handle(IntPtr pmulti, IntPtr peasy)
        {
            CurlMultiCode result;
            if (PlatformType == NETPlatformType.Unknown)
            {
                PlatformType = ProcessPlatformType();
            }


            switch (PlatformType)
            {
                case NETPlatformType.Mono:
                    result = curl_multi_add_handle_linux(pmulti, peasy);
                    break;
                case NETPlatformType.WinX86:
                    result = curl_multi_add_handle_x86(pmulti, peasy);
                    break;
                case NETPlatformType.WinX64:
                    result = curl_multi_add_handle_x64(pmulti, peasy);
                    break;
                default:
                    throw new InvalidOperationException("Can not determine type of NET platform");

            }
            return result;
        }


        [DllImport(CURL_LIB_LINUX, EntryPoint = "curl_multi_remove_handle", CallingConvention = CallingConvention.Cdecl)]
        private static extern CurlMultiCode curl_multi_remove_handle_linux(IntPtr pmulti, IntPtr peasy);
        [DllImport(CURL_LIB_X64, EntryPoint = "curl_multi_remove_handle", CallingConvention = CallingConvention.Cdecl)]
        private static extern CurlMultiCode curl_multi_remove_handle_x64(IntPtr pmulti, IntPtr peasy);
        [DllImport(CURL_LIB_X86, EntryPoint = "curl_multi_remove_handle", CallingConvention = CallingConvention.Cdecl)]
        private static extern CurlMultiCode curl_multi_remove_handle_x86(IntPtr pmulti, IntPtr peasy);
        internal static CurlMultiCode curl_multi_remove_handle(IntPtr pmulti, IntPtr peasy)
        {
            CurlMultiCode result;
            if (PlatformType == NETPlatformType.Unknown)
            {
                PlatformType = ProcessPlatformType();
            }


            switch (PlatformType)
            {
                case NETPlatformType.Mono:
                    result = curl_multi_remove_handle_linux(pmulti, peasy);
                    break;
                case NETPlatformType.WinX86:
                    result = curl_multi_remove_handle_x86(pmulti, peasy);
                    break;
                case NETPlatformType.WinX64:
                    result = curl_multi_remove_handle_x64(pmulti, peasy);
                    break;
                default:
                    throw new InvalidOperationException("Can not determine type of NET platform");

            }
            return result;
        }


        [DllImport(CURL_LIB_LINUX, EntryPoint = "curl_multi_strerror", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr curl_multi_strerror_linux(CurlMultiCode errorNum);
        [DllImport(CURL_LIB_X64, EntryPoint = "curl_multi_strerror", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr curl_multi_strerror_x86(CurlMultiCode errorNum);
        [DllImport(CURL_LIB_X86, EntryPoint = "curl_multi_strerror", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr curl_multi_strerror_x64(CurlMultiCode errorNum);

        internal static IntPtr curl_multi_strerror(CurlMultiCode errorNum)
        {
            IntPtr result;
            if (PlatformType == NETPlatformType.Unknown)
            {
                PlatformType = ProcessPlatformType();
            }


            switch (PlatformType)
            {
                case NETPlatformType.Mono:
                    result = curl_multi_strerror_linux(errorNum);
                    break;
                case NETPlatformType.WinX86:
                    result = curl_multi_strerror_x86(errorNum);
                    break;
                case NETPlatformType.WinX64:
                    result = curl_multi_strerror_x64(errorNum);
                    break;
                default:
                    throw new InvalidOperationException("Can not determine type of NET platform");

            }
            return result;
        }


        [DllImport(CURL_LIB_LINUX, EntryPoint = "curl_multi_perform", CallingConvention = CallingConvention.Cdecl)]
        private static extern CurlMultiCode curl_multi_perform_linux(IntPtr pmulti, ref int runningHandles);
        [DllImport(CURL_LIB_X64, EntryPoint = "curl_multi_perform", CallingConvention = CallingConvention.Cdecl)]
        private static extern CurlMultiCode curl_multi_perform_x86(IntPtr pmulti, ref int runningHandles);
        [DllImport(CURL_LIB_X86, EntryPoint = "curl_multi_perform", CallingConvention = CallingConvention.Cdecl)]
        private static extern CurlMultiCode curl_multi_perform_x64(IntPtr pmulti, ref int runningHandles);
        internal static CurlMultiCode curl_multi_perform(IntPtr pmulti, ref int runningHandles)
        {
            CurlMultiCode result;
            if (PlatformType == NETPlatformType.Unknown)
            {
                PlatformType = ProcessPlatformType();
            }


            switch (PlatformType)
            {
                case NETPlatformType.Mono:
                    result = curl_multi_perform_linux(pmulti, ref runningHandles);
                    break;
                case NETPlatformType.WinX86:
                    result = curl_multi_perform_x86(pmulti, ref runningHandles);
                    break;
                case NETPlatformType.WinX64:
                    result = curl_multi_perform_x64(pmulti, ref runningHandles);
                    break;
                default:
                    throw new InvalidOperationException("Can not determine type of NET platform");

            }
            return result;
        }



        [DllImport(CURL_LIB_LINUX, EntryPoint = "curl_formfree", CallingConvention = CallingConvention.Cdecl)]
        private static extern void curl_formfree_linux(IntPtr pForm);
        [DllImport(CURL_LIB_X64, EntryPoint = "curl_formfree", CallingConvention = CallingConvention.Cdecl)]
        private static extern void curl_formfree_x86(IntPtr pForm);
        [DllImport(CURL_LIB_X86, EntryPoint = "curl_formfree", CallingConvention = CallingConvention.Cdecl)]
        private static extern void curl_formfree_x64(IntPtr pForm);
        internal static void curl_formfree(IntPtr pForm)
        {
        
            if (PlatformType == NETPlatformType.Unknown)
            {
                PlatformType = ProcessPlatformType();
            }


            switch (PlatformType)
            {
                case NETPlatformType.Mono:
                    curl_formfree_linux(pForm);
                    break;
                case NETPlatformType.WinX86:
                    curl_formfree_x86(pForm);
                    break;
                case NETPlatformType.WinX64:
                    curl_formfree_x64(pForm);
                    break;
                default:
                    throw new InvalidOperationException("Can not determine type of NET platform");

            }
           
        }


     

#if !USE_LIBCURLSHIM
        [DllImport(CURL_LIB_LINUX, EntryPoint = "curl_formadd", CallingConvention = CallingConvention.Cdecl)]
        private static extern int curl_formadd_linux(ref IntPtr pHttppost, ref IntPtr pLastPost,
                                                int codeFirst, IntPtr bufFirst,
                                                int codeNext, IntPtr bufNext,
                                                int codeLast);

        [DllImport(CURL_LIB_X64, EntryPoint = "curl_formadd", CallingConvention = CallingConvention.Cdecl)]
        private static extern int curl_formadd_x64(ref IntPtr pHttppost, ref IntPtr pLastPost,
                                                int codeFirst, IntPtr bufFirst,
                                                int codeNext, IntPtr bufNext,
                                                int codeLast);
        [DllImport(CURL_LIB_X86, EntryPoint = "curl_formadd", CallingConvention = CallingConvention.Cdecl)]
        private static extern int curl_formadd_x86(ref IntPtr pHttppost, ref IntPtr pLastPost,
                                                int codeFirst, IntPtr bufFirst,
                                                int codeNext, IntPtr bufNext,
                                                int codeLast);
        internal static int curl_formadd(ref IntPtr pHttppost, ref IntPtr pLastPost,
                                                int codeFirst, IntPtr bufFirst,
                                                int codeNext, IntPtr bufNext,
                                                int codeLast)
        {
            int result;
            if (PlatformType == NETPlatformType.Unknown)
            {
                PlatformType = ProcessPlatformType();
            }


            switch (PlatformType)
            {
                case NETPlatformType.Mono:
                    result = curl_formadd_linux(ref  pHttppost, ref  pLastPost,
                                                codeFirst,  bufFirst,
                                                 codeNext,  bufNext,
                                                codeLast);
                    break;
                case NETPlatformType.WinX86:
                    result = curl_formadd_x86(ref  pHttppost, ref  pLastPost,
                                                codeFirst,  bufFirst,
                                                 codeNext,  bufNext,
                                                codeLast);
                    break;
                case NETPlatformType.WinX64:
                    result = curl_formadd_x64(ref  pHttppost, ref  pLastPost,
                                   codeFirst, bufFirst,
                                    codeNext, bufNext,
                                   codeLast);
                    break;
                default:
                    throw new InvalidOperationException("Can not determine type of NET platform");

            }
            return result;
        }

#endif

        
        [DllImport(CURL_LIB_LINUX, EntryPoint = "curl_share_init", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr curl_share_init_linux();
        [DllImport(CURL_LIB_X64, EntryPoint = "curl_share_init", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr curl_share_init_x64();
        [DllImport(CURL_LIB_X86, EntryPoint = "curl_share_init", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr curl_share_init_x86();
        internal static IntPtr curl_share_init()
        {
            IntPtr result;
            if (PlatformType == NETPlatformType.Unknown)
            {
                PlatformType = ProcessPlatformType();
            }


            switch (PlatformType)
            {
                case NETPlatformType.Mono:
                    result = curl_share_init_linux();
                    break;
                case NETPlatformType.WinX86:

                    result = curl_share_init_x86();              
                    break;
                case NETPlatformType.WinX64:
                    result = curl_share_init_x64();
                    break;
                default:
                    throw new InvalidOperationException("Can not determine type of NET platform");

            }
            return result;
        }


        [DllImport(CURL_LIB_LINUX, EntryPoint = "curl_share_cleanup", CallingConvention = CallingConvention.Cdecl)]
        private static extern CurlShareCode curl_share_cleanup_linux(IntPtr pShare);
        [DllImport(CURL_LIB_X64, EntryPoint = "curl_share_cleanup", CallingConvention = CallingConvention.Cdecl)]
        private static extern CurlShareCode curl_share_cleanup_x64(IntPtr pShare);
        [DllImport(CURL_LIB_X86, EntryPoint = "curl_share_cleanup", CallingConvention = CallingConvention.Cdecl)]
        private static extern CurlShareCode curl_share_cleanup_x86(IntPtr pShare);
        internal static CurlShareCode curl_share_cleanup(IntPtr pShare)
        {
            CurlShareCode result;
            if (PlatformType == NETPlatformType.Unknown)
            {
                PlatformType = ProcessPlatformType();
            }


            switch (PlatformType)
            {
                case NETPlatformType.Mono:
                    result = curl_share_cleanup_linux(pShare);
                    break;
                case NETPlatformType.WinX86:

                    result = curl_share_cleanup_x86(pShare);
                    break;
                case NETPlatformType.WinX64:
                    result = curl_share_cleanup_x64(pShare);
                    break;
                default:
                    throw new InvalidOperationException("Can not determine type of NET platform");

            }
            return result;
        }


        [DllImport(CURL_LIB_LINUX, EntryPoint = "curl_share_strerror", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr curl_share_strerror_linux(CurlShareCode errorCode);
        [DllImport(CURL_LIB_X64, EntryPoint = "curl_share_strerror", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr curl_share_strerror_x86(CurlShareCode errorCode);
        [DllImport(CURL_LIB_X86, EntryPoint = "curl_share_strerror", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr curl_share_strerror_x64(CurlShareCode errorCode);

        internal static IntPtr curl_share_strerror(CurlShareCode errorCode)
        {
            IntPtr result;
            if (PlatformType == NETPlatformType.Unknown)
            {
                PlatformType = ProcessPlatformType();
            }


            switch (PlatformType)
            {
                case NETPlatformType.Mono:
                    result = curl_share_strerror_linux(errorCode);
                    break;
                case NETPlatformType.WinX86:

                    result = curl_share_strerror_x86(errorCode);
                    break;
                case NETPlatformType.WinX64:
                    result = curl_share_strerror(errorCode);
                    break;
                default:
                    throw new InvalidOperationException("Can not determine type of NET platform");

            }
            return result;
        }

        [DllImport(CURL_LIB_LINUX, EntryPoint = "curl_share_setopt", CallingConvention = CallingConvention.Cdecl)]
        private static extern CurlShareCode curl_share_setopt_linux(IntPtr pShare, CurlShareOption optCode, IntPtr option);
        [DllImport(CURL_LIB_X64, EntryPoint = "curl_share_setopt", CallingConvention = CallingConvention.Cdecl)]
        private static extern CurlShareCode curl_share_setopt_x64(IntPtr pShare, CurlShareOption optCode, IntPtr option);
        [DllImport(CURL_LIB_X86, EntryPoint = "curl_share_setopt", CallingConvention = CallingConvention.Cdecl)]
        private static extern CurlShareCode curl_share_setopt_x86(IntPtr pShare, CurlShareOption optCode, IntPtr option);
        internal static CurlShareCode curl_share_setopt(IntPtr pShare,CurlShareOption optCode, IntPtr option)
        {
            CurlShareCode result;
            if (PlatformType == NETPlatformType.Unknown)
            {
                PlatformType = ProcessPlatformType();
            }


            switch (PlatformType)
            {
                case NETPlatformType.Mono:
                    result = curl_share_setopt_linux(pShare, optCode, option);
                    break;
                case NETPlatformType.WinX86:

                    result = curl_share_setopt_x86(pShare, optCode, option);
                    break;
                case NETPlatformType.WinX64:
                    result = curl_share_setopt_x64(pShare, optCode, option);
                    break;
                default:
                    throw new InvalidOperationException("Can not determine type of NET platform");

            }
            return result;
        }

        [DllImport(CURL_LIB_LINUX, EntryPoint = "curl_slist_append", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr curl_slist_append_linux(IntPtr slist, string data);
        [DllImport(CURL_LIB_X64, EntryPoint = "curl_slist_append", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr curl_slist_append_x64(IntPtr slist, string data);
        [DllImport(CURL_LIB_X86, EntryPoint = "curl_slist_append", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr curl_slist_append_x86(IntPtr slist, string data);

        internal static IntPtr curl_slist_append(IntPtr slist, string data)
        {
            IntPtr result;
            if (PlatformType == NETPlatformType.Unknown)
            {
                PlatformType = ProcessPlatformType();
            }


            switch (PlatformType)
            {
                case NETPlatformType.Mono:
                    result = curl_slist_append_linux(slist,data);
                    break;
                case NETPlatformType.WinX86:

                    result = curl_slist_append_x86(slist, data);
                    break;
                case NETPlatformType.WinX64:
                    result = curl_slist_append_x64(slist, data);
                    break;
                default:
                    throw new InvalidOperationException("Can not determine type of NET platform");

            }
            return result;
        }


        [DllImport(CURL_LIB_LINUX, EntryPoint = "curl_slist_free_all", CallingConvention = CallingConvention.Cdecl)]
        private static extern CurlShareCode curl_slist_free_all_linux(IntPtr pList);
        [DllImport(CURL_LIB_X64, EntryPoint = "curl_slist_free_all", CallingConvention = CallingConvention.Cdecl)]
        private static extern CurlShareCode curl_slist_free_all_x86(IntPtr pList);
        [DllImport(CURL_LIB_X86, EntryPoint = "curl_slist_free_all", CallingConvention = CallingConvention.Cdecl)]
        private static extern CurlShareCode curl_slist_free_all_x64(IntPtr pList);
        internal static CurlShareCode curl_slist_free_all(IntPtr pList)
        {
            CurlShareCode result;
            if (PlatformType == NETPlatformType.Unknown)
            {
                PlatformType = ProcessPlatformType();
            }


            switch (PlatformType)
            {
                case NETPlatformType.Mono:
                    result =  curl_slist_free_all_linux(pList);
                    break;
                case NETPlatformType.WinX86:

                    result = curl_slist_free_all_x86(pList);
                    break;
                case NETPlatformType.WinX64:
                    result = curl_slist_free_all_x64(pList);
                    break;
                default:
                    throw new InvalidOperationException("Can not determine type of NET platform");

            }
            return result;
        }


        [DllImport(CURL_LIB_LINUX, EntryPoint = "curl_version_info", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr curl_version_info_linux(CurlVersion ver);
        [DllImport(CURL_LIB_X64, EntryPoint = "curl_version_info", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr curl_version_info_x86(CurlVersion ver);
        [DllImport(CURL_LIB_X86, EntryPoint = "curl_version_info", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr curl_version_info_x64(CurlVersion ver);
        
        internal static IntPtr curl_version_info(CurlVersion ver)
        {
            IntPtr result;
            if (PlatformType == NETPlatformType.Unknown)
            {
                PlatformType = ProcessPlatformType();
            }


            switch (PlatformType)
            {
                case NETPlatformType.Mono:
                    result = curl_version_info_linux(ver);
                    break;
                case NETPlatformType.WinX86:

                    result = curl_version_info_x86(ver);
                    break;
                case NETPlatformType.WinX64:
                    result = curl_version_info_x64(ver);
                    break;
                default:
                    throw new InvalidOperationException("Can not determine type of NET platform");

            }
            return result;
        }



     
#if  USE_LIBCURLSHIM

    // libcurlshim imports
        [DllImport(CURLSHIM_LIB_X64, EntryPoint = "curl_shim_initialize", CallingConvention = CallingConvention.Cdecl)]
        private static extern void curl_shim_initialize_x64();
        [DllImport(CURLSHIM_LIB_X86,EntryPoint = "curl_shim_initialize", CallingConvention = CallingConvention.Cdecl)]
        private static extern void curl_shim_initialize_x86();
        internal static void curl_shim_initialize()
        {
           
            if (PlatformType == NETPlatformType.Unknown)
            {
                PlatformType = ProcessPlatformType();
            }


            switch (PlatformType)
            {
                case NETPlatformType.WinX86:
                    curl_shim_initialize_x86();
                    break;
                case NETPlatformType.WinX64:
                    curl_shim_initialize_x64();
                    break;
                default:
                    throw new InvalidOperationException("Can not run on other platform than Win NET");

            }
        }


        [DllImport(CURLSHIM_LIB_X64,EntryPoint = "curl_shim_cleanup", CallingConvention = CallingConvention.Cdecl)]
        private static extern void curl_shim_cleanup_x64();
        [DllImport(CURLSHIM_LIB_X86,EntryPoint = "curl_shim_cleanup", CallingConvention = CallingConvention.Cdecl)]
        private static extern void curl_shim_cleanup_x86();
        internal static void curl_shim_cleanup()
        {

            if (PlatformType == NETPlatformType.Unknown)
            {
                PlatformType = ProcessPlatformType();
            }


            switch (PlatformType)
            {
                case NETPlatformType.WinX86:
                    curl_shim_cleanup_x86();
                    break;
                case NETPlatformType.WinX64:
                    curl_shim_cleanup_x64();
                    break;
                default:
                    throw new InvalidOperationException("Can not run on other platform than Win NET");

            }
        }

    

        [DllImport(CURLSHIM_LIB_X64,EntryPoint = "curl_shim_alloc_strings", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr curl_shim_alloc_strings_x64();
        [DllImport(CURLSHIM_LIB_X86, EntryPoint = "curl_shim_alloc_strings", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr curl_shim_alloc_strings_x86();

        internal static IntPtr curl_shim_alloc_strings()
        {

            if (PlatformType == NETPlatformType.Unknown)
            {
                PlatformType = ProcessPlatformType();
            }


            switch (PlatformType)
            {
                case NETPlatformType.WinX86:
                    return curl_shim_alloc_strings_x86();
                case NETPlatformType.WinX64:
                    return curl_shim_alloc_strings_x64();
                default:
                    throw new InvalidOperationException("Can not run on other platform than Win NET");

            }
        }


        [DllImport(CURLSHIM_LIB_X64, EntryPoint = "curl_shim_add_string_to_slist", CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi)]
        private static extern IntPtr curl_shim_add_string_to_slist_x64(
            IntPtr pStrings, String str);
        [DllImport(CURLSHIM_LIB_X86, CallingConvention = CallingConvention.Cdecl,
          CharSet = CharSet.Ansi)]
        private static extern IntPtr curl_shim_add_string_to_slist_x86(
            IntPtr pStrings, String str);

        internal static IntPtr curl_shim_add_string_to_slist(
            IntPtr pStrings, String str)
        {
            IntPtr result;
            if (PlatformType == NETPlatformType.Unknown)
            {
                PlatformType = ProcessPlatformType();
            }


            switch (PlatformType)
            {
               
                case NETPlatformType.WinX86:

                    result = curl_shim_add_string_to_slist_x86(pStrings, str);
                    break;
                case NETPlatformType.WinX64:
                    result = curl_shim_add_string_to_slist_x64(pStrings, str);
                    break;
                default:
                    throw new InvalidOperationException("Can not run on other platform than Win NET");

            }
            return result;
        }


        [DllImport(CURLSHIM_LIB_X64, EntryPoint = "curl_shim_get_string_from_slist", CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi)]
        private static extern IntPtr curl_shim_get_string_from_slist_x64(
            IntPtr pSlist, ref IntPtr pStr);
        [DllImport(CURLSHIM_LIB_X86, EntryPoint = "curl_shim_get_string_from_slist", CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi)]
        private static extern IntPtr curl_shim_get_string_from_slist_x86(
            IntPtr pSlist, ref IntPtr pStr);

        internal static IntPtr curl_shim_get_string_from_slist(
              IntPtr pSlist, ref IntPtr pStr)
        {
            IntPtr result;
            if (PlatformType == NETPlatformType.Unknown)
            {
                PlatformType = ProcessPlatformType();
            }


            switch (PlatformType)
            {

                case NETPlatformType.WinX86:

                    result = curl_shim_get_string_from_slist_x86(pSlist, ref pStr);
                    break;
                case NETPlatformType.WinX64:
                    result = curl_shim_get_string_from_slist_x64(pSlist, ref pStr);
                    break;
                default:
                    throw new InvalidOperationException("Can not run on other platform than Win NET");

            }
            return result;
        }


        [DllImport(CURLSHIM_LIB_X64, EntryPoint = "curl_shim_add_string", CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi)]
        private static extern IntPtr curl_shim_add_string_x64(IntPtr pStrings, String str);

        [DllImport(CURLSHIM_LIB_X86, EntryPoint = "curl_shim_add_string", CallingConvention = CallingConvention.Cdecl,
          CharSet = CharSet.Ansi)]
        private static extern IntPtr curl_shim_add_string_x86(IntPtr pStrings, String str);


        internal static IntPtr  curl_shim_add_string(IntPtr pStrings, String str)
        {
            IntPtr result;
            if (PlatformType == NETPlatformType.Unknown)
            {
                PlatformType = ProcessPlatformType();
            }


            switch (PlatformType)
            {

                case NETPlatformType.WinX86:

                    result = curl_shim_add_string_x86(pStrings,str);
                    break;
                case NETPlatformType.WinX64:
                    result = curl_shim_add_string_x64(pStrings, str);
                    break;
                default:
                    throw new InvalidOperationException("Can not run on other platform than Win NET");

            }
            return result;
        }



        [DllImport(CURLSHIM_LIB_X64, EntryPoint = "curl_shim_free_strings", CallingConvention = CallingConvention.Cdecl)]
        private static extern void curl_shim_free_strings_x64(IntPtr pStrings);
        [DllImport(CURLSHIM_LIB_X86, EntryPoint = "curl_shim_free_strings", CallingConvention = CallingConvention.Cdecl)]
        private static extern void curl_shim_free_strings_x86(IntPtr pStrings);

        internal static void curl_shim_free_strings(IntPtr pStrings)
        {
            if (PlatformType == NETPlatformType.Unknown)
            {
                PlatformType = ProcessPlatformType();
            }


            switch (PlatformType)
            {

                case NETPlatformType.WinX86:

                    curl_shim_free_strings_x86(pStrings);
                    break;
                case NETPlatformType.WinX64:
                    curl_shim_free_strings_x64(pStrings);
                    break;
                default:
                    throw new InvalidOperationException("Can not run on other platform than Win NET");

            }
     
        }





        [DllImport(CURLSHIM_LIB_X64, EntryPoint = "curl_shim_install_delegates", CallingConvention = CallingConvention.Cdecl)]
        private static extern int curl_shim_install_delegates_x64(IntPtr pCurl, IntPtr pThis,
            _ShimWriteCallback pWrite, _ShimReadCallback pRead,
            _ShimProgressCallback pProgress, _ShimDebugCallback pDebug,
            _ShimHeaderCallback pHeader, _ShimSslCtxCallback pCtx,
            _ShimIoctlCallback pIoctl);
        [DllImport(CURLSHIM_LIB_X86, EntryPoint = "curl_shim_install_delegates", CallingConvention = CallingConvention.Cdecl)]
        private static extern int curl_shim_install_delegates_x86(IntPtr pCurl, IntPtr pThis,
            _ShimWriteCallback pWrite, _ShimReadCallback pRead,
            _ShimProgressCallback pProgress, _ShimDebugCallback pDebug,
            _ShimHeaderCallback pHeader, _ShimSslCtxCallback pCtx,
            _ShimIoctlCallback pIoctl);



        internal static int  curl_shim_install_delegates(IntPtr pCurl, IntPtr pThis,
            _ShimWriteCallback pWrite, _ShimReadCallback pRead,
            _ShimProgressCallback pProgress, _ShimDebugCallback pDebug,
            _ShimHeaderCallback pHeader, _ShimSslCtxCallback pCtx,
            _ShimIoctlCallback pIoctl)
        {
            int result;
            if (PlatformType == NETPlatformType.Unknown)
            {
                PlatformType = ProcessPlatformType();
            }


            switch (PlatformType)
            {

                case NETPlatformType.WinX86:

                    result = curl_shim_install_delegates_x86(pCurl, pThis,
             pWrite,pRead,
             pProgress, pDebug,
             pHeader, pCtx,
            pIoctl);
                    break;
                case NETPlatformType.WinX64:
                    result = curl_shim_install_delegates_x64(pCurl, pThis,
             pWrite, pRead,
             pProgress, pDebug,
             pHeader, pCtx,
            pIoctl);
                    break;
                default:
                    throw new InvalidOperationException("Can not run on other platform than Win NET");

            }
            return result;
        }

        [DllImport(CURLSHIM_LIB_X64,EntryPoint = "curl_shim_cleanup_delegates", CallingConvention = CallingConvention.Cdecl)]
        private static extern void curl_shim_cleanup_delegates_x64(IntPtr pThis);
        [DllImport(CURLSHIM_LIB_X86,EntryPoint = "curl_shim_cleanup_delegates", CallingConvention = CallingConvention.Cdecl)]
        private static extern void curl_shim_cleanup_delegates_x86(IntPtr pThis);

        internal static void curl_shim_cleanup_delegates(IntPtr pThis)
        {
            if (PlatformType == NETPlatformType.Unknown)
            {
                PlatformType = ProcessPlatformType();
            }


            switch (PlatformType)
            {

                case NETPlatformType.WinX86:

                curl_shim_cleanup_delegates_x86(pThis);
                    break;
                case NETPlatformType.WinX64:
                    curl_shim_cleanup_delegates_x64(pThis);
                    break;
                default:
                    throw new InvalidOperationException("Can not run on other platform than Win NET");

            }
        }

        [DllImport(CURLSHIM_LIB_X64, EntryPoint = "curl_shim_get_file_time", CallingConvention = CallingConvention.Cdecl)]
        private static extern void curl_shim_get_file_time_x64(int unixTime,
            ref int yy, ref int mm, ref int dd, ref int hh, ref int mn, ref int ss);
        [DllImport(CURLSHIM_LIB_X86, EntryPoint = "curl_shim_get_file_time", CallingConvention = CallingConvention.Cdecl)]
        private static extern void curl_shim_get_file_time_x86(int unixTime,
            ref int yy, ref int mm, ref int dd, ref int hh, ref int mn, ref int ss);

        internal static void curl_shim_get_file_time(int unixTime,
            ref int yy, ref int mm, ref int dd, ref int hh, ref int mn, ref int ss)
        {
         
            if (PlatformType == NETPlatformType.Unknown)
            {
                PlatformType = ProcessPlatformType();
            }


            switch (PlatformType)
            {

                case NETPlatformType.WinX86:

                     curl_shim_get_file_time_x86(unixTime,
            ref  yy, ref mm, ref dd, ref hh, ref mn, ref ss);
                    break;
                case NETPlatformType.WinX64:
                    curl_shim_get_file_time_x64(unixTime,
               ref  yy, ref mm, ref dd, ref hh, ref mn, ref ss);
                    break;
                default:
                    throw new InvalidOperationException("Can not run on other platform than Win NET");

            }
        }



        [DllImport(CURLSHIM_LIB_X64, EntryPoint = "curl_shim_get_file_time", CallingConvention = CallingConvention.Cdecl)]
        private static extern void curl_shim_free_slist_x64(IntPtr p);
        [DllImport(CURLSHIM_LIB_X86, EntryPoint = "curl_shim_get_file_time",CallingConvention = CallingConvention.Cdecl)]
        private static extern void curl_shim_free_slist_x86(IntPtr p);
        internal static void curl_shim_free_slist(IntPtr p)
        {

            if (PlatformType == NETPlatformType.Unknown)
            {
                PlatformType = ProcessPlatformType();
            }


            switch (PlatformType)
            {

                case NETPlatformType.WinX86:

                    curl_shim_free_slist_x86(p);
                    break;
                case NETPlatformType.WinX64:
                    curl_shim_free_slist_x64(p);
                    break;
                default:
                    throw new InvalidOperationException("Can not run on other platform than Win NET");

            }
        }


        [DllImport(CURLSHIM_LIB_X64, EntryPoint = "curl_shim_alloc_fd_sets", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr curl_shim_alloc_fd_sets_x64();
        [DllImport(CURLSHIM_LIB_X86,  EntryPoint = "curl_shim_alloc_fd_sets",CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr curl_shim_alloc_fd_sets_x86();

        internal static IntPtr curl_shim_alloc_fd_sets()
        {
            IntPtr result;
            if (PlatformType == NETPlatformType.Unknown)
            {
                PlatformType = ProcessPlatformType();
            }


            switch (PlatformType)
            {

                case NETPlatformType.WinX86:

                    result = curl_shim_alloc_fd_sets_x86();
                    break;
                case NETPlatformType.WinX64:
                    result = curl_shim_alloc_fd_sets_x64();
                    break;
                default:
                    throw new InvalidOperationException("Can not run on other platform than Win NET");

            }
            return result;
        }


        [DllImport(CURLSHIM_LIB_X64, EntryPoint = "curl_shim_free_fd_sets", CallingConvention = CallingConvention.Cdecl)]
        private static extern void curl_shim_free_fd_sets_x64(IntPtr fdsets);
        [DllImport(CURLSHIM_LIB_X86, EntryPoint = "curl_shim_free_fd_sets", CallingConvention = CallingConvention.Cdecl)]
        private static extern void curl_shim_free_fd_sets_x86(IntPtr fdsets);
        internal static void  curl_shim_free_fd_sets(IntPtr fdsets)
        {

            if (PlatformType == NETPlatformType.Unknown)
            {
                PlatformType = ProcessPlatformType();
            }


            switch (PlatformType)
            {

                case NETPlatformType.WinX86:

                    curl_shim_free_fd_sets_x86(fdsets);
                    break;
                case NETPlatformType.WinX64:
                    curl_shim_free_fd_sets_x64(fdsets);
                    break;
                default:
                    throw new InvalidOperationException("Can not run on other platform than Win NET");

            }
        }


        [DllImport(CURLSHIM_LIB_X64, EntryPoint = "curl_shim_multi_fdset", CallingConvention = CallingConvention.Cdecl)]
        private static extern CurlMultiCode curl_shim_multi_fdset_x64(IntPtr multi,
            IntPtr fdsets, ref int maxFD);
        [DllImport(CURLSHIM_LIB_X86,  EntryPoint = "curl_shim_multi_fdset",CallingConvention = CallingConvention.Cdecl)]
        private static extern CurlMultiCode curl_shim_multi_fdset_x86(IntPtr multi,
            IntPtr fdsets, ref int maxFD);



        internal static CurlMultiCode curl_shim_multi_fdset(IntPtr multi,
            IntPtr fdsets, ref int maxFD)
        {
            CurlMultiCode result;
            if (PlatformType == NETPlatformType.Unknown)
            {
                PlatformType = ProcessPlatformType();
            }


            switch (PlatformType)
            {

                case NETPlatformType.WinX86:

                    result = curl_shim_multi_fdset_x86(multi,
                        fdsets, ref  maxFD);
                    break;
                case NETPlatformType.WinX64:
                    result = curl_shim_multi_fdset_x64(multi,
                        fdsets, ref  maxFD);
          
                    break;
                default:
                    throw new InvalidOperationException("Can not run on other platform than Win NET");

            }
            return result;
        }


        [DllImport(CURLSHIM_LIB_X64, EntryPoint = "curl_shim_select", CallingConvention = CallingConvention.Cdecl)]
        private static extern int curl_shim_select_x64(int maxFD, IntPtr fdsets,
            int milliseconds);
        [DllImport(CURLSHIM_LIB_X86,EntryPoint = "curl_shim_select", CallingConvention = CallingConvention.Cdecl)]
        private static extern int curl_shim_select_x86(int maxFD, IntPtr fdsets,
            int milliseconds);

        internal static int curl_shim_select(int maxFD, IntPtr fdsets,
            int milliseconds)

        {
            int result;
            if (PlatformType == NETPlatformType.Unknown)
            {
                PlatformType = ProcessPlatformType();
            }


            switch (PlatformType)
            {

                case NETPlatformType.WinX86:

                    result = curl_shim_select_x86(maxFD, fdsets,
            milliseconds);
                    break;
                case NETPlatformType.WinX64:
                     result = curl_shim_select_x64(maxFD, fdsets,
                milliseconds);

                    break;
                default:
                    throw new InvalidOperationException("Can not run on other platform than Win NET");

            }
            return result;
        }


        [DllImport(CURLSHIM_LIB_X64, EntryPoint = "curl_shim_multi_info_read", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr curl_shim_multi_info_read_x64(IntPtr multi,
            ref int nMsgs);
        [DllImport(CURLSHIM_LIB_X86,EntryPoint = "curl_shim_multi_info_read", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr curl_shim_multi_info_read_x86(IntPtr multi,
            ref int nMsgs);

        internal static IntPtr curl_shim_multi_info_read(IntPtr multi,
            ref int nMsgs)

        {
            IntPtr result;
            if (PlatformType == NETPlatformType.Unknown)
            {
                PlatformType = ProcessPlatformType();
            }


            switch (PlatformType)
            {

                case NETPlatformType.WinX86:

                    result = curl_shim_multi_info_read_x86(multi, ref nMsgs);
                    break;
                case NETPlatformType.WinX64:
                    result = curl_shim_multi_info_read_x64(multi, ref nMsgs);

                    break;
                default:
                    throw new InvalidOperationException("Can not run on other platform than Win NET");

            }
            return result;
        }



        [DllImport(CURLSHIM_LIB_X64, EntryPoint = "curl_shim_multi_info_free", CallingConvention = CallingConvention.Cdecl)]
        private static extern void curl_shim_multi_info_free_x64(IntPtr multiInfo);
        [DllImport(CURLSHIM_LIB_X86, EntryPoint = "curl_shim_multi_info_free", CallingConvention = CallingConvention.Cdecl)]
        private static extern void curl_shim_multi_info_free_x86(IntPtr multiInfo);

        internal static void curl_shim_multi_info_free(IntPtr multiInfo)
        {

            if (PlatformType == NETPlatformType.Unknown)
            {
                PlatformType = ProcessPlatformType();
            }


            switch (PlatformType)
            {

                case NETPlatformType.WinX86:

                    curl_shim_multi_info_free_x86(multiInfo);
                    break;
                case NETPlatformType.WinX64:
                    curl_shim_multi_info_free_x64(multiInfo);
                    break;
                default:
                    throw new InvalidOperationException("Can not run on other platform than Win NET");

            }
        }


        [DllImport(CURLSHIM_LIB_X64, EntryPoint = "curl_shim_formadd", CallingConvention = CallingConvention.Cdecl)]
        private static extern int curl_shim_formadd_x64(IntPtr[] ppForms, IntPtr[] pParams, int nParams);
        [DllImport(CURLSHIM_LIB_X86, EntryPoint = "curl_shim_formadd", CallingConvention = CallingConvention.Cdecl)]
        private static extern int curl_shim_formadd_x86(IntPtr[] ppForms, IntPtr[] pParams, int nParams);

        internal static int curl_shim_formadd(IntPtr[] ppForms, IntPtr[] pParams, int nParams) 
        {
            int result;
            if (PlatformType == NETPlatformType.Unknown)
            {
                PlatformType = ProcessPlatformType();
            }


            switch (PlatformType)
            {

                case NETPlatformType.WinX86:

                    result = curl_shim_formadd_x86( ppForms, pParams,nParams); 
                    break;
                case NETPlatformType.WinX64:
                    result = curl_shim_formadd_x64(ppForms, pParams, nParams); 

                    break;
                default:
                    throw new InvalidOperationException("Can not run on other platform than Win NET");

            }
            return result;
        }


        [DllImport(CURLSHIM_LIB_X64, EntryPoint = "curl_shim_install_share_delegates", CallingConvention = CallingConvention.Cdecl)]
        private static extern int curl_shim_install_share_delegates_x64(IntPtr pShare,
            IntPtr pThis, _ShimLockCallback pLock, _ShimUnlockCallback pUnlock);
        [DllImport(CURLSHIM_LIB_X86, EntryPoint = "curl_shim_install_share_delegates",CallingConvention = CallingConvention.Cdecl)]
        private static extern int curl_shim_install_share_delegates_x86(IntPtr pShare,
            IntPtr pThis, _ShimLockCallback pLock, _ShimUnlockCallback pUnlock);

        internal static int curl_shim_install_share_delegates(IntPtr pShare,
            IntPtr pThis, _ShimLockCallback pLock, _ShimUnlockCallback pUnlock)
        {
            int result;
            if (PlatformType == NETPlatformType.Unknown)
            {
                PlatformType = ProcessPlatformType();
            }


            switch (PlatformType)
            {

                case NETPlatformType.WinX86:

                    result = curl_shim_install_share_delegates_x86(pShare,
             pThis,pLock, pUnlock);
                    break;
                case NETPlatformType.WinX64:
                    result = curl_shim_install_share_delegates_x64(pShare,
              pThis, pLock, pUnlock);

                    break;
                default:
                    throw new InvalidOperationException("Can not run on other platform than Win NET");

            }
            return result;
        }

        [DllImport(CURLSHIM_LIB_X64, EntryPoint = "url_shim_cleanup_share_delegates", CallingConvention = CallingConvention.Cdecl)]
        private static extern void curl_shim_cleanup_share_delegates_x64(IntPtr pShare);
        [DllImport(CURLSHIM_LIB_X86, EntryPoint = "url_shim_cleanup_share_delegates",CallingConvention = CallingConvention.Cdecl)]
        private static extern void curl_shim_cleanup_share_delegates_x86(IntPtr pShare);

        internal static void curl_shim_cleanup_share_delegates(IntPtr pShare)
        {

            if (PlatformType == NETPlatformType.Unknown)
            {
                PlatformType = ProcessPlatformType();
            }


            switch (PlatformType)
            {

                case NETPlatformType.WinX86:

                    curl_shim_cleanup_share_delegates_x86(pShare);
                    break;
                case NETPlatformType.WinX64:
                    curl_shim_cleanup_share_delegates_x86(pShare);
                    break;
                default:
                    throw new InvalidOperationException("Can not run on other platform than Win NET");

            }
        }

        [DllImport(CURLSHIM_LIB_X64, EntryPoint = "curl_shim_get_version_int_value", CallingConvention = CallingConvention.Cdecl)]
        private static extern int curl_shim_get_version_int_value_x64(IntPtr p, int offset);
        [DllImport(CURLSHIM_LIB_X86, EntryPoint = "curl_shim_get_version_int_value", CallingConvention = CallingConvention.Cdecl)]
        private static extern int curl_shim_get_version_int_value_x86(IntPtr p, int offset);

        internal static int curl_shim_get_version_int_value(IntPtr p, int offset)
        {
            int result;
            if (PlatformType == NETPlatformType.Unknown)
            {
                PlatformType = ProcessPlatformType();
            }


            switch (PlatformType)
            {

                case NETPlatformType.WinX86:

                    result = curl_shim_get_version_int_value_x86(p, offset);
                    break;
                case NETPlatformType.WinX64:
                    result = curl_shim_get_version_int_value_x64(p, offset);

                    break;
                default:
                    throw new InvalidOperationException("Can not run on other platform than Win NET");

            }
            return result;
        }

        [DllImport(CURLSHIM_LIB_X64, EntryPoint = "curl_shim_get_version_char_ptr", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr curl_shim_get_version_char_ptr_x64(IntPtr p, int offset);
        [DllImport(CURLSHIM_LIB_X86,  EntryPoint = "curl_shim_get_version_char_ptr",CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr curl_shim_get_version_char_ptr_x86(IntPtr p, int offset);

        internal static IntPtr curl_shim_get_version_char_ptr(IntPtr p, int offset)
        {
            IntPtr result;
            if (PlatformType == NETPlatformType.Unknown)
            {
                PlatformType = ProcessPlatformType();
            }


            switch (PlatformType)
            {

                case NETPlatformType.WinX86:

                    result = curl_shim_get_version_char_ptr_x86(p, offset);
                    break;
                case NETPlatformType.WinX64:
                    result = curl_shim_get_version_char_ptr_x64(p, offset);

                    break;
                default:
                    throw new InvalidOperationException("Can not run on other platform than Win NET");

            }
            return result;
        }


        [DllImport(CURLSHIM_LIB_X64,EntryPoint = "curl_shim_get_number_of_protocols", CallingConvention = CallingConvention.Cdecl)]
        private static extern int curl_shim_get_number_of_protocols_x64(IntPtr p, int offset);
        [DllImport(CURLSHIM_LIB_X86, EntryPoint = "curl_shim_get_number_of_protocols",CallingConvention = CallingConvention.Cdecl)]
        private static extern int curl_shim_get_number_of_protocols_x86(IntPtr p, int offset);
        internal static int curl_shim_get_number_of_protocols(IntPtr p, int offset)
        {
            int result;
            if (PlatformType == NETPlatformType.Unknown)
            {
                PlatformType = ProcessPlatformType();
            }


            switch (PlatformType)
            {

                case NETPlatformType.WinX86:

                    result = curl_shim_get_number_of_protocols_x86(p, offset);
                    break;
                case NETPlatformType.WinX64:
                    result = curl_shim_get_number_of_protocols_x64(p, offset);

                    break;
                default:
                    throw new InvalidOperationException("Can not run on other platform than Win NET");

            }
            return result;
        }



        [DllImport(CURLSHIM_LIB_X64, EntryPoint = "curl_shim_get_protocol_string", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr curl_shim_get_protocol_string_x64(IntPtr p, int offset, int index);
        [DllImport(CURLSHIM_LIB_X86,  EntryPoint = "curl_shim_get_protocol_string" ,CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr curl_shim_get_protocol_string_x86(IntPtr p, int offset, int index);


        internal static IntPtr curl_shim_get_protocol_string(IntPtr p, int offset, int index)
        {
            IntPtr result;
            if (PlatformType == NETPlatformType.Unknown)
            {
                PlatformType = ProcessPlatformType();
            }


            switch (PlatformType)
            {

                case NETPlatformType.WinX86:

                    result = curl_shim_get_protocol_string_x86(p, offset, index);
                    break;
                case NETPlatformType.WinX64:
                    result = curl_shim_get_protocol_string_x64(p, offset, index);
                    break;
                default:
                    throw new InvalidOperationException("Can not run on other platform than Win NET");

            }
            return result;
        }

        internal delegate void _ShimLockCallback(int data, int access, IntPtr userPtr);

        internal delegate void _ShimUnlockCallback(int data, IntPtr userPtr);

        internal delegate int _ShimDebugCallback(CurlInfoType infoType, IntPtr msgBuf, int msgBufSize, IntPtr parm);

        internal delegate int _ShimHeaderCallback(IntPtr buf, int sz, int nmemb, IntPtr stream);

        internal delegate CurlIoError _ShimIoctlCallback(CurlIoCommand cmd, IntPtr parm);

        internal delegate int _ShimProgressCallback(IntPtr parm, double dlTotal, double dlNow, double ulTotal, double ulNow);

        internal delegate int _ShimReadCallback(IntPtr buf, int sz, int nmemb, IntPtr parm);

        internal delegate int _ShimSslCtxCallback(IntPtr ctx, IntPtr parm);

        internal delegate int _ShimWriteCallback(IntPtr buf, int sz, int nmemb, IntPtr parm);
#endif
    }
}