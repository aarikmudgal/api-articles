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
            bool dbConnOk = false;
            string statusMessage = string.Empty;
            try
            {
                _context.CheckConnection(out dbConnOk);
                statusMessage = $"Order service is Healthy";

            }
            catch (Exception ex)
            {
                statusMessage = $"Order database or service not available - {ex.Message}";

            }
            IActionResult response = dbConnOk ? Ok(statusMessage) : StatusCode(500, statusMessage);
            return response;
        }

        // GET: api/Articles
        [HttpGet]
        public async Task<IActionResult> GetArticles()
        {
            try
            {
                var articles = await articleService.GetArticlesAsync();
                return Ok(articles);
            }
            catch (Exception ex)
            {

                return StatusCode(500, $"Error while getting articles - {ex.Message}");
            }
        }

        // GET: api/Articles/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetArticle([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var article = await articleService.GetArticleAsync(id);

            if (article == null)
            {
                return NotFound("The Article ID was not found");
            }

            return Ok(article);
        }

        // PUT: api/Articles/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutArticle([FromRoute] int id, [FromBody] Article article)
        {

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
                ReturnResult result = await articleService.UpdateArticleAsync(id, article);
                if (result.UpdatedArticle == null)
                {
                    return NotFound($"Article with id {id} not found");
                }
                JObject successobj = new JObject()
                {
                    { "StatusMessage", result.StatusMessage },
                    { "Customer", JObject.Parse(JsonConvert.SerializeObject(result.UpdatedArticle)) }
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
        public async Task<IActionResult> PostArticle([FromBody] Article article)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                ReturnResult result = await articleService.InsertArticleAsync(article);
                if (result.UpdatedArticle != null)
                {
                    JObject successobj = new JObject()
                    {
                        { "StatusMessage", result.StatusMessage },
                        { "Customer", JObject.Parse(JsonConvert.SerializeObject(result.UpdatedArticle)) }
                    };
                    return Ok(successobj);
                }
                else
                {
                    return StatusCode(500, result.StatusMessage);
                }
                
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message + " Inner Exception- " + ex.InnerException.Message);
            }
        }

        // DELETE: api/Articles/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteArticleAsync([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var result = await articleService.DeleteArticleAsync(id);
                if (result.UpdatedArticle == null)
                {
                    return NotFound($"Article with id {id} not found");
                }
                JObject successobj = new JObject()
                {
                    { "StatusMessage", result.StatusMessage },
                    { "Customer", JObject.Parse(JsonConvert.SerializeObject(result.UpdatedArticle)) }
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