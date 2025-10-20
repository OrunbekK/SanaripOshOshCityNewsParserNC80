namespace OshCityNewsParser.Configuration
{
    internal class DatabaseOptions
    {
        public string DefaultConnection { get; set; } = string.Empty;

        // Stored Procedure Names
        public const string SP_UPSERT_ARTICLE = "USP_UpsertArticle";
        public const string SP_UPDATE_CHECKSUM = "USP_UpdateChecksum";
        public const string SP_GET_CHECKSUM = "USP_GetChecksum";
        public const string SP_GET_ARTICLES_BY_LANGUAGE = "USP_GetArticlesByLanguage";
    }
}