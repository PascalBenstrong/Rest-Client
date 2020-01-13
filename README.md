# Rest-Client

A simple HttpClient for making http requests

**Supported HTTP METHODS**
- GET
- POST
- PUT
- DELETE

**Supported HTTP request Data**
- BODY
- HEADER/HEADERS

## How To Use

### 1 - Create an Interface with your request methods
### 2 - Add a URL attribute to your interface with the base-url
### 3 - Mark the methods with correct HttpMethod using (GET, POST, PUT, DELETE)
### 4 - Mark the parameters as Header or BODY using the Header or BODY attribute
### 5 - Mark the return type of your method as a Task<Response> or Response
### 6 - Call the GetService method of the RestService class with your type as parameter
### 7 - All done !! you may now make http calls

## Target FrameWork
.NETFramework4.7.2

## Test Example getting the posts from [jsonplaceholder](https://jsonplaceholder.typicode.com/posts)

*Example of Service Interface*

```C#
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
```
*Example of post class*

```C#
  class Post
  {
      public int UserId { get; set; }
      public int Id { get; set; }
      public string Title { get; set; }
      public string body { get; set; }
  }

```


*Instantiating a new Service of your given type*

```C#
  // Create a new RestService of type IExampleInterface
  var testService = RestService.GetService<IExampleInterface>();
```

*Getting the posts*

```C#
// if not using async mark the method return type as Response
// or use var response = testService.Posts().Result;
var response = await testService.Posts();
var posts = response.GetResponse<Post[]>();

Console.WriteLine($"First Post: {posts[0].Title}");
Console.Read();

```

### Author
Pascal Nsunba [Pascal Benstrong](https://github.com/PascalBenstrong/)

### License
[MIT LICENSE](https://github.com/PascalBenstrong/Rest-Client/blob/master/LICENSE)
