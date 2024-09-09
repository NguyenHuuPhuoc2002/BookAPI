namespace BookAPI.Models
{
    public class MailRequest
    {
        public string ToEmail { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty ;
        public string Body { get; set; } = string.Empty;


    }
    public class LinkMailModel
    {
        public string Email { get; set; }
        public string Link { get; set; }
    }
}
