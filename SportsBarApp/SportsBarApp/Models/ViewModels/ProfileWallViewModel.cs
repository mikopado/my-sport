using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SportsBarApp.Models.ViewModels
{
    public enum FriendStatus
    {
        AlreadyFriend,
        NoFriend,
        PendingRequest
    }
    
    public class ProfileWallViewModel
    {
        public Profile UserProfile { get; set; }
        public IEnumerable<Post> Posts { get; set; }
        public IEnumerable<Comment> Comments { get; set; }
        public int ProfileId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public Profile Friend { get; set; }
        public string Name { get; set; }
        public string Photo { get; set; }
        

        
    }
}