using BookAPI.Models;
using BookAPI.Services.Interfaces;
using Common.Exceptions;
using Newtonsoft.Json;

public class TokenValidationMiddleware
{
    private readonly RequestDelegate _next;

    public TokenValidationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        var email = GlobalVariables.maKh;
        var authorizationHeader = context.Request.Headers["Authorization"].ToString();
        if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer "))
        {
            await _next(context);
            return;
        }

        var accessToken = authorizationHeader.Split(" ").Last();

        // Lấy IResponseCacheService từ HttpContext
        var responseCacheService = context.RequestServices.GetService<IResponseCacheService>();

        // Kiểm tra xem token có trong Redis hay không
        var cachedData = await responseCacheService.GetCacheResponseAsync(email);

        // Nếu Redis không có dữ liệu thì tiếp tục request mà không kiểm tra
        if (string.IsNullOrEmpty(cachedData))
        {
            await _next(context);
            return;
        }

        // Deserialize token từ Redis
        var dataString = JsonConvert.DeserializeObject<string>(cachedData);
        var token = JsonConvert.DeserializeObject<LogOutRequestModel>(dataString);

        // Kiểm tra token
        if (token != null && token.Token == accessToken)
        {
          //  throw new AuthorizeException("Token không hợp lệ");
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Token không hợp lệ");
            return;
        }

        // Tiếp tục xử lý request nếu token hợp lệ
        await _next(context);
    }
}
