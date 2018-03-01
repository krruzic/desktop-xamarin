using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TurtleWallet
{
    public partial class SelectionPrompt : Form
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

        public SelectionPrompt()
        {
            InitializeComponent();
            this.Text = "Turtle Wallet";
            //if(IsRunningOnMono())
            //{
            //    this.Width = this.Width + 150;
            //    this.Height = this.Height + 150;
            //}
        }

        private void CreateWalletButton_MouseEnter(object sender, EventArgs e)
        {
            var backcolor = Color.FromArgb(44, 44, 44);
            var forcolor = Color.FromArgb(39, 170, 107);
            var currentButton = (Label)sender;
            currentButton.BackColor = backcolor;
            currentButton.ForeColor = forcolor;
        }

        private void CreateWalletButton_MouseLeave(object sender, EventArgs e)
        {
            var backcolor = Color.FromArgb(52, 52, 52);
            var forcolor = Color.FromArgb(224, 224, 224);
            var currentButton = (Label)sender;
            currentButton.BackColor = backcolor;
            currentButton.ForeColor = forcolor;
        }

        private void SelectWalletButton_MouseEnter(object sender, EventArgs e)
        {
            var backcolor = Color.FromArgb(44, 44, 44);
            var forcolor = Color.FromArgb(39, 170, 107);
            var currentButton = (Label)sender;
            currentButton.BackColor = backcolor;
            currentButton.ForeColor = forcolor;
        }

        private void SelectWalletButton_MouseLeave(object sender, EventArgs e)
        {
            var backcolor = Color.FromArgb(52, 52, 52);
            var forcolor = Color.FromArgb(224, 224, 224);
            var currentButton = (Label)sender;
            currentButton.BackColor = backcolor;
            currentButton.ForeColor = forcolor;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            Utilities.CloseProgram(e);
        }

        private void CreateWalletButton_Click(object sender, EventArgs e)
        {
            CreateWalletPrompt CWP = new CreateWalletPrompt();
            Utilities.Hide(this);
            var CWPreturn = CWP.ShowDialog();
            if(CWPreturn == DialogResult.OK)
            {
                WalletPath = CWP.WalletPath;
                WalletPassword = CWP.WalletPassword;
                Utilities.SetAppClosing(false);
                this.DialogResult = DialogResult.OK;
                Utilities.Close(CWP);
                Utilities.Close(this);
            }
            this.Show();
        }

        private void SelectWalletButton_Click(object sender, EventArgs e)
        {
            var curDir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            OpenFileDialog findWalletDialog = new OpenFileDialog
            {
                InitialDirectory = curDir,
                Filter = "wallet files (*.wallet)|*.wallet|All files (*.*)|*.*",
                FilterIndex = 2,
                RestoreDirectory = true
            };

            if (findWalletDialog.ShowDialog() == DialogResult.OK)
            {
                if(System.IO.File.Exists(findWalletDialog.FileName))
                {
                    WalletPath = findWalletDialog.FileName;
                    var pPrompt = new passwordPrompt();
                    Utilities.Hide(this);
                    var pResult = pPrompt.ShowDialog();
                    if(pResult != DialogResult.OK)
                    {
                        findWalletDialog.Dispose();
                        Utilities.Close(pPrompt);
                        this.Show();
                        return;
                    }
                    else
                    {
                        WalletPassword = pPrompt.WalletPassword;
                        Utilities.Close(pPrompt);
                        Utilities.SetAppClosing(false);
                        this.DialogResult = DialogResult.OK;
                        Utilities.Close(this);
                    }
                }
            }
        }

        public static bool IsRunningOnMono()
        {
            return Type.GetType("Mono.Runtime") != null;
        }
    }
}
