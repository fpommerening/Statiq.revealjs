using Statiq.Core;

namespace FP.Statiq.RevealJS.Business
{
    public class ContentPipeline : Pipeline
    {
        public ContentPipeline(EmbeedImages embeedImages)
        { 

            InputModules.Add(new ReadFiles("*.rsd"));
            ProcessModules.Add(new LoadSlideDesk());
            ProcessModules.Add(new LoadSections());
            ProcessModules.Add(embeedImages);
            PostProcessModules.Add(new RenderDeck());
            PostProcessModules.Add(new EncryptContent());
            OutputModules.Add(new WriteFiles());

        }
    }
}
