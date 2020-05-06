using Microsoft.VisualStudio.TestTools.UnitTesting;
using TheProcessE.RestApiClient;
using System;
using System.Collections.Generic;
using System.Text;
using RestApiClient.NetStandardTests.RestApis;
using System.Diagnostics;
using System.IO;

namespace TheProcessE.RestApiClient.Tests
{
    [TestClass()]
    public class RestServiceTests
    {
        [TestMethod()]
        public void GetServiceTest()
        {
            var service = RestService.GetService<IParteeService>();
            Assert.IsNotNull(service);

            var response = service.V1().Result;
            Debug.WriteLine(response.ResponseBody);
            Console.WriteLine(response.ResponseBody);
            Assert.IsFalse(response.IsConnectionError);
            Assert.IsTrue(response.IsSuccess);
        }

        [TestMethod]
        public void LoginTest()
        {
            var service = RestService.GetService<IParteeService>();
            Assert.IsNotNull(service);

            var data = new
            {
                Password = "password",
                Email = "theprocessdm@gmail.com"
            };

            var response = service.Login(data).Result;
            Debug.WriteLine(response.ResponseBody);
            Console.WriteLine(response.ResponseBody);
            Assert.IsFalse(response.IsConnectionError);
            Assert.IsTrue(response.IsSuccess);
        }

        [TestMethod]
        public void UploadProfileTest()
        {
            var service = RestService.GetService<IParteeService>();
            Assert.IsNotNull(service);

            var exists = File.Exists("C:\\Users\\The Process E\\Pictures\\file_example_JPG_1MB.jpg");
            Assert.IsTrue(exists);

            var stream = File.OpenRead("C:\\Users\\The Process E\\Pictures\\file_example_JPG_1MB.jpg");
            Response response = service.UploadProfile(stream).Result;
            Console.WriteLine(response.ResponseBody);
            Assert.IsTrue(response.IsSuccess);
            response.Dispose();

            response = service.Login(new object()).Result;
            Console.WriteLine(response.ResponseBody);
            Assert.IsTrue(response.IsSuccess);
        }

        [TestMethod]
        public void GetProfileTest()
        {
            var service = RestService.GetService<IParteeService>();
            Assert.IsNotNull(service);

            var data = new
            {
                Password = "password",
                Email = "theprocessdm@gmail.com"
            };

            var response = service.Login(data).Result;
            Debug.WriteLine(response.ResponseBody);
            Console.WriteLine(response.ResponseBody);
            Assert.IsFalse(response.IsConnectionError);
            Assert.IsTrue(response.IsSuccess);
        }

        [TestMethod]
        public void PublishListing()
        {
            var service = RestService.GetService<IParteeService>();
            Assert.IsNotNull(service);

            var data = new
            {
                Password = "password",
                Email = "theprocessdm@gmail.com"
            };

/*            var response = service.Login(data).Result;
            Console.WriteLine(response.ResponseBody);
            Assert.IsFalse(response.IsConnectionError);*/

            var publishData = new
            {
                publish = true
            };

            var listingId = Guid.Parse("b85eec12-ae61-44e8-a7ac-ac2a1124cad7");

            var response = service.PublishListing(listingId, publishData).Result;

            Console.WriteLine("------------------------- Listing Publish result ----------------------------");
            Console.WriteLine(response.ResponseBody);
            Console.WriteLine(response.StatusCode);
            Assert.IsFalse(response.IsConnectionError);

        }
    }
}