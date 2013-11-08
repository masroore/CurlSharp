/***************************************************************************
 *
 * CurlS#arp
 *
 * Copyright (c) 2013 Dr. Masroor Ehsan (masroore@gmail.com)
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

using System;
using System.Runtime.InteropServices;

namespace CurlSharp
{
    /// <summary>
    ///     P/Invoke signatures.
    /// </summary>
    internal static class NativeMethods
    {
#if WIN64
        private const string CURL_LIB = "libcurl64.dll";
        private const string CURLSHIM_LIB = "libcurlshim64.dll";
#else
        private const string CURL_LIB = "libcurl.dll";
        private const string CURLSHIM_LIB = "libcurlshim.dll";
#endif

        // internal delegates from cURL

        // libcurl imports
        [DllImport(CURL_LIB, CallingConvention = CallingConvention.Cdecl)]
        internal static extern CurlCode curl_global_init(int flags);

        [DllImport(CURL_LIB, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void curl_global_cleanup();

        [DllImport(CURL_LIB, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern IntPtr curl_escape(String url, int length);

        [DllImport(CURL_LIB, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern IntPtr curl_unescape(String url, int length);

        [DllImport(CURL_LIB, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void curl_free(IntPtr p);

        [DllImport(CURL_LIB, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr curl_version();

        [DllImport(CURL_LIB, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr curl_easy_init();

        [DllImport(CURL_LIB, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void curl_easy_cleanup(IntPtr pCurl);

        [DllImport(CURL_LIB, CallingConvention = CallingConvention.Cdecl)]
        internal static extern CurlCode curl_easy_setopt(IntPtr pCurl, CurlOption opt, IntPtr parm);

        [DllImport(CURL_LIB, CallingConvention = CallingConvention.Cdecl, EntryPoint = "curl_easy_setopt")]
        internal static extern CurlCode curl_easy_setopt_64(IntPtr pCurl, CurlOption opt, long parm);

        [DllImport(CURL_LIB, CallingConvention = CallingConvention.Cdecl)]
        internal static extern CurlCode curl_easy_perform(IntPtr pCurl);

        [DllImport(CURL_LIB, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr curl_easy_duphandle(IntPtr pCurl);

        [DllImport(CURL_LIB, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr curl_easy_strerror(CurlCode err);

        [DllImport(CURL_LIB, CallingConvention = CallingConvention.Cdecl)]
        internal static extern CurlCode curl_easy_getinfo(IntPtr pCurl, CurlInfo info, ref IntPtr pInfo);

        [DllImport(CURL_LIB, CallingConvention = CallingConvention.Cdecl, EntryPoint = "curl_easy_getinfo")]
        internal static extern CurlCode curl_easy_getinfo_64(IntPtr pCurl,
            CurlInfo info, ref double dblVal);

        [DllImport(CURL_LIB, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void curl_easy_reset(IntPtr pCurl);

        [DllImport(CURL_LIB, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr curl_multi_init();

        [DllImport(CURL_LIB, CallingConvention = CallingConvention.Cdecl)]
        internal static extern CurlMultiCode curl_multi_cleanup(IntPtr pmulti);

        [DllImport(CURL_LIB, CallingConvention = CallingConvention.Cdecl)]
        internal static extern CurlMultiCode curl_multi_add_handle(IntPtr pmulti, IntPtr peasy);

        [DllImport(CURL_LIB, CallingConvention = CallingConvention.Cdecl)]
        internal static extern CurlMultiCode curl_multi_remove_handle(IntPtr pmulti, IntPtr peasy);

        [DllImport(CURL_LIB, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr curl_multi_strerror(CurlMultiCode errorNum);

        [DllImport(CURL_LIB, CallingConvention = CallingConvention.Cdecl)]
        internal static extern CurlMultiCode curl_multi_perform(IntPtr pmulti, ref int runningHandles);

        [DllImport(CURL_LIB, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void curl_formfree(IntPtr pForm);

        [DllImport(CURL_LIB, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr curl_share_init();

        [DllImport(CURL_LIB, CallingConvention = CallingConvention.Cdecl)]
        internal static extern CurlShareCode curl_share_cleanup(IntPtr pShare);

        [DllImport(CURL_LIB, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr curl_share_strerror(CurlShareCode errorCode);

        [DllImport(CURL_LIB, CallingConvention = CallingConvention.Cdecl)]
        internal static extern CurlShareCode curl_share_setopt(IntPtr pShare, CurlShareOption optCode, IntPtr option);

        [DllImport(CURL_LIB, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr curl_version_info(CurlVersion ver);

        // libcurlshim imports
        [DllImport(CURLSHIM_LIB, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void curl_shim_initialize();

        [DllImport(CURLSHIM_LIB, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void curl_shim_cleanup();

        [DllImport(CURLSHIM_LIB, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr curl_shim_alloc_strings();

        [DllImport(CURLSHIM_LIB, CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi)]
        internal static extern IntPtr curl_shim_add_string_to_slist(
            IntPtr pStrings, String str);

        [DllImport(CURLSHIM_LIB, CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi)]
        internal static extern IntPtr curl_shim_get_string_from_slist(
            IntPtr pSlist, ref IntPtr pStr);

        [DllImport(CURLSHIM_LIB, CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi)]
        internal static extern IntPtr curl_shim_add_string(IntPtr pStrings, String str);

        [DllImport(CURLSHIM_LIB, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void curl_shim_free_strings(IntPtr pStrings);

        [DllImport(CURLSHIM_LIB, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int curl_shim_install_delegates(IntPtr pCurl, IntPtr pThis,
            CURL_WRITE_DELEGATE pWrite, CURL_READ_DELEGATE pRead,
            CURL_PROGRESS_DELEGATE pProgress, CURL_DEBUG_DELEGATE pDebug,
            CURL_HEADER_DELEGATE pHeader, CURL_SSL_CTX_DELEGATE pCtx,
            CURL_IOCTL_DELEGATE pIoctl);

        [DllImport(CURLSHIM_LIB, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void curl_shim_cleanup_delegates(IntPtr pThis);

        [DllImport(CURLSHIM_LIB, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void curl_shim_get_file_time(int unixTime,
            ref int yy, ref int mm, ref int dd, ref int hh, ref int mn, ref int ss);

        [DllImport(CURLSHIM_LIB, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void curl_shim_free_slist(IntPtr p);

        [DllImport(CURLSHIM_LIB, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr curl_shim_alloc_fd_sets();

        [DllImport(CURLSHIM_LIB, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void curl_shim_free_fd_sets(IntPtr fdsets);

        [DllImport(CURLSHIM_LIB, CallingConvention = CallingConvention.Cdecl)]
        internal static extern CurlMultiCode curl_shim_multi_fdset(IntPtr multi,
            IntPtr fdsets, ref int maxFD);

        [DllImport(CURLSHIM_LIB, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int curl_shim_select(int maxFD, IntPtr fdsets,
            int timeoutMillis);

        [DllImport(CURLSHIM_LIB, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr curl_shim_multi_info_read(IntPtr multi,
            ref int nMsgs);

        [DllImport(CURLSHIM_LIB, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void curl_shim_multi_info_free(IntPtr multiInfo);

        [DllImport(CURLSHIM_LIB, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int curl_shim_formadd(IntPtr[] ppForms, IntPtr[] pParams, int nParams);

        [DllImport(CURLSHIM_LIB, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int curl_shim_install_share_delegates(IntPtr pShare,
            IntPtr pThis, CURLSH_LOCK_DELEGATE pLock, CURLSH_UNLOCK_DELEGATE pUnlock);

        [DllImport(CURLSHIM_LIB, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void curl_shim_cleanup_share_delegates(IntPtr pShare);

        [DllImport(CURLSHIM_LIB, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int curl_shim_get_version_int_value(IntPtr p, int offset);

        [DllImport(CURLSHIM_LIB, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr curl_shim_get_version_char_ptr(IntPtr p, int offset);

        [DllImport(CURLSHIM_LIB, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int curl_shim_get_number_of_protocols(IntPtr p, int offset);

        [DllImport(CURLSHIM_LIB, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr curl_shim_get_protocol_string(IntPtr p, int offset, int index);

        internal delegate void CURLSH_LOCK_DELEGATE(int data, int access, IntPtr userPtr);

        internal delegate void CURLSH_UNLOCK_DELEGATE(int data, IntPtr userPtr);

        internal delegate int CURL_DEBUG_DELEGATE(CurlInfoType infoType, IntPtr msgBuf, int msgBufSize, IntPtr parm);

        internal delegate int CURL_HEADER_DELEGATE(IntPtr buf, int sz, int nmemb, IntPtr stream);

        internal delegate CurlIoError CURL_IOCTL_DELEGATE(CurlIoCommand cmd, IntPtr parm);

        internal delegate int CURL_PROGRESS_DELEGATE(
            IntPtr parm, double dlTotal, double dlNow, double ulTotal, double ulNow);

        internal delegate int CURL_READ_DELEGATE(IntPtr buf, int sz, int nmemb, IntPtr parm);

        internal delegate int CURL_SSL_CTX_DELEGATE(IntPtr ctx, IntPtr parm);

        internal delegate int CURL_WRITE_DELEGATE(IntPtr buf, int sz, int nmemb, IntPtr parm);
    }
}