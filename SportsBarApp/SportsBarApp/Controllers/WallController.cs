using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SportsBarApp.Models;
using SportsBarApp.ServiceLayer;
using SportsBarApp.Models.DAL;

namespace SportsBarApp.Controllers
{
    public class WallController : Controller
    {
        private ProfileService service = new ProfileService(new AppRepository<Post>(new ProfileDbContext()));

        // GET: Wall
        public ActionResult Wall()
        {

            return View(service.GetPosts());
        }
       
    }
}