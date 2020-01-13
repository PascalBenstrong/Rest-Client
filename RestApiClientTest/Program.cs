using TheProcessE.RestApiClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;

namespace RestApiClientTest
{
    partial class Program
    {
        enum AccountType
        {
            Business, Customer
        }

        class Test
        {
            public Guid Id { get; set; }
            public AccountType AccountType { get; set; }
            public string FullName { get; set; }
            public DateTime DateCreated { get; set; }
            public string Password { get; set; }
            public string Email { get; set; }
            public string PhoneNumber { get; set; }
        }

        class Post
        {
            public int UserId { get; set; }
            public int Id { get; set; }
            public string Title { get; set; }
            public string body { get; set; }
        }


        [URL(@"https://jsonplaceholder.typicode.com")]
        public interface IExampleInterface
        {
            [GET("posts")]
            Task<Response> Posts();
            
            [GET("todos")]
            Response Todos();
            
            [GET("users")]
            Task<Response> Users();
        }
        static void Main(string[] args)
        {

            Console.WriteLine("Testing");

            // Create a new RestService of type IExampleInterface
            var testService = RestService.GetService<IExampleInterface>();

/*            var t = new Test
            {
                Password = "password",
                Email = "example1@gmail.com",
                PhoneNumber = "1234567890"
            };*/


            var response = testService.Posts().Result;
            //response = await testService.Posts();
            var posts = response.GetResponse<Post[]>();

            Console.WriteLine($"First Post: {posts[0].Title}");
            Console.Read();

        }
    }
}
