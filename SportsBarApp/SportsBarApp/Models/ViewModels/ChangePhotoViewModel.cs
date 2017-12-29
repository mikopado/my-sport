using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SportsBarApp.Models.ViewModels
{
    public class ChangePhotoViewModel
    {
        public Image Photo { get; set; }
        public int ProfileId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}