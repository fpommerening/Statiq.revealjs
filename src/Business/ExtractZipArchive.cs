using System.IO.Compression;
using Statiq.Common;

namespace FP.Statiq.RevealJS.Business;

public class ExtractZipArchive : ParallelModule
{
    private readonly string _replaceFolder;

    public ExtractZipArchive(string replaceFolder)
    {
        _replaceFolder = replaceFolder;
    }

    protected override Task<IEnumerable<IDocument>> ExecuteInputAsync(IDocument input, IExecutionContext context)
    {
        var result = new List<IDocument>();

        using(var zip = new ZipArchive(input.ContentProvider.GetStream(), ZipArchiveMode.Read))
        {
            foreach (var entry in zip.Entries)
            {
                if (entry.Length == 0)
                {
                    continue; // Directory
                }

                var outputPath = entry.FullName;
                if (!string.IsNullOrEmpty(_replaceFolder))
                {
                    outputPath = outputPath.Substring(outputPath.IndexOf("/") + 1);
                    outputPath = $"{_replaceFolder}/{outputPath}";
                }
                var mediaType = "text/plain";

                if (MediaTypes.TryGet(entry.FullName, out var mt, true))
                {
                    mediaType = mt;
                }
                result.Add(new Document(outputPath, context.GetContentProvider(entry.Open(), mediaType)));
            }
        }

        return Task.FromResult((IEnumerable<IDocument>) result);
    }
}