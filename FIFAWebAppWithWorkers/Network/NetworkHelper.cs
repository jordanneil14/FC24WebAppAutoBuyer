namespace FIFAWebAppWithWorkers.Network;

public interface INetworkHelper
{
    Task<NetworkResponse> DeleteAsync(string url, Dictionary<string, string> headers);
    Task<NetworkResponse> GetAsync(string url, Dictionary<string, string> headers);
    Task<NetworkResponse> PostAsync(string url, Dictionary<string, string> headers, HttpContent content);
    Task<NetworkResponse> PutAsync(string url, Dictionary<string, string> headers, HttpContent content);
}

public class NetworkHelper : INetworkHelper
{
    public async Task<NetworkResponse> PutAsync(string url, Dictionary<string, string> headers, HttpContent content)
    {
        using var client = new HttpClient();
        foreach (var header in headers)
        {
            client.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);
        }

        var response = await client.PutAsync(url, content);

        return new NetworkResponse
        {
            StatusCode = response.StatusCode,
            Content = await response.Content.ReadAsStringAsync()
        };
    }

    public async Task<NetworkResponse> DeleteAsync(string url, Dictionary<string, string> headers)
    {
        using var client = new HttpClient();
        foreach (var header in headers)
        {
            client.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);
        }

        var response = await client.DeleteAsync(url);

        return new NetworkResponse
        {
            StatusCode = response.StatusCode,
            Content = await response.Content.ReadAsStringAsync()
        };
    }

    public async Task<NetworkResponse> PostAsync(string url, Dictionary<string, string> headers, HttpContent content)
    {
        using var client = new HttpClient();
        foreach (var header in headers)
        {
            client.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);
        }

        var response = await client.PostAsync(url, content);

        return new NetworkResponse
        {
            StatusCode = response.StatusCode,
            Content = await response.Content.ReadAsStringAsync()
        };
    }

    public async Task<NetworkResponse> GetAsync(string url, Dictionary<string, string> headers)
    {
        using var client = new HttpClient();
        foreach (var header in headers)
        {
            client.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);
        }

        var response = await client.GetAsync(url);

        return new NetworkResponse
        {
            StatusCode = response.StatusCode,
            Content = await response.Content.ReadAsStringAsync()
        };
    }
}
