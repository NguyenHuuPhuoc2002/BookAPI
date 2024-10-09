using BookAPI.Data;

namespace BookAPI.Models
{
    public class AppSetting
    {
        public string Secret { get; set; }
        public string ValidIssuer { get; set; }
        public string ValidAudience { get; set; }
    }
    public class CacheSetting
    {
        public TimeSpan Duration { get; set; }
        public TimeSpan? SlidingExpiration { get; set; }
    }
    public static class Caches
    {
        public static string CacheKeyAllBook { get; set; } = "DefaultAllBookKey";
        public static string CacheKeyBookId { get; set; } = "DefaultBookIdKey";
        public static string CacheKeyBookSearch { get; set; } = "DefaultBookSearchKey";

        public static string CacheKeyCategoryID{ get; set; } = "DefaultCategoryIDKey";
        public static string CacheKeyAllCategories{ get; set; } = "DefaultAllCategoriesKey";

        public static string CacheKeyAllPublishers { get; set; } = "DefaultAllPublishersKey";
        public static string CacheKeyPublisherID { get; set; } = "DefaultPublisherIDKey";
        public static string CacheKeyPublisherSearch { get; set; } = "DefaultPublishersSearchKey";

        public static string CacheKeyAllSuppliers { get; set; } = "DefaultAllSuppliersKey";
        public static string CacheKeySupplierID { get; set; } = "DefaultSupplierIDKey";
        public static string CacheKeySuppliersSearch { get; set; } = "DefaultSuppliersSearchKey";
    }

    public static class GlobalVariables
    {
        public static string maKh { get; set; }
    }
    public static class TokenGlobalVariable
    {
        public static string Token { get; set; }
    }
}
