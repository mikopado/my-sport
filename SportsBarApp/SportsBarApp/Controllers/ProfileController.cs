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
using SportsBarApp.Models.ViewModels;
using System.Data.Entity.Infrastructure;

namespace SportsBarApp.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private ProfileService profileService = new ProfileService(new AppRepository<Profile>(new ProfileDbContext()));
        private ProfileService imageService = new ProfileService(new AppRepository<Image>(new ProfileDbContext()));
        private ProfileService postService = new ProfileService(new AppRepository<Post>(new ProfileDbContext()));
        private ProfileService commentService = new ProfileService(new AppRepository<Comment>(new ProfileDbContext()));

        // GET: Profile
        public ActionResult Index()
        {
            var userId = profileService.GetCurrentUserId(User);
            Profile profile = profileService.GetProfileByUserId(userId);
            IEnumerable<Post> posts = postService.GetPosts().OrderByDescending(p => p.Timestamp);
            ProfileWallViewModel wall = new ProfileWallViewModel()
            {
                UserProfile = profile,
                Posts = posts
                
            };

            return View(wall);
        }

        public ActionResult MyProfile()
        {

            var userId = profileService.GetCurrentUserId(User);

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
            ViewBag.GlobalId = profileService.GetCurrentUserId(User);
            
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
            ViewBag.GlobalId = profileService.GetCurrentUserId(User);
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
            ViewBag.GlobalId = profileService.GetCurrentUserId(User);
            return View("MyProfile", profile);
        }

        // POST: Profile/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProfileId, FirstName,LastName,DateOfBirth,City,Country,FavouriteSports,FavouriteTeams,GlobalId")] Profile profile)
        {
            ViewBag.GlobalId = profileService.GetCurrentUserId(User);
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
            Profile profile = profileService.GetProfileByUserId(profileService.GetCurrentUserId(User));

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
        public ActionResult ChangeProfilePhoto([Bind(Include = "ProfilePic, Content")]Image image, HttpPostedFileBase upload)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (upload != null && upload.ContentLength > 0)
                    {
                        
                        using (var reader = new System.IO.BinaryReader(upload.InputStream))
                        {
                            image.Content = reader.ReadBytes(upload.ContentLength);
                        }                        
                        Profile p = profileService.GetProfileByUserId(profileService.GetCurrentUserId(User));
                        p.ProfilePic = image;
                        profileService.Edit(p);

                    }
                   
                    return RedirectToAction("Index");
                }
            }
            catch (RetryLimitExceededException)
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }
            

            return View(image);
        }

        [HttpPost]
        public ActionResult UploadPost([Bind(Include = "Id, Message, Timestamp, ProfileId")]Post post)
        {
            
            post.ProfileId = profileService.GetProfileByUserId(profileService.GetCurrentUserId(User)).ProfileId;
            post.Timestamp = DateTime.Now;
            postService.Add(post);            
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult UploadComment([Bind(Include = "Id, Text, Timestamp, ProfileId, PostId")]Comment comment)
        {

            comment.ProfileId = profileService.GetProfileByUserId(profileService.GetCurrentUserId(User)).ProfileId;
            comment.Timestamp = DateTime.Now;           
            commentService.Add(comment);
            return RedirectToAction("Index");
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
