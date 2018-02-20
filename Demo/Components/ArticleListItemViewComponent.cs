using Demo.Infrastructure;
using Demo.Models.Entities;
using Demo.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Demo.Components
{
    /**
     * This component renders the article item when it's listing. 
     * It injects the current session so it can decide whether the 
     * user owns article or not
     */
    public class ArticleListItemViewComponent : ViewComponent
    {
        private User user;

        public ArticleListItemViewComponent(ISessionRepository session)
        {
            this.user = session.User;
        }

        public IViewComponentResult Invoke(Article article) => View(new ArticleListItemViewModel
        {
            User = user,
            Article = article
        });
    }
}
