using SportsBarApp.Cookies;
using SportsBarApp.Models;
using SportsBarApp.Models.DAL;
using SportsBarApp.Models.ViewModels;
using SportsBarApp.ServiceLayer;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace SportsBarApp.Controllers
{
    [Authorize]
    public class WallController : Controller
    {
        private AppService appService = new AppService(new UnitOfWork(new SportsBarDbContext()));

        // GET: Profile    
        [Route("newsfeed")]
        public ActionResult NewsFeed()
        {
            //The main page after the login or sign up
            // The page has profile personal details, wall with posts and comments and list of friends

            var userId = appService.GetCurrentUserId(User);
            Profile profile = appService.GetProfile(userId);

            //Retrieves cookies if any
            var queryCookie = AppCookie.GetCookie(this, "filterQuery");
            var filters = queryCookie != null ? queryCookie[0].Split() : new string[0];
            var pendings = AppCookie.GetCookie(this, "pendingRequests");

            //Get posts to display in the center page
            IEnumerable<Post> posts = appService.GetPostsByHashtags(filters, profile.ProfileId).OrderByDescending(p => p.Timestamp);
            
            ProfileViewModel wall = new ProfileViewModel()
            {
                CurrentProfile = profile,
                Posts = posts,
                PendingsCount = pendings != null ? int.Parse(pendings[0]) : appService.GetPendingRequests(profile.ProfileId).Count,
                IsThisUser = appService.EnsureIsUserProfile(profile, User),
                Friends = appService.GetFriends(profile.ProfileId),
                Photo = profile.ProfilePic.FileName

            };
            //Save cookies
            AppCookie.SaveCookie(this, "filterQuery", queryCookie);
            AppCookie.SaveCookie(this, "pendingRequests", wall.PendingsCount.ToString());

            return View(wall);
        }


        [Route("Wall/FilterPosts/{query?}")]
        public ActionResult FilterPosts(string query)
        {
            //Filter posts in the center page by hashtags
            Profile profile = appService.GetProfile(appService.GetCurrentUserId(User));
            var filters = !string.IsNullOrWhiteSpace(query) ? query.Split() : new string[0];

            IEnumerable<Post> posts = appService.GetPostsByHashtags(filters, profile.ProfileId).OrderByDescending(p => p.Timestamp);

            AppCookie.SaveCookie(this, "filterQuery", query);

            return PartialView("PostsComments", posts);
        }

        public ActionResult CheckProfileExist()
        {
            //Action to verify the user when brand logo is clicked to access the main page. If user exists and is not logout redirect to the profile main page(wall)
            // otherwise redirect to the create or landing page
            Profile profile = appService.GetProfile(appService.GetCurrentUserId(User));

            if (Request.IsAuthenticated)
            {
                if (profile == null)
                {
                    return RedirectToAction("Create");
                }
                return RedirectToAction("NewsFeed");
            }
            return RedirectToAction("Index", "Home");
        }

    }
}