# Rest-Client

A simple HttpClient for making http requests

**Supported HTTP METHODS**

- GET
- POST
- PUT
- DELETE
- PATCH

**Supported HTTP request Data**

- BODY
- HEADER/HEADERS
- QUERY
- PARAM for path parameters

## How To Use

### 1 - Create an Interface with your request methods

### 2 - Add a URL attribute to your interface with the base-url

### 3 - Mark the methods with correct HttpMethod using (GET, POST, PUT, DELETE, PATCH)

### 4 - Mark the parameters as Header or Body using the HEADER or BODY attribute

### 5 - Mark the return type of your method as a RequestBuilder

### 6 - Call the GetService method of the RestService class with your type as parameter

### 7 - All done !! you may now make http calls

## Target FrameWork

.NET Standard 2.0

## Test Example getting the posts from [json placeholder](https://jsonplaceholder.typicode.com/posts)

_Example of Service Interface_

```C#
  [URL("https://jsonplaceholder.typicode.com")]
  public interface IExampleInterface
  {
      [GET("posts")]
      RequestBuilder GetPosts([PARAM] int? id = null);

      [GET("posts")]
      RequestBuilder GetPostById([QUERY("postId")] int id);

      [POST("posts")]
      RequestBuilder PostPosts<TBody>([BODY] TBody post);

  }
```

_Example of post class_

```C#
  class Post
  {
      public int UserId { get; set; }
      public int Id { get; set; }
      public string Title { get; set; }
      public string body { get; set; }
  }

```

_Instantiating a new Service of your given type_

```C#
  // Create a new RestService of type IExampleInterface
  var testService = RestService.GetService<IExampleInterface>();
```

_Getting the posts_

```C#
// if not using async mark the method return type as Response
// or use var posts = requestBuilder.GetResponseAsync<Post[]>().Result;
var requestBuilder = service.GetPosts(1);
var posts = await requestBuilder.GetResponseAsync<Post>();

Console.WriteLine($"First Post: {posts.Title}");

```

### Author

[Pascal Nsunba](https://github.com/PascalBenstrong/)

### License

[MIT LICENSE](https://github.com/PascalBenstrong/Rest-Client/blob/master/LICENSE)
