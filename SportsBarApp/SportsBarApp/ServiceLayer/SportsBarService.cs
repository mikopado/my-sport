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
using System.IO;

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
            return unit.Profiles.GetElements(x => x.FirstName.StartsWith(term) || x.LastName.StartsWith(term)).Take(8).ToList();
        }

        public FriendRequest FindFriend(int? userId, int? friendId)
        {
            return unit.FriendRequests.GetElement(x => (x.ProfileId == userId && x.FriendId == friendId) || (x.ProfileId == friendId && x.FriendId == userId));

        }

        public List<Profile> GetFriends(int? userID)
        {
            List<Profile> profiles = new List<Profile>();
            var friendships = unit.FriendRequests.GetElements(x => x.IsAccepted && (x.ProfileId == userID || x.FriendId == userID));
            foreach (var item in friendships)
            {
                Profile p = new Profile();

                if (item.ProfileId != userID)
                {
                    p = GetProfileFromId(item.ProfileId);
                }
                else if (item.FriendId != userID)
                {
                    p = GetProfileFromId(item.FriendId);
                }

                profiles.Add(p);
            }
            return profiles;
        }

        public List<FriendRequest> GetPendingRequests(int? id)
        {
            
           return unit.FriendRequests.GetElements(x => !x.IsAccepted && x.FriendId == id).ToList();                
            
        }
       
        public void Add(FriendRequest request)
        {
            unit.FriendRequests.Add(request);
        }

        public FriendRequest GetRequestById(int? id)
        {
            return unit.FriendRequests.GetElement(x => x.Id == id);
        }

        public void CancelRequest(FriendRequest friendRequest)
        {          
            unit.FriendRequests.Remove(friendRequest);
        }


        public Tuple<string, string> GetFriendStatus(Profile user, Profile profile)
        {
            var friendRequest = FindFriend(user.ProfileId, profile.ProfileId);

            if (friendRequest != null)
            {
                return new Tuple<string, string>(friendRequest.IsAccepted ? "Friend" : "Sent Request", "disabled");
               
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

        public void NotifyUser(string friendId, int? userId, int? friendRequestId)
        {
            Profile prof = GetProfileFromId(userId);
            string userName = prof.FirstName + " " + prof.LastName;
            string photoFileName = prof.ProfilePic.FileName;
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<FriendRequestHub>();            
           
            hubContext.Clients.User(friendId).sendFriendRequest(userName, photoFileName, friendRequestId);
        }

       
        public void StoreMetaInfo(Post post)
        {
            List<string> hashtags = SplitStringInHashtags(post.Message);
            foreach (string item in hashtags)
            {
                var existingHashtags = unit.MetaData.GetElement(x => x.Hashtag.Equals(item));
                if(existingHashtags != null)
                {
                    existingHashtags.Posts.Add(post);
                }
                else
                {
                    unit.MetaData.Add(new MetaInfo {Hashtag = item, Posts = new List<Post>() { post } });
                }

            }
        }

        private IEnumerable<Post> GetPostsByHashtags(string hashtag)
        {
            return unit.MetaData.GetElements(x => x.Hashtag.Equals(hashtag)).SelectMany(x => x.Posts);
        }

        public IEnumerable<Post> GetPostsByHashtags(string[] hashtags)
        {
            if (hashtags.Length > 1)
            {
                var posts = new List<Post>();
                foreach (var item in hashtags)
                {
                    posts.AddRange(GetPostsByHashtags(item).Where(x => posts.All(y => y.Id != x.Id)));
                }
                return posts;
            }
            else if(hashtags.Length == 1)
            {
                return GetPostsByHashtags(hashtags[0]);
            }
            else
            {
                return unit.MetaData.GetElements(x => x.Id != - 1).SelectMany(x => x.Posts);
            }
        }

        private List<string> SplitStringInHashtags(string str)
        {
            List<string> hashtags = new List<string>();
            string[] arr = str.Substring(str.IndexOf('#')).Split('#');
            foreach(string s in arr)
            {
                if (!string.IsNullOrWhiteSpace(s) && !s.Contains(" "))
                {
                    hashtags.Add(s);
                }
               
            }
            return hashtags;
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