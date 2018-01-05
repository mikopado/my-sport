using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SportsBarApp.Models
{
    public class FriendRequest
    {
        public int Id { get; set; }
        
        public bool IsAccepted { get; set; }
        [ForeignKey("Profile")]
        public int? ProfileId { get; set; }
        [ForeignKey("Friend")]
        public int? FriendId { get; set; }

        public virtual Profile Profile { get; set; }
        public virtual Profile Friend { get; set; }
    }
}