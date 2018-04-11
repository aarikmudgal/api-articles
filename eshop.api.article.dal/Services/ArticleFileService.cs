//using Newtonsoft.Json;
//using eshop.api.article.dal.Models;
//using System.Collections.Generic;


//namespace eshop.api.article.dal.Services
//{
//    public class ArticleFileService : IArticleService
//    {
//        private static List<Article> articles = null;

//        static ArticleFileService()
//        {
//            LoadArticleFromFile();
//        }

//        private static void LoadArticleFromFile()
//        {
//            articles = new List<Article>(JsonConvert.DeserializeObject<List<Article>>(System.IO.File.ReadAllText(@"articles.json")));
//        }

//        private void WriteToFile()
//        {
//            try
//            {
//                System.IO.File.WriteAllText(@"articles.json", JsonConvert.SerializeObject(articles));
//            }
//            catch (System.Exception)
//            {
//                throw;
//            }
//        }

//        //public IActionResult GetHealth(string health)
//        //{
//        //    bool fileExists = System.IO.File.Exists("./articles.json");
//        //    IActionResult response = fileExists ? Ok("Service is Healthy") : StatusCode(500, "Articles file not available");
//        //    return response;
//        //}
//        public IEnumerable<Article> GetArticles()
//        {
//            return articles;
//        }
//        public Article GetArticle(string id)
//        {
//            foreach (Article article in articles)
//            {
//                if (article.ArticleId == id)
//                {
//                    return article;
//                }
//            }
//            return null;
//        }
//        public List<string> UpdateArticle(string id, Article article)
//        {
//            List<string> result = new List<string>();
//            try
//            {
//                int index = articles.IndexOf(articles.Find(x => x.ArticleId == id));
//                articles.Remove(articles.Find(x => x.ArticleId == id));
//                articles.Insert(index, JsonConvert.DeserializeObject<Article>(article.ToString()));
//                WriteToFile();
//                result.Add("success");
//            }
//            catch (System.Exception ex)
//            {
//                // log error/exception
//                result.Add("true");
//                result.Add(ex.Message);
//            }
//            return result;
//        }
//        public List<string> InsertArticle(Article article)
//        {
//            int maxArticleId;
//            List<string> result = new List<string>();
//            try
//            {
//                // create new customer object
//                Article art = JsonConvert.DeserializeObject<Article>(article.ToString());
//                int.TryParse(articles.Count.ToString(), out maxArticleId);
//                art.ArticleId = (maxArticleId + 1).ToString();

//                // add new customer to list
//                articles.Add(art);
//                WriteToFile();
//                result.Add("success");
//            }
//            catch (System.Exception ex)
//            {
//                // log the exception
//                // internal server errror
//                result.Add("true");
//                result.Add(ex.Message);
//            }
//            return result;
//        }
//        public List<string> DeleteArticle(string id)
//        {
//            List<string> result = new List<string>();
//            try
//            {
//                articles.Remove(articles.Find(x => x.ArticleId == id));
//                WriteToFile();
//                result.Add("success");
//            }
//            catch (System.Exception ex)
//            {
//                result.Add("true");
//                result.Add(ex.Message);
//            }
//            return result;
//        }

//    }
//}
