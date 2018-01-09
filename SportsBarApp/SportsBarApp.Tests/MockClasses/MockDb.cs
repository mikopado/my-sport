using SportsBarApp.Models;
using SportsBarApp.Models.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportsBarApp.Tests.MockClasses
{
    public class MockDb
    {
        public MockRepo<Profile> Profiles { get; set; }
        public MockRepo<FriendRequest> FriendRequests { get; set; }
        public MockRepo<MetaInfo> MetaData { get; set; }
        public MockRepo<Post> Posts { get; set; }
        public MockRepo<Image> Images { get; set; }
        public MockRepo<Comment> Comments { get; set; }

        public MockDb()
        {
            Profiles = new MockRepo<Profile>
            {
                Entities = new HashSet<Profile>
                {
                    new Profile{ProfileId = 1, FirstName = "Mark", LastName = "Lannon", DateOfBirth = new DateTime(1980, 12, 23), GlobalId = new Guid("c04d45ff-3bc4-42f7-b7ec-b556f7e20c14") },
                    new Profile{ProfileId = 2, FirstName = "John", LastName = "Bell", DateOfBirth = new DateTime(1990, 10, 13), GlobalId = new Guid("c14d45ff-3bc4-42f7-b5ec-b556f7e20c14")},
                    new Profile{ProfileId = 3, FirstName = "Carl", LastName = "Cox", DateOfBirth = new DateTime(1988, 2, 3), GlobalId = new Guid("a04d45ff-3bc7-42f7-b7ec-b556f7e20c14") },
                    new Profile{ProfileId = 4, FirstName = "Bill", LastName = "Kop", DateOfBirth = new DateTime(1960, 8, 2), GlobalId = new Guid("c04d45ff-3bc4-32f7-b7ec-c556f7e20c14") },

                }
            };

            FriendRequests = new MockRepo<FriendRequest>
            {
                Entities = new HashSet<FriendRequest>
                {
                    new FriendRequest{Id = 1, ProfileId = 1, FriendId = 2, IsAccepted = false },
                    new FriendRequest{Id = 2, ProfileId = 2, FriendId = 4, IsAccepted = false },
                    new FriendRequest{Id = 3, ProfileId = 3, FriendId = 1, IsAccepted = true },
                    new FriendRequest{Id = 4, ProfileId = 2, FriendId = 3, IsAccepted = true },

                }
            };

            Posts = new MockRepo<Post>
            {
                Entities = new HashSet<Post>
                {
                    new Post{Id = 1, ProfileId = 1, Message = "Hello I'm Mark", Timestamp = new DateTime() },
                    new Post{Id = 2, ProfileId = 2, Message = "Hello I'm John #soccer #football", Timestamp = new DateTime() },
                    new Post{Id = 3, ProfileId = 3, Message = "Hello I'm Carl #football", Timestamp = new DateTime() },
                    new Post{Id = 4, ProfileId = 4, Message = "Hello I'm Bill #soccer", Timestamp = new DateTime() },

                }
            };

            Comments = new MockRepo<Comment>
            {
                Entities = new HashSet<Comment>
                {
                    new Comment{Id = 1, ProfileId = 4, Text = "Hi Mark I'm Bill", PostId = 1 , Timestamp = new DateTime() },
                    new Comment{Id = 2, ProfileId = 3, Text = "Hi John I'm Carl", PostId = 2, Timestamp = new DateTime() },
                    new Comment{Id = 3, ProfileId = 2, Text = "Hi Carl I'm John", PostId = 3, Timestamp = new DateTime() },
                    new Comment{Id = 4, ProfileId = 1, Text = "Hi Bill I'm Mark",PostId = 4, Timestamp = new DateTime() },

                }
            };

            MetaData = new MockRepo<MetaInfo>
            {
                Entities = new HashSet<MetaInfo>
                {
                    new MetaInfo {Id = 1, Hashtag = "soccer", Posts = new List<Post>() {new Post {Id = 2, ProfileId = 2, Message = "Hello I'm John #soccer #football", Timestamp = new DateTime() }, new Post { Id = 4, ProfileId = 4, Message = "Hello I'm Bill #soccer", Timestamp = new DateTime() } } },
                    new MetaInfo {Id = 2, Hashtag = "football", Posts = new List<Post>() {new Post {Id = 2, ProfileId = 2, Message = "Hello I'm John #soccer #football", Timestamp = new DateTime() }, new Post { Id = 3, ProfileId = 3, Message = "Hello I'm Carl #football", Timestamp = new DateTime() } } }
                   
                }
            };

            Images = new MockRepo<Image>();


        }
    }
}
