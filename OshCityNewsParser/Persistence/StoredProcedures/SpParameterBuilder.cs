using Dapper;
using OshCityNewsParser.Models;
using System.Data;

namespace OshCityNewsParser.Persistence.StoredProcedures
{
    internal static class SpParameterBuilder
    {
        internal static DynamicParameters BuildUpsertArticleParameters(Article article)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@UID", article.UID);
            parameters.Add("@LanguageUID", article.LanguageUID);
            parameters.Add("@SequenceNum", article.SequenceNum);
            parameters.Add("@PublishedDate", article.PublishedDate);
            parameters.Add("@Title", article.Title);
            parameters.Add("@Text", article.Text);
            parameters.Add("@Url", article.Url);
            parameters.Add("@ThumbnailUrl", article.ThumbnailUrl);
            parameters.Add("@CheckSum", article.CheckSum);
            parameters.Add("@UpdatedDT", article.UpdatedDT);
            parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
            parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);
            return parameters;
        }

        internal static DynamicParameters BuildUpdateChecksumParameters(byte languageUid, string checksum)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@LanguageUID", languageUid);
            parameters.Add("@CheckSum", checksum);
            parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
            parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);
            return parameters;
        }
    }
}