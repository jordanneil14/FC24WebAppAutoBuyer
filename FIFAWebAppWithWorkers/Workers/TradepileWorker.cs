using FIFAWebAppWithWorkers.Models;
using FIFAWebAppWithWorkers.Shared;
using FIFAWebAppWithWorkers.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Web.Http;

namespace FIFAWebAppWithWorkers.Workers;

public class TradepileWorker : BackgroundService, IWorkerBase
{
    private readonly ILogger<TradepileWorker> Logger;
    private readonly IAPITasks APITasks;
    private readonly TradepileWorkerModel TradepileOptions;
    private readonly SessionAppSettingsModel SessionOptions;
    private readonly IHostApplicationLifetime HostApplicationLifetime;

    public TradepileWorker(
        IAPITasks apiTasks, 
        ILogger<TradepileWorker> logger, 
        IOptions<TradepileWorkerModel> tradepileOptions,
        IOptions<SessionAppSettingsModel> sessionOptions,
        IHostApplicationLifetime hostApplicationLifetime)
    {
        Logger = logger;
        APITasks = apiTasks;
        TradepileOptions = tradepileOptions.Value;
        SessionOptions = sessionOptions.Value;
        HostApplicationLifetime = hostApplicationLifetime;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var errorCount = 0;

        while (stoppingToken.IsCancellationRequested == false)
        {
            try
            {
                await DoWork();
                await Task.Delay(TradepileOptions.Interval, stoppingToken);

                if (errorCount > 0)
                {
                    errorCount = 0;
                    Logger.Log(
                    LogLevel.Warning,
                    $"Resetting errors to 0");
                }
            }
            catch (HttpResponseException ex)
            {
                errorCount++;
                Logger.Log(
                    LogLevel.Warning,
                    $"Status Code {ex.Response.StatusCode} was returned. Error Count: {errorCount}");

                if (errorCount >= 5) HostApplicationLifetime.StopApplication();
            }
            catch (Exception)
            {
                HostApplicationLifetime.StopApplication();
            }                
        }
    }

    public async Task DoWork()
    {
        var response = await APITasks.GetTradepileAsync();
        APITasks.ValidateResponse(response);

        var deserialisedResponse = JsonConvert.DeserializeObject<TradepileModel>(response.Content)!;

        // Filter out any players which are not in our filters
        var filteredPlayers = deserialisedResponse.auctionInfo
            .Where(item => SessionOptions.Players.Any(player => player.Id == item.itemData.assetId && player.SellItem));

        // Clear sold items
        var expiredSoldItems = filteredPlayers.Where(item => item.expires == -1 && item.currentBid > 0);
        foreach (var item in expiredSoldItems)
        {
            Thread.Sleep(4000);

            var player = SessionOptions.Players.Where(p => p.Id == item.itemData.assetId)!.First();
            Logger.Log(LogLevel.Warning, $"{player.Name} sold for {item.currentBid}");

            await APITasks.RemovePlayerFromTradepileAsync(item.tradeId);
        }

        // Relist unsold items back onto the market
        var expiredUnsoldItems = filteredPlayers.Where(item => item.expires <= 0 && item.currentBid == 0);
        foreach (var item in expiredUnsoldItems)
        {
            Thread.Sleep(4000);

            var buyNowPrice = SessionOptions.Players
                .Where(player => player.Id == item.itemData.assetId).FirstOrDefault()!.Price;
            var startingPrice = Utils.GetPreviousPrice(buyNowPrice);
            var sellPlayerResponse = await APITasks.SellPlayerAsync(item.itemData.id, buyNowPrice, startingPrice);
            APITasks.ValidateResponse(sellPlayerResponse);

            var player = SessionOptions.Players.Where(p => p.Id == item.itemData.assetId)!.First();
            Logger.Log(LogLevel.Warning, $"{player.Name} listed for {buyNowPrice}");
        }
    }
}
