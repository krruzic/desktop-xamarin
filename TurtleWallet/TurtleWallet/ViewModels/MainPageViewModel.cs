using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Input;
using TurtleWallet.Models;
using Xamarin.Forms;
using TurtleWallet.Services;
using System.Threading.Tasks;
using TurtleWallet.Utilities;

namespace TurtleWallet.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        private WalletConf _userWalletConf;
        public WalletConf UserWalletConf
        {
            get { return _userWalletConf; }
            set { SetProperty(ref _userWalletConf, value); }
        }
        public ICommand LoadWalletConfCommand { get; private set; }
        public ICommand StopWalletCommand { get; private set; }

        private int first = 0;
        public MainPageViewModel(INavigationService navigationService, ISimpleFileWrapperService fileWrapperService, IWalletdWatcherService walletdWatcherService) 
            : base (navigationService, fileWrapperService, walletdWatcherService)
        {
            Title = "fucking eh";
            LoadWalletConfCommand = new Command(async () => await LoadWalletConf());
            StopWalletCommand = new Command(async () => await StopWallet());
        }

        private async Task StopWallet()
        {
            await WalletdWatcherService.Stop();
        }

        private async Task LoadWalletConf()
        {
            Title = "Loading in wallet conf";
            UserWalletConf = new WalletConf("faucetwallet.bin");
            await FileWrapperService.WriteFileAsync("walletd.conf", UserWalletConf.ToIni());
            await WalletdWatcherService.Init();

            if (first>0)
                await WalletdWatcherService.Start(UserWalletConf, await FileWrapperService.GetAbsolutePath("walletd.conf"));
            first++;
        }
    }
}
