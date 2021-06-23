using System.Threading.Tasks;
using FP.Statiq.RevealJS.Business;
using Statiq.App;
using Statiq.Common;

namespace FP.Statiq.RevealJS
{
    class Program
    {
        public static async Task<int> Main(string[] args)
        {
            return 
            await Bootstrapper
                .Factory
                .CreateDefault(args)

                .ConfigureServices((services, config) =>
                {
                })
                //.AddDefaultConfigurationFiles()
                //.AddInputPath(@"C:\projects\Statiq.revealjs\data")
                .AddPipeline<ContentPipeline>()
                .AddPipeline<FrameworkPipeline>()

                //.BuildPipeline("", builder => builder
                //    .WithProcessModules(new LoadSlideDesk())
                //    .WithProcessModules(new LoadSections())
                //    .WithProcessModules (new EmbeedImages())
                //    .WithPostProcessModules((IModule)new RenderDeck())
                //    .WithPostProcessModules((IModule)new EncryptContent())
                //    .WithOutputWriteFiles())
                .RunAsync();
        }
    }
}

