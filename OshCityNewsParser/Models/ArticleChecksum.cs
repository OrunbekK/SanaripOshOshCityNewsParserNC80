namespace OshCityNewsParser.Models
{
    internal class ArticleChecksum
    {
        public int SequenceNum { get; set; }
        public DateTime DateISO { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public byte[] ImageBytes { get; set; } = Array.Empty<byte>();
        public string ResultChecksum { get; set; } = string.Empty;
    }
}