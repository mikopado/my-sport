using System.Collections.Generic;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SportsBarApp.Models;
using System.IO;
using SportsBarApp.ServiceLayer;
using SportsBarApp.Models.DAL;
using SportsBarApp.Models.ViewModels;
using System.Data.Entity.Infrastructure;




namespace SportsBarApp.Controllers
{
    [Authorize]    
    public class ProfileController : Controller
    {
        private AppService appService; 

        //For testing purpose
        public ProfileController()
        {
            appService = new AppService(new UnitOfWork(new SportsBarDbContext()));
        }
        //For testing purpose
        public ProfileController(IUnitOfWork unit)
        {
            appService = new AppService(unit);
        }
        
        [Route("profile/{id}")]
        public ActionResult MyProfile(int? id)
        {
            //Action to launch my profile page where edit, view profile deatils
            
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "An error occurred whilst processing your request.");
            }
            Profile profile = appService.GetProfile(id);
            if (profile == null)
            {
                return HttpNotFound();
            }

            ProfileViewModel vm = new ProfileViewModel
            {
                CurrentProfile = profile,
                IsThisUser = appService.EnsureIsUserProfile(profile, User),
                CurrentView = "profile",
                Photo = profile.ProfilePic.FileName
            };
            
            if (!vm.IsThisUser)
            {
                var user = appService.GetProfile(appService.GetCurrentUserId(User));

                vm.FriendStatus = appService.GetFriendStatus(user, profile).Item1;
                vm.ButtonStatus = appService.GetFriendStatus(user, profile).Item2;
                vm.User = user;
                
                vm.Photo = user.ProfilePic.FileName;
            }
            
            return View(vm);
        }


        // GET: Profile/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "An error occurred whilst processing your request.");
            }
            Profile profile = appService.GetProfile(id);
            if (profile == null)
            {
                return HttpNotFound();
            }
           
            ProfileViewModel vm = new ProfileViewModel
            {
                CurrentProfile = profile,
                IsThisUser = appService.EnsureIsUserProfile(profile, User),
                CurrentView = "profile",
                Photo = profile.ProfilePic.FileName
            };

            ViewBag.Partial = "Details"; //To check if it's edit or view section in Profile section of My profile page
           
            return View("MyProfile", vm);
        }

        // GET: Profile/Create
        public ActionResult Create()
        {
            ViewBag.GlobalId = appService.GetCurrentUserId(User);

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
                //Image pic = new Image { FileName = "../../Content/images/avatar-default.png" };
                profile.ProfilePic = pic;
                appService.Add(profile);
                appService.Save();
                return RedirectToAction("NewsFeed", "Wall");
            }
            ViewBag.GlobalId = appService.GetCurrentUserId(User);
            return View(profile);
        }

        // GET: Profile/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "An error occurred whilst processing your request.");
            }
            Profile profile = appService.GetProfile(id);
            bool isUser = appService.EnsureIsUserProfile(profile, User);
            if (profile == null || !isUser)
            {
                return HttpNotFound();
            }
           
            ProfileViewModel vm = new ProfileViewModel
            {
                CurrentProfile = profile,
                CurrentView = "profile",
                IsThisUser = isUser,
                Photo = profile.ProfilePic.FileName
            };

            ViewBag.Partial = "Edit";
            
            ViewBag.GlobalId = appService.GetCurrentUserId(User);
            return View("MyProfile", vm);
        }

        // POST: Profile/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CurrentProfile")] ProfileViewModel profileVm)
        {
            ViewBag.GlobalId = appService.GetCurrentUserId(User);
            if (ModelState.IsValid)
            {
                appService.Edit(profileVm.CurrentProfile);
                appService.Save();                
                return RedirectToAction("Details", new { id = profileVm.CurrentProfile.ProfileId });

            }
           

            return RedirectToAction("MyProfile", new { id = profileVm.CurrentProfile.ProfileId });

        }

        // GET: Profile/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "An error occurred whilst processing your request.");
            }
            Profile profile = appService.GetProfile(id);
            if (profile == null || !appService.EnsureIsUserProfile(profile, User))
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
            Profile profile = appService.GetProfile(id);
            appService.Remove(profile);
            appService.Save();

            return RedirectToAction("Index", "Home");
        }       

        public ActionResult ChangeProfilePhoto(int? id)
        {
            //Change the profile photo
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "An error occurred whilst processing your request.");
            }
            Profile profile = appService.GetProfile(id);
            bool isUser = appService.EnsureIsUserProfile(profile, User);

            if (profile == null || !isUser)
            {
                return HttpNotFound();
            }
             
            ProfileViewModel chphoto = new ProfileViewModel
            {
                CurrentProfile = profile,
                CurrentView = "profile",
                IsThisUser = isUser,
                Photo = profile.ProfilePic.FileName,
                
            };
            
            return PartialView("ChangeProfilePhoto", chphoto);
        }

        [HttpPost]
       
        public ActionResult ChangeProfilePhoto(HttpPostedFileBase upload)
        {
            //Save new image into database from the file uploaded
            Profile profile = appService.GetProfile(appService.GetCurrentUserId(User));
            Image image = profile.ProfilePic;
            try
            {
                if (ModelState.IsValid)
                {
                    if (upload != null && upload.ContentLength > 0)
                    {

                        using (var reader = new BinaryReader(upload.InputStream))
                        {
                            image.Content = reader.ReadBytes(upload.ContentLength);

                        }
                        appService.Edit(profile);
                        appService.Save();

                    }

                    return RedirectToAction("MyProfile", new {id = profile.ProfileId });
                }
            }
            catch (RetryLimitExceededException)
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }
            ProfileViewModel chphoto = new ProfileViewModel
            {
                CurrentProfile = profile,
                CurrentView = "profile",
                IsThisUser = appService.EnsureIsUserProfile(profile, User),
                Photo = profile.ProfilePic.FileName
            };

            return PartialView("ChangeProfilePhoto", chphoto);
        }
       
        [Route("Profile/Search")]
        public ActionResult Search(string search)
        {
            //Handles the autocomplete search box at the left side of navbar

            Profile user = appService.GetProfile(appService.GetCurrentUserId(User));
            var friends = appService.SearchProfiles(search);
            List<ProfileViewModel> searchResult = new List<ProfileViewModel>();

            foreach (var friend in friends)
            {
                if (!friend.Equals(user))
                {
                    searchResult.Add(new ProfileViewModel
                    {
                        CurrentProfile = friend,
                        Name = friend.FirstName + " " + friend.LastName,
                        Photo = friend.ProfilePic.FileName
                    });

                }
            }
            return Json(searchResult, JsonRequestBehavior.AllowGet);

        }


       
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                appService.DisposeContext();
            }
            base.Dispose(disposing);
        }

        
    }
}
