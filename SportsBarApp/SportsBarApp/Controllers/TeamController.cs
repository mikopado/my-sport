using SportsBarApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SportsBarApp.Controllers
{
    public class TeamController : Controller
    {
        // GET: Team      
        
        public ActionResult SelectFavouriteTeam()
        {            
            return View();
        }
        [HttpPost]
        public ActionResult SelectFavouriteTeam(TeamViewModel model)
        {
            
            return RedirectToAction("DisplayArticles", "Team", model);
        }
        //[HttpPost]
        //public ActionResult SelectFavouriteTeam(TeamViewModel model)
        //{
        //    model.Articles = new List<Article>() { new Article(model.Team), new Article(model.Team), new Article(model.Team), new Article(model.Team) }; 
        //    return View("DisplayArticles", model);
        //}

        [Route("team/articles/{team}")]
        public ActionResult DisplayArticles(TeamViewModel model, string team)
        {
            model.Articles = new List<string>() { (new Article(model.Team)).CreateArticle(), (new Article(model.Team)).CreateArticle(), (new Article(model.Team)).CreateArticle(), (new Article(model.Team)).CreateArticle() };
            return View(model);
        }
    }
}