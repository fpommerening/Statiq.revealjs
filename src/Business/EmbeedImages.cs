using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AngleSharp.Dom;
using Statiq.Common;
using Statiq.Html;
using IDocument = Statiq.Common.IDocument;

namespace FP.Statiq.RevealJS.Business
{
    public class EmbeedImages : ProcessHtml
    {

        public EmbeedImages(HttpClient httpClient) : base("IMG",
            (document, context, element) => ProcessElement(httpClient, document, context, element))
        {
        }

        private static void ProcessElement(HttpClient httpClient, IDocument document, IExecutionContext context, IElement imgElement)
        {
            ProcessElementAsync(httpClient, document, context, imgElement).GetAwaiter().GetResult();
        }

        private static async Task ProcessElementAsync(HttpClient httpClient, IDocument document, IExecutionContext context, IElement imgElement)
        {
            
            var src = imgElement.GetAttribute("src");
            if (string.IsNullOrEmpty(src))
            {
                return;
            }

            if (src.StartsWith("data:image"))
            {
                return; // already replaced;
            }

            byte[] imageData = null;

            if (src.StartsWith("http"))
            {
                var imageResult = await httpClient.GetAsync(src);
                imageResult.EnsureSuccessStatusCode();
                imageData = await imageResult.Content.ReadAsByteArrayAsync();
            }
            else
            {
                var baseUrl = context.Settings["baseUrl"].ToString();
                var sectionPath = document[MetadataKeys.SectionPath].ToString();
                var path = Path.Combine(baseUrl, Path.GetDirectoryName(sectionPath), src);
                imageData = await File.ReadAllBytesAsync(path);
            }

            var imageDataDecoded = Convert.ToBase64String(imageData);
            if (src.EndsWith("svg"))
            {
                imgElement.SetAttribute("src", $"data:image/svg+xml;base64, {imageDataDecoded}");
            }
            else if (src.StartsWith("png"))
            {
                imgElement.SetAttribute("src", $"data:image/png;base64, {imageDataDecoded}");
            }
            else if (src.EndsWith("jpeg") || src.EndsWith("jpg"))
            {
                imgElement.SetAttribute("src", $"data:image/jpeg;base64, {imageDataDecoded}");
            }
        }

    }
}
