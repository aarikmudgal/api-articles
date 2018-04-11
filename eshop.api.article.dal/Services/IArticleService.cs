using System;
using System.Collections.Generic;
using eshop.api.article.dal.Models;

namespace eshop.api.article.dal.Services
{
    public interface IArticleService
    {
        IEnumerable<Article> GetArticles();
        Article GetArticle(int id);
        bool UpdateArticle(int id, Article article, out Article updatedArticle, out string statusMessage);
        bool InsertArticle(Article article, out Article addedArticle, out string statusMessage);
        bool DeleteArticle(int id, out Article deletedArticle, out string statusMessage);
        bool GetImage(string imageName, out Byte[] imageByteStream, out string statusMessage);
    }
}
