using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SportsBarApp.Models
{
    public class ProfileViewModel
    {
        
        public int ProfileId { get; set; }
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime Birthday { get; set; }
        public string City { get; set; }
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }        
        public string NickName { get; set; }
        public string Country { get; set; }
        [Display(Name = "Favourite Team")]
        public string FavouriteTeams { get; set; }
        [Display(Name = "Favourite Sport")]
        public string FavouriteSports { get; set; }

  
    }
}