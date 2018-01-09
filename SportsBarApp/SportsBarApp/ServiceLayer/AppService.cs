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
    public class AppService
    {
        private readonly UnitOfWork unit;

        public AppService(UnitOfWork unit)
        {
            this.unit = unit;
            
        }

        /**********************************************
         *              CRUD FUNCTIONALITIES
         *******************************************/
        /// <summary>
        /// Add new profile to database
        /// </summary>
        /// <param name="profile">Profile object to add</param>
        public void Add(Profile profile)
        {
            unit.Profiles.Add(profile);
        }
        /// <summary>
        /// Modify an existing entity into database
        /// </summary>
        /// <param name="profile">Profile object to modify</param>
        public void Edit(Profile profile)
        {
            unit.SportsBarDb.Entry(profile).State = EntityState.Modified;
        }
        /// <summary>
        /// Remove entity from database
        /// </summary>
        /// <param name="profile">Profile object to be removed</param>
        public void Remove(Profile profile)
        {
            unit.SportsBarDb.Entry(profile).State = EntityState.Deleted;
            unit.Profiles.Remove(profile);
        }
        /// <summary>
        /// Add new image to database
        /// </summary>
        /// <param name="image">Image object to add</param>
        public void Add(Image image)
        {
            unit.Images.Add(image);
        }
        /// <summary>
        /// Add new post to database
        /// </summary>
        /// <param name="post">Post object to add</param>
        public void Add(Post post)
        {
            unit.Posts.Add(post);
        }
        /// <summary>
        /// Add new comment to database
        /// </summary>
        /// <param name="comment">Comment object to add</param>
        public void Add(Comment comment)
        {
            unit.Comments.Add(comment);
        }

        /****************************************
         *              METADATA ENTITY
         ***********************************/
        /// <summary>
        /// Add metadata related to a post to MetaInfo table 
        /// </summary>
        /// <param name="post">Post where metadata belong to</param>
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
        /// <summary>
        /// Divide the given string in a collection of hashtags. Removes the hash symbol 
        /// and keeps only the keywords
        /// </summary>
        /// <param name="str">Given string</param>
        /// <returns>A collection of keywords (i.e. hashtags)</returns>
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

        /*************************************************
         *              PROFILE METHODS
         ********************************************/
        //Get profile object from Guid from account 
        /// <summary>
        /// Get profile object from the given Guid object.
        /// </summary>
        /// <param name="id">Guid(globally unique identifier) object</param>
        /// <returns>Profile object</returns>
        public Profile GetProfile(Guid id)
        {
            return unit.Profiles.GetElement(x => x.GlobalId == id);
        }
        /// <summary>
        /// Get profile object from the given Id.
        /// </summary>
        /// <param name="id">Integer value representing the Id attribute into database</param>
        /// <returns>Profile object</returns>
        public Profile GetProfile(int? id)
        {
            return unit.Profiles.GetElement(x => x.ProfileId == id);
        }
        /// <summary>
        /// Get the Profile Id from Guid
        /// </summary>
        /// <param name="guid">Guid object</param>
        /// <returns>Integer value representing the Profile Id.</returns>
        public int GetProfileId(Guid guid)
        {
            return GetProfile(guid).ProfileId;
        }  
        /// <summary>
        /// Get the Guid of the current user
        /// </summary>
        /// <param name="user">Define the current User</param>
        /// <returns>Guid object (globally unique identifier)</returns>
        public Guid GetCurrentUserId(IPrincipal user)
        {
            return new Guid(user.Identity.GetUserId());
        }
        /// <summary>
        /// Get account username to allow friend request notification to a specific user in Signal R
        /// </summary>
        /// <param name="id">User id</param>
        /// <returns>A string representing Account's username</returns>
        public string GetIdentityFromUserId(int? id)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            Guid globalId = GetProfile(id).GlobalId;
            return db.Users.Where(x => x.Id == globalId.ToString()).Select(x => x.UserName).FirstOrDefault();

        }        
        /// <summary>
        /// Check if the profile belongs to the current user
        /// </summary>
        /// <param name="profile">Profile object currently visited</param>
        /// <param name="user">Current user</param>
        /// <returns>A boolean value. True if the profile is the current user, false otherwise.</returns>
        public bool EnsureIsUserProfile(Profile profile, IPrincipal user)
        {
            return profile.GlobalId == new Guid(user.Identity.GetUserId());
        }
        
        /// <summary>
        /// Search users in database. For autocomplete search box 
        /// </summary>
        /// <param name="term">the string to search</param>
        /// <returns>IEnumerable of profile which are found from searching</returns>
        public IEnumerable<Profile> SearchProfiles(string term)
        {
            return unit.Profiles.GetElements(x => x.FirstName.StartsWith(term) || x.LastName.StartsWith(term)).Take(8).ToList();
        }

        /// <summary>
        /// Get all friends for a specific user from FriendRequests table
        /// </summary>
        /// <param name="userID">Current profile Id</param>
        /// <returns>IEnumerable of Profile objects which represent user's friends.</returns>
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

        /***************************************************************
         *  HUB METHOD FOR WEB-SOCKET CONNECTION CLIENT-SERVER - SIGNALR 
         **********************************************************/
      
        /// <summary>
        /// This method is called when Add Friend button is clicked. Ask friendship action in Friends Controller handles the request
        /// Then NotifyUser sends the request back to a specific client which is the user who is going to receive the friendship request
        /// </summary>
         /// <param name="friendId">The user to be notified</param>
         /// <param name="userId">The user who sends the request</param>
         /// <param name="friendRequestId">FriendRequest object's Id.</param>
        public void NotifyUser(string friendId, int? userId, int? friendRequestId)
        {
            Profile prof = GetProfile(userId);
            string userName = prof.FirstName + " " + prof.LastName;
            string photoFileName = prof.ProfilePic.FileName;
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<FriendRequestHub>();

            hubContext.Clients.User(friendId).sendFriendRequest(userName, photoFileName, friendRequestId);
        }

        /**********************************************************
         * *                       POSTS
         * ********************************************************/
        /// <summary>
        /// Get all posts objects from database
        /// </summary>
        /// <returns>IEnumerable of Post objects</returns>
        private IEnumerable<Post> GetPosts()
        {
            return unit.Posts.GetElements(x => x.Id != 0);            
        }
        /// <summary>
        /// Retrieve a collection of only Post objects uploaded by current user and its Friends.
        /// </summary>
        /// <param name="userId">Profile object id</param>
        /// <returns>IEnumerable of Post objects</returns>
        public IEnumerable<Post> GetPostsFriends(int? userId)
        {
            var friends = GetFriends(userId).ToList();
            friends.Add(GetProfile(userId));
            return GetPosts().Where(y => friends.Contains(y.Profile));
        }
        /// <summary>
        /// Retrieve all posts written by the given user.
        /// </summary>
        /// <param name="id">User id. Profile object</param>
        /// <returns>IEnumerable of Post objects</returns>
        public IEnumerable<Post> GetPostsByUser(int id)
        {
            IEnumerable<Post> posts = unit.Posts.GetElements(x => x.ProfileId == id);
            return posts;
        }
        /// <summary>
        /// Retrieve a collection of posts filter by hashtag
        /// </summary>
        /// <param name="hashtag">a string which represents the filter keyword</param>
        /// <param name="userId">Id of the current user</param>
        /// <returns>iEnumerable of Post objects</returns>
        private IEnumerable<Post> GetPostsByHashtags(string hashtag, int? userId)
        {
            var hashPosts = unit.MetaData.GetElements(x => x.Hashtag.Equals(hashtag)).SelectMany(x => x.Posts);
            return hashPosts.Intersect(GetPostsFriends(userId));
        }
        /// <summary>
        /// Retrieve all the posts filter by given keywords (in an array of strings)
        /// </summary>
        /// <param name="hashtags">array of strings which represent the keywords to use for filtering</param>
        /// <param name="userID">Id of the current user</param>
        /// <returns>a collection of Post objects</returns>
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

        /// <summary>
        /// Add new friendRequest to database
        /// </summary>
        /// <param name="request">FriendRequest object to add</param>
        public void Add(FriendRequest request)
        {
            unit.FriendRequests.Add(request);
        }
        /***********************************************************
         *                      FRIENDS AND FRIENDS REQUESTS
         ***********************************************************/
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId">current user id</param>
        /// <param name="friendId">friend id</param>
        /// <returns>A FriendRequest object</returns>
        public FriendRequest FindFriend(int? userId, int? friendId)
        {
            return unit.FriendRequests.GetElement(x => (x.ProfileId == userId && x.FriendId == friendId) || (x.ProfileId == friendId && x.FriendId == userId));
        }
        /// <summary>
        /// Get all the pending friendship requests of the given user
        /// </summary>
        /// <param name="id">user's id</param>
        /// <returns>Collection of FriendRequest objects</returns>
        public List<FriendRequest> GetPendingRequests(int? id)
        {
            return unit.FriendRequests.GetElements(x => !x.IsAccepted && x.FriendId == id).ToList();
        }
        /// <summary>
        /// Get Friendship Requests by FriendRequest's Id
        /// </summary>
        /// <param name="id">FriendRequest's Id</param>
        /// <returns>A FriendRequest object</returns>
        public FriendRequest GetRequestById(int? id)
        {
            return unit.FriendRequests.GetElement(x => x.Id == id);
        }
        /// <summary>
        /// Remove either friendship or friendship request from database
        /// </summary>
        /// <param name="friendRequest">FriendRequest object</param>
        public void CancelRequest(FriendRequest friendRequest)
        {
            unit.FriendRequests.Remove(friendRequest);
        }
        /// <summary>
        /// Get the friendship status between two users. Friend, No Friend or the request has been sent
        /// </summary>
        /// <param name="user">Current user</param>
        /// <param name="profile">A user who is been visited</param>
        /// <returns>A tuple of strings. First item is the friend status and second one a button status</returns>
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