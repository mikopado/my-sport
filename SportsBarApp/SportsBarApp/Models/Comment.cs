using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SportsBarApp.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public DateTime Timestamp { get; set; }

        
        public int? ProfileId { get; set; }
        public int? PostId { get; set; }

        public virtual Profile Profile { get; set; }
        public virtual Post Post { get; set; }

    }
}