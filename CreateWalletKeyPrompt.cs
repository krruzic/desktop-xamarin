using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TurtleWallet
{
    public partial class CreateWalletKeyPrompt : TurtleWalletForm
    {

        public CreateWalletKeyPrompt(string keyOutput)
        {
            InitializeComponent();
            KeysTextbox.Text = keyOutput;
            this.Text = "Turtle Wallet";
        }

        private void CreateWalletButton_Click(object sender, EventArgs e)
        {
            Utilities.Close(this);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            Utilities.CloseProgram(e);
        }
    }
}
