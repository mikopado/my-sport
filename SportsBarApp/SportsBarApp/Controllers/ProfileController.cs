using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.AspNet.Identity;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SportsBarApp.Models;
using System.IO;
using System.Security.Principal;
using SportsBarApp.ServiceLayer;
using SportsBarApp.Models.DAL;

namespace SportsBarApp.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private ProfileService service = new ProfileService(new AppRepository<Profile>(new ProfileDbContext()));

        // GET: Profile
        public ActionResult Index()
        {
            var userId = service.GetCurrentProfileId(User);

            return View(service.GetProfileByUserId(userId));
        }

        public ActionResult MyProfile()
        {

            var userId = service.GetCurrentProfileId(User);

            return View(service.GetProfileByUserId(userId));
        }

        // GET: Profile/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Profile profile = service.Find(id);
            if (profile == null)
            {
                return HttpNotFound();
            }

            ViewBag.Partial = "Details";
            return View("MyProfile", profile);
        }

        // GET: Profile/Create
        public ActionResult Create()
        {
            ViewBag.GlobalId = service.GetCurrentProfileId(User);
            
            return View();
        }

        // POST: Profile/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Profile profile)
        {
           
            if (ModelState.IsValid)
            {

                service.Add(profile);
                return RedirectToAction("Index");
            }
            ViewBag.GlobalId = service.GetCurrentProfileId(User);
            return View(profile);
        }
        
        // GET: Profile/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Profile profile = service.Find(id);
            if (profile == null)
            {
                return HttpNotFound();
            }
            ViewBag.Partial = "Edit";
            ViewBag.GlobalId = service.GetCurrentProfileId(User);
            return View("MyProfile", profile);
        }

        // POST: Profile/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProfileId, FirstName,LastName,DateOfBirth,City,Country,FavouriteSports,FavouriteTeams,GlobalId")] Profile profile)
        {
            ViewBag.GlobalId = service.GetCurrentProfileId(User);
            if (ModelState.IsValid)
            {
                service.Edit(profile);
                ViewBag.Partial = "Details";
                return RedirectToAction("MyProfile", profile);
                
            }
            ViewBag.Partial = "Edit";
            
            return View("MyProfile", profile);
        }

        // GET: Profile/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Profile profile = service.Find(id);
            if (profile == null)
            {
                return HttpNotFound();
            }
            return View(profile);
        }

        // POST: Profile/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Profile profile = service.Find(id);
            service.Remove(profile);
            return RedirectToAction("Index");
        }

        public ActionResult CheckProfileExist()
        {
            Profile profile = service.GetProfileByUserId(service.GetCurrentProfileId(User));

            if (Request.IsAuthenticated)
            {
                if (profile == null)
                {
                    return RedirectToAction("Create");
                }
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index", "Home");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                service.DisposeContext();
            }
            base.Dispose(disposing);
        }

        
    }
}
