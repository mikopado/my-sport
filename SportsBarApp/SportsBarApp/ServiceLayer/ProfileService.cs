using Microsoft.AspNet.Identity;
using SportsBarApp.Models;
using SportsBarApp.Models.DAL;
using SportsBarApp.Models.ViewModels;
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
        private IRepository<Post> postRepo;
        private IRepository<Comment> commentRepo;
        private IRepository<ProfileWallViewModel> wallRepo;

        public ProfileService(IRepository<Profile> repo)
        {
            this.repo = repo;
            
        }
        public ProfileService(IRepository<Image> image)
        {
            this.imageRepo = image;
        }
        public ProfileService(IRepository<Comment> comment)
        {
            this.commentRepo = comment;
        }

        public ProfileService(IRepository<Post> post)
        {
            this.postRepo = post;
        }
        public ProfileService(IRepository<ProfileWallViewModel> wall)
        {
            this.wallRepo = wall;
        }

        public Profile GetProfileByUserId(Guid id)
        {
            return repo.GetElement(x => x.GlobalId == id);
        }


        public int GetProfileId(Guid guid)
        {
            return GetProfileByUserId(guid).ProfileId;
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

        public Guid GetCurrentUserId(IPrincipal user)
        {
            return new Guid(user.Identity.GetUserId());
        }

        public bool EnsureIsUserProfile(Profile profile, IPrincipal user)
        {
            return profile.GlobalId == GetCurrentUserId(user);
        }

        public void Add(Image image)
        {
            //if(imageRepo.GetElement(x => x.Id == image.Id) != null)
            //{
            //    imageRepo.Update(image);
            //}
            //else
            //{
            imageRepo.Add(image);
            

        }
        public void Add(Post post)
        {
            postRepo.Add(post);
        }
        public void Add(Comment comment)
        {
            commentRepo.Add(comment);
        }
        public IEnumerable<Post> GetPosts()
        {
            IEnumerable<Post> posts = postRepo.GetElements(x => x.Id != 0);
            return posts;
        }



        public void DisposeContext()
        {
            repo.Dispose();
        }
    }
}