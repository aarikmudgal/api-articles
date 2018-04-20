using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using eshop.api.article.dal.Models;

namespace eshop.api.article.dal.Services
{
    public interface IArticleService
    {
        Task<IList<ArticleWithStatus>> GetArticlesAsync();
        Task<Article> GetArticleAsync(int id);
        Task<ReturnResult> UpdateArticleAsync(int id, Article article);
        Task<ReturnResult> InsertArticleAsync(Article article);
        Task<ReturnResult> DeleteArticleAsync(int id);
        bool GetImage(string imageName, out Byte[] imageByteStream, out string statusMessage);
    }
}
