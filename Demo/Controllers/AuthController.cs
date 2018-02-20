using System.Linq;
using System.Threading.Tasks;
using Demo.Models;
using Demo.Models.ViewModels;
using Demo.Library.Extensions;
using Microsoft.AspNetCore.Mvc;
using Demo.Infrastructure;
using Demo.Filters;

namespace Demo.Controllers
{
    public class AuthController : Controller
    {
        IDataRepository repository;
        ISessionRepository session;

        public AuthController(IDataRepository repository, ISessionRepository session)
        {
            this.repository = repository;
            this.session = session;
        }

        public ViewResult Login(string returnURL)
        {
            return View(new LoginViewModel
            {
                ReturnURL = returnURL
            });
        }

        /**
         * TODO: DataController already implements the login functionality
         * We need to change the below duplicate code and use DataController
         * class
         */
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateModelAttribute]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            var user = (from u in this.repository.Users
                        where u.Email == model.EMailAddress && u.Password == model.Password
                        select u).FirstOrDefault();

            if (user != null)
            {
                this.session.User = user;

                if (Request.IsAjaxRequest())
                {
                    return Redirect("/");
                }

                return Redirect(model?.ReturnURL ?? "/");
            }

            TempData["error"] = "Invalid Username or Password";
            return View(model);
        }

        public async Task<RedirectToRouteResult> Logout()
        {
            this.session.Destroy();
            return RedirectToRoute("Home");
        }

        public JsonResult GetUsers() => new JsonResult
        (
            (
                from p in this.repository.Users
                select p
            ).OrderBy(p => p.UserType).ToList()
        );
    }
}