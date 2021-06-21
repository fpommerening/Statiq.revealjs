using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FP.Statiq.RevealJS.Properties;
using Statiq.Common;

namespace FP.Statiq.RevealJS.Business
{
    public class RenderDeck : Module
    {
        protected override async Task<IEnumerable<IDocument>> ExecuteContextAsync(IExecutionContext context)
        {
            var orderedDocuments = context.Inputs.Sort(new DocumentMetadataComparer<IDocument, int>("Position"));
            var sb = new StringBuilder();
            foreach (var document in orderedDocuments)
            {
                var reader = document.ContentProvider.GetTextReader();
                sb.AppendLine(await reader.ReadToEndAsync());
            }

            var content = Resources.reveal_template.Replace("[[SLIDES]]", sb.ToString());
            var firstInput = orderedDocuments[0];
            var access = firstInput[MetadataKeys.SlideDeskAccess];

            var doc = new Document($"{access}/index.html", firstInput, context.GetContentProvider(content,
                MediaTypes.Html));


            return doc.Yield();
        }
    }
}
