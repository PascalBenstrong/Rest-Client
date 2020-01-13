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

        static void Main(string[] args)
        {

            Console.WriteLine("Testing");
            var testService = RestService.getService<ITestService>();

            var t = new Test
            {
                Password = "password",
                Email = "example1@gmail.com",
                PhoneNumber = "1234567890"
            };

            
            var result = testService.login(t).Result;
            var user = result.IsConnectionError ? new Test() : result.GetResult<Test>();

            Console.WriteLine($"Result from Proxy: {result.HttpResponseMessage?.StatusCode}");
            Console.Read();
        }
    }
}
