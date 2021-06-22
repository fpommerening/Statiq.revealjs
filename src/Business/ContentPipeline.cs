using Statiq.Core;
using Statiq.Minification;

namespace FP.Statiq.RevealJS.Business
{
    public class ContentPipeline : Pipeline
    {
        public ContentPipeline()
        { 
            InputModules.Add(new ReadFiles("*.rsd"));
            ProcessModules.Add(new LoadSlideDesk());
            ProcessModules.Add(new LoadSections());
            ProcessModules.Add(new EmbeedImages());
            PostProcessModules.Add(new RenderDeck());
            PostProcessModules.Add(new EncryptContent());
            PostProcessModules.Add(new MinifyHtml());
            OutputModules.Add(new WriteFiles());
        }
    }
}
