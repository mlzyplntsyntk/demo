using System;
using Demo.Filters;
using Demo.Infrastructure;
using Demo.Models;
using Demo.Models.Entities;
using Demo.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Demo.Library.Extensions;

namespace Demo.Controllers
{
    /**
     * Controls The Interaction between Views and Models related to
     * Articles entity, involving List, Publish and Like Actions
     * 
     * Demonstrates the usage of ViewModels, Dependency Injection, 
     * Aspect Oriented Programming using Attribute Filters like Authentication
     */
    public class ArticleController : Controller
    {
        IDataController controller;

        public int PageSize { get; set; } = 12;

        public ArticleController(IDataRepository repository, 
            ISessionRepository session,
            IDataController controller)
        {
            this.controller = controller;
        }

        public ActionResult List(string type, int page = 1)
        {
            String _type = "latest";
            switch(type)
            {
                case "Popular":
                    _type = "likes";
                    break;
                case "MostRead":
                    _type = "reads";
                    break;
            }
            ArticleListViewModel model = controller.ArticleList(page, PageSize, _type, true);

            ViewBag.ListTitle = $"List of {model.ViewTitle}";

            if (Request.IsAjaxRequest())
            {
                return PartialView("_ArticleList", model);
            }

            return View("List", model);
        }

        public IActionResult Read(String name, int id)
        {
            return View(this.controller.ArticleDetail(id));
        }

        [Authorization(UserType.MANAGER)]
        public IActionResult Create()
        {
            ViewBag.FormTitle = "Create New Article";
            ViewBag.ButtonTitle = "Create Article";
            return View(new Article());
        }

        /**
         * To demonstrate the usage of login implementation on form submission
         * i want to make sure that the request headers contain XMLHttpRequest
         */
        [HttpPost]
        [Authorization(UserType.MANAGER)]
        [ValidateModel]
        public IActionResult Create(Article model)
        {
            if (!Request.IsAjaxRequest())
            {
                TempData["message"] = "Need XMLHttpRequest";
                return Redirect("/Home/Forbidden");
            }

            var result = this.controller.ArticleCreate(model);
            if (result.Id >0)
            {
                TempData["message"] = $"{result.Title} has been published";
                String redirectURL = result.GetFriendlyURL();
                return new JsonResult(new
                {
                    Status = true,
                    articleURL = redirectURL
                });
            } else
            {
                return new JsonResult(new
                {
                    Status = false
                });
            }
        }
        
        [Authorization]
        public IActionResult Edit(int id)
        {
            ViewBag.FormTitle = "Updating Existing Article";
            ViewBag.ButtonTitle = "Update Article";
            var articleDetail = this.controller.ArticleDetail(id);
            return View("Edit", articleDetail.Article);
        }

        [HttpPost]
        [Authorization(UserType.MANAGER)]
        [ValidateModel]
        [ValidateArticleOwner]
        public IActionResult Edit(int id, Article model)
        {
            this.controller.ArticleEdit(id, model);

            TempData["message"] = $"Article has been updated";
            String redirectURL = model.GetFriendlyURL();
            return Redirect("/" + redirectURL);
        }

        [Authorization(UserType.MANAGER)]
        [ValidateArticleOwner]
        public IActionResult Delete(int id, bool? confirm)
        {
            if (confirm != null)
            {
                this.controller.ArticleDelete(id);
                TempData["message"] = "The article deleted successfully";
                return Redirect("/");
            }
            else
            {
                var articleDetail = this.controller.ArticleDetail(id);
                return View(articleDetail.Article);
            }
            
        }

        [Authorization]
        public JsonResult Like(int id)
        {
            return controller.ArticleLike(id);
        }

        [Authorization]
        public JsonResult UndoLike(int id)
        {
            return controller.ArticleUndoLike(id);
        }

    }
}