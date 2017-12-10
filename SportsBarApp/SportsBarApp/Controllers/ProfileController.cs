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
using System.Threading.Tasks;

namespace SportsBarApp.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private ProfileService profileService = new ProfileService(new AppRepository<Profile>(new ProfileDbContext()));
        private ProfileService imageService = new ProfileService(new AppRepository<Image>(new ProfileDbContext()));

        // GET: Profile
        public ActionResult Index()
        {
            var userId = profileService.GetCurrentProfileId(User);

            return View(profileService.GetProfileByUserId(userId));
        }

        public ActionResult MyProfile()
        {

            var userId = profileService.GetCurrentProfileId(User);

            return View(profileService.GetProfileByUserId(userId));
        }

        // GET: Profile/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Profile profile = profileService.Find(id);
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
            ViewBag.GlobalId = profileService.GetCurrentProfileId(User);
            
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
                
                profileService.Add(profile);
                return RedirectToAction("Index");
            }
            ViewBag.GlobalId = profileService.GetCurrentProfileId(User);
            return View(profile);
        }
        
        // GET: Profile/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Profile profile = profileService.Find(id);
            if (profile == null)
            {
                return HttpNotFound();
            }
            ViewBag.Partial = "Edit";
            ViewBag.GlobalId = profileService.GetCurrentProfileId(User);
            return View("MyProfile", profile);
        }

        // POST: Profile/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProfileId, FirstName,LastName,DateOfBirth,City,Country,FavouriteSports,FavouriteTeams,GlobalId")] Profile profile)
        {
            ViewBag.GlobalId = profileService.GetCurrentProfileId(User);
            if (ModelState.IsValid)
            {
                profileService.Edit(profile);
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
            Profile profile = profileService.Find(id);
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
            Profile profile = profileService.Find(id);
            profileService.Remove(profile);
            return RedirectToAction("Index");
        }

        public ActionResult CheckProfileExist()
        {
            Profile profile = profileService.GetProfileByUserId(profileService.GetCurrentProfileId(User));

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

        public ActionResult ChangeProfilePhoto(int id)
        {
            Profile profile = profileService.Find(id);
            ViewBag.FirstName = profile.FirstName;
            ViewBag.LastName = profile.LastName;
            ViewBag.ID = id;
            if (profile.ProfilePic == null)
            {
                return View();
            }
            return View(profile.ProfilePic);
        }

        [HttpPost]
        public ActionResult ChangeProfilePhoto([Bind(Include = "ImageId, BinImage")]Image image)
        {
            if (ModelState.IsValid)
            {
                imageService.AddOrUpdate(image);
                Profile p = profileService.GetProfileByUserId(profileService.GetCurrentProfileId(User));
                //p.ProfilePic = image;
                profileService.Edit(p);
                return RedirectToAction("Index");
            }

            return View(image);
        }



        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                profileService.DisposeContext();
            }
            base.Dispose(disposing);
        }

        
    }
}
