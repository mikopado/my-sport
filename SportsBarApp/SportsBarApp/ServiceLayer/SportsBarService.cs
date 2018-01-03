using Microsoft.AspNet.Identity;
using SportsBarApp.Models;
using SportsBarApp.Models.DAL;
using SportsBarApp.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using SportsBarApp.Hubs;
using Microsoft.AspNet.SignalR;
using System.Web.Security;


namespace SportsBarApp.ServiceLayer
{
    public class SportsBarService
    {
        private readonly UnitOfWork unit;

        public SportsBarService(UnitOfWork unit)
        {
            this.unit = unit;
            
        }
        //Get profile object from Guid from account 
        public Profile GetProfileByUserId(Guid id)
        {
            return unit.Profiles.GetElement(x => x.GlobalId == id);
        }


        public int GetProfileId(Guid guid)
        {
            return GetProfileByUserId(guid).ProfileId;
        }

        public Profile Find(int? id)
        {
            return unit.Profiles.GetElement(x => x.ProfileId == id);
        }

        public void Add(Profile profile)
        {
            unit.Profiles.Add(profile);
        }

        public void Edit(Profile profile)
        {
            unit.SportsBarDb.Entry(profile).State = EntityState.Modified;            
        }

        public void Remove(Profile profile)
        {
            unit.SportsBarDb.Entry(profile).State = EntityState.Deleted;
            unit.Profiles.Remove(profile);
        }

        public Guid GetCurrentUserId(IPrincipal user)
        {
            return new Guid(user.Identity.GetUserId());
        }

        public Profile GetProfileFromId(int? id)
        {
           return unit.Profiles.GetElement(x => x.ProfileId == id);
        }

        public bool EnsureIsUserProfile(Profile profile, IPrincipal user)
        {            
            return profile.GlobalId == new Guid(user.Identity.GetUserId());
        }

        public void Add(Image image)
        {            
            unit.Images.Add(image);   
        }
        public void Add(Post post)
        {
            unit.Posts.Add(post);
        }
        public void Add(Comment comment)
        {
            unit.Comments.Add(comment);
        }
        public IEnumerable<Post> GetPosts()
        {
            IEnumerable<Post> posts = unit.Posts.GetElements(x => x.Id != 0);
            return posts;
        }
        public IEnumerable<Post> GetPostsByUser(int id)
        {
            IEnumerable<Post> posts = unit.Posts.GetElements(x => x.ProfileId == id);
            return posts;
        }

        public IEnumerable<Profile> SearchProfiles(string term)
        {            
            return unit.Profiles.GetElements(x => x.FirstName.StartsWith(term) || x.LastName.StartsWith(term)).ToList();
        }

        public FriendRequest FindFriend(int userId, int friendId)
        {
            return unit.FriendRequests.GetElement(x => x.ProfileId == userId && x.FriendId == friendId);

        }

        public IEnumerable<Profile> GetPendingRequests(int? id)
        {
            List<Profile> profiles = new List<Profile>();
            if (id != null)
            {
                var profilesId = unit.FriendRequests.GetElements(x => !x.IsAccepted && x.FriendId == id).Select(x => x.ProfileId);
                
                if (profilesId.Count() > 0)
                {
                    foreach (var item in profilesId)
                    {
                        profiles.Add(GetProfileFromId(item));
                    }

                }
            }
            
            return profiles;
        }

       
        public void Add(FriendRequest request)
        {
            unit.FriendRequests.Add(request);
        }

        public Tuple<string, string> GetFriendStatus(Profile user, Profile profile)
        {
            var friendRequest = FindFriend(user.ProfileId, profile.ProfileId);

            if (friendRequest != null)
            {
                return new Tuple<string, string>(friendRequest.IsAccepted ? "Friend" : "Pending Request", "disabled");
               
            }
            else
            {
                return new Tuple<string, string>("Add Friend", "");
                
            }
        }

        public string GetIdentityFromUserId(int? id)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            Guid globalId = GetProfileFromId(id).GlobalId;

            return db.Users.Where(x => x.Id == globalId.ToString()).Select(x => x.UserName).FirstOrDefault();
           
        }

        public void NotifyUser(string userId, string message)
        {
           
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<FriendRequestHub>();            
           
            hubContext.Clients.User(userId).sendFriendRequest(message);
        }

        public void Save()
        {
            unit.Commit();
        }

        public void DisposeContext()
        {
            unit.Dispose();
        }
    }
}