using Statiq.Core;

namespace FP.Statiq.RevealJS.Business
{
    public class FrameworkPipeline : Pipeline
    {
        public FrameworkPipeline()
        {
            InputModules.Add(new DownloadGitHub("fpommerening", "presentation", "reveal-js-4.2.1"));
            ProcessModules.Add(new ExtractZipArchive("presentation"));
            OutputModules.Add(new WriteFiles());
        }
    }
}
