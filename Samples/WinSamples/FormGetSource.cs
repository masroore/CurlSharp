using System;
using System.Text;
using System.Windows.Forms;
using CurlSharp;

namespace WinSamples
{
    public partial class FormGetSource : Form
    {
        public FormGetSource()
        {
            InitializeComponent();
        }

        private void buttonGetSource_Click(object sender, EventArgs e)
        {
            textBoxSource.Clear();
            Curl.GlobalInit((int) CurlInitFlag.All);

            using (var easy = new CurlEasy())
            {
                easy.AutoReferer = true;
                easy.FollowLocation = true;
                easy.Url = textBoxUrl.Text.Trim();
                easy.WriteFunction = OnWriteData;
                easy.Perform();
            }

            Curl.GlobalCleanup();
        }

        private Int32 OnWriteData(Byte[] buf, Int32 size, Int32 nmemb, Object extraData)
        {
            Console.Write(Encoding.UTF8.GetString(buf));
            textBoxSource.Text += Encoding.UTF8.GetString(buf);
            return size*nmemb;
        }
    }
}