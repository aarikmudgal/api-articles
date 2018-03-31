using System.Collections.Generic;
using System.Linq;
using eshop.api.article.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace eshop.api.article.Controllers
{
    [Route("api/[controller]")]
    public class ArticlesController : Controller
    {
        private static List<Article> articles = null;

        static ArticlesController()
        {
            LoadArticleFromFile();
        }

        private static void LoadArticleFromFile()
        {
            articles = new List<Article>(JsonConvert.DeserializeObject<List<Article>>(System.IO.File.ReadAllText(@"articles.json")));
        }

        private void WriteToFile()
        {
            try
            {
                System.IO.File.WriteAllText(@"articles.json", JsonConvert.SerializeObject(articles));
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        // GET api/articles/health
        [HttpGet]
        [Route("health")]
        public IActionResult GetHealth(string health)
        {
            bool fileExists = System.IO.File.Exists("./articles.json");
            IActionResult response = fileExists ? Ok("Service is Healthy") : StatusCode(500, "Articles file not available");
            return response;
        }

        // GET api/articles
        [HttpGet]
        public IActionResult GetArticles()
        {
            return new ObjectResult(articles);
        }

        // GET api/articles/5
        [HttpGet("{id}")]
        public IActionResult GetArticleById(int id)
        {
            Article article = articles.Find(a => a.ArticleId == id);
            if(article != null)
            {
                return new ObjectResult(article);
            }
            
            return NotFound($"Article with Id - {id} not found");
        }

        // POST api/articles
        [HttpPost]
        public IActionResult AddArticle([FromBody]JObject value)
        {
            int maxArticleId;
            try
            {
                // create new customer object
                Article art = JsonConvert.DeserializeObject<Article>(value.ToString());
                int.TryParse(articles.Count.ToString(), out maxArticleId);
                art.ArticleId = maxArticleId + 1;

                // add new customer to list
                articles.Add(art);
                WriteToFile();
            }
            catch (System.Exception ex)
            {
                // log the exception
                // internal server errror
                return StatusCode(500, ex.Message);
            }
            return Ok($"Article added successfully. New Article Id - {maxArticleId + 1}");
        }

        // PUT api/articles/5
        [HttpPut("{id}")]
        public IActionResult UpdateArticle(int id, [FromBody]JObject value)
        {
            try
            {
                Article inputArticle = JsonConvert.DeserializeObject<Article>(value.ToString());

                Article article = articles.Find(a => a.ArticleId == id);
                if(article == null)
                {
                    return NotFound($"Article with id - {id} not found");
                }

                article.DeepCopy(inputArticle);
                WriteToFile();
            }
            catch (System.Exception ex)
            {
                // log error/exception
                return StatusCode(500, ex.Message);
            }
            return Ok($"Article with Id - {id} updated successfully");
        }

        // DELETE api/articles/5
        [HttpDelete("{id}")]
        public IActionResult DeleteArticle(int id)
        {
            try
            {
                Article article = articles.Find(x => x.ArticleId == id);
                if(article == null)
                {
                    return NotFound($"Article with id - {id} not found");
                }
                articles.Remove(article);
                WriteToFile();
            }
            catch (System.Exception ex)
            {
                // log the exception
                return StatusCode(500, ex.Message);
            }
            return Ok($"Article with Id - {id} deleted successfully");        
        }
    }
}
