namespace CSharpSampleApp.Services.Search
{
    public class Highlight
    {
        /// <summary>
        /// Highlighted file content
        /// </summary>
        /// <example>&lt;em&gt;London&lt;/em&gt; is the capital of Great Britain.</example>
        public string Content;

        /// <summary>
        /// Highlighted file or folder name
        /// </summary>
        /// <example>&lt;em&gt;London&lt;/em&gt; is the capital...</example>
        public string Name;
    }
}