
namespace eshop.api.article.dal.Models
{
    public class Article
    {
        public int ArticleId { get; set; }
        public string ArticleName { get; set; }
        public string ArticleDescription { get; set; }
        public double ArticlePrice { get; set; }
        public string ArticleImageUrl { get; set; }
    }
}
