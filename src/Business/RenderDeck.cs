using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using FP.Statiq.RevealJS.Properties;
using Statiq.Common;
using Statiq.Html;

namespace FP.Statiq.RevealJS.Business
{
    public class RenderDeck : Module
    {
        protected override async Task<IEnumerable<IDocument>> ExecuteContextAsync(IExecutionContext context)
        {
            var orderedDocuments = context.Inputs.Sort(new DocumentMetadataComparer<IDocument, int>("Position"));
            var firstInput = orderedDocuments[0];

            var parser = new HtmlParser();
            var htmlDocument = await parser.ParseDocumentAsync(Resources.reveal_template);

            var sb = new StringBuilder();
            foreach (var reader in orderedDocuments.Select(document => document.ContentProvider.GetTextReader()))
            {
                sb.AppendLine(await reader.ReadToEndAsync());
            }
            FillElement(htmlDocument, "div.slides", e => e.InnerHtml = sb.ToString());

            var copyright = firstInput[MetadataKeys.SlideDeskCopyright]?.ToString();
            FillElement(htmlDocument, "div.copyright", e => e.InnerHtml = copyright);

            htmlDocument.Head.Title = firstInput[MetadataKeys.SlideDeskTitle].ToString();

            var access = firstInput[MetadataKeys.SlideDeskAccess];

            using (var contentStream = context.GetContentStream())
            using (var writer = contentStream.GetWriter())
            {
                htmlDocument.ToHtml(writer, ProcessingInstructionFormatter.Instance);
                writer.Flush();
                var doc = new Document($"{access}/index.html", firstInput, context.GetContentProvider(contentStream,
                    MediaTypes.Html));
                return doc.Yield();
            }

        }

        private void FillElement(IHtmlDocument htmlDocument, string querySelector, Action<AngleSharp.Dom.IElement> fillAction )
        {
            var element = htmlDocument.QuerySelector(querySelector);
            if (element != null)
            {
                fillAction(element);
            }
        }
    }
}
