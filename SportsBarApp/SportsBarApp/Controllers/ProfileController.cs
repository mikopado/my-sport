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

            ProfileViewModel wall = new ProfileViewModel()
            {
                CurrentProfile = profile,
                Posts = posts,
                PendingRequests = service.GetPendingRequests(profile.ProfileId),
                IsThisUser = service.EnsureIsUserProfile(profile, User),
                Friends = service.GetFriends(profile.ProfileId),
                Photo = profile.ProfilePic.FileName
                
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
            
            ProfileViewModel vm = new ProfileViewModel
            {
                CurrentProfile = profile,
                IsThisUser = service.EnsureIsUserProfile(profile, User),
                CurrentView = "profile",
                PendingRequests = service.GetPendingRequests(profile.ProfileId),
                Photo = profile.ProfilePic.FileName
            };

            if (!vm.IsThisUser)
            {
                var user = service.GetProfileByUserId(service.GetCurrentUserId(User));
                
                vm.FriendStatus = service.GetFriendStatus(user, profile).Item1;
                vm.ButtonStatus = service.GetFriendStatus(user, profile).Item2;
                vm.User = user;
                vm.PendingRequests = service.GetPendingRequests(user.ProfileId);
                vm.Photo = user.ProfilePic.FileName;
            }
            
            
            return View(vm);
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

            ProfileViewModel vm = new ProfileViewModel
            {
                CurrentProfile = profile,
                IsThisUser = service.EnsureIsUserProfile(profile, User),
                CurrentView = "profile",
                PendingRequests = service.GetPendingRequests(profile.ProfileId),
                Photo = profile.ProfilePic.FileName
            };
           
            ViewBag.Partial = "Details"; //To check if it's edit or view section in Profile section of My profile page
           
            return View("MyProfile", vm);
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
                Image pic = new Image {FileName = "../../Content/images/avatar-default.png" };
                profile.ProfilePic = pic;
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
            bool isUser = service.EnsureIsUserProfile(profile, User);
            if (profile == null || !isUser)
            {
                return HttpNotFound();
            }

            ProfileViewModel vm = new ProfileViewModel
            {
                CurrentProfile = profile,
                CurrentView = "profile",
                IsThisUser = isUser,
                PendingRequests = service.GetPendingRequests(profile.ProfileId),
                Photo = profile.ProfilePic.FileName
            };

            ViewBag.Partial = "Edit";
           
            ViewBag.GlobalId = service.GetCurrentUserId(User);
            return View("MyProfile", vm);
        }

        // POST: Profile/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Profile")] ProfileViewModel profileVm)
        {
            ViewBag.GlobalId = service.GetCurrentUserId(User);
            if (ModelState.IsValid)
            {
                service.Edit(profileVm.CurrentProfile);
                service.Save();
                ViewBag.Partial = "Details";
                return RedirectToAction("MyProfile", new {id = profileVm.CurrentProfile.ProfileId });
                
            }
            ViewBag.Partial = "Edit";
            
            return View("MyProfile", profileVm);
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
            bool isUser = service.EnsureIsUserProfile(profile, User);
            if (profile == null || !isUser)
            {
                return HttpNotFound();
            }            
            
            ProfileViewModel chphoto = new ProfileViewModel
            {
                CurrentProfile = profile,
                CurrentView = "profile",
                PendingRequests = service.GetPendingRequests(profile.ProfileId),
                IsThisUser = isUser,
                Photo = profile.ProfilePic.FileName
            };                    
            
            return View(chphoto);
        }

        [HttpPost]
        public ActionResult ChangeProfilePhoto(HttpPostedFileBase upload)
        {
            Profile profile = service.GetProfileByUserId(service.GetCurrentUserId(User));
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
                        
                        //profile.ProfilePic = image;
                        service.Edit(profile);
                        service.Save();

                    }
                   
                    return RedirectToAction("Index");
                }
            }
            catch (RetryLimitExceededException)
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }
            ProfileViewModel chphoto = new ProfileViewModel
            {
               CurrentProfile = profile,
               CurrentView = "profile"
            };
            
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
            
            //Get a coolections of post by user id. Shows only theposts written by the current user
            IEnumerable<Post> posts = service.GetPostsByUser(profile.ProfileId).OrderByDescending(p => p.Timestamp);

            ProfileViewModel wvm = new ProfileViewModel()
            {
                CurrentProfile = profile,
                Posts = posts,                
                CurrentView = "activity",
                IsThisUser = service.EnsureIsUserProfile(profile, User),
                PendingRequests = service.GetPendingRequests(profile.ProfileId),
                Photo = profile.ProfilePic.FileName
            };
           
            if (!wvm.IsThisUser)
            {
                var user = service.GetProfileByUserId(service.GetCurrentUserId(User));

                wvm.FriendStatus = service.GetFriendStatus(user, profile).Item1;
                wvm.ButtonStatus = service.GetFriendStatus(user, profile).Item2;
                wvm.User = user;
                wvm.PendingRequests = service.GetPendingRequests(user.ProfileId);
                wvm.Photo = user.ProfilePic.FileName;
            }
            
            return View(wvm);
        }
        
        
        public ActionResult Search(string search)
        {

            
            Profile user = service.GetProfileByUserId(service.GetCurrentUserId(User));
            var friends = service.SearchProfiles(search);
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

        public ActionResult Friends(int? id)
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
            ProfileViewModel wvm = new ProfileViewModel
            {
                CurrentProfile = profile,
                IsThisUser = service.EnsureIsUserProfile(profile, User),
                Friends = service.GetFriends(profile.ProfileId),
                Photo = profile.ProfilePic.FileName,
             

            };

            if (!wvm.IsThisUser)
            {               
                
                var user = service.GetProfileByUserId(service.GetCurrentUserId(User));

                wvm.FriendStatus = service.GetFriendStatus(user, profile).Item1;
                wvm.ButtonStatus = service.GetFriendStatus(user, profile).Item2;
                wvm.User = user;
                
                wvm.Photo = user.ProfilePic.FileName;
                
            }
            wvm.PendingRequests = wvm.IsThisUser ? service.GetPendingRequests(profile.ProfileId) : service.GetPendingRequests(wvm.User.ProfileId);


            return View(wvm);
        }
        [HttpPost]
        public ActionResult AskFriendship([Bind(Include = "FriendId, ProfileId")] FriendRequest friendRequest)
        {
            
            friendRequest.IsAccepted = false;
           
            if (ModelState.IsValid)
            {
                service.Add(friendRequest);             
                
                service.Save();             
                
                service.NotifyUser(service.GetIdentityFromUserId(friendRequest.FriendId), friendRequest.ProfileId, friendRequest.Id);
            }

            return RedirectToAction("Friends", new {id = friendRequest.ProfileId });

        }

        [HttpPost]
        public void AcceptFriendship(int? id)
        {
            FriendRequest friendRequest = service.GetRequestById(id);
            friendRequest.IsAccepted = true;
            
            service.Save();
            //return RedirectToAction("Friends", new {id =  });
        }

        [HttpPost]
        public void IgnoreFriendshipRequest(int? id)
        {
            FriendRequest friendRequest = service.GetRequestById(id);
            
            service.CancelRequest(friendRequest);            
            service.Save();
            //return RedirectToAction("Friends", new {id =  });
        }

        [HttpPost]
        public void RemoveFriendship(int? id)
        {
            Profile profile = service.GetProfileByUserId(service.GetCurrentUserId(User));
            FriendRequest friendRequest = service.FindFriend(profile.ProfileId, id);
            
            //service.CancelFriendship(profile.ProfileId, id);
            
            service.CancelRequest(friendRequest);
            service.Save();
            //return RedirectToAction("Friends", new { id =  });
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
