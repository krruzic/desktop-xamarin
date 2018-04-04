using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TurtleWallet.Models;

namespace TurtleWallet.Services
{
    public interface IWalletdWatcherService
    {
        bool IsRunning { get; set; }
        Task Init();

        Task GetStatus();

        Task Start(WalletConf wallet, string filename);

        Task Stop();
    }
}
