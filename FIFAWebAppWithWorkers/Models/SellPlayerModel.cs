namespace FIFAWebAppWithWorkers.Models
{
    public class SellPlayerModel
    {
        public int buyNowPrice { get; set; }
        public int duration { get; set; }
        public ItemData itemData { get; set; }

        public class ItemData
        {
            public long id { get; set; }
        }

        public int startingBid { get; set; }
    }
}
