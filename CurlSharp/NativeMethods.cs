/***************************************************************************
 *
 * CurlS#arp
 *
 * Copyright (c) 2013-2017 Dr. Masroor Ehsan (masroore@gmail.com)
 * Portions copyright (c) 2004, 2005 Jeff Phillips (jeff@jeffp.net)
 * Portions copyright (c) 2017 Katelyn Gigante (https://github.com/silasary)
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
        private const string CURL_LIB_WIN = "libcurl.dll"; // should be in amd64 directory

        private const string CURL_LIB_UNIX = "libcurl";

        private const string CURLSHIM_LIB_WIN = "libcurlshim.dll";

        private const string LIBC_LINUX = "libc";

        private const string WINSOCK_LIB = "ws2_32.dll";

        private const string LIB_DIR_WIN64 = "amd64";

        private const string LIB_DIR_WIN32 = "i386";

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetDllDirectory(string lpPathName);

        internal enum NETPlatformType
        {
            Unknown,
            Unix,
            Win64,
            Win32
        }

        internal static string AssemblyDirectory
        {
            get
            {
                var codeBase = Assembly.GetEntryAssembly().CodeBase;
                var uri = new UriBuilder(codeBase);
                var path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }

        internal static NETPlatformType PlatformType { get; set; } = NETPlatformType.Unknown;

        internal static NETPlatformType ProcessPlatformType()
        {
            var dllSubFolder = string.Empty;
            var type = GetPlatformType();

            if (type == NETPlatformType.Unknown)
                throw new InvalidOperationException("Can not determine type of NET platform");

            switch (type)
            {
                case NETPlatformType.Win64:
                    dllSubFolder = LIB_DIR_WIN64;
                    break;
                case NETPlatformType.Win32:
                    dllSubFolder = LIB_DIR_WIN32;
                    break;
            }

            if ((type == NETPlatformType.Win32) || (type == NETPlatformType.Win64))
            {
                var path = Path.Combine(AssemblyDirectory, dllSubFolder);
                SetDllDirectory(path);
            }

            return type;
        }

        internal static NETPlatformType GetPlatformType()
        {
            var type = NETPlatformType.Unknown;
            
            if ((Type.GetType("Mono.Runtime") != null) && (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)))
                type = NETPlatformType.Unix;
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                type = NETPlatformType.Unix;
            else
                switch (IntPtr.Size)
                {
                    case 8:
                        type = NETPlatformType.Win64;
                        break;
                    case 4:
                        type = NETPlatformType.Win32;
                        break;
                }

            return type;
        }

        // internal delegates from cURL

        // libcurl imports
        [DllImport(CURL_LIB_UNIX, EntryPoint = "curl_global_init", CallingConvention = CallingConvention.Cdecl)]
        private static extern CurlCode curl_global_init_unix(int flags);

        [DllImport(CURL_LIB_WIN, EntryPoint = "curl_global_init", CallingConvention = CallingConvention.Cdecl)]
        private static extern CurlCode curl_global_init_win(int flags);

        private static void InitPlatformType()
        {
            if (PlatformType == NETPlatformType.Unknown) PlatformType = ProcessPlatformType();
        }

        internal static CurlCode curl_global_init(int flags)
        {
            InitPlatformType();

            switch (PlatformType)
            {
                case NETPlatformType.Unix:
                    return curl_global_init_unix(flags);
                default:
                    return curl_global_init_win(flags);
            }
        }

        [DllImport(CURL_LIB_UNIX, EntryPoint = "curl_global_cleanup", CallingConvention = CallingConvention.Cdecl)]
        private static extern void curl_global_cleanup_unix();

        [DllImport(CURL_LIB_WIN, EntryPoint = "curl_global_cleanup", CallingConvention = CallingConvention.Cdecl)]
        private static extern void curl_global_cleanup_win();

        internal static void curl_global_cleanup()
        {
            InitPlatformType();

            if (PlatformType == NETPlatformType.Unix)
                curl_global_cleanup_unix();
            else
                curl_global_cleanup_win();
        }

        [DllImport(CURL_LIB_UNIX, EntryPoint = "curl_escape", CallingConvention = CallingConvention.Cdecl,
             CharSet = CharSet.Ansi)]
        private static extern IntPtr curl_escape_unix(string url, int length);

        [DllImport(CURL_LIB_WIN, EntryPoint = "curl_escape", CallingConvention = CallingConvention.Cdecl,
             CharSet = CharSet.Ansi)]
        private static extern IntPtr curl_escape_win(string url, int length);

        internal static IntPtr curl_escape(string url, int length)
        {
            InitPlatformType();
            return PlatformType == NETPlatformType.Unix ? curl_escape_unix(url, length) : curl_escape_win(url, length);
        }

        [DllImport(CURL_LIB_UNIX, EntryPoint = "curl_unescape", CallingConvention = CallingConvention.Cdecl,
             CharSet = CharSet.Ansi)]
        private static extern IntPtr curl_unescape_unix(string url, int length);

        [DllImport(CURL_LIB_WIN, EntryPoint = "curl_unescape", CallingConvention = CallingConvention.Cdecl,
             CharSet = CharSet.Ansi)]
        private static extern IntPtr curl_unescape_win(string url, int length);


        internal static IntPtr curl_unescape(string url, int length)
        {
            InitPlatformType();
            return PlatformType == NETPlatformType.Unix
                ? curl_unescape_unix(url, length)
                : curl_unescape_win(url, length);
        }

        [DllImport(CURL_LIB_UNIX, EntryPoint = "curl_free", CallingConvention = CallingConvention.Cdecl)]
        private static extern void curl_free_unix(IntPtr p);

        [DllImport(CURL_LIB_WIN, EntryPoint = "curl_free", CallingConvention = CallingConvention.Cdecl)]
        private static extern void curl_free_win(IntPtr p);

        internal static void curl_free(IntPtr p)
        {
            InitPlatformType();

            if (PlatformType == NETPlatformType.Unix)
                curl_free_unix(p);
            else
                curl_free_win(p);
        }

        [DllImport(CURL_LIB_UNIX, EntryPoint = "curl_version", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr curl_version_unix();

        [DllImport(CURL_LIB_WIN, EntryPoint = "curl_version", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr curl_version_win();


        internal static IntPtr curl_version()
        {
            InitPlatformType();
            return PlatformType == NETPlatformType.Unix ? curl_version_unix() : curl_version_win();
        }

        [DllImport(CURL_LIB_UNIX, EntryPoint = "curl_easy_init", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr curl_easy_init_unix();

        [DllImport(CURL_LIB_WIN, EntryPoint = "curl_easy_init", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr curl_easy_init_win();

        internal static IntPtr curl_easy_init()
        {
            InitPlatformType();
            return PlatformType == NETPlatformType.Unix ? curl_easy_init_unix() : curl_easy_init_win();
        }

        [DllImport(CURL_LIB_UNIX, EntryPoint = "curl_easy_cleanup", CallingConvention = CallingConvention.Cdecl)]
        private static extern void curl_easy_cleanup_unix(IntPtr pCurl);

        [DllImport(CURL_LIB_WIN, EntryPoint = "curl_easy_cleanup", CallingConvention = CallingConvention.Cdecl)]
        private static extern void curl_easy_cleanup_win(IntPtr pCurl);

        internal static void curl_easy_cleanup(IntPtr pCurl)
        {
            InitPlatformType();

            if (PlatformType == NETPlatformType.Unix)
                curl_easy_cleanup_unix(pCurl);
            else
                curl_easy_cleanup_win(pCurl);
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate int _CurlGenericCallback(IntPtr ptr, int sz, int nmemb, IntPtr userdata);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate int _CurlProgressCallback(
            IntPtr extraData,
            double dlTotal,
            double dlNow,
            double ulTotal,
            double ulNow);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate int _CurlDebugCallback(
            IntPtr ptrCurl,
            CurlInfoType infoType,
            string message,
            int size,
            IntPtr ptrUserData);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate int _CurlSslCtxCallback(IntPtr ctx, IntPtr parm);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate CurlIoError _CurlIoctlCallback(CurlIoCommand cmd, IntPtr parm);

        // curl_easy_setopt() overloads
        [DllImport(CURL_LIB_UNIX, EntryPoint = "curl_easy_setopt", CallingConvention = CallingConvention.Cdecl)]
        private static extern CurlCode curl_easy_setopt_unix(IntPtr pCurl, CurlOption opt, IntPtr parm);

        [DllImport(CURL_LIB_WIN, EntryPoint = "curl_easy_setopt", CallingConvention = CallingConvention.Cdecl)]
        private static extern CurlCode curl_easy_setopt_win(IntPtr pCurl, CurlOption opt, IntPtr parm);

        internal static CurlCode curl_easy_setopt(IntPtr pCurl, CurlOption opt, IntPtr parm)
        {
            InitPlatformType();

            return PlatformType == NETPlatformType.Unix
                ? curl_easy_setopt_unix(pCurl, opt, parm)
                : curl_easy_setopt_win(pCurl, opt, parm);
        }

        [DllImport(CURL_LIB_UNIX, EntryPoint = "curl_easy_setopt", CallingConvention = CallingConvention.Cdecl)]
        private static extern CurlCode curl_easy_setopt_unix(IntPtr pCurl, CurlOption opt, string parm);

        [DllImport(CURL_LIB_WIN, EntryPoint = "curl_easy_setopt", CallingConvention = CallingConvention.Cdecl)]
        private static extern CurlCode curl_easy_setopt_win(IntPtr pCurl, CurlOption opt, string parm);

        internal static CurlCode curl_easy_setopt(IntPtr pCurl, CurlOption opt, string parm)
        {
            InitPlatformType();

            return PlatformType == NETPlatformType.Unix
                ? curl_easy_setopt_unix(pCurl, opt, parm)
                : curl_easy_setopt_win(pCurl, opt, parm);
        }

        [DllImport(CURL_LIB_UNIX, EntryPoint = "curl_easy_setopt", CallingConvention = CallingConvention.Cdecl)]
        private static extern CurlCode curl_easy_setopt_unix(IntPtr pCurl, CurlOption opt, byte[] parm);

        [DllImport(CURL_LIB_WIN, EntryPoint = "curl_easy_setopt", CallingConvention = CallingConvention.Cdecl)]
        private static extern CurlCode curl_easy_setopt_win(IntPtr pCurl, CurlOption opt, byte[] parm);

        internal static CurlCode curl_easy_setopt(IntPtr pCurl, CurlOption opt, byte[] parm)
        {
            InitPlatformType();

            return PlatformType == NETPlatformType.Unix
                ? curl_easy_setopt_unix(pCurl, opt, parm)
                : curl_easy_setopt_win(pCurl, opt, parm);
        }

        [DllImport(CURL_LIB_UNIX, EntryPoint = "curl_easy_setopt", CallingConvention = CallingConvention.Cdecl)]
        private static extern CurlCode curl_easy_setopt_unix(IntPtr pCurl, CurlOption opt, long parm);

        [DllImport(CURL_LIB_WIN, EntryPoint = "curl_easy_setopt", CallingConvention = CallingConvention.Cdecl)]
        private static extern CurlCode curl_easy_setopt_win(IntPtr pCurl, CurlOption opt, long parm);


        internal static CurlCode curl_easy_setopt(IntPtr pCurl, CurlOption opt, long parm)
        {
            InitPlatformType();

            return PlatformType == NETPlatformType.Unix
                ? curl_easy_setopt_unix(pCurl, opt, parm)
                : curl_easy_setopt_win(pCurl, opt, parm);
        }

        [DllImport(CURL_LIB_UNIX, EntryPoint = "curl_easy_setopt", CallingConvention = CallingConvention.Cdecl)]
        private static extern CurlCode curl_easy_setopt_unix(IntPtr pCurl, CurlOption opt, bool parm);

        [DllImport(CURL_LIB_WIN, EntryPoint = "curl_easy_setopt", CallingConvention = CallingConvention.Cdecl)]
        private static extern CurlCode curl_easy_setopt_win(IntPtr pCurl, CurlOption opt, bool parm);

        internal static CurlCode curl_easy_setopt(IntPtr pCurl, CurlOption opt, bool parm)
        {
            InitPlatformType();

            return PlatformType == NETPlatformType.Unix
                ? curl_easy_setopt_unix(pCurl, opt, parm)
                : curl_easy_setopt_win(pCurl, opt, parm);
        }

        [DllImport(CURL_LIB_UNIX, EntryPoint = "curl_easy_setopt", CallingConvention = CallingConvention.Cdecl)]
        private static extern CurlCode curl_easy_setopt_cb_unix(
            IntPtr pCurl,
            CurlOption opt,
            _CurlGenericCallback parm);

        [DllImport(CURL_LIB_WIN, EntryPoint = "curl_easy_setopt", CallingConvention = CallingConvention.Cdecl)]
        private static extern CurlCode curl_easy_setopt_cb_win(IntPtr pCurl, CurlOption opt, _CurlGenericCallback parm);

        internal static CurlCode curl_easy_setopt_cb(IntPtr pCurl, CurlOption opt, _CurlGenericCallback parm)
        {
            CurlCode result;
            InitPlatformType();

            switch (PlatformType)
            {
                case NETPlatformType.Unix:
                    result = curl_easy_setopt_cb_unix(pCurl, opt, parm);
                    break;
                default:
                    result = curl_easy_setopt_cb_win(pCurl, opt, parm);
                    break;
            }

            return result;
        }

        [DllImport(CURL_LIB_UNIX, EntryPoint = "curl_easy_setopt", CallingConvention = CallingConvention.Cdecl)]
        private static extern CurlCode curl_easy_setopt_cb_unix(
            IntPtr pCurl,
            CurlOption opt,
            _CurlProgressCallback parm);

        [DllImport(CURL_LIB_WIN, EntryPoint = "curl_easy_setopt", CallingConvention = CallingConvention.Cdecl)]
        private static extern CurlCode curl_easy_setopt_cb_win(IntPtr pCurl, CurlOption opt, _CurlProgressCallback parm);

        internal static CurlCode curl_easy_setopt_cb(IntPtr pCurl, CurlOption opt, _CurlProgressCallback parm)
        {
            CurlCode result;
            InitPlatformType();

            switch (PlatformType)
            {
                case NETPlatformType.Unix:
                    result = curl_easy_setopt_cb_unix(pCurl, opt, parm);
                    break;

                default:
                    result = curl_easy_setopt_cb_win(pCurl, opt, parm);
                    break;
            }

            return result;
        }

        [DllImport(CURL_LIB_UNIX, EntryPoint = "curl_easy_setopt", CallingConvention = CallingConvention.Cdecl)]
        private static extern CurlCode curl_easy_setopt_cb_unix(IntPtr pCurl, CurlOption opt, _CurlDebugCallback parm);

        [DllImport(CURL_LIB_WIN, EntryPoint = "curl_easy_setopt", CallingConvention = CallingConvention.Cdecl)]
        private static extern CurlCode curl_easy_setopt_cb_win(IntPtr pCurl, CurlOption opt, _CurlDebugCallback parm);

        internal static CurlCode curl_easy_setopt_cb(IntPtr pCurl, CurlOption opt, _CurlDebugCallback parm)
        {
            CurlCode result;
            InitPlatformType();

            switch (PlatformType)
            {
                case NETPlatformType.Unix:
                    result = curl_easy_setopt_cb_unix(pCurl, opt, parm);
                    break;
                default:
                    result = curl_easy_setopt_cb_win(pCurl, opt, parm);
                    break;
            }

            return result;
        }

        [DllImport(CURL_LIB_UNIX, EntryPoint = "curl_easy_setopt", CallingConvention = CallingConvention.Cdecl)]
        private static extern CurlCode curl_easy_setopt_cb_unix(IntPtr pCurl, CurlOption opt, _CurlSslCtxCallback parm);

        [DllImport(CURL_LIB_WIN, EntryPoint = "curl_easy_setopt", CallingConvention = CallingConvention.Cdecl)]
        private static extern CurlCode curl_easy_setopt_cb_win(IntPtr pCurl, CurlOption opt, _CurlSslCtxCallback parm);

        internal static CurlCode curl_easy_setopt_cb(IntPtr pCurl, CurlOption opt, _CurlSslCtxCallback parm)
        {
            CurlCode result;
            InitPlatformType();

            switch (PlatformType)
            {
                case NETPlatformType.Unix:
                    result = curl_easy_setopt_cb_unix(pCurl, opt, parm);
                    break;
                default:
                    result = curl_easy_setopt_cb_win(pCurl, opt, parm);
                    break;
            }

            return result;
        }

        [DllImport(CURL_LIB_UNIX, EntryPoint = "curl_easy_setopt", CallingConvention = CallingConvention.Cdecl)]
        private static extern CurlCode curl_easy_setopt_cb_unix(IntPtr pCurl, CurlOption opt, _CurlIoctlCallback parm);

        [DllImport(CURL_LIB_WIN, EntryPoint = "curl_easy_setopt", CallingConvention = CallingConvention.Cdecl)]
        private static extern CurlCode curl_easy_setopt_cb_win(IntPtr pCurl, CurlOption opt, _CurlIoctlCallback parm);

        internal static CurlCode curl_easy_setopt_cb(IntPtr pCurl, CurlOption opt, _CurlIoctlCallback parm)
        {
            CurlCode result;
            InitPlatformType();

            switch (PlatformType)
            {
                case NETPlatformType.Unix:
                    result = curl_easy_setopt_cb_unix(pCurl, opt, parm);
                    break;
                default:
                    result = curl_easy_setopt_cb_win(pCurl, opt, parm);
                    break;
            }

            return result;
        }

#if !USE_LIBCURLSHIM
        [DllImport(CURL_LIB_UNIX, EntryPoint = "curl_multi_fdset", CallingConvention = CallingConvention.Cdecl)]
        private static extern CurlMultiCode curl_multi_fdset_unix(IntPtr pmulti,
            [In, Out] ref fd_set read_fd_set,
            [In, Out] ref fd_set write_fd_set,
            [In, Out] ref fd_set exc_fd_set,
            [In, Out] ref int max_fd);

        [DllImport(CURL_LIB_WIN, EntryPoint = "curl_multi_fdset", CallingConvention = CallingConvention.Cdecl)]
        private static extern CurlMultiCode curl_multi_fdset_win(IntPtr pmulti,
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
            InitPlatformType();


            switch (PlatformType)
            {
                case NETPlatformType.Unix:
                    result = curl_multi_fdset_unix(pmulti, ref read_fd_set,
                        ref write_fd_set, ref exc_fd_set, ref max_fd);
                    break;
                default:
                    result = curl_multi_fdset_win(pmulti, ref read_fd_set,
                        ref write_fd_set, ref exc_fd_set, ref max_fd);
                    break;
            }
            return result;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal unsafe struct fd_set
        {
            internal uint fd_count;

            // [MarshalAs(UnmanagedType.ByValArray, SizeConst = FD_SETSIZE)] internal IntPtr[] fd_array;
            internal fixed uint fd_array [FD_SETSIZE];

            internal const int FD_SETSIZE = 64;

            internal void Cleanup()
            {
                // fd_array = null;
            }

            internal static fd_set Create()
            {
                return new fd_set
                {
                    // fd_array = new IntPtr[FD_SETSIZE],
                    fd_count = 0
                };
            }

            internal static fd_set Create(IntPtr socket)
            {
                var handle = Create();
                handle.fd_count = 1;
                handle.fd_array[0] = (uint) socket;
                return handle;
            }
        }

        internal static unsafe void FD_ZERO(fd_set fds)
        {
            for (var i = 0; i < fd_set.FD_SETSIZE; i++)
                fds.fd_array[i] = 0;
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
                    tv_sec = milliseconds/1000,
                    tv_usec = milliseconds%1000*1000
                };
            }
        }

        [DllImport(LIBC_LINUX, EntryPoint = "select")]
        private static extern int select_unix(
            int nfds, // number of sockets, (ignored in winsock)
            [In, Out] ref fd_set readfds, // read sockets to watch
            [In, Out] ref fd_set writefds, // write sockets to watch
            [In, Out] ref fd_set exceptfds, // error sockets to watch
            ref timeval timeout);


        [DllImport(WINSOCK_LIB, EntryPoint = "select")]
        private static extern int select_win(
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
            InitPlatformType();

            switch (PlatformType)
            {
                case NETPlatformType.Unix:
                    result = select_unix(
                        nfds, // number of sockets, (ignored in winsock)
                        ref readfds, // read sockets to watch
                        ref writefds, // write sockets to watch
                        ref exceptfds, // error sockets to watch
                        ref timeout);
                    break;
                default:
                    result = select_win(
                        nfds, // number of sockets, (ignored in winsock)
                        ref readfds, // read sockets to watch
                        ref writefds, // write sockets to watch
                        ref exceptfds, // error sockets to watch
                        ref timeout);
                    break;
            }
            return result;
        }


        // [DllImport(WINSOCK_LIB, EntryPoint = "select")]
        // internal static int select(int ndfs, fd_set* readfds, fd_set* writefds, fd_set* exceptfds, timeval* timeout);
#endif

        [DllImport(CURL_LIB_UNIX, EntryPoint = "curl_easy_perform", CallingConvention = CallingConvention.Cdecl)]
        private static extern CurlCode curl_easy_perform_unix(IntPtr pCurl);

        [DllImport(CURL_LIB_WIN, EntryPoint = "curl_easy_perform", CallingConvention = CallingConvention.Cdecl)]
        private static extern CurlCode curl_easy_perform_win(IntPtr pCurl);

        internal static CurlCode curl_easy_perform(IntPtr pCurl)
        {
            InitPlatformType();

            return PlatformType == NETPlatformType.Unix ? curl_easy_perform_unix(pCurl) : curl_easy_perform_win(pCurl);
        }

        [DllImport(CURL_LIB_UNIX, EntryPoint = "curl_easy_duphandle", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr curl_easy_duphandle_unix(IntPtr pCurl);

        [DllImport(CURL_LIB_WIN, EntryPoint = "curl_easy_duphandle", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr curl_easy_duphandle_win(IntPtr pCurl);

        internal static IntPtr curl_easy_duphandle(IntPtr pCurl)
        {
            InitPlatformType();

            return PlatformType == NETPlatformType.Unix
                ? curl_easy_duphandle_unix(pCurl)
                : curl_easy_duphandle_win(pCurl);
        }

        [DllImport(CURL_LIB_UNIX, EntryPoint = "curl_easy_strerror", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr curl_easy_strerror_unix(CurlCode err);

        [DllImport(CURL_LIB_WIN, EntryPoint = "curl_easy_strerror", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr curl_easy_strerror_win(CurlCode err);

        internal static IntPtr curl_easy_strerror(CurlCode err)
        {
            InitPlatformType();

            return PlatformType == NETPlatformType.Unix ? curl_easy_strerror_unix(err) : curl_easy_strerror_win(err);
        }

        [DllImport(CURL_LIB_UNIX, EntryPoint = "curl_easy_getinfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern CurlCode curl_easy_getinfo_unix(IntPtr pCurl, CurlInfo info, ref IntPtr pInfo);

        [DllImport(CURL_LIB_WIN, EntryPoint = "curl_easy_getinfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern CurlCode curl_easy_getinfo_win(IntPtr pCurl, CurlInfo info, ref IntPtr pInfo);

        internal static CurlCode curl_easy_getinfo(IntPtr pCurl, CurlInfo info, ref IntPtr pInfo)
        {
            InitPlatformType();

            return PlatformType == NETPlatformType.Unix
                ? curl_easy_getinfo_unix(pCurl, info, ref pInfo)
                : curl_easy_getinfo_win(pCurl, info, ref pInfo);
        }

        [DllImport(CURL_LIB_UNIX, EntryPoint = "curl_easy_getinfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern CurlCode curl_easy_getinfo_unix(IntPtr pCurl, CurlInfo info, ref double dblVal);

        [DllImport(CURL_LIB_WIN, EntryPoint = "curl_easy_getinfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern CurlCode curl_easy_getinfo_win(IntPtr pCurl, CurlInfo info, ref double dblVal);

        internal static CurlCode curl_easy_getinfo(IntPtr pCurl, CurlInfo info, ref double pInfo)
        {
            CurlCode result;
            InitPlatformType();

            switch (PlatformType)
            {
                case NETPlatformType.Unix:
                    result = curl_easy_getinfo_unix(pCurl, info, ref pInfo);
                    break;
                default:
                    result = curl_easy_getinfo_win(pCurl, info, ref pInfo);
                    break;
            }

            return result;
        }

        [DllImport(CURL_LIB_UNIX, EntryPoint = "curl_easy_reset", CallingConvention = CallingConvention.Cdecl)]
        private static extern void curl_easy_reset_unix(IntPtr pCurl);

        [DllImport(CURL_LIB_WIN, EntryPoint = "curl_easy_reset", CallingConvention = CallingConvention.Cdecl)]
        private static extern void curl_easy_reset_win(IntPtr pCurl);

        internal static void curl_easy_reset(IntPtr pCurl)
        {
            InitPlatformType();

            if (PlatformType == NETPlatformType.Unix)
                curl_easy_reset_unix(pCurl);
            else
                curl_easy_reset_win(pCurl);
        }

        [DllImport(CURL_LIB_UNIX, EntryPoint = "curl_multi_init", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr curl_multi_init_unix();

        [DllImport(CURL_LIB_WIN, EntryPoint = "curl_multi_init", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr curl_multi_init_win();

        internal static IntPtr curl_multi_init()
        {
            InitPlatformType();

            return PlatformType == NETPlatformType.Unix ? curl_multi_init_unix() : curl_multi_init_win();
        }

        [DllImport(CURL_LIB_UNIX, EntryPoint = "curl_multi_cleanup", CallingConvention = CallingConvention.Cdecl)]
        private static extern CurlMultiCode curl_multi_cleanup_unix(IntPtr pmulti);

        [DllImport(CURL_LIB_WIN, EntryPoint = "curl_multi_cleanup", CallingConvention = CallingConvention.Cdecl)]
        private static extern CurlMultiCode curl_multi_cleanup_win(IntPtr pmulti);

        internal static CurlMultiCode curl_multi_cleanup(IntPtr pmulti)
        {
            InitPlatformType();

            return PlatformType == NETPlatformType.Unix
                ? curl_multi_cleanup_unix(pmulti)
                : curl_multi_cleanup_win(pmulti);
        }

        [DllImport(CURL_LIB_UNIX, EntryPoint = "curl_multi_add_handle", CallingConvention = CallingConvention.Cdecl)]
        private static extern CurlMultiCode curl_multi_add_handle_unix(IntPtr pmulti, IntPtr peasy);

        [DllImport(CURL_LIB_WIN, EntryPoint = "curl_multi_add_handle", CallingConvention = CallingConvention.Cdecl)]
        private static extern CurlMultiCode curl_multi_add_handle_win(IntPtr pmulti, IntPtr peasy);

        internal static CurlMultiCode curl_multi_add_handle(IntPtr pmulti, IntPtr peasy)
        {
            InitPlatformType();

            return PlatformType == NETPlatformType.Unix
                ? curl_multi_add_handle_unix(pmulti, peasy)
                : curl_multi_add_handle_win(pmulti, peasy);
        }

        [DllImport(CURL_LIB_UNIX, EntryPoint = "curl_multi_remove_handle", CallingConvention = CallingConvention.Cdecl)
        ]
        private static extern CurlMultiCode curl_multi_remove_handle_unix(IntPtr pmulti, IntPtr peasy);

        [DllImport(CURL_LIB_WIN, EntryPoint = "curl_multi_remove_handle", CallingConvention = CallingConvention.Cdecl)]
        private static extern CurlMultiCode curl_multi_remove_handle_win(IntPtr pmulti, IntPtr peasy);

        internal static CurlMultiCode curl_multi_remove_handle(IntPtr pmulti, IntPtr peasy)
        {
            InitPlatformType();

            return PlatformType == NETPlatformType.Unix
                ? curl_multi_remove_handle_unix(pmulti, peasy)
                : curl_multi_remove_handle_win(pmulti, peasy);
        }

        [DllImport(CURL_LIB_UNIX, EntryPoint = "curl_multi_setopt", CallingConvention = CallingConvention.Cdecl)
        ]
        private static extern CurlMultiCode curl_multi_setopt_unix(IntPtr pmulti, CurlMultiOption opt, bool parm);

        [DllImport(CURL_LIB_WIN, EntryPoint = "curl_multi_setopt", CallingConvention = CallingConvention.Cdecl)]
        private static extern CurlMultiCode curl_multi_setopt_win(IntPtr pmulti, CurlMultiOption opt, bool parm);

        internal static CurlMultiCode curl_multi_setopt(IntPtr pmulti, CurlMultiOption opt, bool parm)
        {
            InitPlatformType();

            return PlatformType == NETPlatformType.Unix
                ? curl_multi_setopt_unix(pmulti, opt, parm)
                : curl_multi_setopt_win(pmulti, opt, parm);
        }

        [DllImport(CURL_LIB_UNIX, EntryPoint = "curl_multi_setopt", CallingConvention = CallingConvention.Cdecl)
        ]
        private static extern CurlMultiCode curl_multi_setopt_unix(IntPtr pmulti, CurlMultiOption opt, long parm);

        [DllImport(CURL_LIB_WIN, EntryPoint = "curl_multi_setopt", CallingConvention = CallingConvention.Cdecl)]
        private static extern CurlMultiCode curl_multi_setopt_win(IntPtr pmulti, CurlMultiOption opt, long parm);

        internal static CurlMultiCode curl_multi_setopt(IntPtr pmulti, CurlMultiOption opt, long parm)
        {
            InitPlatformType();

            return PlatformType == NETPlatformType.Unix
                ? curl_multi_setopt_unix(pmulti, opt, parm)
                : curl_multi_setopt_win(pmulti, opt, parm);
        }


        [DllImport(CURL_LIB_UNIX, EntryPoint = "curl_multi_strerror", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr curl_multi_strerror_unix(CurlMultiCode errorNum);


        [DllImport(CURL_LIB_WIN, EntryPoint = "curl_multi_strerror", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr curl_multi_strerror_win(CurlMultiCode errorNum);

        internal static IntPtr curl_multi_strerror(CurlMultiCode errorNum)
        {
            InitPlatformType();

            return PlatformType == NETPlatformType.Unix
                ? curl_multi_strerror_unix(errorNum)
                : curl_multi_strerror_win(errorNum);
        }

        [DllImport(CURL_LIB_UNIX, EntryPoint = "curl_multi_perform", CallingConvention = CallingConvention.Cdecl)]
        private static extern CurlMultiCode curl_multi_perform_unix(IntPtr pmulti, ref int runningHandles);

        [DllImport(CURL_LIB_WIN, EntryPoint = "curl_multi_perform", CallingConvention = CallingConvention.Cdecl)]
        private static extern CurlMultiCode curl_multi_perform_win(IntPtr pmulti, ref int runningHandles);

        internal static CurlMultiCode curl_multi_perform(IntPtr pmulti, ref int runningHandles)
        {
            InitPlatformType();

            return PlatformType == NETPlatformType.Unix
                ? curl_multi_perform_unix(pmulti, ref runningHandles)
                : curl_multi_perform_win(pmulti, ref runningHandles);
        }

        [DllImport(CURL_LIB_UNIX, EntryPoint = "curl_formfree", CallingConvention = CallingConvention.Cdecl)]
        private static extern void curl_formfree_unix(IntPtr pForm);

        [DllImport(CURL_LIB_WIN, EntryPoint = "curl_formfree", CallingConvention = CallingConvention.Cdecl)]
        private static extern void curl_formfree_win(IntPtr pForm);

        internal static void curl_formfree(IntPtr pForm)
        {
            InitPlatformType();

            switch (PlatformType)
            {
                case NETPlatformType.Unix:
                    curl_formfree_unix(pForm);
                    break;
                default:
                    curl_formfree_win(pForm);
                    break;
            }
        }

#if !USE_LIBCURLSHIM
        [DllImport(CURL_LIB_UNIX, EntryPoint = "curl_formadd", CallingConvention = CallingConvention.Cdecl)]
        private static extern int curl_formadd_unix(ref IntPtr pHttppost, ref IntPtr pLastPost,
            int codeFirst, IntPtr bufFirst,
            int codeNext, IntPtr bufNext,
            int codeLast);

        [DllImport(CURL_LIB_WIN, EntryPoint = "curl_formadd", CallingConvention = CallingConvention.Cdecl)]
        private static extern int curl_formadd_win(ref IntPtr pHttppost, ref IntPtr pLastPost,
            int codeFirst, IntPtr bufFirst,
            int codeNext, IntPtr bufNext,
            int codeLast);

        internal static int curl_formadd(ref IntPtr pHttppost, ref IntPtr pLastPost,
            int codeFirst, IntPtr bufFirst,
            int codeNext, IntPtr bufNext,
            int codeLast)
        {
            InitPlatformType();

            return PlatformType == NETPlatformType.Unix
                ? curl_formadd_unix(ref pHttppost, ref pLastPost,
                    codeFirst, bufFirst,
                    codeNext, bufNext,
                    codeLast)
                : curl_formadd_win(ref pHttppost, ref pLastPost,
                    codeFirst, bufFirst,
                    codeNext, bufNext,
                    codeLast);
        }

#endif

        [DllImport(CURL_LIB_UNIX, EntryPoint = "curl_share_init", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr curl_share_init_unix();

        [DllImport(CURL_LIB_WIN, EntryPoint = "curl_share_init", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr curl_share_init_win();

        internal static IntPtr curl_share_init()
        {
            InitPlatformType();

            return PlatformType == NETPlatformType.Unix ? curl_share_init_unix() : curl_share_init_win();
        }

        [DllImport(CURL_LIB_UNIX, EntryPoint = "curl_share_cleanup", CallingConvention = CallingConvention.Cdecl)]
        private static extern CurlShareCode curl_share_cleanup_unix(IntPtr pShare);

        [DllImport(CURL_LIB_WIN, EntryPoint = "curl_share_cleanup", CallingConvention = CallingConvention.Cdecl)]
        private static extern CurlShareCode curl_share_cleanup_win(IntPtr pShare);

        internal static CurlShareCode curl_share_cleanup(IntPtr pShare)
        {
            InitPlatformType();

            return PlatformType == NETPlatformType.Unix
                ? curl_share_cleanup_unix(pShare)
                : curl_share_cleanup_win(pShare);
        }

        [DllImport(CURL_LIB_UNIX, EntryPoint = "curl_share_strerror", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr curl_share_strerror_unix(CurlShareCode errorCode);

        [DllImport(CURL_LIB_WIN, EntryPoint = "curl_share_strerror", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr curl_share_strerror_win(CurlShareCode errorCode);

        internal static IntPtr curl_share_strerror(CurlShareCode errorCode)
        {
            InitPlatformType();

            return PlatformType == NETPlatformType.Unix
                ? curl_share_strerror_unix(errorCode)
                : curl_share_strerror_win(errorCode);
        }

        [DllImport(CURL_LIB_UNIX, EntryPoint = "curl_share_setopt", CallingConvention = CallingConvention.Cdecl)]
        private static extern CurlShareCode curl_share_setopt_unix(
            IntPtr pShare,
            CurlShareOption optCode,
            IntPtr option);

        [DllImport(CURL_LIB_WIN, EntryPoint = "curl_share_setopt", CallingConvention = CallingConvention.Cdecl)]
        private static extern CurlShareCode curl_share_setopt_win(IntPtr pShare, CurlShareOption optCode, IntPtr option);

        internal static CurlShareCode curl_share_setopt(IntPtr pShare, CurlShareOption optCode, IntPtr option)
        {
            InitPlatformType();

            return PlatformType == NETPlatformType.Unix
                ? curl_share_setopt_unix(pShare, optCode, option)
                : curl_share_setopt_win(pShare, optCode, option);
        }

        [DllImport(CURL_LIB_UNIX, EntryPoint = "curl_slist_append", CallingConvention = CallingConvention.Cdecl,
             CharSet = CharSet.Ansi)]
        private static extern IntPtr curl_slist_append_unix(IntPtr slist, string data);

        [DllImport(CURL_LIB_WIN, EntryPoint = "curl_slist_append", CallingConvention = CallingConvention.Cdecl,
             CharSet = CharSet.Ansi)]
        private static extern IntPtr curl_slist_append_win(IntPtr slist, string data);

        internal static IntPtr curl_slist_append(IntPtr slist, string data)
        {
            InitPlatformType();

            return PlatformType == NETPlatformType.Unix
                ? curl_slist_append_unix(slist, data)
                : curl_slist_append_win(slist, data);
        }

        [DllImport(CURL_LIB_UNIX, EntryPoint = "curl_slist_free_all", CallingConvention = CallingConvention.Cdecl)]
        private static extern void curl_slist_free_all_unix(IntPtr pList);

        [DllImport(CURL_LIB_WIN, EntryPoint = "curl_slist_free_all", CallingConvention = CallingConvention.Cdecl)]
        private static extern void curl_slist_free_all_win(IntPtr pList);

        internal static void curl_slist_free_all(IntPtr pList)
        {
            InitPlatformType();
            if (PlatformType == NETPlatformType.Unix) curl_slist_free_all_unix(pList);
            else curl_slist_free_all_win(pList);
        }

        [DllImport(CURL_LIB_UNIX, EntryPoint = "curl_version_info", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr curl_version_info_unix(CurlVersion ver);

        [DllImport(CURL_LIB_WIN, EntryPoint = "curl_version_info", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr curl_version_info_win(CurlVersion ver);

        internal static IntPtr curl_version_info(CurlVersion ver)
        {
            InitPlatformType();
            return PlatformType == NETPlatformType.Unix ? curl_version_info_unix(ver) : curl_version_info_win(ver);
        }

#if  USE_LIBCURLSHIM

        private static void ShimInitPlatform()
        {
            InitPlatformType();
            if ((PlatformType == NETPlatformType.Unknown) || (PlatformType == NETPlatformType.Unix))
                throw new InvalidOperationException("Can not run on other platform than Win NET");
        }

        // libcurlshim imports

        [DllImport(CURLSHIM_LIB_WIN, EntryPoint = "curl_shim_initialize", CallingConvention = CallingConvention.Cdecl)]
        private static extern void curl_shim_initialize_win();

        internal static void curl_shim_initialize()
        {
            ShimInitPlatform();
            curl_shim_initialize_win();
        }

        [DllImport(CURLSHIM_LIB_WIN, EntryPoint = "curl_shim_cleanup", CallingConvention = CallingConvention.Cdecl)]
        private static extern void curl_shim_cleanup_win();

        internal static void curl_shim_cleanup()
        {
            ShimInitPlatform();
            curl_shim_cleanup_win();
        }

        [DllImport(CURLSHIM_LIB_WIN, EntryPoint = "curl_shim_alloc_strings", CallingConvention = CallingConvention.Cdecl
         )]
        private static extern IntPtr curl_shim_alloc_strings_win();

        internal static IntPtr curl_shim_alloc_strings()
        {
            ShimInitPlatform();
            return curl_shim_alloc_strings_win();
        }

        [DllImport(CURLSHIM_LIB_WIN, EntryPoint = "curl_shim_add_string_to_slist",
             CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr curl_shim_add_string_to_slist_win(IntPtr pStrings, string str);

        internal static IntPtr curl_shim_add_string_to_slist(IntPtr pStrings, string str)
        {
            ShimInitPlatform();
            return curl_shim_add_string_to_slist_win(pStrings, str);
        }

        [DllImport(CURLSHIM_LIB_WIN, EntryPoint = "curl_shim_get_string_from_slist",
             CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr curl_shim_get_string_from_slist_win(IntPtr pSlist, ref IntPtr pStr);

        internal static IntPtr curl_shim_get_string_from_slist(IntPtr pSlist, ref IntPtr pStr)
        {
            ShimInitPlatform();
            return curl_shim_get_string_from_slist_win(pSlist, ref pStr);
        }

        [DllImport(CURLSHIM_LIB_WIN, EntryPoint = "curl_shim_add_string", CallingConvention = CallingConvention.Cdecl,
             CharSet = CharSet.Ansi)]
        private static extern IntPtr curl_shim_add_string_win(IntPtr pStrings, string str);

        internal static IntPtr curl_shim_add_string(IntPtr pStrings, string str)
        {
            ShimInitPlatform();
            return curl_shim_add_string_win(pStrings, str);
        }

        [DllImport(CURLSHIM_LIB_WIN, EntryPoint = "curl_shim_free_strings", CallingConvention = CallingConvention.Cdecl)
        ]
        private static extern void curl_shim_free_strings_win(IntPtr pStrings);

        internal static void curl_shim_free_strings(IntPtr pStrings)
        {
            ShimInitPlatform();
            curl_shim_free_strings_win(pStrings);
        }

        [DllImport(CURLSHIM_LIB_WIN, EntryPoint = "curl_shim_install_delegates",
             CallingConvention = CallingConvention.Cdecl)]
        private static extern int curl_shim_install_delegates_win(
            IntPtr pCurl,
            IntPtr pThis,
            _ShimWriteCallback pWrite,
            _ShimReadCallback pRead,
            _ShimProgressCallback pProgress,
            _ShimDebugCallback pDebug,
            _ShimHeaderCallback pHeader,
            _ShimSslCtxCallback pCtx,
            _ShimIoctlCallback pIoctl);

        internal static int curl_shim_install_delegates(
            IntPtr pCurl,
            IntPtr pThis,
            _ShimWriteCallback pWrite,
            _ShimReadCallback pRead,
            _ShimProgressCallback pProgress,
            _ShimDebugCallback pDebug,
            _ShimHeaderCallback pHeader,
            _ShimSslCtxCallback pCtx,
            _ShimIoctlCallback pIoctl)
        {
            ShimInitPlatform();

            return curl_shim_install_delegates_win(
                pCurl,
                pThis,
                pWrite,
                pRead,
                pProgress,
                pDebug,
                pHeader,
                pCtx,
                pIoctl);
        }

        [DllImport(CURLSHIM_LIB_WIN, EntryPoint = "curl_shim_cleanup_delegates",
             CallingConvention = CallingConvention.Cdecl)]
        private static extern void curl_shim_cleanup_delegates_win(IntPtr pThis);

        internal static void curl_shim_cleanup_delegates(IntPtr pThis)
        {
            ShimInitPlatform();
            curl_shim_cleanup_delegates_win(pThis);
        }

        [DllImport(CURLSHIM_LIB_WIN, EntryPoint = "curl_shim_get_file_time", CallingConvention = CallingConvention.Cdecl
         )]
        private static extern void curl_shim_get_file_time_win(
            int unixTime,
            ref int yy,
            ref int mm,
            ref int dd,
            ref int hh,
            ref int mn,
            ref int ss);

        internal static void curl_shim_get_file_time(
            int unixTime,
            ref int yy,
            ref int mm,
            ref int dd,
            ref int hh,
            ref int mn,
            ref int ss)
        {
            ShimInitPlatform();
            curl_shim_get_file_time_win(unixTime, ref yy, ref mm, ref dd, ref hh, ref mn, ref ss);
        }

        [DllImport(CURLSHIM_LIB_WIN, EntryPoint = "curl_shim_get_file_time", CallingConvention = CallingConvention.Cdecl
         )]
        private static extern void curl_shim_free_slist_win(IntPtr p);

        internal static void curl_shim_free_slist(IntPtr p)
        {
            ShimInitPlatform();
            curl_shim_free_slist_win(p);
        }

        [DllImport(CURLSHIM_LIB_WIN, EntryPoint = "curl_shim_alloc_fd_sets", CallingConvention = CallingConvention.Cdecl
         )]
        private static extern IntPtr curl_shim_alloc_fd_sets_win();

        internal static IntPtr curl_shim_alloc_fd_sets()
        {
            ShimInitPlatform();
            return curl_shim_alloc_fd_sets_win();
        }

        [DllImport(CURLSHIM_LIB_WIN, EntryPoint = "curl_shim_free_fd_sets", CallingConvention = CallingConvention.Cdecl)
        ]
        private static extern void curl_shim_free_fd_sets_win(IntPtr fdsets);

        internal static void curl_shim_free_fd_sets(IntPtr fdsets)
        {
            ShimInitPlatform();
            curl_shim_free_fd_sets_win(fdsets);
        }

        [DllImport(CURLSHIM_LIB_WIN, EntryPoint = "curl_shim_multi_fdset", CallingConvention = CallingConvention.Cdecl)]
        private static extern CurlMultiCode curl_shim_multi_fdset_win(IntPtr multi, IntPtr fdsets, ref int maxFD);

        internal static CurlMultiCode curl_shim_multi_fdset(IntPtr multi, IntPtr fdsets, ref int maxFD)
        {
            ShimInitPlatform();
            return curl_shim_multi_fdset_win(multi, fdsets, ref maxFD);
        }

        [DllImport(CURLSHIM_LIB_WIN, EntryPoint = "curl_shim_select", CallingConvention = CallingConvention.Cdecl)]
        private static extern int curl_shim_select_win(int maxFD, IntPtr fdsets, int milliseconds);

        internal static int curl_shim_select(int maxFD, IntPtr fdsets, int milliseconds)
        {
            ShimInitPlatform();
            return curl_shim_select_win(maxFD, fdsets, milliseconds);
        }

        [DllImport(CURLSHIM_LIB_WIN, EntryPoint = "curl_shim_multi_info_read",
             CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr curl_shim_multi_info_read_win(IntPtr multi, ref int nMsgs);

        internal static IntPtr curl_shim_multi_info_read(IntPtr multi, ref int nMsgs)
        {
            ShimInitPlatform();
            return curl_shim_multi_info_read_win(multi, ref nMsgs);
        }

        [DllImport(CURLSHIM_LIB_WIN, EntryPoint = "curl_shim_multi_info_free",
             CallingConvention = CallingConvention.Cdecl)]
        private static extern void curl_shim_multi_info_free_win(IntPtr multiInfo);

        internal static void curl_shim_multi_info_free(IntPtr multiInfo)
        {
            ShimInitPlatform();
            curl_shim_multi_info_free_win(multiInfo);
        }

        [DllImport(CURLSHIM_LIB_WIN, EntryPoint = "curl_shim_formadd", CallingConvention = CallingConvention.Cdecl)]
        private static extern int curl_shim_formadd_win(IntPtr[] ppForms, IntPtr[] pParams, int nParams);

        internal static int curl_shim_formadd(IntPtr[] ppForms, IntPtr[] pParams, int nParams)
        {
            ShimInitPlatform();
            return curl_shim_formadd_win(ppForms, pParams, nParams);
        }

        [DllImport(CURLSHIM_LIB_WIN, EntryPoint = "curl_shim_install_share_delegates",
             CallingConvention = CallingConvention.Cdecl)]
        private static extern int curl_shim_install_share_delegates_win(
            IntPtr pShare,
            IntPtr pThis,
            _ShimLockCallback pLock,
            _ShimUnlockCallback pUnlock);

        internal static int curl_shim_install_share_delegates(
            IntPtr pShare,
            IntPtr pThis,
            _ShimLockCallback pLock,
            _ShimUnlockCallback pUnlock)
        {
            ShimInitPlatform();
            return curl_shim_install_share_delegates_win(pShare, pThis, pLock, pUnlock);
        }

        [DllImport(CURLSHIM_LIB_WIN, EntryPoint = "url_shim_cleanup_share_delegates",
             CallingConvention = CallingConvention.Cdecl)]
        private static extern void curl_shim_cleanup_share_delegates_win(IntPtr pShare);

        internal static void curl_shim_cleanup_share_delegates(IntPtr pShare)
        {
            ShimInitPlatform();
            curl_shim_cleanup_share_delegates_win(pShare);
        }

        [DllImport(CURLSHIM_LIB_WIN, EntryPoint = "curl_shim_get_version_int_value",
             CallingConvention = CallingConvention.Cdecl)]
        private static extern int curl_shim_get_version_int_value_win(IntPtr p, int offset);

        internal static int curl_shim_get_version_int_value(IntPtr p, int offset)
        {
            ShimInitPlatform();
            return curl_shim_get_version_int_value_win(p, offset);
        }

        [DllImport(CURLSHIM_LIB_WIN, EntryPoint = "curl_shim_get_version_char_ptr",
             CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr curl_shim_get_version_char_ptr_win(IntPtr p, int offset);

        internal static IntPtr curl_shim_get_version_char_ptr(IntPtr p, int offset)
        {
            ShimInitPlatform();
            return curl_shim_get_version_char_ptr_win(p, offset);
        }

        [DllImport(CURLSHIM_LIB_WIN, EntryPoint = "curl_shim_get_number_of_protocols",
             CallingConvention = CallingConvention.Cdecl)]
        private static extern int curl_shim_get_number_of_protocols_win(IntPtr p, int offset);

        internal static int curl_shim_get_number_of_protocols(IntPtr p, int offset)
        {
            ShimInitPlatform();
            return curl_shim_get_number_of_protocols_win(p, offset);
        }

        [DllImport(CURLSHIM_LIB_WIN, EntryPoint = "curl_shim_get_protocol_string",
             CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr curl_shim_get_protocol_string_win(IntPtr p, int offset, int index);

        internal static IntPtr curl_shim_get_protocol_string(IntPtr p, int offset, int index)
        {
            ShimInitPlatform();
            return curl_shim_get_protocol_string_win(p, offset, index);
        }

        internal delegate void _ShimLockCallback(int data, int access, IntPtr userPtr);

        internal delegate void _ShimUnlockCallback(int data, IntPtr userPtr);

        internal delegate int _ShimDebugCallback(CurlInfoType infoType, IntPtr msgBuf, int msgBufSize, IntPtr parm);

        internal delegate int _ShimHeaderCallback(IntPtr buf, int sz, int nmemb, IntPtr stream);

        internal delegate CurlIoError _ShimIoctlCallback(CurlIoCommand cmd, IntPtr parm);

        internal delegate int _ShimProgressCallback(
            IntPtr parm,
            double dlTotal,
            double dlNow,
            double ulTotal,
            double ulNow);

        internal delegate int _ShimReadCallback(IntPtr buf, int sz, int nmemb, IntPtr parm);

        internal delegate int _ShimSslCtxCallback(IntPtr ctx, IntPtr parm);

        internal delegate int _ShimWriteCallback(IntPtr buf, int sz, int nmemb, IntPtr parm);
#endif
    }
}