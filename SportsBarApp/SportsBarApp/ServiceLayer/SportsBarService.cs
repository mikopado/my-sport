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

        /*GET PROFILE METHODS*/
        //Get profile object from Guid from account 
        public Profile GetProfile(Guid id)
        {
            return unit.Profiles.GetElement(x => x.GlobalId == id);
        }
        public Profile GetProfile(int? id)
        {
            return unit.Profiles.GetElement(x => x.ProfileId == id);
        }
        public int GetProfileId(Guid guid)
        {
            return GetProfile(guid).ProfileId;
        }       
        public Guid GetCurrentUserId(IPrincipal user)
        {
            return new Guid(user.Identity.GetUserId());
        }     

        public bool EnsureIsUserProfile(Profile profile, IPrincipal user)
        {
            return profile.GlobalId == new Guid(user.Identity.GetUserId());
        }
        public IEnumerable<Profile> SearchProfiles(string term)
        {
            return unit.Profiles.GetElements(x => x.FirstName.StartsWith(term) || x.LastName.StartsWith(term)).Take(8).ToList();
        }
        
        public string GetIdentityFromUserId(int? id)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            Guid globalId = GetProfile(id).GlobalId;

            return db.Users.Where(x => x.Id == globalId.ToString()).Select(x => x.UserName).FirstOrDefault();

        }

        /*CRUD FUNCTIONALITIES*/
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
        public void Add(FriendRequest request)
        {
            unit.FriendRequests.Add(request);
        }
        public void StoreMetaInfo(Post post)
        {
            List<string> hashtags = SplitStringInHashtags(post.Message);
            foreach (string item in hashtags)
            {
                var existingHashtags = unit.MetaData.GetElement(x => x.Hashtag.Equals(item));
                if (existingHashtags != null)
                {
                    existingHashtags.Posts.Add(post);
                }
                else
                {
                    unit.MetaData.Add(new MetaInfo { Hashtag = item, Posts = new List<Post>() { post } });
                }

            }
        }
        /*POSTS*/
        private IEnumerable<Post> GetPosts()
        {
            IEnumerable<Post> posts = unit.Posts.GetElements(x => x.Id != 0);
            return posts;
        }
        public IEnumerable<Post> GetPostsFriends(int? userId)
        {
            var friends = GetFriends(userId).ToList();
            friends.Add(GetProfile(userId));            
            return GetPosts().Where(y => friends.Contains(y.Profile)); 
        }
        public IEnumerable<Post> GetPostsByUser(int id)
        {
            IEnumerable<Post> posts = unit.Posts.GetElements(x => x.ProfileId == id);
            return posts;
        }
        private IEnumerable<Post> GetPostsByHashtags(string hashtag, int? userId)
        {
            var hashPosts = unit.MetaData.GetElements(x => x.Hashtag.Equals(hashtag)).SelectMany(x => x.Posts);
            return hashPosts.Intersect(GetPostsFriends(userId));
        }

        public List<Post> GetPostsByHashtags(string[] hashtags, int? userID)
        {
            var posts = new List<Post>();
            if (unit.MetaData != null)
            {
                if (hashtags.Length > 1)
                {

                    foreach (var item in hashtags)
                    {
                        posts.AddRange(GetPostsByHashtags(item, userID).Where(x => posts.All(y => y.Id != x.Id)));
                    }

                }
                else if (hashtags.Length == 1)
                {
                    posts = GetPostsByHashtags(hashtags[0], userID).ToList();
                }
                else
                {
                    posts = GetPostsFriends(userID).ToList();
                }
            }
            return posts;
        }


        private List<string> SplitStringInHashtags(string str)
        {
            List<string> hashtags = new List<string>();
            if (str.Contains("#"))
            {
                string[] arr = str.Substring(str.IndexOf('#')).Split('#');
                foreach (string s in arr)
                {
                    if (!string.IsNullOrWhiteSpace(s) && !s.Contains(" "))
                    {
                        hashtags.Add(s);
                    }

                }
            }

            return hashtags;
        }
        /*FRIENDS AND FRIENDS REQUESTS*/
        public FriendRequest FindFriend(int? userId, int? friendId)
        {
            return unit.FriendRequests.GetElement(x => (x.ProfileId == userId && x.FriendId == friendId) || (x.ProfileId == friendId && x.FriendId == userId));
        }

        public IEnumerable<Profile> GetFriends(int? userID)
        {
            List<Profile> profiles = new List<Profile>();
            var friendships = unit.FriendRequests.GetElements(x => x.IsAccepted && (x.ProfileId == userID || x.FriendId == userID));
            foreach (var item in friendships)
            {
                Profile p = new Profile();

                if (item.ProfileId != userID)
                {
                    p = GetProfile(item.ProfileId);
                }
                else if (item.FriendId != userID)
                {
                    p = GetProfile(item.FriendId);
                }

                profiles.Add(p);
            }
            return profiles;
        }

        public List<FriendRequest> GetPendingRequests(int? id)
        {
            return unit.FriendRequests.GetElements(x => !x.IsAccepted && x.FriendId == id).ToList();                
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

        /*HUB METHOD FOR WEB-SOCKET CONNECTION CLIENT-SERVER - SIGNALR */
        public void NotifyUser(string friendId, int? userId, int? friendRequestId)
        {
            Profile prof = GetProfile(userId);
            string userName = prof.FirstName + " " + prof.LastName;
            string photoFileName = prof.ProfilePic.FileName;
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<FriendRequestHub>();            
           
            hubContext.Clients.User(friendId).sendFriendRequest(userName, photoFileName, friendRequestId);
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