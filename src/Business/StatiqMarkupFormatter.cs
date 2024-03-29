using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html;

namespace FP.Statiq.RevealJS.Business;

public class StatiqMarkupFormatter : IMarkupFormatter
{
    private static readonly IMarkupFormatter Formatter = HtmlMarkupFormatter.Instance;

    public static readonly IMarkupFormatter Instance = new StatiqMarkupFormatter();

    public string CloseTag(IElement element, bool selfClosing) => Formatter.CloseTag(element, selfClosing);

    public string Doctype(IDocumentType doctype) => Formatter.Doctype(doctype);

    public string OpenTag(IElement element, bool selfClosing) => Formatter.OpenTag(element, selfClosing);

    public string Text(ICharacterData text) => Formatter.Text(text);

    public string Processing(IProcessingInstruction processing) => Formatter.Processing(processing);

    public string LiteralText(ICharacterData text) => Formatter.LiteralText(text);

    public string Comment(IComment comment)
    {
        if (comment.Data.StartsWith("?") && comment.Data.EndsWith("?"))
        {
            // This was probably a shortcode, so uncomment it
            return $"<{comment.Data}>";
        }
        return Formatter.Comment(comment);
    }
}