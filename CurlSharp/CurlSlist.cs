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

namespace CurlSharp
{
    /// <summary>
    ///     This class wraps a linked list of strings used in <c>cURL</c>. Use it
    ///     to build string lists where they're required, such as when calling
    ///     <see cref="CurlEasy.SetOpt" /> with <see cref="CurlOption.Quote" />
    ///     as the option.
    /// </summary>
    public class CurlSlist
    {
        private IntPtr _pStringList;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <exception cref="System.InvalidOperationException">
        ///     This is thrown
        ///     if <see cref="Curl" /> hasn't bee properly initialized.
        /// </exception>
        public CurlSlist()
        {
            Curl.EnsureCurl();
            _pStringList = IntPtr.Zero;
        }

        /// <summary>
        ///     Destructor
        /// </summary>
        ~CurlSlist()
        {
            Dispose(false);
        }

        /// <summary>
        ///     Append a string to the list.
        /// </summary>
        /// <param name="str">The <c>string</c> to append.</param>
        public void Append(string str)
        {
            _pStringList = NativeMethods.curl_shim_add_string_to_slist(_pStringList, str);
        }

        /// <summary>
        ///     Free all internal strings.
        /// </summary>
        public void FreeAll()
        {
            GC.SuppressFinalize(this);
            Dispose(true);
        }

        internal IntPtr GetHandle()
        {
            return _pStringList;
        }

        private void Dispose(bool disposing)
        {
            lock (this)
            {
                // no if (disposing) pattern to clean up managed objects
                if (_pStringList != IntPtr.Zero)
                {
                    NativeMethods.curl_shim_free_slist(_pStringList);
                    _pStringList = IntPtr.Zero;
                }
            }
        }
    }
}