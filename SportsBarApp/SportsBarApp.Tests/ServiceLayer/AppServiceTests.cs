using Microsoft.VisualStudio.TestTools.UnitTesting;
using SportsBarApp.Models;
using SportsBarApp.ServiceLayer;
using SportsBarApp.Tests.MockClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportsBarApp.ServiceLayer.Tests
{
    [TestClass()]
    public class AppServiceTests
    {
        MockUnit unit;
        AppService service;

        [TestInitialize()]
        public void Initialize()
        {
            unit = new MockUnit(new MockDb());
            service = new AppService(unit);
        }
        [TestCleanup()]
        public void Cleanup()
        {
            unit = null;
            service = null;
        }

        [TestMethod()]
        public void AddProfileTest()
        {
            Profile profile = new Profile { ProfileId = 8, FirstName = "Bill", LastName = "Bab", DateOfBirth = new DateTime(1950, 7, 12), GlobalId = new Guid() };
            int count = unit.Profiles.Count();

            service.Add(profile);

            Assert.AreEqual(count + 1, unit.Profiles.Count());
        }

        
        [TestMethod()]
        public void RemoveProfileTest()
        {
            Profile profile = service.GetProfile(4);
            int count = unit.Profiles.Count();

            service.Remove(profile);

            Assert.AreEqual(count - 1, unit.Profiles.Count());
        }        

        [TestMethod()]
        public void StoreMetaInfoTestIfAdded()
        {
            Post post = new Post {Id = 7, Message = "Great match #basket", Timestamp = new DateTime() };
            int count = unit.MetaData.Count();
            service.StoreMetaInfo(post);

            int actual = unit.MetaData.Count();

            Assert.AreEqual(count + 1, actual);
        }

        [TestMethod()]
        public void StoreMetaInfoTestIfAddedCorrectKeyword()
        {
            Post post = new Post { Id = 7, Message = "Great match #basket", Timestamp = new DateTime() };
            
            service.StoreMetaInfo(post);
            var actual = unit.MetaData.GetElement(x => x.Hashtag == "basket");
            string actualKeyword = actual.Hashtag;

            Assert.AreEqual("basket", actualKeyword);
        }
        [TestMethod()]
        public void StoreMetaInfoTestIfNoHashtagsInPost()
        {
            Post post = new Post { Id = 7, Message = "Great match basket", Timestamp = new DateTime() };
            var expected = unit.MetaData.Count();
            service.StoreMetaInfo(post);
            int actual = unit.MetaData.Count();           

            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void GetProfileByIdTest()
        {
            Profile profile = new Profile { ProfileId = 4, FirstName = "Bill", LastName = "Kop", DateOfBirth = new DateTime(1960, 8, 2), GlobalId = new Guid("c04d45ff-3bc4-32f7-b7ec-c556f7e20c14") };

            Profile actual = service.GetProfile(4);

            Assert.AreEqual(Newtonsoft.Json.JsonConvert.SerializeObject(profile),
                Newtonsoft.Json.JsonConvert.SerializeObject(actual));
        }

        [TestMethod()]
        public void GetProfileNotInDBTest()
        {
            Profile profile = service.GetProfile(33);

            Assert.AreEqual(null, profile);
        }
        

        [TestMethod()]
        public void SearchProfilesTest()
        {
            List<Profile> expected = new List<Profile>()
            {
                new Profile{ProfileId = 1, FirstName = "Mark", LastName = "Lannon", DateOfBirth = new DateTime(1980, 12, 23), GlobalId = new Guid("c04d45ff-3bc4-42f7-b7ec-b556f7e20c14") },
            };

            List<Profile> actual = service.SearchProfiles("M").ToList();
            

            Assert.AreEqual(expected.Count, actual.Count);
            if (actual.Count == expected.Count)
            {
                Assert.AreEqual(Newtonsoft.Json.JsonConvert.SerializeObject(expected[0]),
                Newtonsoft.Json.JsonConvert.SerializeObject(actual[0]));
            }
        }

        [TestMethod()]
        public void SearchProfilesIncludeFirstNameAndLastNameTest()
        {
            List<Profile> expected = new List<Profile>()
            {
                new Profile{ProfileId = 2, FirstName = "John", LastName = "Bell", DateOfBirth = new DateTime(1990, 10, 13), GlobalId = new Guid("c14d45ff-3bc4-42f7-b5ec-b556f7e20c14")},                
                new Profile{ProfileId = 4, FirstName = "Bill", LastName = "Kop", DateOfBirth = new DateTime(1960, 8, 2), GlobalId = new Guid("c04d45ff-3bc4-32f7-b7ec-c556f7e20c14") },
             };

            List<Profile> actual = service.SearchProfiles("B").ToList();


            Assert.AreEqual(expected.Count, actual.Count);
            if (actual.Count == expected.Count)
            {
                for (int i = 0; i < actual.Count; i++)
                {
                    Assert.AreEqual(Newtonsoft.Json.JsonConvert.SerializeObject(expected[i]),
                                   Newtonsoft.Json.JsonConvert.SerializeObject(actual[i]));
                
                }
                
                
            }
        }

        [TestMethod()]
        public void SearchProfilesNothingFoundTest()
        {
            List<Profile> expected = new List<Profile>();
            
            List<Profile> actual = service.SearchProfiles("X").ToList();

            Assert.AreEqual(expected.Count, actual.Count);
            
        }

        [TestMethod()]
        public void GetFriendsTest()
        {
            List<Profile> expected = new List<Profile>()
            {
                new Profile{ProfileId = 1, FirstName = "Mark", LastName = "Lannon", DateOfBirth = new DateTime(1980, 12, 23), GlobalId = new Guid("c04d45ff-3bc4-42f7-b7ec-b556f7e20c14") },
                new Profile{ProfileId = 2, FirstName = "John", LastName = "Bell", DateOfBirth = new DateTime(1990, 10, 13), GlobalId = new Guid("c14d45ff-3bc4-42f7-b5ec-b556f7e20c14")},
                
            };

            List<Profile> actual = service.GetFriends(3).ToList();

            Assert.AreEqual(expected.Count, actual.Count);
            if (actual.Count == expected.Count)
            {
                for (int i = 0; i < actual.Count; i++)
                {
                    Assert.AreEqual(Newtonsoft.Json.JsonConvert.SerializeObject(expected[i]),
                                   Newtonsoft.Json.JsonConvert.SerializeObject(actual[i]));

                }

            }
        }

        [TestMethod()]
        public void GetFriendsNoFriendsFoundTest()
        {
            List<Profile> expected = new List<Profile>();
            

            List<Profile> actual = service.GetFriends(4).ToList();

            Assert.AreEqual(expected.Count, actual.Count);
            
        }

        [TestMethod()]
        public void GetFriendsCheckWhenFriendshipOnlyRequestsTest()
        {
            List<Profile> expected = new List<Profile>()
            {
                new Profile{ProfileId = 3, FirstName = "Carl", LastName = "Cox", DateOfBirth = new DateTime(1988, 2, 3), GlobalId = new Guid("a04d45ff-3bc7-42f7-b7ec-b556f7e20c14") },

            };

            List<Profile> actual = service.GetFriends(2).ToList();

            Assert.AreEqual(expected.Count, actual.Count);
            if (actual.Count == expected.Count)
            {
                
                Assert.AreEqual(Newtonsoft.Json.JsonConvert.SerializeObject(expected[0]),
                                Newtonsoft.Json.JsonConvert.SerializeObject(actual[0]));               

            }

        }
        
        [TestMethod()]
        public void GetPostsFriendsTest()
        {
            
            List<Post> expected = new List<Post>()
            {
                new Post{Id = 1, ProfileId = 1, Message = "Hello I'm Mark", Timestamp = new DateTime() },
                new Post{Id = 2, ProfileId = 2, Message = "Hello I'm John #soccer #football", Timestamp = new DateTime() },
                new Post{Id = 3, ProfileId = 3, Message = "Hello I'm Carl #football", Timestamp = new DateTime() },
                
            };

            List<Post> actual = service.GetPostsFriends(3).ToList();

            Assert.AreEqual(expected.Count, actual.Count);
            if (actual.Count == expected.Count)
            {
                for (int i = 0; i < actual.Count; i++)
                {
                    Assert.AreEqual(Newtonsoft.Json.JsonConvert.SerializeObject(expected[i]),
                                Newtonsoft.Json.JsonConvert.SerializeObject(actual[i]));
                }
            }
        }

        [TestMethod()]
        public void GetPostsFriendsNoFriendsTest()
        {

            List<Post> expected = new List<Post>()
            {
                new Post{Id = 4, ProfileId = 4, Message = "Hello I'm Bill #soccer", Timestamp = new DateTime() },

            };

            List<Post> actual = service.GetPostsFriends(4).ToList();

            Assert.AreEqual(expected.Count, actual.Count);
            if (actual.Count == expected.Count)
            {
                for (int i = 0; i < actual.Count; i++)
                {
                    Assert.AreEqual(Newtonsoft.Json.JsonConvert.SerializeObject(expected[i]),
                                Newtonsoft.Json.JsonConvert.SerializeObject(actual[i]));
                }
            }
        }

        [TestMethod()]
        public void GetPostsByUserTest()
        {
            List<Post> expected = new List<Post>()
            {
               new Post{Id = 3, ProfileId = 3, Message = "Hello I'm Carl #football", Timestamp = new DateTime() },

            };

            List<Post> actual = service.GetPostsByUser(3).ToList();

            Assert.AreEqual(expected.Count, actual.Count);

            if (actual.Count == expected.Count)
            {
                for (int i = 0; i < actual.Count; i++)
                {
                    Assert.AreEqual(Newtonsoft.Json.JsonConvert.SerializeObject(expected[i]),
                                Newtonsoft.Json.JsonConvert.SerializeObject(actual[i]));
                }
            }
        }

        [TestMethod()]
        public void GetPostsByUserNoUserTest()
        {
            List<Post> expected = new List<Post>();
            
            List<Post> actual = service.GetPostsByUser(8).ToList();

            Assert.AreEqual(expected.Count, actual.Count);
            
        }

        
        [TestMethod()]
        public void FindFriendRequestTest()
        {
            FriendRequest expected = new FriendRequest { Id = 2, ProfileId = 2, FriendId = 4, IsAccepted = false };

            var actual = service.FindFriend(2, 4);

            Assert.AreEqual(Newtonsoft.Json.JsonConvert.SerializeObject(expected),
                                Newtonsoft.Json.JsonConvert.SerializeObject(actual));

            var actual2 = service.FindFriend(4, 2);

            Assert.AreEqual(Newtonsoft.Json.JsonConvert.SerializeObject(expected),
                               Newtonsoft.Json.JsonConvert.SerializeObject(actual));

        }       

        [TestMethod()]
        public void GetPendingRequestsTest()
        {
            List<FriendRequest> expected = new List<FriendRequest>() { new FriendRequest { Id = 1, ProfileId = 1, FriendId = 2, IsAccepted = false } };

            var actual = service.GetPendingRequests(2);

            Assert.AreEqual(expected.Count, actual.Count);

            if (actual.Count == expected.Count)
            {
                for (int i = 0; i < actual.Count; i++)
                {
                    Assert.AreEqual(Newtonsoft.Json.JsonConvert.SerializeObject(expected[i]),
                                Newtonsoft.Json.JsonConvert.SerializeObject(actual[i]));
                }
            }
        }

        [TestMethod()]
        public void GetPendingRequestsAlreadyFriendTest()
        {
            List<FriendRequest> expected = new List<FriendRequest>();

            var actual = service.GetPendingRequests(1);

            Assert.AreEqual(expected.Count, actual.Count);
            
        }

               

        
    }
}