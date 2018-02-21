using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TurtleWallet
{
    public partial class Splash : Form
    {
        public string WalletPath
        {
            get;
            set;
        }

        public string WalletPassword
        {
            get;
            set;
        }

        protected override CreateParams CreateParams
        {
            get
            {
                const int CS_DROPSHADOW = 0x20000;
                CreateParams cp = base.CreateParams;
                cp.ClassStyle |= CS_DROPSHADOW;
                return cp;
            }
        }

        public Splash(string _wallet, string _password)
        {
            InitializeComponent();
            WalletPath = _wallet;
            WalletPassword = _password;
            versionLabel.Text = "v" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
        }

        private void Splash_Load(object sender, EventArgs e)
        {
            string curDir = Directory.GetCurrentDirectory();
            StatusLabel.Text = "Connecting to daemon ...";
            splashBackgroundWorker.RunWorkerAsync();
        }

        private void SplashBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            int failcount = 0;
            string globalLastline = "";
            try
            {
                var connReturn = ConnectionManager.StartDaemon(WalletPath, WalletPassword);
                if (connReturn.Item1 == false)
                {
                    this.StatusLabel.BeginInvoke((MethodInvoker)delegate () { this.StatusLabel.Text = "Daemon Connection Failed: " + connReturn.Item2; });
                    System.Threading.Thread.Sleep(5000);
                    Environment.Exit(3);
                }
               
                while (true)
                {
                    string lastLine = "";
                    bool lineWasUpdated = false;
                    if (System.IO.File.Exists("walletd.log"))
                    {
                        var fs = new FileStream("walletd.log", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                        using (var sr = new StreamReader(fs))
                        {
                            while (!sr.EndOfStream)
                            {
                                string linechache = sr.ReadLine();
                                if (String.IsNullOrWhiteSpace(linechache))
                                    break;
                                lastLine = linechache;
                                if (globalLastline == lastLine)
                                    lineWasUpdated = false;
                                else
                                    lineWasUpdated = true;
                                globalLastline = linechache;
                                if (lastLine.Contains("The password is wrong") || lastLine.Contains("Restored view public key doesn't correspond to secret key"))
                                {
                                    try
                                    {
                                        connReturn.Item3.CancelOutputRead();
                                        connReturn.Item3.Kill();
                                    }
                                    catch { }
                                    MessageBox.Show("The password is incorrect!", "TurtleCoin Wallet");
                                    //will restart at selection screen instead of exiting
                                    Program.jumpBack = true;
                                    break;
                                }
                            }
                        }
                        fs.Close();
                    }

                    try
                    {

                        if (lastLine.Contains("Imported block with index"))
                        {
                            string stdUpdate = lastLine.Split(new string[] { "    " }, StringSplitOptions.None)[1];
                            this.StatusLabel.BeginInvoke((MethodInvoker)delegate () { this.StatusLabel.Text = stdUpdate; });
                            System.Threading.Thread.Sleep(3000);
                            continue;
                        }

                        if (Process.GetProcessesByName("walletd").Length < 1 || connReturn.Item3 == null)
                        {
                            throw new Exception("Daemon exited!");
                        }
                        var resp = ConnectionManager.Request("getStatus");
                        if (resp.Item1 == false)
                        {
                            throw new Exception("No RPC connection/Failed");
                        }
                        var block_count = (int)resp.Item3["blockCount"];
                        var known_block_count = (int)resp.Item3["knownBlockCount"];
                        if(known_block_count == 0)
                        {
                            this.StatusLabel.BeginInvoke((MethodInvoker)delegate () { this.StatusLabel.Text = "Waiting on known block count..." ; });
                            continue;
                        }
                        this.StatusLabel.BeginInvoke((MethodInvoker)delegate () { this.StatusLabel.Text = "Syncing... [" + block_count.ToString() + " / " + known_block_count.ToString() + "]"; });
                        if(known_block_count > 0 && (block_count >= known_block_count - 1))
                        {
                            this.StatusLabel.BeginInvoke((MethodInvoker)delegate () { this.StatusLabel.Text = "Wallet is synced, opening..."; });
                            e.Result = connReturn.Item3;
                            break;
                        }

                    }
                    catch (Exception ex)
                    {
                        if (Process.GetProcessesByName("walletd").Length < 1 || connReturn.Item3 == null)
                        {
                            throw new Exception("Daemon exited!");
                        }
                        if (lineWasUpdated == false)
                        {
                            failcount += 1;
                            if (failcount >= 100)
                            {
                                throw new Exception("MAXTRYERROR: " + ex.Message);
                            }
                            this.StatusLabel.BeginInvoke((MethodInvoker)delegate () { this.StatusLabel.Text = "Waiting on Daemon, trying..(" + failcount.ToString() + "/100)"; });
                        }
                        else
                        {
                            this.StatusLabel.BeginInvoke((MethodInvoker)delegate () { this.StatusLabel.Text = "Waiting on Daemon to start sync..."; });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.StatusLabel.BeginInvoke((MethodInvoker)delegate () { this.StatusLabel.Text = "Daemon Connection Failed: " + ex.Message ; });
                MessageBox.Show("Daemon Connection Failed: " + ex.Message, "TurtleCoin Wallet", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(3);
            }
        }

        private void SplashBackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {

        }

        private void SplashBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (Program.jumpBack)
                this.Close();
            else
            {
                this.Hide();
                var walletWindow = new wallet(WalletPath, WalletPassword, (Process)e.Result);
                walletWindow.Closed += (s, args) => this.Close();
                walletWindow.Show();
            }
        }
    }
}
