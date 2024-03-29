using Statiq.Core;

namespace FP.Statiq.RevealJS.Business;

public class FrameworkPipeline : Pipeline
{
    public FrameworkPipeline()
    {
        InputModules.Add(new DownloadGitHub("fpommerening", "presentation", "reveal-js-5.0.5").CacheResponses());
        ProcessModules.Add(new ExtractZipArchive("presentation"));
        OutputModules.Add(new WriteFiles());
    }
}