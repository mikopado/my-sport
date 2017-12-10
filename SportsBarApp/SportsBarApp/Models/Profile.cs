using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SportsBarApp.Models
{
    public class Profile
    {
        
        public int ProfileId { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        
        
        [Display(Name = "Date of Birth")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DateOfBirth { get; set; }

        public string City { get; set; }        
        
        public string Country { get; set; }

        [Display(Name = "Favourite Teams")]
        public string FavouriteTeams { get; set; }

        [Display(Name = "Favourite Sports")]
        public string FavouriteSports { get; set; }
        
        public Guid GlobalId { get; set; }

        public Image ProfilePic { get; set; }


    }
}