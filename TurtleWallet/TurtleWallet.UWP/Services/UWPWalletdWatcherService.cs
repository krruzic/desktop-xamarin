using System;
using Xamarin.Forms;
using TurtleWallet.Services;
using TurtleWalletUWP.Services;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Foundation.Collections;
using System.Collections.Generic;
using Windows.ApplicationModel.AppService;
using TurtleWallet;
using TurtleWallet.Models;
using System.Runtime.Serialization;
using System.IO;
using System.Text;

[assembly: Dependency(typeof(UWPWalletdWatcherService))]
namespace TurtleWalletUWP.Services
{
    public class UWPWalletdWatcherService : IWalletdWatcherService
    {
        public bool IsRunning { get; set; }

        public async Task Init()
        {
            if (!IsRunning)
            {
                await FullTrustProcessLauncher.LaunchFullTrustProcessForCurrentAppAsync();
            }
            IsRunning = true;
        }

        public Task GetStatus()
        {
            throw new NotImplementedException();
        }

        public async Task Start(WalletConf wallet, string filename)
        {
            if (!IsRunning)
                throw new Exception();
            ValueSet valueSet = new ValueSet
            {
                { "Command", "Init" },
                { "password", wallet.RPCPassword },
                { "port", wallet.Port },
                { "filename", filename }
            };
            AppServiceResponse response = await App.Connection.SendMessageAsync(valueSet);
            System.Diagnostics.Debug.WriteLine("*** Message Returned: ***");
            System.Diagnostics.Debug.WriteLine("  RESPONSE IS: " + response.Message["Response"] as string);
            System.Diagnostics.Debug.WriteLine("  STATUS IS: " + response.Message["Status"] as string);

            valueSet = new ValueSet
            {
                { "Command", "Start" }
            };
            response = await App.Connection.SendMessageAsync(valueSet);
            System.Diagnostics.Debug.WriteLine("*** Message Returned: ***");
            System.Diagnostics.Debug.WriteLine("  RESPONSE IS: " + response.Message["Response"] as string);
            System.Diagnostics.Debug.WriteLine("  STATUS IS: " + response.Message["Status"] as string);
        }

        public async Task Stop()
        {
            if (!IsRunning)
                throw new Exception();
            ValueSet valueSet = new ValueSet
            {
                { "Command", "Stop" }
            };
            AppServiceResponse response = await App.Connection.SendMessageAsync(valueSet);
            System.Diagnostics.Debug.WriteLine("*** Message Returned: ***");
            System.Diagnostics.Debug.WriteLine("  RESPONSE IS: " + response.Message["Response"] as string);
            System.Diagnostics.Debug.WriteLine("  STATUS IS: " + response.Message["Status"] as string);
        }
    }
}
