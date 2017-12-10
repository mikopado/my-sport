using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SportsBarApp.Controllers
{
    public class WallController : Controller
    {
        // GET: Wall
        public ActionResult Wall()
        {
            return View();
        }
        [HttpPost]
        public ActionResult UploadPost(string post)
        {
            return View();
        }
    }
}