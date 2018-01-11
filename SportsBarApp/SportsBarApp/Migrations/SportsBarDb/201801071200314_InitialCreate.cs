namespace SportsBarApp.Migrations.SportsBarDb
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Comments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Text = c.String(),
                        Timestamp = c.DateTime(nullable: false),
                        ProfileId = c.Int(),
                        PostId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Posts", t => t.PostId)
                .ForeignKey("dbo.Profiles", t => t.ProfileId)
                .Index(t => t.ProfileId)
                .Index(t => t.PostId);
            
            CreateTable(
                "dbo.Posts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Message = c.String(),
                        ProfileId = c.Int(),
                        Timestamp = c.DateTime(nullable: false),
                        MetaInfo_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Profiles", t => t.ProfileId)
                .ForeignKey("dbo.MetaInfoes", t => t.MetaInfo_Id)
                .Index(t => t.ProfileId)
                .Index(t => t.MetaInfo_Id);
            
            CreateTable(
                "dbo.Profiles",
                c => new
                    {
                        ProfileId = c.Int(nullable: false, identity: true),
                        FirstName = c.String(nullable: false),
                        LastName = c.String(nullable: false),
                        DateOfBirth = c.DateTime(nullable: false),
                        City = c.String(),
                        Country = c.String(),
                        FavouriteTeams = c.String(),
                        FavouriteSports = c.String(),
                        GlobalId = c.Guid(nullable: false),
                        ProfilePic_Id = c.Int(),
                    })
                .PrimaryKey(t => t.ProfileId)
                .ForeignKey("dbo.Images", t => t.ProfilePic_Id)
                .Index(t => t.ProfilePic_Id);
            
            CreateTable(
                "dbo.Images",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Content = c.Binary(),
                        FileName = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.FriendRequests",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        IsAccepted = c.Boolean(nullable: false),
                        ProfileId = c.Int(),
                        FriendId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Profiles", t => t.FriendId)
                .ForeignKey("dbo.Profiles", t => t.ProfileId)
                .Index(t => t.ProfileId)
                .Index(t => t.FriendId);
            
            CreateTable(
                "dbo.MetaInfoes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Hashtag = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Posts", "MetaInfo_Id", "dbo.MetaInfoes");
            DropForeignKey("dbo.FriendRequests", "ProfileId", "dbo.Profiles");
            DropForeignKey("dbo.FriendRequests", "FriendId", "dbo.Profiles");
            DropForeignKey("dbo.Comments", "ProfileId", "dbo.Profiles");
            DropForeignKey("dbo.Posts", "ProfileId", "dbo.Profiles");
            DropForeignKey("dbo.Profiles", "ProfilePic_Id", "dbo.Images");
            DropForeignKey("dbo.Comments", "PostId", "dbo.Posts");
            DropIndex("dbo.FriendRequests", new[] { "FriendId" });
            DropIndex("dbo.FriendRequests", new[] { "ProfileId" });
            DropIndex("dbo.Profiles", new[] { "ProfilePic_Id" });
            DropIndex("dbo.Posts", new[] { "MetaInfo_Id" });
            DropIndex("dbo.Posts", new[] { "ProfileId" });
            DropIndex("dbo.Comments", new[] { "PostId" });
            DropIndex("dbo.Comments", new[] { "ProfileId" });
            DropTable("dbo.MetaInfoes");
            DropTable("dbo.FriendRequests");
            DropTable("dbo.Images");
            DropTable("dbo.Profiles");
            DropTable("dbo.Posts");
            DropTable("dbo.Comments");
        }
    }
}
