using Demo.Models.Entities;
using Demo.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Demo.Infrastructure
{
    /**
     * Interface to create and inject the DataController class
     */
    public interface IDataController
    {
        ArticleListViewModel ArticleList(int pageNumber, int pageSize, string orderby, bool withPaging);
        
        ArticleDetailViewModel ArticleDetail(int id);
        void ArticleDelete(int id);
        void ArticleEdit(int id, Article article);
        Article ArticleCreate(Article article);
        JsonResult ArticleLike(int id);
        JsonResult ArticleUndoLike(int id);
    }
}
