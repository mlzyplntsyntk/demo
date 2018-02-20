using Demo.Models.Entities;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Threading.Tasks;

namespace Demo.Models
{
    public interface IDataRepository
    {
        IQueryable<User> Users { get; }
        IQueryable<Article> Articles { get; }
        IQueryable<ArticleAction> ArticleActions { get; }

        /**
         *  I do not want to expose AppDbContext related
         *  methods to application, so i want my DataRepository
         *  to handle such actions.
         */

        int Save(AbstractEntity entity);
        int Remove(AbstractEntity entity);
        int Remove(IQueryable<AbstractEntity> entities);
        int SaveChanges();
        int SaveAttached(AbstractEntity a);
    }
}
