namespace eshop.api.article.Models
{
    public class Article
    {
        public int ArticleId { get; set; }
        public string ArticleName { get; set; }
        public string ArticleDescription { get; set; }
        public double ArticlePrice { get; set; }
        public string ArticleImageUrl { get; set; }

        public void DeepCopy(Article inputArticle)
        {
            this.ArticleName = inputArticle.ArticleName;
            this.ArticleDescription = inputArticle.ArticleDescription;
            this.ArticlePrice = inputArticle.ArticlePrice;
            this.ArticleImageUrl = inputArticle.ArticleImageUrl;
        }
    }

   
}
