using SportsBarApp.Cookies;
using SportsBarApp.Models;
using SportsBarApp.Models.DAL;
using SportsBarApp.Models.ViewModels;
using SportsBarApp.ServiceLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace SportsBarApp.Controllers
{
    [Authorize]
    public class FriendsController : Controller
    {
        private AppService appService = new AppService(new UnitOfWork(new SportsBarDbContext()));
        
        /*FRIENDS AND FRIEND REQUESTS*/
        [Route("friends/{id}")]
        public ActionResult Friends(int? id)
        {
            //In the my profile page, shows activity for the current user and also the text area t upload new post
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Profile profile = appService.GetProfile(id);
            if (profile == null)
            {
                return HttpNotFound();
            }

            //Instantiate ProfileViewModel with some properties
            ProfileViewModel wvm = new ProfileViewModel
            {
                CurrentProfile = profile,
                IsThisUser = appService.EnsureIsUserProfile(profile, User),
                Friends = appService.GetFriends(profile.ProfileId),
                Photo = profile.ProfilePic.FileName,
                
            };
            //If the examined profile doesn't belong to current user then change some properties to display
            if (!wvm.IsThisUser)
            {

                var user = appService.GetProfile(appService.GetCurrentUserId(User));

                wvm.FriendStatus = appService.GetFriendStatus(user, profile).Item1;
                wvm.ButtonStatus = appService.GetFriendStatus(user, profile).Item2;
                wvm.User = user;
                wvm.Photo = user.ProfilePic.FileName;
                wvm.PendingRequests = appService.GetPendingRequests(user.ProfileId);
               
            }
            else
            {
                wvm.PendingRequests = appService.GetPendingRequests(profile.ProfileId);

            }

            return View(wvm);
        }

        [HttpPost]
        [Route("Friends/AskFriendship")]
        [ValidateAntiForgeryToken]
        public ActionResult AskFriendship([Bind(Include = "FriendId, ProfileId")] FriendRequest friendRequest)
        {
            //Send friendship request to a specific user after clicking add fried button

            friendRequest.IsAccepted = false;

            if (ModelState.IsValid)
            {
                appService.Add(friendRequest);
                appService.Save();

                //Send the request to the specific client using primarily web socket
                appService.NotifyUser(appService.GetIdentityFromUserId(friendRequest.FriendId), friendRequest.ProfileId, friendRequest.Id);
            }
            

            return RedirectToAction("MyProfile", "Profile",  new { id = friendRequest.FriendId });

        }

        [HttpPost]
        [Route("Friends/AcceptFriendship/{id}")]
        public int AcceptFriendship(int? id)
        {
            Profile profile = appService.GetProfile(appService.GetCurrentUserId(User));
            var count = int.Parse(AppCookie.GetCookie(this, "pendingRequests" + profile.ProfileId)[0]);
            if(id != null)
            {
                FriendRequest friendRequest = appService.GetRequestById(id);
                friendRequest.IsAccepted = true;
                appService.Save();
                //Save the new pending request count as cookie
                --count;
                AppCookie.SaveCookie(this, "pendingRequests" + profile.ProfileId, count.ToString());
            }            

            return count;
        }

        [HttpPost]
        [Route("Friends/IgnoreFriendshipRequest/{id}")]
        public int IgnoreFriendshipRequest(int? id)
        {
            Profile profile = appService.GetProfile(appService.GetCurrentUserId(User));
            var count = int.Parse(AppCookie.GetCookie(this, "pendingRequests" + profile.ProfileId)[0]);
            if (id != null)
            {
                FriendRequest friendRequest = appService.GetRequestById(id);
                appService.CancelRequest(friendRequest);
                appService.Save();

                //Edit the pending request cookie with new value
                --count;
                AppCookie.SaveCookie(this, "pendingRequests" + profile.ProfileId, count.ToString());                
            }
            return count;
        }

        [HttpGet]
        [Route("Friends/IncreasePendingCookie")]
        public int IncreasePendingCookie()
        {
            Profile profile = appService.GetProfile(appService.GetCurrentUserId(User));
            //Increase the pending request cookie when a friendship request has been sent
            //var count = int.Parse(AppCookie.GetCookie(this, "pendingRequests" + profile.ProfileId)[0]) + 1;
            var count = appService.GetPendingRequests(profile.ProfileId).Count;
            AppCookie.SaveCookie(this, "pendingRequests" + profile.ProfileId, count.ToString());
            return count;
        }

        [HttpPost]
        [Route("Friends/RemoveFriendship/{id}")]
        public void RemoveFriendship(int? id)
        {
            if(id != null)
            {
                Profile profile = appService.GetProfile(appService.GetCurrentUserId(User));
                FriendRequest friendRequest = appService.FindFriend(profile.ProfileId, id);
                appService.CancelRequest(friendRequest);
                appService.Save();
            }
           

        }
    }
}