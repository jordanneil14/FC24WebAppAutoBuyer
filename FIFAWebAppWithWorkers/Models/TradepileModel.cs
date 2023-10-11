namespace FIFAWebAppWithWorkers.Models;

public class TradepileModel
{
    public int credits { get; set; }
    public List<AuctionInfo> auctionInfo { get; set; }
    public BidTokens bidTokens { get; set; }

    public class AuctionInfo
    {
        public long tradeId { get; set; }
        public ItemData itemData { get; set; }
        public string tradeState { get; set; }
        public int buyNowPrice { get; set; }
        public int currentBid { get; set; }
        public int offers { get; set; }
        public bool watched { get; set; }
        public string bidState { get; set; }
        public int startingBid { get; set; }
        public int confidenceValue { get; set; }
        public int expires { get; set; }
        public object sellerName { get; set; }
        public int sellerEstablished { get; set; }
        public int sellerId { get; set; }
        public bool? tradeOwner { get; set; }
        public int coinsProcessed { get; set; }
        public string tradeIdStr { get; set; }
    }

    public class BidTokens
    {
    }

    public class ItemData
    {
        public long id { get; set; }
        public int timestamp { get; set; }
        public string formation { get; set; }
        public bool untradeable { get; set; }
        public int assetId { get; set; }
        public int rating { get; set; }
        public string itemType { get; set; }
        public int resourceId { get; set; }
        public int owners { get; set; }
        public int discardValue { get; set; }
        public string itemState { get; set; }
        public int cardsubtypeid { get; set; }
        public int lastSalePrice { get; set; }
        public string injuryType { get; set; }
        public int injuryGames { get; set; }
        public string preferredPosition { get; set; }
        public List<object> statsList { get; set; }
        public List<object> lifetimeStats { get; set; }
        public int contract { get; set; }
        public int teamid { get; set; }
        public int rareflag { get; set; }
        public int playStyle { get; set; }
        public int leagueId { get; set; }
        public int loyaltyBonus { get; set; }
        public int pile { get; set; }
        public int nation { get; set; }
        public int marketDataMinPrice { get; set; }
        public int marketDataMaxPrice { get; set; }
        public int resourceGameYear { get; set; }
        public string guidAssetId { get; set; }
        public List<int> groups { get; set; }
        public List<int> attributeArray { get; set; }
        public int skillmoves { get; set; }
        public int weakfootabilitytypecode { get; set; }
        public int attackingworkrate { get; set; }
        public int defensiveworkrate { get; set; }
        public int preferredfoot { get; set; }
        public List<string> possiblePositions { get; set; }
        public int? assists { get; set; }
        public int? lifetimeAssists { get; set; }
        public List<int> statsArray { get; set; }
        public List<int> lifetimeStatsArray { get; set; }
    }
}


