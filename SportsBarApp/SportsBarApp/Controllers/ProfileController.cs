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
                Posts = posts,
                ProfileId = profile.ProfileId,
                FirstName = profile.FirstName,
                LastName = profile.LastName
                
            };

            return View(wall);
        }

        public ActionResult MyProfile(int? id)
        {
            //Action to launch my profile page where edit, view profile deatils
            //var userId = service.GetCurrentUserId(User);
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Profile profile = service.Find(id);
            if (profile == null)
            {
                return HttpNotFound();
            }
            ViewBag.Domain = "profile";
            ViewBag.IsThisUser = service.EnsureIsUserProfile(profile, User);
            if (!ViewBag.IsThisUser)
            {
                var user = service.GetProfileByUserId(service.GetCurrentUserId(User));
                
                ViewBag.FriendStatus = service.FriendStatus(user, profile).Item1;
                ViewBag.ButtonStatus = service.FriendStatus(user, profile).Item2;
            }
            
            return View(profile);
        }

        public ActionResult MyProfileNavbar(int? id)
        {
            //Action related to myProfile button on navbar. When user visits other user profiles 
            //the action must redirect to the current user profile
            Profile profile = service.GetProfileFromId(id);
            
            if(service.EnsureIsUserProfile(profile, User))
            {
                return RedirectToAction("MyProfile", new {id = id });
            }
            return RedirectToAction("MyProfile", new { id = service.GetProfileId(service.GetCurrentUserId(User)) });
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

            ViewBag.IsThisUser = service.EnsureIsUserProfile(profile, User);
           
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
            if (profile == null || !service.EnsureIsUserProfile(profile, User))
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
            if (profile == null || !service.EnsureIsUserProfile(profile, User))
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
           //TODO IS NOT WORKING issue with Profile ID foreign key in Comment table
            Profile profile = service.Find(id);
            service.Remove(profile);
            service.Save();
            
            return RedirectToAction("Index", "Home");
        }

        public ActionResult CheckProfileExist()
        {
            //Action to verify the user when brand logo is clicked to access the main page. If user exists and is not logout redirect to the profile main page(wall)
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

        public ActionResult ChangeProfilePhoto(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Profile profile = service.Find(id);
            if (profile == null || !service.EnsureIsUserProfile(profile, User))
            {
                return HttpNotFound();
            }
            //View bags to keep showing user name in the my profile page
            //ViewBag.FirstName = profile.FirstName;
            //ViewBag.LastName = profile.LastName;
            //ViewBag.ID = id;
            
            ChangePhotoViewModel chphoto = new ChangePhotoViewModel
            {
                FirstName = profile.FirstName,
                LastName = profile.LastName,
                Photo = profile.ProfilePic,
                ProfileId = profile.ProfileId
            };

            //if (profile.ProfilePic == null)
            //{
            //    return View();
            //}            
            ViewBag.Domain = "profile";
            return View(chphoto);
        }

        [HttpPost]
        public ActionResult ChangeProfilePhoto([Bind(Include = "Photo")]Image image, HttpPostedFileBase upload)
        {
            Profile p = service.GetProfileByUserId(service.GetCurrentUserId(User));
            try
            {
                if (ModelState.IsValid)
                {
                    if (upload != null && upload.ContentLength > 0)
                    {
                        
                        using (var reader = new BinaryReader(upload.InputStream))
                        {
                            image.Content = reader.ReadBytes(upload.ContentLength);
                            image.FileName = upload.FileName;
                            
                        }                        
                        
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
            ChangePhotoViewModel chphoto = new ChangePhotoViewModel
            {
                FirstName = p.FirstName,
                LastName = p.LastName,
                ProfileId = p.ProfileId,
                Photo = p.ProfilePic
            };
            ViewBag.Domain = "profile";
            return View(chphoto);
        }

        [HttpPost]
        public ActionResult UploadPost([Bind(Include = "Id, Message, Timestamp, ProfileId")]Post post, string page)
        {
            
            post.ProfileId = service.GetProfileByUserId(service.GetCurrentUserId(User)).ProfileId;
            post.Timestamp = DateTime.Now;
            service.Add(post);
            service.Save();
            //If the post is uploaded into activity, stay in the activity page
            if(page.Equals("activity"))
            {
                return RedirectToAction("Activity", new { id= post.ProfileId });
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
            if(page.Equals("activity"))
            {
                return RedirectToAction("Activity", new { id = comment.ProfileId });
            }
            return RedirectToAction("Index");
        }

        public ActionResult Activity(int? id)
        {
            //In the my profile page, shows activity for the current user and also the text area t upload new post
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Profile profile = service.Find(id);
            if (profile == null)
            {
                return HttpNotFound();
            }
            //var userId = service.GetCurrentUserId(User);
            //Profile profile = service.GetProfileByUserId(userId);

            //Get a coolections of post by user id. Shows only theposts written by the current user
            IEnumerable<Post> posts = service.GetPostsByUser(profile.ProfileId).OrderByDescending(p => p.Timestamp);

            ProfileWallViewModel wvm = new ProfileWallViewModel()
            {
                UserProfile = profile,
                Posts = posts,
                FirstName = profile.FirstName,
                LastName = profile.LastName,
                ProfileId = profile.ProfileId
            };
            ViewBag.Domain = "activity";
            ViewBag.IsThisUser = service.EnsureIsUserProfile(profile, User);
            if (!ViewBag.IsThisUser)
            {
                var user = service.GetProfileByUserId(service.GetCurrentUserId(User));
                
                ViewBag.FriendStatus = service.FriendStatus(user, profile).Item1;
                ViewBag.ButtonStatus = service.FriendStatus(user, profile).Item2;
            }
            return View(wvm);
        }
        
        
        public ActionResult Search(string search)
        {
            //string term = Request["search"];
            Profile user = service.GetProfileByUserId(service.GetCurrentUserId(User));
            var friends = service.SearchProfiles(search);
            List<ProfileWallViewModel> searchResult = new List<ProfileWallViewModel>();
            foreach (var friend in friends)
            {
                if (!friend.Equals(user))
                {
                    searchResult.Add(new ProfileWallViewModel
                    {
                        Friend = friend,
                        Name = friend.FirstName + " " + friend.LastName,
                        Photo = friend.ProfilePic != null ? String.Format("data:image/jpg;base64,{0}", Convert.ToBase64String(friend.ProfilePic.Content)) : "../Content/images/avatar-default.png"
                    });

                }
            }
            return Json(searchResult, JsonRequestBehavior.AllowGet);
            
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
