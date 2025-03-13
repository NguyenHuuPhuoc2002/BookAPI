namespace BookAPI.Services.Interfaces
{
    public interface IGoogleService
    {
        string GetGoogleAuthUrl(string redirectUri);
    }
}
