using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SportsBarApp.Models.ViewModels
{

    
    public class ProfileWallViewModel
    {
        public Profile Profile { get; set; }
        public IEnumerable<Post> Posts { get; set; }
        public IEnumerable<Comment> Comments { get; set; }
       
        public Profile User { get; set; }
        public string Name { get; set; }
        public string Photo { get; set; }

        public string FriendStatus { get; set; }
        public string ButtonStatus { get; set; }

        public bool IsThisUser { get; set; }

        public string CurrentView { get; set; }
        public FriendRequest Friendship { get; set; }



    }
}