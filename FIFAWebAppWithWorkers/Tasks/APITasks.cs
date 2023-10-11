using FIFAWebAppWithWorkers.Models;
using FIFAWebAppWithWorkers.Network;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net;
using System.Numerics;
using System.Security.Policy;
using System.Web.Http;

namespace FIFAWebAppWithWorkers.Tasks
{
    public interface IAPITasks
    {
        Task<NetworkResponse> BuyPlayerAsync(long tradeId, int price);
        Task<NetworkResponse> GetTradepileAsync();
        Task<NetworkResponse> MovePlayerToTradepileAsync(long id);
        Task<NetworkResponse> SearchPlayerAsync(PlayerModel player);
        Task<NetworkResponse> SellPlayerAsync(long id, int buyNowPrice, int startingPrice);
        Task<NetworkResponse> RemovePlayerFromTradepileAsync(long id);
        void ValidateResponse(NetworkResponse response);
    }

    public class APITasks : IAPITasks
    {
        private readonly INetworkHelper NetworkHelper;

        public APITasks(
            INetworkHelper networkHelper, 
            IOptions<SessionAppSettingsModel> sessionOptions)
        {
            NetworkHelper = networkHelper;
            Headers.Add("X-UT-SID", sessionOptions.Value.ID);
        }

        private readonly Dictionary<string, string> Headers = new()
        {
            { "Host", "utas.mob.v1.fut.ea.com" },
            { "Connection", "keep-alive" },
            { "sec-ch-ua", @"Google Chrome""; v = ""105"", ""Not)A;Brand""; v = ""8"", ""Chromium""; v = ""105""" },
            { "Cache-Control", "no-cache" },
            { "Content-Type", "application/json" },
            { "sec-ch-ua-mobile", "0" },
            { "User-Agent", "Mozilla/5.0(Windows NT 10.0; Win64; x64) AppleWebKit / 537.36(KHTML, like Gecko) Chrome / 105.0.0.0 Safari / 537.36" },
            { "sec-ch-ua-platform", "Windows"},
            { "Accept", "*/*" },
            { "Origin", "https://www.ea.com" },
            { "Sec-Fetch-Site", "same-site" },
            { "Sec-Fetch-Mode", "cors" },
            { "Sec-Fetch-Dest", "empty" },
            { "Referer", "https://www.ea.com/" },
            { "Accept-Language", "en-GB,en-US;q=0.9,en;q=0.8" },
        };

        public void ValidateResponse(NetworkResponse response)
        {
            if (response.StatusCode == HttpStatusCode.OK) return;
            if (response.StatusCode == (HttpStatusCode)461) return; //Item sold
            if (response.StatusCode == (HttpStatusCode)478) return; //Invalid bid
            if (response.StatusCode == (HttpStatusCode)470) return; //Insufficient credits
            throw new HttpResponseException(response.StatusCode);
        }

        public async Task<NetworkResponse> GetTradepileAsync()
        {
            var url = "https://utas.mob.v1.fut.ea.com/ut/game/fc24/tradepile";
            var response = await NetworkHelper.GetAsync(url, Headers);
            return new NetworkResponse
            {
                StatusCode = response.StatusCode,
                Content = response.Content
            };
        }

        public async Task<NetworkResponse> MovePlayerToTradepileAsync(long id)
        {
            var url = "https://utas.mob.v1.fut.ea.com/ut/game/fc24/item";
            var movetoTradepileObject = new MoveToTradepileModel
            {
                itemData = new List<MoveToTradepileModel.ItemDatum>()
                {
                    new MoveToTradepileModel.ItemDatum
                    {
                        id = id,
                        pile = "trade"
                    }
                }
            };
            var movetoTradepileObjectSerialised = JsonConvert.SerializeObject(movetoTradepileObject);

            var response = await NetworkHelper.PutAsync(
                url, 
                Headers, 
                new StringContent(movetoTradepileObjectSerialised));

            return new NetworkResponse
            {
                StatusCode = response.StatusCode,
                Content = response.Content
            };
        }

        public async Task<NetworkResponse> SellPlayerAsync(long id, int buyNowPrice, int startingPrice)
        {
            var url = "https://utas.mob.v1.fut.ea.com/ut/game/fc24/auctionhouse";

            var bidObject = new SellPlayerModel
            {
                duration = 3600,
                buyNowPrice = buyNowPrice,
                startingBid = startingPrice,
                itemData = new SellPlayerModel.ItemData
                {
                    id = id
                }
            };
            var bidObjectSerialised = JsonConvert.SerializeObject(bidObject);

            var response = await NetworkHelper.PostAsync(
                url, 
                Headers, 
                new StringContent(bidObjectSerialised));

            return new NetworkResponse
            {
                StatusCode = response.StatusCode,
                Content = response.Content
            };
        }
        public async Task<NetworkResponse> BuyPlayerAsync(long tradeId, int price)
        {
            var url = $"https://utas.mob.v1.fut.ea.com/ut/game/fc24/trade/{tradeId}/bid";

            var bidObject = new BidModel { bid = price };
            var bidObjectSerialised = JsonConvert.SerializeObject(bidObject);

            var response = await NetworkHelper.PutAsync(url, Headers, new StringContent(bidObjectSerialised));

            return new NetworkResponse
            {
                StatusCode = response.StatusCode,
                Content = response.Content
            };
        }

        public async Task<NetworkResponse> SearchPlayerAsync(PlayerModel player)
        {
            // Randomise search to ensure results are updated
            var list = new List<int> { 50, 100, 150, 200, 250, 300 };
            var list2 = new List<int> { 350, 400, 450, 500, 550 };
            var minPrice = list[new Random().Next(list.Count)];
            var minBuyNowPrice = list2[new Random().Next(list2.Count)];
            var url = $"https://utas.mob.v1.fut.ea.com/ut/game/fc24/transfermarket?num=21&start=0&type=player&maskedDefId={player.Id}&micr={minPrice}&minb={minBuyNowPrice}&maxb={player.Price}";

            var response = await NetworkHelper.GetAsync(url, Headers);

            return new NetworkResponse
            {
                StatusCode = response.StatusCode,
                Content = response.Content
            };
        }

        public async Task<NetworkResponse> RemovePlayerFromTradepileAsync(long id)
        {
            var url = $"https://utas.mob.v1.fut.ea.com/ut/game/fc24/trade/{id}";
            var response = await NetworkHelper.DeleteAsync(url, Headers);

            return new NetworkResponse
            {
                StatusCode = response.StatusCode,
                Content = response.Content
            };
        }
    }
}
