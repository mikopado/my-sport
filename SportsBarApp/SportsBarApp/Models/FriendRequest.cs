using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SportsBarApp.Models
{
    public class FriendRequest
    {
        public int Id { get; set; }
        
        public bool IsAccepted { get; set; }

        public int? ProfileId { get; set; }
        public int? FriendId { get; set; }

        public virtual Profile Profile { get; set; }
        public virtual Profile Friend { get; set; }
    }
}