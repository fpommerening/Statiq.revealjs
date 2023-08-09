using System.Threading.Tasks;
using FP.Statiq.RevealJS.Business;
using Microsoft.Extensions.DependencyInjection;
using Statiq.App;
using Statiq.Common;

namespace FP.Statiq.RevealJS
{
    class Program
    {
        public static async Task<int> Main(string[] args)
        {
            
            return await Bootstrapper
                .Factory
                .CreateDefault(args)
                .ConfigureServices((services, config) =>
                {
                    services.AddSingleton<ImageCache>();
                })
                .AddPipeline<ContentPipeline>()
                .AddPipeline<FrameworkPipeline>()
                .AddCommand<FileWatchCommand>("watch")
                .RunAsync();
        }
    }
}

