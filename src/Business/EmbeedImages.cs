using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using Microsoft.Extensions.DependencyInjection;
using IDocument = Statiq.Common.IDocument;

namespace FP.Statiq.RevealJS.Business;

public class EmbeedImages : ParallelModule
{
    protected override async Task<IEnumerable<IDocument>> ExecuteInputAsync(IDocument input, IExecutionContext context)
    {
       return await ProcessElementsAsync(input, context);
    }

    private static async Task<IEnumerable<IDocument>> ProcessElementsAsync(
        IDocument input,
        IExecutionContext context)
    {
        IHtmlDocument originalHtmlDocument = await input.ParseHtmlAsync(false);
        if (originalHtmlDocument is null)
        {
            return input.Yield();
        }

        try
        {
            IHtmlDocument htmlDocument = (IHtmlDocument)originalHtmlDocument.Clone();
            IElement[] elements = htmlDocument.QuerySelectorAll("IMG").ToArray();

            if (elements.Length > 0 && elements[0] is not null)
            {
                foreach (IElement element in elements)
                {
                    await ProcessImgElementAsync(input, context, element);
                }

                if (originalHtmlDocument.Equals(htmlDocument))
                {
                    return input.Yield();
                }

                IContentProvider contentProvider = context.GetContentProvider(htmlDocument);
                IDocument output = input.Clone(contentProvider);
                return output.Yield();
            }

            return input.Yield();
        }
        catch (Exception ex)
        {
            context.LogWarning(input, $"Exception while processing HTML {ex.Message}");
            return input.Yield();
        }
    }


    private static async Task ProcessImgElementAsync(IDocument document, IExecutionContext context, IElement imgElement)
    {
        var imageCache = context.Services.GetService<ImageCache>();
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
            imageData = await imageCache.DownloadImage(context, src);
        }
        else
        {
            var baseUrl = context.Settings["baseUrl"].ToString()!;
            var sectionPath = document[MetadataKeys.SectionPath].ToString();
            var path = string.IsNullOrEmpty(sectionPath)
                ? Path.Combine(baseUrl, src)
                : Path.Combine(baseUrl, Path.GetDirectoryName(sectionPath)!, src);
            imageData = await File.ReadAllBytesAsync(path);
        }

        var imageDataDecoded = Convert.ToBase64String(imageData);
        if (src.EndsWith("svg"))
        {
            imgElement.SetAttribute("src", $"data:image/svg+xml;base64, {imageDataDecoded}");
        }
        else if (src.EndsWith("png"))
        {
            imgElement.SetAttribute("src", $"data:image/png;base64, {imageDataDecoded}");
        }
        else if (src.EndsWith("jpeg") || src.EndsWith("jpg"))
        {
            imgElement.SetAttribute("src", $"data:image/jpeg;base64, {imageDataDecoded}");
        }
    }
}