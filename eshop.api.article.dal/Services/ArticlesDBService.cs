using System;
using System.Collections.Generic;
using eshop.api.article.dal.Models;
using eshop.api.article.dal.DBContext;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace eshop.api.article.dal.Services
{
    public class ArticlesDBService : IArticleService
    {
        private readonly ArticleContext _context;

        public ArticlesDBService(ArticleContext context)
        {
            _context = context;
            CheckConnection();
        }
        private void CheckConnection()
        {
            try
            {
                _context.Database.GetDbConnection();
                _context.Database.OpenConnection();
            }
            catch (Exception)
            {
                // log db connectivity issue
                throw;
            }
        }
        public IEnumerable<Article> GetArticles()
        {
            try
            {
                return _context.Articles;
            }
            catch (Exception e)
            {
                throw e;
            }
            
        }
        public Article GetArticle(int id)
        {
            try
            {
                return _context.Articles.SingleOrDefault(m => m.ArticleId == id);
            }
            catch (Exception e)
            {
                throw e;
            }
            
        }
        public bool UpdateArticle(int id, Article article, out Article updatedArticle, out string statusMessage)
        {
            _context.Entry(article).State = EntityState.Modified;


            try
            {
                if (!ArticleExists(id))
                {
                    updatedArticle = null;
                    statusMessage = $"The Article ID {id} does not exist.";
                }

                //byte[] bytes = Encoding.UTF8.GetBytes(article.Password);
                //string encodedPassword = Convert.ToBase64String(bytes);
                //article.Password = encodedPassword;

                _context.SaveChanges();
                statusMessage = $"Article details updated successfully for customer Id - {id}";
                updatedArticle = article;
                return true;
            }
            catch (DbUpdateConcurrencyException e)
            {
                statusMessage = e.Message;
                throw e;
            }
        }
        public bool InsertArticle(Article article, out Article addedArticle, out string statusMessage)
        {
            try
            {
                //article.ArticleId = Guid.NewGuid().ToString();

                _context.Articles.Add(article);
                int status = _context.SaveChanges();
                addedArticle = article;
                statusMessage = "New Article added successfully";
                return true;
            }
            catch (Exception)
            {
                //statusMessage = e.Message;
                addedArticle = null;
                throw;
            }
        }
        public bool DeleteArticle(int id, out Article deletedArticle, out string statusMessage)
        {
            try
            {
                var article = _context.Articles.SingleOrDefault(m => m.ArticleId == id);
                if (article == null)
                {
                    deletedArticle = null;
                    statusMessage = $"Article with id - {id} not found";
                    return false;
                }
                _context.Articles.Remove(article);
                _context.SaveChanges();
                deletedArticle = article;
                statusMessage = $"Article with id - {id} deleted successfully";
                return true;
            }
            catch (Exception e)
            {
                statusMessage = e.Message;
                deletedArticle = null;
                throw e;
            }
        }
        public bool GetImage(string imageName, out Byte[] imageByteStream, out string statusMessage)
        {
            try
            {
                imageByteStream = System.IO.File.ReadAllBytes(@"images/" + imageName);   // You can use your own method over here.         
                statusMessage = $"Image with name - {imageName} was found";
                return true;
            }
            catch (Exception e)
            {
                statusMessage = e.Message;
                imageByteStream = null;
                throw e;
            }
            
        }

        private bool ArticleExists(int id)
        {
            return _context.Articles.Any(e => e.ArticleId == id);
        }
        
    }
}
