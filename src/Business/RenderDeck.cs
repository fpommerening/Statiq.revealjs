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

namespace FP.Statiq.RevealJS.Business
{
    public class RenderDeck : Module
    {
        protected override async Task<IEnumerable<IDocument>> ExecuteContextAsync(IExecutionContext context)
        {
            var groupedDocuments = context.Inputs.Sort(new DocumentMetadataComparer<IDocument, int>("Position"))
                .GroupBy<string>(MetadataKeys.SlideDeskAccess);

            var result = new List<IDocument>();

            foreach (var docs in groupedDocuments)
            {
                result.Add(await CreateSlideDesk(docs.ToList(), context));
            }
            return result;
        }

        private async Task<IDocument> CreateSlideDesk(List<IDocument> orderedDocuments, IExecutionContext context)
        {
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
            FillElement(htmlDocument, "div.copyright", e => e.TextContent = copyright);

            htmlDocument.QuerySelector("title").TextContent = firstInput[MetadataKeys.SlideDeskTitle].ToString();
            SetTheme(htmlDocument, firstInput);
            InitRevealScript(htmlDocument, firstInput);

            using (var contentStream = context.GetContentStream())
            using (var writer = contentStream.GetWriter())
            {
                htmlDocument.ToHtml(writer, StatiqMarkupFormatter.Instance);
                writer.Flush();
                var doc = new Document($"{firstInput[MetadataKeys.SlideDeskAccess]}/index.html", firstInput, context.GetContentProvider(contentStream,
                    MediaTypes.Html));
                return doc;
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

        private void SetTheme(IHtmlDocument htmlDocument, IDocument input)
        {
            var cssPath = Path.Combine("../presentation/dist/theme/", input[MetadataKeys.SlideDeskTheme].ToString());
            var links = htmlDocument.QuerySelectorAll("link");
            links.Single(x=>x.Id == "theme").SetAttribute("href", cssPath);
        }

        private void InitRevealScript(IHtmlDocument htmlDocument, IDocument input)
        {
            var scripts = htmlDocument.QuerySelectorAll("script");
            var script = scripts.Single(x => x.Id == "reveal-init");
            var sb = new StringBuilder();
            sb.AppendLine("Reveal.initialize({");
            sb.AppendLine("controls: false,");
            sb.AppendLine("progress: true,");
            sb.AppendLine("history: true,");
            sb.AppendLine("center: true,");
            sb.AppendLine("hash: true,");
            sb.AppendLine("width: 1250,");
            sb.AppendLine("height: 720,");
            sb.AppendLine("slideNumber: false,");
            sb.AppendLine("margin: 0.0,");
            sb.AppendLine("plugins:[RevealZoom, RevealNotes, RevealSearch, RevealHighlight, RevealMenu],");
            sb.AppendLine("menu:");
            sb.AppendLine("{");
            sb.AppendLine("numbers: 'c',");
            sb.AppendLine("openSlideNumber: true,");
            sb.AppendLine("themes: true,");
            sb.AppendLine("themesPath: '../presentation/dist/theme/',");
            sb.AppendLine("transitions: true,");
            sb.AppendLine("custom: false");
            sb.AppendLine("},");

            var multiplexUrl = input[MetadataKeys.MultiplexUrl]?.ToString();
            var multiplexSocketId = input[MetadataKeys.MultiplexId]?.ToString();
            var multiplexSecret = input[MetadataKeys.MultiplexSecret]?.ToString();

            if (string.IsNullOrEmpty(multiplexUrl) || string.IsNullOrEmpty(multiplexSocketId))
            {
                sb.AppendLine("dependencies:[");
                sb.AppendLine("{ src: '../presentation/plugin/countdown/countdown.js' },");
                sb.AppendLine("]");
            }
            else
            {
                sb.AppendLine("multiplex: {");

                sb.AppendLine($"secret: '{multiplexSecret}',");
                sb.AppendLine($"id: '{multiplexSocketId}',");
                sb.AppendLine($"url: '{multiplexUrl}'");
                sb.AppendLine("},");

                sb.AppendLine("dependencies:[");
                sb.AppendLine("{ src: '../presentation/plugin/countdown/countdown.js' },");
                sb.AppendLine("{ src: '../presentation/plugin/multiplex/socket.io.js', async: true },");
                if (string.IsNullOrEmpty(multiplexSecret))
                {
                    sb.AppendLine("{ src: '../presentation/plugin/multiplex/client.js', async: true }");
                }
                else
                {
                    //sb.AppendLine("{ src: '../presentation/plugin/multiplex/client.js', async: true },");
                    sb.AppendLine("{ src: '../presentation/plugin/multiplex/master.js', async: true }");
                }
                
                sb.AppendLine("]");
            }

            sb.AppendLine("});");

            script.TextContent = sb.ToString();
        }
    }
}
