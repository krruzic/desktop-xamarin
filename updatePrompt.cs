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
    public partial class UpdatePrompt : Form
    {
        public UpdatePrompt()
        {
            InitializeComponent();
            this.Text = "Turtle Wallet";
        }

        private void UpdatePrompt_Load(object sender, EventArgs e)
        {
            updateWorker.RunWorkerAsync();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            Utilities.CloseProgram(e);
        }

        void UpdateRequest()
        {
            try
            {
                System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                string thisVersionString = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
                bool needsUpdate = false;
                var builtURL = "https://api.github.com/repos/turtlecoin/desktop-xamarin/releases/latest";

                var cli = new WebClient();
                cli.Headers[HttpRequestHeader.ContentType] = "application/json";
                cli.Headers[HttpRequestHeader.UserAgent] = "TurtleCoin Wallet " + thisVersionString;
                string response = cli.DownloadString(new Uri(builtURL));

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
            catch (Exception ex)
            {
                MessageBox.Show("Failed to check for updates! " + ex.Message + Environment.NewLine + ex.InnerException, "TurtleCoin Wallet");
            }
        }

        private void UpdateWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Utilities.Close(this);
        }

        private void UpdateWorker_DoWork(object sender, DoWorkEventArgs e)
        {
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

            UpdateRequest();
        }
    }

}
