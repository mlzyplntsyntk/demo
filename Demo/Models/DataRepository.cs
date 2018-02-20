using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Threading.Tasks;
using Demo.Models.Entities;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Demo.Models
{
    public class DataRepository : IDataRepository
    {
        AppDbContext context;

        public DataRepository(AppDbContext ctx)
        {
            this.context = ctx;
        }

        public IQueryable<User> Users => this.context.Users;
        public IQueryable<Article> Articles => this.context.Articles;
        public IQueryable<ArticleAction> ArticleActions => this.context.ArticleActions;

        public int SaveAttached(AbstractEntity a)
        {
            this.context.Attach(a);
            return this.context.SaveChanges();
        }

        public int SaveChanges()
        {
            return this.context.SaveChanges();
        }

        public int Save(AbstractEntity entity)
        {
            if (entity.Id == 0)
            {
                context.Add(entity);
                return this.SaveChanges();
            } else
            {
                context.Update(entity);
                return this.SaveChanges();
            }
        }

        public int Remove(AbstractEntity entity)
        {
            context.Remove(entity);
            return this.SaveChanges();
        }

        public int Remove(IQueryable<AbstractEntity> entities) { 
            context.RemoveRange(entities);
            return this.SaveChanges();
        }
    }
}
