using Microsoft.AspNet.Identity;
using SportsBarApp.Models;
using SportsBarApp.Models.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace SportsBarApp.ServiceLayer
{
    public class ProfileService
    {
        private IRepository<Profile> repo;

        public ProfileService(IRepository<Profile> repo)
        {
            this.repo = repo;
        }

        public Profile GetProfileByUserId(Guid id)
        {
            return repo.GetElement(x => x.GlobalId == id);
        }

        public Profile Find(int? id)
        {
            return repo.GetElement(x => x.ProfileId == id);
        }

        public void Add(Profile profile)
        {
            repo.Add(profile);
        }

        public void Edit(Profile profile)
        {
            repo.Update(profile);
        }

        public void Remove(Profile profile)
        {
            repo.Remove(profile);
        }

        public Guid GetCurrentProfileId(IPrincipal User)
        {
            return new Guid(User.Identity.GetUserId());
        }

        public string GetCurrentUser(IPrincipal User)
        {
            return User.Identity.GetUserName();
        }

        public void DisposeContext()
        {
            repo.Dispose();
        }
    }
}