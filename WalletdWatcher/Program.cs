using System;
using System.Linq;
using System.Threading;
using Windows.Foundation.Collections;
using Windows.ApplicationModel.AppService;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;
using TurtleCoinRPC;
using Spooky;

namespace WalletdWatcher
{
    class Program
    {
        static AppServiceConnection connection = null;
        static Process _process;
        static string _walletRPCPassword;
        static string _walletConfigAbsFileName = "walletd.conf";
        private static WalletRPCHttpClient _walletClient;
        static int _walletPort;

        static string _curLocation;
        static bool _initialized = false;
        static void Main(string[] args)
        {
            Thread appServiceThread = new Thread(new ThreadStart(ThreadProc));
            appServiceThread.Start();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("*****************************");
            Console.WriteLine("**** Classic desktop app ****");
            Console.WriteLine("*****************************");
            Console.ReadLine();

        }

        static async void ThreadProc()
        {
            connection = new AppServiceConnection
            {
                AppServiceName = "CommunicationService",
                PackageFamilyName = Windows.ApplicationModel.Package.Current.Id.FamilyName
            };
            connection.RequestReceived += HandleMessageFromApp;
            AppServiceConnectionStatus status = await connection.OpenAsync();
            switch (status)
            {
                case AppServiceConnectionStatus.Success:
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Connection established - waiting for requests");
                    Console.WriteLine();
                    break;
                case AppServiceConnectionStatus.AppNotInstalled:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("The app AppServicesProvider is not installed.");
                    return;
                case AppServiceConnectionStatus.AppUnavailable:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("The app AppServicesProvider is not available.");
                    return;
                case AppServiceConnectionStatus.AppServiceUnavailable:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(string.Format("The app AppServicesProvider is installed but it does not provide the app service {0}.", connection.AppServiceName));
                    return;
                case AppServiceConnectionStatus.Unknown:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(string.Format("An unkown error occurred while we were trying to open an AppServiceConnection."));
                    return;
            }
        }

        /// <summary>
        /// Receives message from UWP app and sends a response back
        /// </summary>
        private static void HandleMessageFromApp(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {
            string key = args.Request.Message["Command"] as string;
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("KEY IS: " + key);
            ValueSet response = BuildResponseValueSet("Unknown Error!", false);
            if (key == "Init")
            {
                _curLocation = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
                if (_initialized) return;
                Console.WriteLine("INIT ATTEMPT RECEIVED!");
                Init(args.Request.Message["password"] as string, args.Request.Message["filename"] as string, (int)args.Request.Message["port"]);
                response = BuildResponseValueSet("Walletd Initialized!", true);
                _initialized = true;
            }
            else
            {
                Console.WriteLine("Key is NOT init, key is " + key + " and initialized " + _initialized);
                if (!_initialized) return;
                if (key == "Start")
                {
                    Console.WriteLine("START ATTEMPT RECEIVED!");
                    Start();
                    response = BuildResponseValueSet("Walletd Started!", true);
                }
                if (key == "Restart")
                {

                }
                if (key == "Stop")
                {
                    Stop();
                    response = BuildResponseValueSet("Walletd Stopped!", true);
                }
            }
            try
            {
                args.Request.SendResponseAsync(response).Completed += delegate { };
            } 
            catch (Exception ex) {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error sending back message! " + ex.Message);
            }
        }

        private static ValueSet BuildResponseValueSet(string message, bool status)
        {
            return new ValueSet {
                {"Response", message },
                {"Status", status }
            };
        }


        private static void Init(string wc, string fn, int port, bool reinit = false)
        {
            if (string.IsNullOrEmpty(wc)) return;

            if (_initialized && !reinit) return;

            _process = new Process();
            _process.StartInfo.RedirectStandardOutput = false;
            _process.StartInfo.UseShellExecute = true;
            _process.StartInfo.CreateNoWindow = false;

            _walletRPCPassword = wc;
            _walletPort = port;
            _walletConfigAbsFileName = fn;
            _walletClient = new WalletRPCHttpClient(new Uri(string.Format("http://localhost:{0}/json_rpc", _walletPort)), _walletRPCPassword);

            // Setup executable and parameters
            Console.WriteLine("**************************");
            Console.WriteLine("***BEGIN INIT PROCEDURE***");
            Console.WriteLine("**************************");

            _process.StartInfo.WorkingDirectory = _curLocation;
            _process.StartInfo.FileName = _curLocation + "\\walletd.exe";
            _process.StartInfo.Arguments = string.Format("--rpc-password {0} --local --config {1}", _walletRPCPassword, _walletConfigAbsFileName);

            Console.WriteLine("INITIALIZED TO START: " + _process.StartInfo.FileName);
            Console.WriteLine("WITH ARGS: " + _process.StartInfo.Arguments);

            _process.Exited += WalletdLoop;
        }

        private static void WalletdLoop(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private static void Start()
        {
            // Go
            Console.WriteLine("ATTEMPTING TO START: " + _process.StartInfo.FileName);
            Console.WriteLine("WITH ARGS: " + _process.StartInfo.Arguments);

            bool success = _process.Start();
            if (!success)
                throw new System.IO.IOException();
        }
        private static async void Stop()
        {
            Console.WriteLine(_walletClient._password);
            var answer = await _walletClient.Invoke<dynamic>
            (
                "save",
                new Dictionary<string, object>() { }
            ).ConfigureAwait(false);

            Console.WriteLine("RPC SAYS: " + answer);
            Console.WriteLine("Attempting to Kill Walletd!");
            
        }
        private static void Kill()
        {
            _process.Kill();
        }

        private static void Shutdown()
        {

        }

        private static void Restart()
        {
            Console.WriteLine("Process was killed; launching again");
            Init(_walletRPCPassword, _walletConfigAbsFileName, _walletPort, true);
            Start();
        }
    }
}
