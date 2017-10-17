using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace SportsBarApp.Models
{
    public class Article
    {
        public string Title { get; set; }
        

        public Article(string title)
        {
            Title = title;
        }

        public string CreateArticle()
        {

            return string.Format(@"{0} Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod
                            tempor incididunt ut labore et dolore magna aliqua. {0} Ut enim ad minim veniam,
                            quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo
                            consequat. {0} Duis aute irure dolor in reprehenderit in voluptate velit esse
                            cillum dolore eu fugiat nulla pariatur. {0} Excepteur sint occaecat cupidatat non
                            proident, sunt in culpa qui officia deserunt mollit anim id est laborum.", Title.ToUpper());
             
        }
    }
}