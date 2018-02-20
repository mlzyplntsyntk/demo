using Demo.Infrastructure;
using Demo.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace Demo.Filters
{
    /**
     * Authorizes the user depending on the usertype
     * or availability at the HttpContext 
     * */
    public class AuthorizationAttribute : Attribute, IAuthorizationFilter
    {
        UserType? userType;

        public AuthorizationAttribute() { }

        public AuthorizationAttribute(UserType userType)
        {
            this.userType = userType;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            SessionRepository session = new SessionRepository(context.HttpContext.Session);
            if (session.User is null || session.User.Id == 0) {
                context.Result = new RedirectToRouteResult("Auth", new {
                    returnURL = context.HttpContext.Request.Path.Value
                });
                return;
            }
            if (this.userType != null && !session.User.UserType.Equals(this.userType)) {
                context.Result = new RedirectResult("/Home/Forbidden");
                return;
            }
        }
    }
}
