using Statiq.Core;

namespace FP.Statiq.RevealJS.Business
{
    public class FrameworkPipeline : Pipeline
    {
        public FrameworkPipeline()
        {
            InputModules.Add(new DownloadGitHub("fpommerening", "presentation", "reveal-js-4.1.2"));
            ProcessModules.Add(new ExtractZipArchive("presentation"));
            OutputModules.Add(new WriteFiles());
        }
    }
}
