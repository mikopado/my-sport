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
        private SportsBarService service = new SportsBarService(new UnitOfWork(new SportsBarDbContext()));
        
        // GET: Profile
        public ActionResult Index()
        {
            //The main page after the login or sign up
            // The page has profile personal details, wall with posts and comments and list of friends

            var userId = service.GetCurrentUserId(User);
            Profile profile = service.GetProfileByUserId(userId);
            IEnumerable<Post> posts = service.GetPosts().OrderByDescending(p => p.Timestamp);
            ProfileWallViewModel wall = new ProfileWallViewModel()
            {
                UserProfile = profile,
                Posts = posts
                
            };

            return View(wall);
        }

        public ActionResult MyProfile()
        {
            //Action to launch my profile page where edit, view profile deatils
            var userId = service.GetCurrentUserId(User);

            ViewBag.Domain = "profile"; //To check from which action the main layout is open. Useful to get the User name into the profile page
           
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

            ViewBag.Partial = "Details"; //To check if it's edit or view section in Profile section of My profile page
            ViewBag.Domain = "profile";
            return View("MyProfile", profile);
        }

        // GET: Profile/Create
        public ActionResult Create()
        {
            ViewBag.GlobalId = service.GetCurrentUserId(User);
            
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
                service.Save();
                return RedirectToAction("Index");
            }
            ViewBag.GlobalId = service.GetCurrentUserId(User);
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
            ViewBag.Domain = "profile";
            ViewBag.GlobalId = service.GetCurrentUserId(User);
            return View("MyProfile", profile);
        }

        // POST: Profile/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProfileId, FirstName,LastName,DateOfBirth,City,Country,FavouriteSports,FavouriteTeams,GlobalId")] Profile profile)
        {
            ViewBag.GlobalId = service.GetCurrentUserId(User);
            if (ModelState.IsValid)
            {
                service.Edit(profile);
                service.Save();
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
            service.Save();
            return RedirectToAction("Index");
        }

        public ActionResult CheckProfileExist()
        {
            //Action to verify the user when brand ic clicked to access the main page. If user exists and is not logout redirect to the profile main page(wall)
            // otherwise redirect to the create or landing page
            Profile profile = service.GetProfileByUserId(service.GetCurrentUserId(User));

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
            Profile profile = service.Find(id);
            //View bags to keep showing user name in the my profile page
            ViewBag.FirstName = profile.FirstName;
            ViewBag.LastName = profile.LastName;
            ViewBag.ID = id;
            ViewBag.Domain = "image";
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
                        Profile p = service.GetProfileByUserId(service.GetCurrentUserId(User));
                        p.ProfilePic = image;
                        service.Edit(p);
                        service.Save();

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
        public ActionResult UploadPost([Bind(Include = "Id, Message, Timestamp, ProfileId")]Post post, string page)
        {
            
            post.ProfileId = service.GetProfileByUserId(service.GetCurrentUserId(User)).ProfileId;
            post.Timestamp = DateTime.Now;
            service.Add(post);
            service.Save();
            //If the post is uploaded into activity, stay in the activity page
            if(page.Equals("post-profile"))
            {
                return RedirectToAction("Activity");
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult UploadComment([Bind(Include = "Id, Text, Timestamp, ProfileId, PostId")]Comment comment, string page)
        {

            comment.ProfileId = service.GetProfileByUserId(service.GetCurrentUserId(User)).ProfileId;
            comment.Timestamp = DateTime.Now;
            service.Add(comment);
            service.Save();
            if(page.Equals("post-profile"))
            {
                return RedirectToAction("Activity");
            }
            return RedirectToAction("Index");
        }

        public ActionResult Activity()
        {
            //In the my profile page, shows activity for the current user and also the text area t upload new post

            var userId = service.GetCurrentUserId(User);
            Profile profile = service.GetProfileByUserId(userId);

            //Get a coolections of post by user id. Shows only theposts written by the current user
            IEnumerable<Post> posts = service.GetPostsByUser(profile.ProfileId).OrderByDescending(p => p.Timestamp);

            ProfileWallViewModel wvm = new ProfileWallViewModel()
            {
                UserProfile = profile,
                Posts = posts
            };
            ViewBag.Domain = "post-profile";
            return View(wvm);
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
