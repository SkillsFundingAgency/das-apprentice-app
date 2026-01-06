using System.Text.RegularExpressions;

namespace SFA.DAS.ApprenticeApp.Domain.Models
{
    public class ApprenticeAppArticlePage
    {
        public ApprenticeAppPageSysProperties? Sys { get; set; }
        public string? Slug { get; set; }
        public string? Heading { get; set; }
        public object? Content { get; set; }
        public string? Id { get; set; }
        public int? ArticleOrder { get; set; }
        
        public string? ArticleText
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Content?.ToString()))
                    return "";
        
                var html = Markdig.Markdown.ToHtml(Content.ToString());
        
                // Regex to find <a> tags and add target="_blank"
                var pattern = @"<a\s+(?![^>]*\btarget\s*=)[^>]*>";
                html = Regex.Replace(html, pattern, match =>
                {
                    string tag = match.Value;
            
                    // Check if tag already has target attribute
                    if (!Regex.IsMatch(tag, @"\btarget\s*="))
                    {
                        // Insert target="_blank" after the opening <a
                        int insertPosition = tag.IndexOf('>');
                        if (insertPosition > 0)
                        {
                            tag = tag.Insert(insertPosition, " target=\"_blank\"");
                        }
                    }
                    return tag;
                }, RegexOptions.IgnoreCase);
        
                return html;
            }
        }
    }
}