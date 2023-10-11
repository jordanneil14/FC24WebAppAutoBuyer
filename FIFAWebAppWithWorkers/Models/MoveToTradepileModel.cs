namespace FIFAWebAppWithWorkers.Models
{
    public class MoveToTradepileModel
    {
        public List<ItemDatum> itemData { get; set; }

        public class ItemDatum
        {
            public long id { get; set; }
            public string pile { get; set; }
        }
    }
}
