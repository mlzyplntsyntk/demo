using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Demo.Models.ViewModels;
using Demo.Infrastructure;

namespace Demo.Controllers
{
    public class HomeController : Controller
    {
        IDataController controller;
        public HomeController(IDataController controller)
        {
            this.controller = controller;
        }

        public IActionResult Forbidden() => View();

        public IActionResult Contact() => View();

        public IActionResult About() => View();

        public ViewResult Index() => View(new List<ArticleListViewModel>
        {
            this.controller.ArticleList(1, 3, "latest", false),

            this.controller.ArticleList(1, 3, "likes", false),

            this.controller.ArticleList(1,3,"reads", false)

        });
           
    }
}
