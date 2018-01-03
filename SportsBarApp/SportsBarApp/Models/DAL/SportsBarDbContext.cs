using SportsBarApp.Models.DAL;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SportsBarApp.Models
{
    public class SportsBarDbContext : DbContext
    {
        // You can add custom code to this file. Changes will not be overwritten.
        // 
        // If you want Entity Framework to drop and regenerate your database
        // automatically whenever you change your model schema, please use data migrations.
        // For more information refer to the documentation:
        // http://msdn.microsoft.com/en-us/data/jj591621.aspx
    
        public SportsBarDbContext() : base("name=SportsBarDbContext")
        {
            //To allow changes in database schema when domain classes are modified. Do not use in production
            Database.SetInitializer<SportsBarDbContext>(new DropCreateDatabaseIfModelChanges<SportsBarDbContext>());

        }

        public DbSet<Profile> Profiles { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<FriendRequest> FriendRequests { get; set; }

        
        
    }
}
