using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace eshop.api.article.dal.DBContext
{
    public class ArticleContext : DbContext
    {
        public ArticleContext(DbContextOptions<ArticleContext> options) : base(options)
        {

        }
        public DbSet<eshop.api.article.dal.Models.Article> Articles { get; set; }

        public void CheckConnection(out bool dbStatusOK)
        {
            try
            {

                this.Database.OpenConnection();
                this.Database.ExecuteSqlCommand("SELECT 1");
                this.Database.CloseConnection();
                dbStatusOK = true;
            }
            catch (Exception ex)
            {
                dbStatusOK = false;
                throw ex;
            }
            finally
            {
                this.Database.CloseConnection();
            }
        }
    }
}
