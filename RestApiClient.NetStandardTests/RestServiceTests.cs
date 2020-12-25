using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestApiClient.NetStandardTests.RestApis;
using System.Threading.Tasks;

namespace TheProcessE.RestApiClient.Tests
{
    [TestClass()]
    public class RestServiceTests
    {
        private IExampleInterface service;
        private TestContext testContext;

        public TestContext TestContext
        {
            get { return testContext; }
            set { testContext = value; }
        }

        [TestInitialize()]
        public void Init()
        {
            service = RestService.GetService<IExampleInterface>();
        }

        [TestMethod()]
        public async Task PostsTest()
        {
            var requestBuilder = service.Posts();
            var posts = await requestBuilder.GetResponseSuppressExceptionAsync<Post[]>();

            Assert.IsFalse(requestBuilder.IsConnectionError);
            Assert.IsTrue(requestBuilder.IsSuccess);
            Assert.IsNotNull(posts);
            TestContext.WriteLine(posts[0].Title);
        }

        [TestMethod()]
        public async Task PostTest()
        {
            var requestBuilder = service.Posts(1);
            var post = await requestBuilder.GetResponseSuppressExceptionAsync<Post>();

            Assert.IsFalse(requestBuilder.IsConnectionError);
            Assert.IsTrue(requestBuilder.IsSuccess);
            Assert.IsNotNull(post);
            TestContext.WriteLine(post.Title);
        }

        [TestMethod()]
        public async Task PostAPostTest()
        {
            var requestBuilder = service.Posts();
            var posts = await requestBuilder.GetResponseSuppressExceptionAsync<Post[]>();

            Assert.IsFalse(requestBuilder.IsConnectionError);
            Assert.IsTrue(requestBuilder.IsSuccess);
            Assert.IsNotNull(posts);

            var post = posts[posts.Length - 1];
            post.Id = posts.Length + 1;
            var createBuilder = service.PostPosts(post);
            var createResponse = await createBuilder.GetResponseSuppressExceptionAsync<Post>();

            Assert.IsFalse(requestBuilder.IsConnectionError);
            Assert.IsTrue(requestBuilder.IsSuccess);
            Assert.AreEqual(post, createResponse);

            TestContext.WriteLine(createResponse.Title);
        }
    }
}