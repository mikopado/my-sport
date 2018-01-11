using SportsBarApp.Models.DAL;
using System;
using SportsBarApp.Models;

namespace SportsBarApp.Tests.MockClasses
{
    public class MockUnit : IUnitOfWork
    {
        public MockDb Db { get; set; }
        public IRepository<Profile> Profiles { get; set; }
        public IRepository<FriendRequest> FriendRequests { get; set; }
        public IRepository<Post> Posts { get; set; }
        public IRepository<Comment> Comments { get; set; }
        public IRepository<MetaInfo> MetaData { get; set; }
        public IRepository<Image> Images { get; set; }

        public MockUnit(MockDb db)
        {
            this.Db = db;
            Profiles = db.Profiles;
            FriendRequests = db.FriendRequests;
            Posts = db.Posts;
            Comments = db.Comments;
            Images = db.Images;
            MetaData = db.MetaData;
        }

        public void Update(Profile element)
        {
            throw new NotImplementedException();
        }

        public void Delete(Profile element)
        {
            Profiles.Remove(element);
        }

        public void Commit()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
