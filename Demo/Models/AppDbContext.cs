using Demo.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace Demo.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            :base(options)
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<Article> Articles { get; set; }
        public DbSet<ArticleAction> ArticleActions { get; set; }

        /**
         * In my app's logic, i would like to denormalize my Articles
         * table in order to prevent count(*) queries to ArticleActions
         * table. The below implementation is not the best practice for
         * this purpose, i could create a trigger to ArticleActions table
         * to make it simpler and more effective. I just wanted to play
         * with the EF's SaveAction Process.
         **/
        public override int SaveChanges()
        {
            /**
             * This is why i have created a global static variable in my
             * Program Class. If it's initializing, i know that there will 
             * be plenty of writes to my db, so i will not track down
             * ArticleActions table.
             **/
            if (Program.Initializing)
            {
                return base.SaveChanges();
            }

            var otherActions = ChangeTracker.Entries<AbstractEntity>()
                .Select(p => p);
            var auditArticleActions = ChangeTracker.Entries<AbstractEntity>()
                .Where(p => p.Entity is ArticleAction)
                .Select(p=>p).ToList();

            int result = base.SaveChanges();

            if (auditArticleActions.Count()>0)
            {
                UpdateLikesReads(auditArticleActions);
            }
            
            return result;
        }

        private void UpdateLikesReads(IEnumerable<EntityEntry<AbstractEntity>> actions)
        {
            foreach (EntityEntry modified in actions)
            {
                ArticleAction action = (ArticleAction)modified.Entity;

                this.Database.ExecuteSqlCommand(
                    GetSqlCommandForColumn(action.ArticleActionType),
                    new SqlParameter("actionType", action.ArticleActionType),
                    new SqlParameter("Id", action.Article.Id)
                );
            }
        }

        public static string GetSqlCommandForColumn(ArticleActionType type)
        {
            return String.Format(@"update Articles set {0} = 
                (
                    select count(*) from ArticleActions 
                    where ArticleActions.ArticleId = Articles.Id and ArticleActions.ArticleActionType = @actionType
                ) where Id=@Id", type == ArticleActionType.LIKE ? "TotalLikes" : "TotalReads");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
           
        }
    }
}
