using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using FP.Statiq.RevealJS.Models;
using Statiq.Common;

namespace FP.Statiq.RevealJS.Business
{
    public class LoadSlideDesk : ParallelModule
    {
        protected override async Task<IEnumerable<IDocument>> ExecuteInputAsync(IDocument input, IExecutionContext context)
        {
            var slidedesk = await JsonSerializer.DeserializeAsync<SlideDesk>(input.GetContentStream());
            var documents = new List<IDocument>();

            var index = 0;
            var metadataItems = new MetadataItems
            {
                {MetadataKeys.SlideDeskAccess, slidedesk.Access},
                {MetadataKeys.SlideDeskTitle, slidedesk.Title},
                {MetadataKeys.SlideDeskDescription, slidedesk.Description},
                {MetadataKeys.SlideDeskTheme, slidedesk.Theme},
                {MetadataKeys.SlideDeskPassword, slidedesk.Password}
            };

            foreach (var section in slidedesk.Sections)
            {
                index += 10;
                metadataItems.Add(MetadataKeys.SectionPosition, index);
                if (!string.IsNullOrEmpty(section.Content))
                {
                    metadataItems.Add(MetadataKeys.SectionPath, string.Empty);
                    documents.Add(new Document(metadataItems, context.GetContentProvider(section.Content, MediaTypes.HtmlFragment)));
                }
                else
                {
                    metadataItems.Add(MetadataKeys.SectionPath, section.Path);
                    documents.Add(new Document(metadataItems));
                }
                metadataItems = new MetadataItems();
            }

            return documents;

        }
    }
}
