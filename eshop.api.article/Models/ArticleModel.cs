namespace eshop.api.article.Models
{
    public class ArticleModel
    {
        public Article[] Articles { get; set; }
    }
    public class Article
    {
        public string ArticleId { get; set; }
        public string ArticleName { get; set; }
        public string ArticleDescription { get; set; }
        public string ArticlePrice { get; set; }
        public string ArticleImageUrl { get; set; }
    }
}
