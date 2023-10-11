using FIFAWebAppWithWorkers.Models;
using FIFAWebAppWithWorkers.Shared;
using FIFAWebAppWithWorkers.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net;
using System.Web.Http;

namespace FIFAWebAppWithWorkers.Workers;

public class SearchWorker : BackgroundService, IWorkerBase
{
    private readonly ILogger<SearchWorker> Logger;
    private readonly IAPITasks APITasks;
    private readonly SearchWorkerAppSettingsModel SearchOptions;
    private readonly SessionAppSettingsModel SessionOptions;
    private readonly IHostApplicationLifetime HostApplicationLifetime;

    public SearchWorker(
        IAPITasks apiTasks,
        ILogger<SearchWorker> logger,
        IOptions<SearchWorkerAppSettingsModel> searchWorkerOptions,
        IOptions<SessionAppSettingsModel> sessionOptions,
        IHostApplicationLifetime hostApplicationLifetime)
    {
        Logger = logger;
        APITasks = apiTasks;
        SearchOptions = searchWorkerOptions.Value;
        SessionOptions = sessionOptions.Value;
        HostApplicationLifetime = hostApplicationLifetime;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var errorCount = 0;

        while (cancellationToken.IsCancellationRequested == false)
        {
            try
            {
                await DoWork();
                await Task.Delay(SearchOptions.Interval, cancellationToken);

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
        foreach (var player in SessionOptions.Players)
        {
            var marketResults = await APITasks.SearchPlayerAsync(player);
            APITasks.ValidateResponse(marketResults);

            var deserialisedResponse = JsonConvert.DeserializeObject<SearchResultsModel>(marketResults.Content)!;

            if (deserialisedResponse.auctionInfo.Count == 0)
            {
                player.Price = Utils.GetNextPrice(player.Price);
                continue;
            }

            if (deserialisedResponse.auctionInfo.Count > 20)
            {
                player.Price = Utils.GetPreviousPrice(player.Price);
                continue;
            }

            // Buy players where profit can be made
            var playersToBuy = deserialisedResponse.auctionInfo
                .Where(p => p.buyNowPrice < (player.Price * .94))
                .OrderBy(p => p.buyNowPrice);

            foreach (var playerToBuy in playersToBuy)
            {
                var response = await APITasks.BuyPlayerAsync(playerToBuy.tradeId, playerToBuy.buyNowPrice);
                APITasks.ValidateResponse(response);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var buyPlayerResponse = JsonConvert.DeserializeObject<BidResponseModel>(response.Content);

                    Logger.Log(
                        LogLevel.Warning,
                        $"{player.Name} was purchased for {playerToBuy.buyNowPrice}. Remaining credits: {buyPlayerResponse!.credits}");

                    response = await APITasks.MovePlayerToTradepileAsync(playerToBuy.itemData.id);
                    continue;
                }

                if (response.StatusCode == (HttpStatusCode)470)
                {
                    Logger.Log(LogLevel.Warning, $"{player.Name} was not purchased for {playerToBuy.buyNowPrice} due to " +
                        $"insufficient credits available. Sleeping for 1 minute");
                    Thread.Sleep(60000);
                    continue;
                }
            }
        }
    }
}
