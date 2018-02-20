using Demo.Infrastructure;
using Demo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
using System.Threading.Tasks;

namespace Demo.Filters
{
    /**
     * Checks if the user owns the given article or not
     * Useful when the user tries to edit or delete the article
     */
    public class ValidateArticleOwnerAttribute : TypeFilterAttribute
    {
        public ValidateArticleOwnerAttribute() : base(typeof(ValidateArticleOwnerAttributeImplementation))
        {

        }

        private class ValidateArticleOwnerAttributeImplementation : IAsyncActionFilter
        {
            IDataRepository repository;
            ISessionRepository session;

            public ValidateArticleOwnerAttributeImplementation(IDataRepository repository,
                ISessionRepository session)
            {
                this.repository = repository;
                this.session = session;
            }

            public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
            {
                if (context.ActionArguments.ContainsKey("id"))
                {
                    var id = context.ActionArguments["id"] as int?;
                    if (id.HasValue)
                    {
                        if (this.repository.Articles.Where(p=>p.Id == id.Value && p.UserId == this.session.User.Id).Count()>0)
                        {
                            await next();
                        }
                    }
                }
                
                context.Result = new RedirectResult("/Home/Forbidden");
            }
        }
    }
    
}
