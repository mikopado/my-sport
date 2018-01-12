using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SportsBarApp.Models
{
    public class MetaInfo
    {
        public int Id { get; set; }
        public string Hashtag { get; set; }

        public virtual ICollection<Post> Posts { get; set; }
    }
}