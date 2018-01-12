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
 
    public class ActivityController : Controller
    {
        private AppService appService = new AppService(new UnitOfWork(new SportsBarDbContext()));
        
        [Route("activity/{id}")]
        public ActionResult Activity(int? id)
        {
            //In the my profile page, shows activity for the current user and also the text area to upload new post
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Profile profile = appService.GetProfile(id);
            if (profile == null)
            {
                return HttpNotFound();
            }

            //Get a collections of post by user id. Shows only the posts written by the current user
            IEnumerable<Post> posts = appService.GetPostsByUser(profile.ProfileId).OrderByDescending(p => p.Timestamp);

            //Instantiate a ProfileViewModel with some properties
            ProfileViewModel wvm = new ProfileViewModel()
            {
                CurrentProfile = profile,
                Posts = posts,
                CurrentView = "activity",
                IsThisUser = appService.EnsureIsUserProfile(profile, User),
                Photo = profile.ProfilePic.FileName
            };

            //If the profile examined is not the current user then change some properties
            if (!wvm.IsThisUser)
            {
                var user = appService.GetProfile(appService.GetCurrentUserId(User));

                wvm.FriendStatus = appService.GetFriendStatus(user, profile).Item1;
                wvm.ButtonStatus = appService.GetFriendStatus(user, profile).Item2;
                wvm.User = user;
                wvm.Photo = user.ProfilePic.FileName;
            }

            return View(wvm);
        }

        /*POSTS*/
        [HttpPost]
        [Route("Activity/UploadPost")] 
        [ValidateAntiForgeryToken]
        public ActionResult UploadPost([Bind(Include = "Id, Message, Timestamp, ProfileId")]Post post, string page)
        {

            post.ProfileId = appService.GetProfile(appService.GetCurrentUserId(User)).ProfileId;
            post.Timestamp = DateTime.Now;

            if (ModelState.IsValid && !string.IsNullOrWhiteSpace(post.Message))
            {
                appService.Add(post);
                //Save also hashtags if any
                appService.StoreMetaInfo(post);
                appService.Save();
            }

            //If the post is uploaded into activity, stay in the activity page
            if (page.Equals("activity"))
            {
                return RedirectToAction("Activity", new { id = post.ProfileId });
            }
            return RedirectToAction("NewsFeed", "Wall");
        }

        [HttpPost]
        [Route("Activity/UploadComment")]
        [ValidateAntiForgeryToken]
        public ActionResult UploadComment([Bind(Include = "Id, Text, Timestamp, ProfileId, PostId")]Comment comment, string page)
        {

            comment.ProfileId = appService.GetProfile(appService.GetCurrentUserId(User)).ProfileId;
            comment.Timestamp = DateTime.Now;

            if (ModelState.IsValid && !string.IsNullOrWhiteSpace(comment.Text))
            {
                appService.Add(comment);
                appService.Save();
            }
            if (page.Equals("activity"))
            {
                return RedirectToAction("Activity", new { id = comment.ProfileId });
            }
            return RedirectToAction("NewsFeed", "Wall");
        }

        

    }
}