using System;
using System.Text.Json;
using System.Threading.Tasks;
using AngleSharp.Dom;
using FP.Statiq.RevealJS.Business;
using FP.Statiq.RevealJS.Models;
using Jint.Parser.Ast;
using Microsoft.Extensions.DependencyInjection;
using Statiq.App;
using Statiq.Common;
using Statiq.Html;
using Statiq.Images;

namespace FP.Statiq.RevealJS
{
    class Program
    {
        public static async Task<int> Main(string[] args)
        {
            args = new[] {"--base", "test"};

            return 
            await Bootstrapper
                .Factory
                .CreateDefault(args)
                
                .ConfigureServices((services, config) =>
                {
                    services.AddHttpClient();
                    services.AddTransient<EmbeedImages>();
                })
                .AddDefaultConfigurationFiles()
                .AddInputPath(@"C:\projects\Statiq.revealjs\data")
                .AddPipeline<ContentPipeline>()

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

