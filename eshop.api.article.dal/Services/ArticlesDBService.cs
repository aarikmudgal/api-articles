using System;
using System.Collections.Generic;
using eshop.api.article.dal.Models;
using eshop.api.article.dal.DBContext;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;

namespace eshop.api.article.dal.Services
{
    public class ArticlesDBService : IArticleService
    {
        private readonly ArticleContext _context;
        private readonly string _inventoryEndPoint = "http://10.3.245.113:8004/api/inventory";

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
        public async Task<IList<ArticleWithStatus>> GetArticlesAsync()
        {
            try
            {
                var task = _context.Articles.ToListAsync();

                // get inventory and update articles with IN STOCK/ OUT OF STOCK status
                var inventory = await GetInventoryAsycn();

                var articles = await task;

                var updatedArticleList = UpdateArticleStockStatus(inventory, articles);

                return updatedArticleList;
            }
            catch (Exception e)
            {
                throw e;
            }
            
        }

        private List<ArticleWithStatus> UpdateArticleStockStatus(IList<ArticleStock> inventory, List<Article> articles)
        {
            List<ArticleWithStatus> articleListWithStatus = new List<ArticleWithStatus>();
            foreach (var item in articles)
            {
                ArticleWithStatus articleWithStatus = new ArticleWithStatus()
                {
                    ArticleId = item.ArticleId,
                    ArticleDescription = item.ArticleDescription,
                    ArticleName = item.ArticleName,
                    ArticlePrice = item.ArticlePrice,
                    ArticleImageUrl = item.ArticleImageUrl
                };
                var article = inventory == null ? null : inventory.ToList().Find(a => a.ArticleId == item.ArticleId);
                if (article == null)
                {
                    // Article not found in inventory
                    articleWithStatus.ArticleStockStatus = "UNKNOWN";
                }
                else
                {
                    string stockStatus = article.TotalQuantity > 0 ? "IN STOCK" : "OUT OF STOCK";
                    //articleStatus.ArticleStockStatus = stockStatus;
                    articleWithStatus.ArticleStockStatus = stockStatus;
                }
                articleListWithStatus.Add(articleWithStatus);


            }
            return articleListWithStatus;
        }

        private async Task<IList<ArticleStock>> GetInventoryAsycn()
        {
            IList<ArticleStock> inventory = null;
            try
            {
                string url = _inventoryEndPoint;
                HttpClient client = new HttpClient();

                HttpResponseMessage response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    inventory = await response.Content.ReadAsAsync<IList<ArticleStock>>();
                }
                return inventory;
            }
            catch (Exception)
            {
                //throw;
                // inventory service is down
                return inventory;
            }
           
            
        }
        public async Task<Article> GetArticleAsync(int id)
        {
            try
            {
                return await _context.Articles.SingleOrDefaultAsync(m => m.ArticleId == id);
            }
            catch (Exception e)
            {
                throw e;
            }
            
        }
        public async Task<ReturnResult> UpdateArticleAsync(int id, Article article)
        {
            _context.Entry(article).State = EntityState.Modified;

            ReturnResult result = new ReturnResult();
            try
            {
                if (!ArticleExists(id))
                {
                    result.UpdatedArticle = null;
                    result.StatusMessage = $"The Article ID {id} does not exist.";
                    return result;
                }

                int status = await _context.SaveChangesAsync();
                result.StatusMessage = $"Article details updated successfully for customer Id - {id}";
                result.UpdatedArticle = article;
                return result;
            }
            catch (DbUpdateConcurrencyException e)
            {
                result.UpdatedArticle = null;
                result.StatusMessage = e.Message;
                throw e;
            }
        }
        public async Task<ReturnResult> InsertArticleAsync(Article article)
        {
            ReturnResult result = new ReturnResult()
            {
                StatusMessage = "",
                UpdatedArticle = null
            };
            try
            {
                await _context.Articles.AddAsync(article);
                int saved = await _context.SaveChangesAsync();
                if (saved > 0)
                {
                    result.UpdatedArticle = article;
                    result.StatusMessage = "New Article added successfully";
                }
                else
                {
                    result.StatusMessage = "Issue while Article add !";
                }
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<ReturnResult> DeleteArticleAsync(int id)
        {
            ReturnResult result = new ReturnResult();
            try
            {
                var article = await _context.Articles.SingleOrDefaultAsync(m => m.ArticleId == id);
                if (article == null)
                {
                    result.UpdatedArticle = null;
                    result.StatusMessage = $"Article with id - {id} not found";
                    return result;
                }
                _context.Articles.Remove(article);
                int deleted = await _context.SaveChangesAsync();
                if (deleted > 0)
                {
                    result.UpdatedArticle = article;
                    result.StatusMessage = $"Article with id - {id} deleted successfully";
                }
                else
                {
                    throw new Exception($"Some issue with delete of Article with id - {id}");
                }
                return result;
            }
            catch (Exception e)
            {
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
