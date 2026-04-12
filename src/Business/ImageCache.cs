using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Statiq.Common;

namespace FP.Statiq.RevealJS.Business;

public class ImageCache
{
    private readonly ConcurrentDictionary<string, byte[]> _files = new ConcurrentDictionary<string, byte[]>();
    public async Task<byte[]>DownloadImage(IExecutionContext context, string src)
    {
        if (_files.TryGetValue(src, out var dataFromCache))
        {
            return dataFromCache;
        }
        var uri = new Uri(src);
        context.Log(LogLevel.Information, $"Downloading {uri}");
        var imageResult = await context.SendHttpRequestWithRetryAsync(() => new HttpRequestMessage(HttpMethod.Get, uri), 2);
        context.Log(LogLevel.Information, $"Downloaded {uri}");
        imageResult.EnsureSuccessStatusCode();
        var data = await imageResult.Content.ReadAsByteArrayAsync();
        _files[src] = data;
        return data;
    }
}