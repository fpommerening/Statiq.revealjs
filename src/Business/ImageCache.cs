using System.Collections.Concurrent;
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
            
        var imageResult = await context.SendHttpRequestWithRetryAsync(src);
        imageResult.EnsureSuccessStatusCode();
        var data = await imageResult.Content.ReadAsByteArrayAsync();
        _files[src] = data;
        return data;
    }
}