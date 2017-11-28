using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SportsBarApp.Controllers
{
   
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.IsHomePage = true;           
            return View();
        }

       
    }
}