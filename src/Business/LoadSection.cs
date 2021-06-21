using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Statiq.Common;


namespace FP.Statiq.RevealJS.Business
{
    public class LoadSections : ParallelModule
    {
        protected override async Task<IEnumerable<IDocument>> ExecuteInputAsync(IDocument input, IExecutionContext context)
        {
            var baseUrl = context.Settings["baseUrl"].ToString();
            var metadataItems = input.GetEnumerable();

            if (!(input.ContentProvider is NullContent))
            {
                return input.Yield();
            }

            var path = Path.Combine(baseUrl, input[MetadataKeys.SectionPath]+".html");
            var data = await File.ReadAllTextAsync(path);
            return (new Document(metadataItems, context.GetContentProvider($"<section>{data}</section>", MediaTypes.HtmlFragment))).Yield();
        }
    }
}
