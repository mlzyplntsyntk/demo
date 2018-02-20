using System;
using System.Collections.Generic;
using System.Linq;
using Demo.Filters;
using Demo.Infrastructure;
using Demo.Models;
using Demo.Models.Entities;
using Demo.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Dynamic.Core;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Demo.Controllers
{
    /**
     * this api will provide all the article dependent operations like
     * get set update and delete
     * the purpose of this Web API is to demonstrate 3rd party apps 
     * to consume article services from an independent context
     * 
     * It also extends from an IDataController context, so my MVC app
     * will be able to inject this service as an object
     */
    [Route("[controller]")]
    public class DataController : Controller, IDataController
    {
        IDataRepository repository;
        ISessionRepository session;
        public DataController(IDataRepository repository, ISessionRepository session)
        {
            this.repository = repository;
            this.session = session;
        }

        [HttpGet("articles")]
        // GET: data/articles?pageSize=12&pageNumber=1&orderby=latest&withPaging=true
        public ArticleListViewModel ArticleList(int pageNumber, 
            int pageSize, 
            string orderby,
            bool withPaging = false)
        {
            ArticleListViewModel model = new ArticleListViewModel();
            model.ViewKey = orderby;

            IQueryable<Article> articleSearch = from a in this.repository.Articles
                                                join u in this.repository.Users on a.User.Id equals u.Id
                                                select new Article
                                                {
                                                    Id = a.Id,
                                                    User = new User
                                                    {
                                                        Id = u.Id,
                                                        FirstName = u.FirstName,
                                                        LastName = u.LastName
                                                    },
                                                    UserId = u.Id,
                                                    CreationTime = a.CreationTime,
                                                    Title = a.Title,
                                                    TotalLikes = a.TotalLikes,
                                                    TotalReads = a.TotalReads
                                                };

            if ("reads".Equals(orderby))
            {
                model.ArticleOrder = "TotalReads desc";
                model.ViewTitle = "Most Read Articles";
                model.PageAction = "MostRead";
            }

            if ("likes".Equals(orderby))
            {
                model.ArticleOrder = "TotalLikes desc";
                model.ViewTitle = "Popular Articles";
                model.PageAction = "Popular";
            }

            model.Articles = articleSearch.OrderBy(model.ArticleOrder)
                                            .Skip((pageNumber - 1) * pageSize)
                                            .Take(pageSize);
            
            model.PagingInfo = new ArticleListPagingViewModel
            {
                TotalItems = withPaging ? this.repository.Articles.Count() : 0,
                CurrentPage = pageNumber,
                ItemsPerPage = pageSize
            };

            return model;
        }

        [HttpGet("{id}")]
        // GET data/5
        [ExternalAuthorizationAttribute]
        public ArticleDetailViewModel ArticleDetail(int id)
        {
            if (this.session.User != null)
                ApplyArticleAction(id, ArticleActionType.READ);

            var article = (from a in this.repository.Articles
                           join u in this.repository.Users on a.UserId equals u.Id
                           where a.Id == id
                           select new Article
                           {
                               Id = a.Id,
                               User = new User {
                                   Id = u.Id,
                                   FirstName = u.FirstName,
                                   LastName = u.LastName
                               },
                               CreationTime = a.CreationTime,
                               Title = a.Title,
                               Content = a.Content,
                               TotalLikes = a.TotalLikes,
                               TotalReads = a.TotalReads
                           }).FirstOrDefault();

            ArticleDetailViewModel model = new ArticleDetailViewModel
            {
                Article = article,
                Liked = this.session.User != null ?
                        (from p in this.repository.ArticleActions
                         where p.Article.Id == id &&
                         p.User.Id == this.session.User.Id &&
                         p.ArticleActionType == ArticleActionType.LIKE
                         select p).Count() > 0 : false,
                User = this.session.User
            };

            return model;
        }

        [HttpGet("like")]
        // GET data/like?id=5
        [Authorization]
        public JsonResult ArticleLike(int id)
        {
            bool status = ApplyArticleAction(id, ArticleActionType.LIKE);
            return Json(new {
                Status = status,
                Entity = this.ArticleDetail(id)
            });
        }

        [HttpGet("undolike")]
        // GET data/undolike?id=5
        [Authorization]
        public JsonResult ArticleUndoLike(int id)
        {
            bool status = ApplyArticleAction(id, ArticleActionType.UNLIKE);
            return Json(new {
                Status = status,
                Entity = this.ArticleDetail(id)
            });
        }

        [HttpPost]
        // POST data
        [Authorization(UserType.MANAGER)]
        [ValidateModel]
        public Article ArticleCreate([FromBody]Article model)
        {
            var user = (from u in this.repository.Users
                        where u.Id == this.session.User.Id
                        select u).FirstOrDefault();

            model.User = user;

            this.repository.Save(model);
            return model;
        }

        [HttpPut("{id}")]
        // PUT data/5
        [Authorization(UserType.MANAGER)]
        [ValidateModel]
        [ValidateArticleOwner]
        public void ArticleEdit(int id, [FromBody]Article model)
        {
            var article = (from a in this.repository.Articles
                           where a.Id == model.Id && a.UserId == this.session.User.Id
                           select a).FirstOrDefault();
            article.Title = model.Title;
            article.Content = model.Content;
            this.repository.SaveAttached(article);

        }

        [HttpDelete("{id}")]
        // DELETE api/<controller>/5
        [Authorization(UserType.MANAGER)]
        [ValidateArticleOwner]
        public void ArticleDelete(int id)
        {
            var article = (from a in this.repository.Articles
                           where a.Id == id
                           select a).FirstOrDefault();
            this.repository.Remove((from a in this.repository.ArticleActions
                                    where a.Article.Id == id
                                    select a));
            this.repository.Remove(article);
        }
        
        [HttpPost("login")]
        // POST data
        public User UserLogin(string username, string password)
        {
            var user = (from u in this.repository.Users
                        where u.Email == username && u.Password == password
                        select u).FirstOrDefault();

            if (user != null)
            {
                var claims = new Claim[] {
                    new Claim(ClaimTypes.Name, user.FirstName),
                    new Claim(ClaimTypes.SerialNumber, user.Id.ToString()),
                    new Claim(JwtRegisteredClaimNames.NameId, user.Id.ToString()),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email)
                };
                var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Program.tokenKey));

                var token = new JwtSecurityToken(
                    issuer: "Demo",
                    audience: "ClientApp",
                    claims: claims,
                    notBefore: DateTime.Now,
                    expires: DateTime.Now.AddDays(28),
                    signingCredentials: new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256)
                    
                );

                var tokenHandler = new JwtSecurityTokenHandler();
                user.UserToken = tokenHandler.WriteToken(token);

                return user;
            }

            return null;
        }

        // checks whether the current user liked or read the article
        // if this article does not belong to him/her. 
        // if actionType is LIKE and he/she liked, removes otherwise stores the LIKE action  
        // if actionType is READ and he/she did not, stores the READ action
        private bool ApplyArticleAction(int id, ArticleActionType type)
        {
            var article = (from a in this.repository.Articles
                           where a.Id == id
                           select a).FirstOrDefault();
            if (article != null && article.UserId == this.session.User.Id)
            {
                return false;
            }

            var queryType = type.Equals(ArticleActionType.UNLIKE) ? ArticleActionType.LIKE : type;

            var articleAction = (from a in this.repository.ArticleActions
                                 where a.User.Id == this.session.User.Id &&
                                         a.Article.Id == id &&
                                         a.ArticleActionType == queryType
                                 select a).FirstOrDefault();

            switch (type)
            {
                case ArticleActionType.LIKE:
                case ArticleActionType.READ:
                    if (articleAction == null)
                    {
                        var user = this.repository.Users.Single(u => u.Id == this.session.User.Id);

                        this.repository.Save(new ArticleAction
                        {
                            User = new Models.Entities.User { Id = this.session.User.Id },
                            Article = new Article { Id = article.Id },
                            ArticleActionType = type
                        });
                    }
                    break;
                case ArticleActionType.UNLIKE:
                    if (articleAction != null)
                    {
                        this.repository.Remove(articleAction);
                    }
                    break;
            }
            return true;
        }

    }
}
