namespace OshCityNewsParser.Models
{
    internal class Article
    {
        public Guid UID { get; set; } = Guid.NewGuid();
        public byte LanguageUID { get; set; }
        public int SequenceNum { get; set; }
        public DateTime PublishedDate { get; set; } = DateTime.Now;
        public string Title { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string ThumbnailUrl { get; set; } = string.Empty;
        public string CheckSum { get; set; } = string.Empty;
        public DateTime UpdatedDT { get; set; } = DateTime.Now;
    }
}