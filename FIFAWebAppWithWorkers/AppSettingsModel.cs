using FIFAWebAppWithWorkers.Models;

namespace FIFAWebAppWithWorkers;

public class SearchWorkerAppSettingsModel
{
    public int Interval { get; set; }
}

public class TradepileWorkerModel
{ 
    public int Interval { get; set; }
}

public class SessionAppSettingsModel
{ 
    public string ID { get; set; } = String.Empty;
    public IEnumerable<PlayerModel> Players { get; set; }
}
