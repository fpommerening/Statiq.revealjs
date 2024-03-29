﻿using System;
using System.IO;
using System.Threading.Tasks;
using AngleSharp.Dom;
using Microsoft.Extensions.DependencyInjection;
using Statiq.Common;
using Statiq.Core;
using IDocument = Statiq.Common.IDocument;

namespace FP.Statiq.RevealJS.Business
{
    public class EmbeedImages : ProcessHtml
    {
        public EmbeedImages() : base("IMG",
            (document, context, element) =>  ProcessElement(document, context, element))
        {
        }

        private static void ProcessElement(IDocument document, IExecutionContext context, IElement imgElement)
        {
            ProcessElementAsync(document, context, imgElement).GetAwaiter().GetResult();
        }

        private static async Task ProcessElementAsync(IDocument document, IExecutionContext context, IElement imgElement)
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
                var baseUrl = context.Settings["baseUrl"].ToString();
                var sectionPath = document[MetadataKeys.SectionPath].ToString();
                var path = string.IsNullOrEmpty(sectionPath)
                    ? Path.Combine(baseUrl, src)
                    : Path.Combine(baseUrl, Path.GetDirectoryName(sectionPath), src);
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
}
