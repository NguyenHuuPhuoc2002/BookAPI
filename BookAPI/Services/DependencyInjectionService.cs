using BookAPI.Repositories.Interfaces;
using BookAPI.Repositories.UnitOfWork;
using BookAPI.Services.Interfaces;
using BookAPI.Services;
using EcommerceWeb.Services;
using Service.Interface;
using Service;

namespace BookAPI.Repositories
{
    public static class DependencyInjectionService
    {
        public static IServiceCollection AddService(this IServiceCollection services)
        {
            services.AddScoped<ISachService, SachService>();
            services.AddScoped<IRefreshTokenService, RefreshTokenService>();
            services.AddScoped<ILoaiService, LoaiService>();
            services.AddScoped<IGioHangService, GioHangService>();
            services.AddScoped<IGioHangChiTietService, GioHangChiTietService>();
            services.AddScoped<IHoaDonService, HoaDonService>();
            services.AddScoped<IChiTietHoaDonService, ChiTietHoaDonService>();
            services.AddSingleton<IVnPayService, VnPayService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddSingleton<IMailService, MailService>();
            services.AddScoped<IPublisherService, PublisherService>();
            services.AddScoped<ISupplierService, SupplierService>();
            services.AddScoped<IUserRoleService, UserRoleService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<CacheService>();
            services.AddScoped<IResponseCacheService, ResponseCacheService>();
            services.AddScoped<IGoogleService, GoogleService>();
            services.AddScoped<IFacebookService, FacebookService>();

            return services;
        }
    }
}
