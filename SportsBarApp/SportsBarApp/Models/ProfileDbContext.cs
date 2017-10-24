using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SportsBarApp.Models
{
    public class ProfileDbContext : DbContext
    {
        // You can add custom code to this file. Changes will not be overwritten.
        // 
        // If you want Entity Framework to drop and regenerate your database
        // automatically whenever you change your model schema, please use data migrations.
        // For more information refer to the documentation:
        // http://msdn.microsoft.com/en-us/data/jj591621.aspx
    
        public ProfileDbContext() : base("name=ProfileDbContext")
        {
            //To allow changes in database schema when domain classes are modified. Do not use in production
            Database.SetInitializer<ProfileDbContext>(new DropCreateDatabaseIfModelChanges<ProfileDbContext>());
        }

        public System.Data.Entity.DbSet<SportsBarApp.Models.Profile> Profiles { get; set; }
        //public System.Data.Entity.DbSet<SportsBarApp.Models.Team> Teams { get; set; }
        //public System.Data.Entity.DbSet<SportsBarApp.Models.Sport> Sports { get; set; }
        //public System.Data.Entity.DbSet<SportsBarApp.Models.Image> Images { get; set; }

    }
}
