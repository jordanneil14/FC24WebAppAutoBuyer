using System.Net;

namespace FIFAWebAppWithWorkers.Network;

public class NetworkResponse
{
    public HttpStatusCode StatusCode { get; set; }
    public string Content { get; set; } = "";
}
