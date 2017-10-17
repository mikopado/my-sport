using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SportsBarApp.Models
{
    public class TeamViewModel
    {
        public string Team { get; set; }
        public IList<Article> Articles { get; set; }

    }
}