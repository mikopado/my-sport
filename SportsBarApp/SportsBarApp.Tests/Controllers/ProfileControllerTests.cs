using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web.Mvc;
using SportsBarApp.Tests.MockClasses;

namespace SportsBarApp.Controllers.Tests
{
    [TestClass()]
    public class ProfileControllerTests
    {
        ProfileController controller;

        [TestInitialize]
        public void Initialize()
        {
            controller = new ProfileController(new MockUnit(new MockDb()));
        }
        [TestCleanup]
        public void Cleanup()
        {
            controller = null;
        }

        [TestMethod()]
        public void MyProfileCheckIfBadRequestReturnTest()
        {
            
            // Act
            var actual = controller.MyProfile(null);

            // Assert
            Assert.IsInstanceOfType(actual, typeof(HttpStatusCodeResult));
            var httpResult = actual as HttpStatusCodeResult;
            Assert.AreEqual(400, httpResult.StatusCode);
            Assert.AreEqual("An error occurred whilst processing your request.", httpResult.StatusDescription);
        }

        [TestMethod()]
        public void MyProfileCheckIfHttpNotFoundReturnTest()
        {

            // Act
            var actual = controller.MyProfile(88);

            // Assert
            Assert.IsInstanceOfType(actual, typeof(HttpNotFoundResult));
        }

        [TestMethod()]
        public void DetailsIfPassNullAsIdBadRequestReturnTest()
        {
            // Act
            var actual = controller.Details(null);

            // Assert
            Assert.IsInstanceOfType(actual, typeof(HttpStatusCodeResult));
            var httpResult = actual as HttpStatusCodeResult;
            Assert.AreEqual(400, httpResult.StatusCode);
            Assert.AreEqual("An error occurred whilst processing your request.", httpResult.StatusDescription);

        }

        [TestMethod()]
        public void DetailsIfNotFoundResultReturnTest()
        {
            // Act
            var actual = controller.Details(7);

            // Assert
            Assert.IsInstanceOfType(actual, typeof(HttpNotFoundResult));
        }


        
        [TestMethod()]
        public void EditIfNullValueIsPassedBadRequestReturnTest()
        {
            // Act
            int? id = null;
            var actual = controller.Edit(id);

            // Assert
            Assert.IsInstanceOfType(actual, typeof(HttpStatusCodeResult));
            var httpResult = actual as HttpStatusCodeResult;
            Assert.AreEqual(400, httpResult.StatusCode);
            Assert.AreEqual("An error occurred whilst processing your request.", httpResult.StatusDescription);

        }
        [TestMethod()]
        public void EditTryIfOtherResultCodeIsPassedReturnTest()
        {
            // Act
            int? id = null;
            var actual = controller.Edit(id);

            // Assert
            Assert.IsInstanceOfType(actual, typeof(HttpStatusCodeResult));
            var httpResult = actual as HttpStatusCodeResult;
            Assert.AreNotEqual(500, httpResult.StatusCode);
            
        }


        [TestMethod()]
        public void DeleteIfNullValueIsPassedBadRequestReturnTest()
        {
            var actual = controller.Delete(null);

            // Assert
            Assert.IsInstanceOfType(actual, typeof(HttpStatusCodeResult));
            var httpResult = actual as HttpStatusCodeResult;
            Assert.AreEqual(400, httpResult.StatusCode);
            Assert.AreEqual("An error occurred whilst processing your request.", httpResult.StatusDescription);

        }

        
        [TestMethod()]
        public void ChangeProfilePhotoIfNullValueIsPassedBadRequestReturnTest()
        {
            int? id = null;
            var actual = controller.ChangeProfilePhoto(id);

            // Assert
            Assert.IsInstanceOfType(actual, typeof(HttpStatusCodeResult));
            var httpResult = actual as HttpStatusCodeResult;
            Assert.AreEqual(400, httpResult.StatusCode);
            Assert.AreEqual("An error occurred whilst processing your request.", httpResult.StatusDescription);


        }

      
    }
}