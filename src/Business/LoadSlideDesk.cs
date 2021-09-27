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
                {MetadataKeys.SlideDeskTitle, slidedesk.Title},
                {MetadataKeys.SlideDeskDescription, slidedesk.Description},
                {MetadataKeys.SlideDeskTheme, string.IsNullOrEmpty(slidedesk.Theme) ? "white.css" : slidedesk.Theme},
                {MetadataKeys.SlideDeskPassword, slidedesk.Password},
                {MetadataKeys.SlideDeskCopyright, slidedesk.Copyright}
            };

            if (slidedesk.Multiplex != null && !string.IsNullOrEmpty(slidedesk.Multiplex.SocketId))
            {
                metadataItems.Add(new KeyValuePair<string, object>(MetadataKeys.MultiplexId, slidedesk.Multiplex.SocketId));
                metadataItems.Add(new KeyValuePair<string, object>(MetadataKeys.MultiplexUrl, slidedesk.Multiplex.Url));
                if (!string.IsNullOrEmpty(slidedesk.Multiplex.Secret))
                {
                    metadataItems.Add(new KeyValuePair<string, object>(MetadataKeys.MultiplexSecret, slidedesk.Multiplex.Secret));
                }
                else
                {
                    metadataItems.Add(new KeyValuePair<string, object>(MetadataKeys.MultiplexSecret, string.Empty));
                }
            }
            else
            {
                metadataItems.Add(new KeyValuePair<string, object>(MetadataKeys.MultiplexId, string.Empty));
                metadataItems.Add(new KeyValuePair<string, object>(MetadataKeys.MultiplexUrl, string.Empty));
                metadataItems.Add(new KeyValuePair<string, object>(MetadataKeys.MultiplexSecret, string.Empty));
            }


            foreach (var section in slidedesk.Sections)
            {
                index += 10;
                metadataItems.Add(MetadataKeys.SectionPosition, index);
                metadataItems.Add(MetadataKeys.SlideDeskAccess, slidedesk.Access);
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
