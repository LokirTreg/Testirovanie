namespace Common
{
    public class SharedConfiguration
    {
     public static string? HostName { get; set; }= null!; //: "localhost",
        public static string? UserName { get; set; } = null!; //: "guest",
        public static string? Password { get; set; } = null!; //: "guest",
        public static string? RequestQueue { get; set; } = null!; //: "Queue",
        public static string? ResponseQueue { get; set; } = null!; //: "QueueResponse"

        public static void UpdateSharedConfiguration(
            string? hostName,
            string? userName,
            string? password,
            string? requestQueue,
            string? responseQueue
        )
        {
            HostName = hostName;
            UserName = userName;
            Password = password;
            RequestQueue = requestQueue;
            ResponseQueue = responseQueue;
        }
    }
}