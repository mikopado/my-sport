﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SportsBarApp.Models.DAL
{
    public class UnitOfWork : IUnitOfWork
    {
        public SportsBarDbContext SportsBarDb { get; set; }
        public IRepository<Profile> Profiles { get; set; }
        public IRepository<Image> Images { get; set; }
        public IRepository<Comment> Comments { get; set; }
        public IRepository<Post> Posts { get; set; }
        public IRepository<FriendRequest> FriendRequests { get; set; }
        public IRepository<MetaInfo> MetaData { get; set; }
        
        public UnitOfWork(SportsBarDbContext db)
        {
            SportsBarDb = db;
            Profiles = new AppRepository<Profile>(SportsBarDb);
            Images = new AppRepository<Image>(SportsBarDb);
            Comments = new AppRepository<Comment>(SportsBarDb);
            Posts = new AppRepository<Post>(SportsBarDb);
            FriendRequests = new AppRepository<FriendRequest>(SportsBarDb);
            MetaData = new AppRepository<MetaInfo>(SportsBarDb);
        }
        public void Dispose()
        {
            SportsBarDb.Dispose();

        }

        public void Commit()
        {
            SportsBarDb.SaveChanges();
        }

        public void Update(Profile element)
        {
            SportsBarDb.Entry(element).State = EntityState.Modified;
        }

        public void Delete(Profile element)
        {
            SportsBarDb.Entry(element).State = EntityState.Deleted;
            Profiles.Remove(element);
        }
    }
}