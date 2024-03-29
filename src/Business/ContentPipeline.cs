using Statiq.Common;
using Statiq.Core;
using Statiq.Minification;

namespace FP.Statiq.RevealJS.Business;

public class ContentPipeline : Pipeline
{
    public ContentPipeline(IReadOnlySettings readOnlySettings)
    {
        var patterns = "*.rsd";
        if(readOnlySettings.TryGetValue("patterns", out var settingValue) &&
           settingValue != null && !string.IsNullOrEmpty(settingValue.ToString()))
        {
            patterns = settingValue.ToString();
        }
        InputModules.Add(new ReadFiles(patterns));
        ProcessModules.Add(new LoadSlideDesk());
        ProcessModules.Add(new LoadSections());
        ProcessModules.Add(new EmbeedImages());
        PostProcessModules.Add(new RenderDeck());
        PostProcessModules.Add(new EncryptContent());
        PostProcessModules.Add(new MinifyHtml());
        OutputModules.Add(new WriteFiles());
    }
}