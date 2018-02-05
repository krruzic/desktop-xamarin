using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TurtleWallet
{
    public partial class updatePrompt : Form
    {
        public updatePrompt()
        {
            InitializeComponent();
        }

        private void updatePrompt_Load(object sender, EventArgs e)
        {
            updateWorker.RunWorkerAsync();
        }

        void updateRequest()
        {
            try
            {
                string thisVersionString = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
                bool needsUpdate = false;
                var builtURL = "https://api.github.com/repos/turtlecoin/desktop-xamarin/releases/latest";

                var cli = new WebClient();
                cli.Headers[HttpRequestHeader.ContentType] = "application/json";
                cli.Headers[HttpRequestHeader.UserAgent] = "TurtleCoin Wallet " + thisVersionString;
                string response = cli.DownloadString(builtURL);

                var jobj = JObject.Parse(response);

                string gitVersionString = jobj["tag_name"].ToString();
               

                var gitVersion = new Version(gitVersionString);
                var thisVersion = new Version(thisVersionString);

                var result = gitVersion.CompareTo(thisVersion);
                if (result > 0)
                    needsUpdate = true;
                else if (result < 0)
                    needsUpdate = false;
                else
                    needsUpdate = false;

                if (needsUpdate)
                {
                    foreach (var item in jobj["assets"])
                    {
                        string name = item["name"].ToString();
                        if (name.Contains("TurtleWallet.exe"))
                        {
                            DialogResult dialogResult = MessageBox.Show("A new version of TurtleCoin Wallet is out. Download?", "TurtleCoin Wallet", MessageBoxButtons.YesNo);
                            if (dialogResult == DialogResult.No)
                            {
                                return;
                            }
                            var dl = new WebClient();
                            dl.Headers[HttpRequestHeader.UserAgent] = "TurtleCoin Wallet " + thisVersionString;
                            dl.DownloadFile(item["browser_download_url"].ToString(), "TurtleWallet_update.exe");
                            System.Diagnostics.Process.Start("TurtleWallet_update.exe");
                            Environment.Exit(0);
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Failed to check for updates!", "TurtleCoin Wallet");
            }
        }

        private void updateWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.Close();
        }

        private void updateWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                var curDir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                var walletdexe = System.IO.Path.Combine(curDir, "walletd.exe");
                long length = new System.IO.FileInfo(walletdexe).Length;
                if (length != 6162432)
                {
                    this.updaterLabel.BeginInvoke((MethodInvoker)delegate ()
                    {
                        updaterLabel.Text = "Updating daemon, please wait..";
                    });
                    var dl = new WebClient();
                    dl.Headers[HttpRequestHeader.UserAgent] = "TurtleCoin Wallet ";
                    dl.DownloadFile("https://github.com/turtlecoin/turtlecoin/releases/download/v0.3.0/TurtleCoin-v0.3.0-windows-CLI.zip", "daemon.zip");
                    if (!System.IO.File.Exists("daemon.zip"))
                    {
                        MessageBox.Show("Could not update daemon because it could not be downloaded. Please manually update!", "TurtleCoin Wallet");
                    }
                    else
                    {
                        using (ZipArchive archive = ZipFile.OpenRead("daemon.zip"))
                        {
                            if (System.IO.File.Exists("walletd.exe"))
                                try { System.Diagnostics.Process.GetProcessesByName("walletd")[0].Kill(); } catch { }
                                System.IO.File.Delete("walletd.exe");
                            foreach (ZipArchiveEntry entry in archive.Entries.Where(efile => efile.FullName.Contains("walletd")))
                            {
                                entry.ExtractToFile("walletd.exe");
                            }
                        }
                        System.Threading.Thread.Sleep(500);
                        System.IO.File.Delete("daemon.zip");
                    }
                }
            }
            catch
            {
                MessageBox.Show("Failed to check/update daemon!", "TurtleCoin Wallet");
            }

            try
            {
                if (System.AppDomain.CurrentDomain.FriendlyName == "TurtleWallet_update.exe")
                {
                    System.Threading.Thread.Sleep(500);
                    System.IO.File.Copy("TurtleWallet_update.exe", "TurtleWallet.exe", true);
                    System.Diagnostics.Process.Start("TurtleWallet.exe");
                    Environment.Exit(0);
                }
                else if (System.AppDomain.CurrentDomain.FriendlyName == "TurtleWallet.exe")
                {
                    if (System.IO.File.Exists("TurtleWallet_update.exe"))
                        System.IO.File.Delete("TurtleWallet_update.exe");
                }
            }
            catch
            {
                MessageBox.Show("Failed to check for updates!", "TurtleCoin Wallet");
            }

            updateRequest();
        }
    }

}
