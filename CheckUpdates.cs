using Octokit;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace dmc3music
{
    public partial class CheckUpdates : Form
    {
        public CheckUpdates()
        {
            InitializeComponent();
        }

        private void RecenterLabel1()
        {
            label1.Left = ((Width - label1.Width) / 2) - 10;
        }

        private async void CheckUpdates_Load(object sender, EventArgs e)
        {
            try
            {
                await CheckGitHubNewerVersion();
            }
            catch (Exception err)
            {
                label1.Text = "Could not check for updates";
            }

            RecenterLabel1();
        }

        private static readonly WebClient wc = new WebClient();
        private static readonly ManualResetEvent handle = new ManualResetEvent(true);
        public IReadOnlyList<ReleaseAsset> latestAsset { get; set; }
        public string releaseZip { get; set; } = "updatetmp.zip";

        private void ReplaceInstance()
        {
            try
            {
                string extractPath = @".\extract";
                if (Directory.Exists(extractPath))
                {
                    Directory.Delete(extractPath, true);
                }

                ZipFile.ExtractToDirectory(releaseZip, extractPath);

                string updatePath = Path.GetFullPath("./extract/dmc3tools/");
                string currentPath = System.AppDomain.CurrentDomain.BaseDirectory;

                string updateBat = $@":start
                    tasklist /FI ""IMAGENAME eq dmc3tools.exe"" /fo csv 2>NUL | find /I ""dmc3tools.exe"">NUL
                    if ""%ERRORLEVEL%"" == ""0"" goto start
                    xcopy /e /k /h /i /Y ""{updatePath}"" ""{currentPath}""
                    start dmc3tools.exe
                    @RD /S /Q extract
                    del ""{releaseZip}""
                    (goto) 2>nul & del ""%~f0""";

                File.WriteAllText("update.bat", updateBat);

                Process p = new Process();
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.FileName = "update.bat";
                p.Start();

                System.Windows.Forms.Application.Exit();
            }
            catch { }
        }

        private void WcOnDownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (!e.Cancelled && e.Error == null)
            {
                label1.Text = "Download complete";
                RecenterLabel1();
                progressBar1.Visible = false;
                button1.Visible = false;
                button2.Visible = true;
            }
            handle.Set();
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

        private async Task CheckGitHubNewerVersion()
        {
            GitHubClient client = new GitHubClient(new ProductHeaderValue("644"));

            Release latestRelease = await client.Repository.Release.GetLatest(398084113);
            Version latestGitHubVersion = new Version(latestRelease.TagName);

            Assembly assembly = Assembly.GetExecutingAssembly();
            Version localVersion = new Version(FileVersionInfo.GetVersionInfo(assembly.Location).ProductVersion);

            latestAsset = await client.Repository.Release.GetAllAssets("644", "dmc3music", latestRelease.Id);

            int versionComparison = localVersion.CompareTo(latestGitHubVersion);

            if (versionComparison < 0)
            {
                label1.Text = $"New update available! (Version: {latestGitHubVersion})";
                button1.Visible = true;
            }
            else if (versionComparison > 0)
            {
                label1.Text = "Somehow, you have a later version than the release";
            }
            else
            {
                label1.Text = "Already up to date!";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Visible = false;
            progressBar1.Visible = true;

            wc.DownloadProgressChanged += WcOnDownloadProgressChanged;
            wc.DownloadFileCompleted += WcOnDownloadFileCompleted;
            wc.DownloadFileAsync(new Uri(latestAsset[0].BrowserDownloadUrl), releaseZip);
            handle.WaitOne();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ReplaceInstance();
        }
    }
}
