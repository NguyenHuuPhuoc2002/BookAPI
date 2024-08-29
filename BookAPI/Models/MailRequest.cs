namespace BookAPI.Models
{
    public class MailRequest
    {
        public string ToEmail { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }


    }
    public class LinkMailModel
    {
        public string Email { get; set; }
        public string Link { get; set; }
    }
}
