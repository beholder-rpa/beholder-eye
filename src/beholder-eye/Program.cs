namespace beholder_eye
{
    using Microsoft.AspNetCore.SignalR.Client;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using StackExchange.Redis;
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    class Program
    {
        private static CancellationTokenSource _ctsSource;

        private static NativeMethods.ConsoleCtrlEventHandler _handler;
        private static ILogger<Program> _logger;

        private static BeholderEye _beholderEye;
        private static CancellationTokenSource _beholderCtsSource;

        #region Trap application termination
        private static bool Handler(NativeMethods.CtrlType sig)
        {
            Console.WriteLine("Stopping due to external CTRL-C, or process kill, or shutdown");

            if (_ctsSource != null)
            {
                _ctsSource.Cancel();
            }

            Thread.Sleep(500);

            //shutdown right away so there are no lingering threads
            Environment.Exit(0);

            return true;
        }
        #endregion
        
        static async Task Main()
        {
            // Handle Ctrl-C + Close Window to gracefully exit.
            _handler += new NativeMethods.ConsoleCtrlEventHandler(Handler);
            NativeMethods.SetConsoleCtrlHandler(_handler, true);

            // Stand up DI
            var services = new ServiceCollection();
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables();
            var configuration = builder.Build();
            var startup = new Startup(configuration);
            startup.ConfigureServices(services);

            var serviceProvider = services.BuildServiceProvider();
            _logger = serviceProvider.GetService<ILogger<Program>>();

            // Configure the eye of the beholder
            _beholderEye = serviceProvider.GetRequiredService<BeholderEye>();

            // Connect to the Nexus
            var beholderNexusUrl = configuration["beholder_nexus_url"];
            _beholderEye.NexusConnection = await ConnectToHub(beholderNexusUrl);

            var redisUrl = configuration["beholder_redis_url"];
            _beholderEye.Redis = ConnectionMultiplexer.Connect(redisUrl);

            using (_ctsSource = new CancellationTokenSource())
            {
                // wait for process application termination.
                await Task.Delay(Timeout.Infinite, _ctsSource.Token)
                    .ContinueWith(_ => { }, TaskContinuationOptions.OnlyOnCanceled);
            }
        }

        private static async Task<HubConnection> ConnectToHub(string nexusUrl)
        {
            _logger.LogInformation($"Connecting to {nexusUrl}...");
            var nexusConnection = new HubConnectionBuilder()
                    .WithUrl(nexusUrl)
                    .WithAutomaticReconnect(new IndefiniteRetryPolicy())
                    .Build();

            // Start observing using the specified request.
            nexusConnection.On("StartObserving", (ObservationRequest req) =>
            {
                // Already Observing -- return.
                if (_beholderCtsSource != null)
                {
                    nexusConnection.SendAsync("Info", "Beholder Eye was already observing.");
                    return;
                }

                _beholderCtsSource = new CancellationTokenSource();
                _beholderEye.ObserveWithUnwaveringSight(req, _beholderCtsSource.Token);
            });

            // Stop observing the focus areas.
            nexusConnection.On("StopObserving", () =>
            {
                // Not Observing -- return.
                if (_beholderCtsSource != null)
                {
                    nexusConnection.SendAsync("Info", "Beholder Eye was not observing.");
                    return;
                }

                try
                {
                    _beholderCtsSource.Cancel();
                }
                finally
                {
                    _beholderCtsSource.Dispose();
                    _beholderCtsSource = null;
                }
            });

            nexusConnection.On("Align", (AlignRequest req) =>
            {
                // Not Observing -- return.
                if (_beholderCtsSource != null)
                {
                    nexusConnection.SendAsync("Info", "Beholder Eye was not observing.");
                    return;
                }

                _beholderEye.AlignRequest = req;
            });

            nexusConnection.On("Snapshot", (SnapshotRequest req) =>
            {
                // Not Observing -- return.
                if (_beholderCtsSource != null)
                {
                    nexusConnection.SendAsync("Info", "Beholder Eye was not observing.");
                    return;
                }

                _beholderEye.SnapshotRequest = req;
            });

            nexusConnection.Reconnected += async (arg) =>
            {
                // Report that we've reconnected.
                await nexusConnection.SendAsync("Status", "Eye Connected.");
                _logger.LogInformation($"Reconnected to {nexusUrl}.");
            };

            // TODO: Add capability to request to observe a screen region for changes and/or monitor a region and compare against a provided image.

            // Start the connection.
            await IndefiniteRetryPolicy.ConnectWithRetryAsync(nexusConnection, CancellationToken.None);

            // Report that we've connected.
            await nexusConnection.SendAsync("Status", "Eye Connected.");

            _logger.LogInformation($"Connected to {nexusUrl}.");
            return nexusConnection;
        }
    }
}
