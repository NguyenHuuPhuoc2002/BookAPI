namespace BookAPI.Models
{
    public class AppSetting
    {
        public string Secret { get; set; }
        public string ValidIssuer { get; set; }
        public string ValidAudience { get; set; }
    }
    public static class GlobalVariables
    {
        public static string maKh { get; set; }
    }
}
