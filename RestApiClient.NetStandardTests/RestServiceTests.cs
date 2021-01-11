using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestApiClient.NetStandardTests.RestApis;
using System.Threading.Tasks;

namespace TheProcessE.RestApiClient.Tests
{
    [TestClass()]
    public class RestServiceTests
    {
        private IExampleInterface _service;
        private TestContext testContext;

        public TestContext TestContext
        {
            get { return testContext; }
            set { testContext = value; }
        }

        [TestInitialize()]
        public void Init()
        {
            _service = RestService.GetService<IExampleInterface>();
        }

        [TestMethod()]
        public async Task GetPostsTest()
        {
            var requestBuilder = _service.GetPosts();
            var posts = await requestBuilder.GetResponseAsync<Post[]>();

            Assert.IsFalse(requestBuilder.IsConnectionError);
            Assert.IsTrue(requestBuilder.IsSuccess);
            Assert.IsNotNull(posts);
            TestContext.WriteLine(posts[0].Title);
        }

        [TestMethod()]
        public async Task GetPostsAsStringTest()
        {
            var requestBuilder = _service.GetPosts();
            var posts = await requestBuilder.GetResponseAsStringAsync();

            Assert.IsFalse(requestBuilder.IsConnectionError);
            Assert.IsTrue(requestBuilder.IsSuccess);
            Assert.IsFalse(string.IsNullOrWhiteSpace(posts));
            TestContext.WriteLine(posts);
        }

        [TestMethod()]
        public async Task GetPostTest()
        {
            var requestBuilder = _service.GetPosts(1);
            var post = await requestBuilder.GetResponseAsync<Post>();

            Assert.IsFalse(requestBuilder.IsConnectionError);
            Assert.IsTrue(requestBuilder.IsSuccess);
            Assert.IsNotNull(post);
            TestContext.WriteLine(post.Title);
        }

        [TestMethod()]
        public async Task GetPostWithParamTest()
        {
            var requestBuilder = _service.GetPostWithParam(1);
            var post = await requestBuilder.GetResponseAsync<Post>();

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

            var createBuilder = _service.PostPosts(post);
            var createResponse = await createBuilder.GetResponseAsync<Post>();

            post.Id = createResponse.Id;
            Assert.IsFalse(createBuilder.IsConnectionError);
            Assert.IsTrue(createBuilder.IsSuccess);
            Assert.AreEqual(post, createResponse);

            TestContext.WriteLine(createResponse.Title);
        }

        [TestMethod()]
        public async Task GetPostByIdTest()
        {
            var requestBuilder = _service.GetPostById(1);
            var post = await requestBuilder.GetResponseAsync<Post[]>();

            Assert.IsFalse(requestBuilder.IsConnectionError);
            Assert.IsTrue(requestBuilder.IsSuccess);
            Assert.IsNotNull(post);
            TestContext.WriteLine(post[0].Title);
        }
    }
}