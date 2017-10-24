using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SportsBarApp.Models
{
    public class Profile
    {
        [Key]
        public int ProfileId { get; set; }
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        
        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }
        public string City { get; set; }
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        public string NickName { get; set; }
        public string Country { get; set; }
        [Display(Name = "Favourite Teams")]
        public string FavouriteTeams { get; set; }
        [Display(Name = "Favourite Sports")]
        public string FavouriteSports { get; set; }
        //[Display(Name = "Profile Image")]
        //public Image ProfilePic { get; set; }
       
        public Guid GlobalId { get; set; }


    }
}