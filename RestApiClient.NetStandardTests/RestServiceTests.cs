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
        public async Task GetPostsTest()
        {
            var requestBuilder = service.GetPosts();
            var posts = await requestBuilder.GetResponseSuppressExceptionAsync<Post>();

            Assert.IsFalse(requestBuilder.IsConnectionError);
            Assert.IsTrue(requestBuilder.IsSuccess);
            Assert.IsNotNull(posts);
            TestContext.WriteLine(posts.Title);
        }

        [TestMethod()]
        public async Task GetPostTest()
        {
            var requestBuilder = service.GetPosts(1);
            var post = await requestBuilder.GetResponseSuppressExceptionAsync<Post>();

            Assert.IsFalse(requestBuilder.IsConnectionError);
            Assert.IsTrue(requestBuilder.IsSuccess);
            Assert.IsNotNull(post);
            TestContext.WriteLine(post.Title);
        }

        [TestMethod()]
        public async Task PostAPostTest()
        {
            var post = new Post
            {
                Body = "Lorem ipsum dolor sit amet, consectetur adipiscing elit.",
                UserId = 1,
                Title = "John Doe"

            };

            var createBuilder = service.PostPosts(post);
            var createResponse = await createBuilder.GetResponseSuppressExceptionAsync<Post>();

            post.Id = createResponse.Id;
            Assert.IsFalse(createBuilder.IsConnectionError);
            Assert.IsTrue(createBuilder.IsSuccess);
            Assert.AreEqual(post, createResponse);

            TestContext.WriteLine(createResponse.Title);
        }

        [TestMethod()]
        public async Task GetPostByIdTest()
        {
            var requestBuilder = service.GetPostById(1);
            var post = await requestBuilder.GetResponseSuppressExceptionAsync<Post[]>();

            Assert.IsFalse(requestBuilder.IsConnectionError);
            Assert.IsTrue(requestBuilder.IsSuccess);
            Assert.IsNotNull(post);
            TestContext.WriteLine(post[0].Title);
        }
    }
}