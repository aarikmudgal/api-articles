using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using eshop.api.article.dal.Models;
using eshop.api.article.dal.DBContext;
using eshop.api.article.dal.Services;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace eshop.api.article.Controllers
{
    [Produces("application/json")]
    [Route("api/Articles")]
    public class ArticlesController : Controller
    {
        private readonly ArticleContext _context;

        IArticleService articleService;
        
        public bool DBDriven = true;

        public ArticlesController(ArticleContext context)
        {
            _context = context;
            if (DBDriven)
            {
                articleService = new ArticlesDBService(_context);
            }
            //else
            //{
            //    articleService = new ArticleFileService();
            //}
            
        }


        [HttpGet]
        [Route("images/{imageName}")]
        public IActionResult Get(string imageName)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var status = articleService.GetImage(imageName, out Byte[] imageByteStream, out string statusMessage);

                if (imageByteStream == null)
                {
                    return NotFound(statusMessage);
                }

                return File(imageByteStream, "image/jpg");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
            
        }

        // GET api/Articles/health
        [HttpGet]
        [Route("health")]
        public IActionResult GetHealth(string health)
        {
            //bool fileExists = System.IO.File.Exists("./customers.json");
            bool dbConnOk = false;
            string statusMessage = string.Empty;
            try
            {
                if (_context.CheckConnection())
                {
                    dbConnOk = true;
                    statusMessage = "Article Service is Healthy";
                }

            }
            catch (Exception ex)
            {
                statusMessage = $"Articles database not available - {ex.Message}";

            }
            IActionResult response = dbConnOk ? Ok("Article Service is Healthy") : StatusCode(500, "Articles database not available");
            return response;
        }

        // GET: api/Articles
        [HttpGet]
        public IActionResult GetArticles()
        {
            try
            {
                return new ObjectResult(articleService.GetArticles());
            }
            catch (Exception ex)
            {

                return StatusCode(500, $"Error while getting articles - {ex.Message}");
            }
        }

        // GET: api/Articles/5
        [HttpGet("{id}")]
        public IActionResult GetArticle([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var article = articleService.GetArticle(id);

            if (article == null)
            {
                return NotFound("The Article ID was not found");
            }

            return Ok(article);
        }

        // PUT: api/Articles/5
        [HttpPut("{id}")]
        public IActionResult PutArticle([FromRoute] int id, [FromBody] Article article)
        {
            string statusMessage;
            Article updatedArticle;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != article.ArticleId)
            {
                return BadRequest();
            }
            try
            {
                bool status = articleService.UpdateArticle(id, article, out updatedArticle, out statusMessage);
                if (updatedArticle == null)
                {
                    return NotFound($"Article with id {id} not found");
                }
                JObject successobj = new JObject()
                {
                    { "StatusMessage", statusMessage },
                    { "Customer", JObject.Parse(JsonConvert.SerializeObject(updatedArticle)) }
                };
                return Ok(successobj);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
                //throw;
            }
        }

        // POST: api/Articles
        [HttpPost]
        public IActionResult PostArticle([FromBody] Article article)
        {
            Article addedArticle;
            string statusMessage;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                articleService.InsertArticle(article, out addedArticle, out statusMessage);
                JObject successobj = new JObject()
                {
                    { "StatusMessage", statusMessage },
                    { "Customer", JObject.Parse(JsonConvert.SerializeObject(addedArticle)) }
                };
                return Ok(successobj);
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex.Message + " Inner Exception- " + ex.InnerException.Message);
            }
        }

        // DELETE: api/Articles/5
        //public async Task<IActionResult> DeleteArticle([FromRoute] string id)
        [HttpDelete("{id}")]
        public IActionResult DeleteArticle([FromRoute] int id)
        {
            Article deletedArticle;
            string statusMessage;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var status = articleService.DeleteArticle(id, out deletedArticle, out statusMessage);
                if (deletedArticle == null)
                {
                    return NotFound($"Article with id {id} not found");
                }
                JObject successobj = new JObject()
                {
                    { "StatusMessage", statusMessage },
                    { "Customer", JObject.Parse(JsonConvert.SerializeObject(deletedArticle)) }
                };
                return Ok(successobj);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

    }
}

//{"articleId":"3","articleName":"Biscuit","articleDescription":"ArticleDB","articlePrice":"6.5"}