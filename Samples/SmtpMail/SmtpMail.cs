using System;
using System.Runtime.InteropServices;
using CurlSharp;

namespace SmtpMail
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct UploadContext
    {
        public int LinesRead;
    }

    internal class SmtpMail
    {
        private static void Main(string[] args)
        {
            try
            {
                Curl.GlobalInit(CurlInitFlag.All);

                using (var curl = new CurlEasy())
                {
                    /* This is the URL for your mailserver */
                    curl.Url = "smtp://localhost:25";

                    /* Note that this option isn't strictly required, omitting it will 
                     * result in libcurl sending the MAIL FROM command with empty sender 
                     * data. All autoresponses should have an empty reverse-path, and 
                     * should be directed to the address in the reverse-path which 
                     * triggered them. Otherwise, they could cause an endless loop. 
                     * See RFC 5321 Section 4.5.5 for more details. */
                    curl.SetOpt(CurlOption.MailFrom, "<from@example.org>");

                    /* Add two recipients, in this particular case they correspond to 
                     * the To: and Cc: addressees in the header, but they could be any 
                     * kind of recipient. */
                    using (var recipients = new CurlSlist())
                    {
                        recipients.Append("<to@example.net>");
                        recipients.Append("<cc@example.org>");
                        curl.SetOpt(CurlOption.MailRcpt, recipients);

                        /* We're using a callback function to specify the payload (the 
                         * headers and body of the message). You could just use the 
                         * ReadData option to  specify a FILE pointer to read from. */
                        curl.ReadFunction = PayloadSource;
                        curl.ReadData = new UploadContext();
                        curl.Upload = true;

                        var res = curl.Perform();
                        if (res != CurlCode.Ok)
                        {
                            Console.WriteLine("CurlEasy.Perform() failed: " + res);
                        }
                    }

                    /* curl won't send the QUIT command until you call cleanup, so you should be
                     * able to re-use this connection for additional messages (setting
                     * CURLOPT_MAIL_FROM and CURLOPT_MAIL_RCPT as required, and calling
                     * Perform() again. It may not be a good idea to keep the
                     * connection open for a very long time though (more than a few minutes may
                     * result in the server timing out the connection), and you do want to clean
                     * up in the end.
                     */
                }

                Curl.GlobalCleanup();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.ReadLine();
            }
            Console.ReadLine();
        }

        private static byte[] GetBytes(string str)
        {
            var bytes = new byte[str.Length*sizeof (char)];
            Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        private static int PayloadSource(byte[] buf, int size, int nmemb, object extradata)
        {
            var payloadText = new[]
                              {
                                  "Date: Mon, 29 Nov 2010 21:54:29 +1100\r\n",
                                  "To: <to@example.net>\r\n",
                                  "From: <from@example.org> (Example User)\r\n",
                                  "Cc: <cc@example.org> (Another example User)\r\n",
                                  "Message-ID: <dcd7cb36-11db-487a-9f3a-e652a9458efd@rfcpedant.example.org>\r\n",
                                  "Subject: SMTP example message\r\n",
                                  "\r\n", /* empty line to divide headers from body, see RFC5322 */
                                  "The body of the message starts here.\r\n",
                                  "\r\n",
                                  "It could be a lot of lines, could be MIME encoded, whatever.\r\n",
                                  "Check RFC5322.\r\n"
                              };

            var ctxUpload = (UploadContext) extradata;

            if ((ctxUpload.LinesRead >= 0) &&
                (ctxUpload.LinesRead < payloadText.Length) &&
                (size != 0) && (nmemb != 0) &&
                ((size*nmemb) > 0))
            {
                var line = payloadText[ctxUpload.LinesRead++];
                var lineBuf = GetBytes(line);
                Buffer.BlockCopy(lineBuf, 0, buf, 0, lineBuf.Length);
                return lineBuf.Length;
            }
            return 0;
        }
    }
}