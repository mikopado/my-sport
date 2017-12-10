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
        private IRepository<Image> imageRepo;

        public ProfileService(IRepository<Profile> repo)
        {
            this.repo = repo;
            
        }
        public ProfileService(IRepository<Image> image)
        {
            this.imageRepo = image;
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

        public Guid GetCurrentProfileId(IPrincipal user)
        {
            return new Guid(user.Identity.GetUserId());
        }

        public bool EnsureIsUserProfile(Profile profile, IPrincipal user)
        {
            return profile.GlobalId == GetCurrentProfileId(user);
        }

        public void AddOrUpdate(Image image)
        {
            if(imageRepo.GetElement(x => x.ImageId == image.ImageId) != null)
            {
                imageRepo.Update(image);
            }
            else
            {
                imageRepo.Add(image);
            }

        }

        

        public void DisposeContext()
        {
            repo.Dispose();
        }
    }
}