using System;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Threading;
using System.Windows.Forms;

namespace dmc3music
{
    public partial class GetMusic : Form
    {
        public GetMusic()
        {
            InitializeComponent();
        }

        private void RecenterLabel1()
        {
            label1.Left = ((Width - label1.Width) / 2) - 10;
        }

        private static readonly WebClient wc = new WebClient();
        private static readonly ManualResetEvent handle = new ManualResetEvent(true);

        private void GetMusic_Load(object sender, EventArgs e)
        {
            if (Directory.Exists("tracks"))
            {
                if (Directory.GetFiles("tracks").Length >= 70)
                {
                    label1.Text = "Already have the tracks folder";
                    RecenterLabel1();
                    return;
                }
                else
                {
                    Directory.Delete("tracks", true);
                }
            }

            try
            {
                progressBar1.Visible = true;
                wc.DownloadProgressChanged += WcOnDownloadProgressChanged;
                wc.DownloadFileCompleted += WcOnDownloadFileCompleted;
                wc.DownloadFileAsync(new Uri(@"https://github.com/644/dmc3music/releases/download/tracks/tracks.zip"), "tracks.zip");
                handle.WaitOne();
            }
            catch
            {
                label1.Text = "Problem downloading the tracks zipfile";
                RecenterLabel1();
            }
        }

        public string ReadableSize(long fileSizeInBytes)
        {
            string[] suffixes = { "B", "KiB", "MiB" };
            double sizeResult = fileSizeInBytes * 1.0;
            int suffixIndex = 0;

            while (sizeResult > 1024 && suffixIndex < suffixes.Length)
            {
                sizeResult /= 1024;
                suffixIndex++;
            }

            return $"{sizeResult:0.00} {suffixes[suffixIndex]}";
        }

        private void WcOnDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage >= 0 && e.ProgressPercentage <= 100)
            {
                string readRecv = ReadableSize(e.BytesReceived);
                string maxRecv = ReadableSize(e.TotalBytesToReceive);

                label1.Text = $"Downloaded {readRecv} of {maxRecv} ({e.ProgressPercentage}%)";
                RecenterLabel1();
                progressBar1.Value = e.ProgressPercentage;
            }
        }

        private void WcOnDownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (!e.Cancelled && e.Error == null)
            {
                label1.Text = "Download complete";
                RecenterLabel1();
                try
                {
                    ZipFile.ExtractToDirectory("tracks.zip", System.AppDomain.CurrentDomain.BaseDirectory);
                    File.Delete("tracks.zip");
                    progressBar1.Visible = false;
                    label1.Text = "Successfully downloaded and extracted the tracks";
                    RecenterLabel1();
                }
                catch
                {
                    label1.Text = "Unable to extract the tracks zipfile";
                    RecenterLabel1();
                }
            }
            handle.Set();
        }
    }
}
