using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SportsBarApp.Models
{
    public class Post
    {
        public int Id { get; set; }

        public string Message { get; set; }
        public int? ProfileId { get; set; }
        public DateTime Timestamp { get; set; }

        public virtual Profile Profile { get; set; }
        public virtual List<Comment> Comments { get; set; }
    }
}