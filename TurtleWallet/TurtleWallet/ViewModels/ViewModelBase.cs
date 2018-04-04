using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Text;
using TurtleWallet.Services;

namespace TurtleWallet.ViewModels
{
    public class ViewModelBase : BindableBase, INavigationAware, IDestructible
    {
        protected INavigationService NavigationService { get; private set; }
        protected ISimpleFileWrapperService FileWrapperService { get; private set; }
        protected IWalletdWatcherService WalletdWatcherService { get; private set; }

        private string _title;
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public ViewModelBase(INavigationService navigationService, ISimpleFileWrapperService fileWrapperService, IWalletdWatcherService walletdWatcherService)
        {
            NavigationService = navigationService;
            FileWrapperService = fileWrapperService;
            WalletdWatcherService = walletdWatcherService;
        }

        public virtual void OnNavigatedFrom(NavigationParameters parameters)
        {
            
        }

        public virtual void OnNavigatedTo(NavigationParameters parameters)
        {
            
        }

        public virtual void OnNavigatingTo(NavigationParameters parameters)
        {
            
        }

        public virtual void Destroy()
        {
            
        }
    }
}
