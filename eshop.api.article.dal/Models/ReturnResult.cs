using System;
using System.Collections.Generic;
using System.Text;

namespace eshop.api.article.dal.Models
{
    public class ReturnResult
    {
        public string StatusMessage { get; set; }
        public Article UpdatedArticle { get; set; }
    }
}
