using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;
using Statiq.App;
using Statiq.Common;

namespace FP.Statiq.RevealJS.Business
{
    public class FileWatchCommand : PipelinesCommand<PipelinesCommandSettings>
    {
        
        public FileWatchCommand(IConfiguratorCollection configurators, Settings settings, IServiceCollection serviceCollection, IFileSystem fileSystem, Bootstrapper bootstrapper) :
            base(configurators, settings, serviceCollection, fileSystem, bootstrapper)
        {
            
        }
        
        bool hasChanges = false;
        
        protected override async Task<int> ExecuteEngineAsync(CommandContext commandContext, PipelinesCommandSettings commandSettings,
            IEngineManager engineManager)
        {
            SetPipelines(commandContext, commandSettings, engineManager);
            
            using CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            var consoleListener = new ConsoleListener(() =>
            {
                cancellationTokenSource.Cancel();
                return Task.CompletedTask;
            });
            
            // first Run
            var completeResult = await engineManager.ExecuteAsync(cancellationTokenSource);

            if (completeResult != ExitCode.Normal)
            {
                return (int)completeResult;
            }
            do
            {
                var slidesFolder = new DirectoryInfo(commandSettings.InputPaths.First()).Parent;

                using FileSystemWatcher fileSystemWatcher = new FileSystemWatcher(slidesFolder.FullName);
                fileSystemWatcher.IncludeSubdirectories = true;
                fileSystemWatcher.Changed += FileSystemWatcherOnChanged;
                fileSystemWatcher.Created += FileSystemWatcherOnChanged;
                fileSystemWatcher.Deleted += FileSystemWatcherOnChanged;
                fileSystemWatcher.EnableRaisingEvents = true;

                while (!cancellationTokenSource.IsCancellationRequested)
                {
                    if (hasChanges)
                    {
                        hasChanges = false;
                        var result2 = await engineManager.ExecuteAsync(cancellationTokenSource);
                        if (result2 != ExitCode.Normal)
                        {
                            return (int)result2;
                        }
                    }

                    await Task.Delay(TimeSpan.FromSeconds(5), cancellationTokenSource.Token);
                }
            } while (!cancellationTokenSource.IsCancellationRequested);

            return 0;
        }

        private void FileSystemWatcherOnChanged(object sender, FileSystemEventArgs e)
        {
            if (!hasChanges && !e.FullPath.Contains("\\.git\\") && !e.FullPath.Contains("/.git/"))
            {
                hasChanges = true;
            }
        }
    }
}