
using Newtonsoft.Json;


namespace ApiConsole
{
        
    // Models to hold API data
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
    }

    public class Post
    {
        public int UserId { get; set; }
        public int Id { get; set; }
        public string Title { get; set; }
    }

    public class Comment
    {
        public int PostId { get; set; }
        public int Id { get; set; }
        public string Body { get; set; }
    }

    // Service interfaces
    public interface IApiService
    {
        Task<List<User>> GetUsersAsync();
        Task<List<Post>> GetPostsAsync(int userId);
        Task<List<Comment>> GetCommentsAsync(int postId);
    }

    // Service implementation
    public class ApiService : IApiService
    {
        private readonly HttpClient _httpClient;

        public ApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<User>> GetUsersAsync()
        {
            var response = await _httpClient.GetAsync("https://jsonplaceholder.typicode.com/users");
            var users = JsonConvert.DeserializeObject<List<User>>(await response.Content.ReadAsStringAsync());
            
            return users;

        }

        public async Task<List<Post>> GetPostsAsync(int userId)
        {
            var response = await _httpClient.GetAsync($"https://jsonplaceholder.typicode.com/posts?userId={userId}");
            var posts = JsonConvert.DeserializeObject<List<Post>>(await response.Content.ReadAsStringAsync());
            return posts;

        }

        public async Task<List<Comment>> GetCommentsAsync(int postId)
        {
            var response = await _httpClient.GetAsync($"https://jsonplaceholder.typicode.com/comments?postId={postId}");
            var comments = JsonConvert.DeserializeObject<List<Comment>>(await response.Content.ReadAsStringAsync());
            
            return comments;
        }
    }

    // Controller
    public class ApiController
    {
        private readonly IApiService _apiService;

        public ApiController(IApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task PrintAllUsersAsync()
        {
            var users = await _apiService.GetUsersAsync();
            foreach (var user in users)
            {
                Console.WriteLine($"User: {user.Username}");
            }
        }

        public async Task DisplayPostsByUserAsync(int userId)
        {
            var posts = await _apiService.GetPostsAsync(userId);
            foreach (var post in posts)
            {
                Console.WriteLine($"Post by User {userId}: {post.Title}");
            }
        }

        public async Task DisplayCommentsByPostAsync(int postId)
        {
            var comments = await _apiService.GetCommentsAsync(postId);
            foreach (var comment in comments)
            {
                Console.WriteLine($"Comment for Post {postId}: {comment.Body}");
            }
        }
    }

    class Program
    {
        static async Task Main(string[] args)
        {
            using (var httpClient = new HttpClient())
            {
                var apiService = new ApiService(httpClient);
                var apiController = new ApiController(apiService);

                // Print all users
                await apiController.PrintAllUsersAsync();

                Console.WriteLine("Select a user ID:");
                int selectedUserId = int.Parse(Console.ReadLine());

                // Display posts by selected user
                await apiController.DisplayPostsByUserAsync(selectedUserId);

                Console.WriteLine("Select a post ID:");
                int selectedPostId = int.Parse(Console.ReadLine());

                // Display comments by selected post
                await apiController.DisplayCommentsByPostAsync(selectedPostId);
            }
        }
    }

}


