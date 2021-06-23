using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Statiq.Common;

namespace FP.Statiq.RevealJS.Business
{
    public class ExtractZipArchive : ParallelModule
    {
        private readonly string _replaceFolder;

        public ExtractZipArchive()
        {

        }

        public ExtractZipArchive(string replaceFolder)
        {
            _replaceFolder = replaceFolder;
        }

        protected override async Task<IEnumerable<IDocument>> ExecuteInputAsync(IDocument input, IExecutionContext context)
        {
            var sourceUri = (Uri) input["SourceUri"];
            var result = new List<IDocument>();

            using(var zip = new ZipArchive(input.ContentProvider.GetStream(), ZipArchiveMode.Read))
            {
                foreach (var entry in zip.Entries)
                {
                    if (entry.Length == 0)
                    {
                        continue; // Directory
                    }

                    var outputPath = entry.FullName;
                    if (!string.IsNullOrEmpty(_replaceFolder))
                    {
                        outputPath = outputPath.Substring(outputPath.IndexOf("/") + 1);
                        outputPath = $"{_replaceFolder}/{outputPath}";
                    }
                    var mediaType = "text/plain";

                    if (MediaTypes.TryGet(entry.FullName, out var mt, true))
                    {
                        mediaType = mt;
                    }
                    result.Add(new Document(outputPath, context.GetContentProvider(entry.Open(), mediaType)));
                }
            }

            return result;
        }
    }
}
