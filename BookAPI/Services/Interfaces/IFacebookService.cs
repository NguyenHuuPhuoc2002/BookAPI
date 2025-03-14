namespace BookAPI.Services.Interfaces
{
    public interface IFacebookService
    {
        string GetFacebookAuthUrl(string redirectUri);
    }
}
