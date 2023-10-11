
using FIFAWebAppWithWorkers;
using FIFAWebAppWithWorkers.Network;
using FIFAWebAppWithWorkers.Tasks;
using FIFAWebAppWithWorkers.Workers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Configuration;
using System.Runtime;


internal class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureLogging((context, builder) =>
            {
            })
            .ConfigureServices((hostContext, services) =>
            {
                services.AddSingleton<IWorkerBase, SearchWorker>();
                services.AddSingleton<IWorkerBase, TradepileWorker>();
                services.AddSingleton<IAPITasks, APITasks>();
                services.AddSingleton<INetworkHelper, NetworkHelper>();

                services.AddHostedService<TradepileWorker>();
                services.AddHostedService<SearchWorker>();

                services.AddOptions<TradepileWorkerModel>().Bind(hostContext.Configuration.GetSection("TradepileWorker"));
                services.AddOptions<SearchWorkerAppSettingsModel>().Bind(hostContext.Configuration.GetSection("SearchWorker"));
                services.AddOptions<SessionAppSettingsModel>().Bind(hostContext.Configuration.GetSection("Session"));
            });
    
}