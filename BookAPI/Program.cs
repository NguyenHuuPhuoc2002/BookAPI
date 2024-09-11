using BookAPI.Data;
using BookAPI.Database;
using BookAPI.Helper;
using BookAPI.Models;
using BookAPI.Repositories;
using BookAPI.Repositories.Interfaces;
using BookAPI.Seeding;
using BookAPI.Services;
using BookAPI.Services.Interfaces;
using EcommerceWeb.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Service;
using Service.Interface;
using System.Text;

/*namespace BookAPI
{
    public class Program
    {
        public static async void Main(string[] args)
        {*/
            #region Logging
            // Cấu hình console để sử dụng UTF-8
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            //Cấu hình logging
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console() // Ghi log ra console
                .WriteTo.File("Logs/log.txt",
                    rollingInterval: RollingInterval.Day, // Tạo file log mới mỗi ngày
                    retainedFileCountLimit: 7, // Giữ lại log của 7 ngày gần nhất
                    encoding: System.Text.Encoding.UTF8) // Ghi log vào file với UTF-8
                .CreateLogger();
            #endregion

            var builder = WebApplication.CreateBuilder(args);

            //CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins",
                    builder => builder.AllowAnyOrigin()
                                      .AllowAnyMethod()
                                      .AllowAnyHeader());
            });

            builder.Services.Configure<AppSetting>(builder.Configuration.GetSection("JWT"));

            #region Identity
            //Identity
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<DataContext>().AddDefaultTokenProviders();
            //authentication - Token 
            builder.Services.AddAuthentication(options => {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options => {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = builder.Configuration["JWT:ValidAudience"],
                    ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"]))
                };
            });
            #endregion

            builder.Host.UseSerilog();
            // Add services to the container.

            builder.Services.AddControllers();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            // Thêm dịch vụ Swagger
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                // Cấu hình để sử dụng Bearer Token trong Swagger
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Nhập 'Bearer' [space] và sau đó là token của bạn trong ô dưới đây.\n\nVí dụ: 'Bearer 12345abcdef'"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
            });

            //connect database
            builder.Services.AddDbContext<DataContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DbBook")));

            #region Repository - Service
            // Repository
            builder.Services.AddScoped<ISachRepository, SachRepository>();
            builder.Services.AddScoped<ILoaiRepository, LoaiRepository>();
            builder.Services.AddScoped<IGioHangRepository, GioHangRepository>();
            builder.Services.AddScoped<IGioHangChiTietRepository, GioHangChiTietRepository>();
            builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            builder.Services.AddScoped<IHoaDonRepository, HoaDonRepository>();
            builder.Services.AddScoped<IChiTietHoaDonRepository, ChiTietHoaDonRepository>();
            builder.Services.AddScoped<IAccountRepository, AccountRepository>();
            builder.Services.AddScoped<IPublisherRepository, PublisherRepository>();
            builder.Services.AddScoped<ISupplierRepository, SupplierRepository>();
            builder.Services.AddScoped<IUserRoleRepository, UserRoleRepository>();
            builder.Services.AddScoped<IRoleRepository, RoleRepository>();

            // Service
            builder.Services.AddScoped<ISachService, SachService>();
            builder.Services.AddScoped<IRefreshTokenService, RefreshTokenService>();
            builder.Services.AddScoped<ILoaiService, LoaiService>();
            builder.Services.AddScoped<IGioHangService, GioHangService>();
            builder.Services.AddScoped<IGioHangChiTietService, GioHangChiTietService>();
            builder.Services.AddScoped<IHoaDonService, HoaDonService>();
            builder.Services.AddScoped<IChiTietHoaDonService, ChiTietHoaDonService>();
            builder.Services.AddSingleton<IVnPayService, VnPayService>();
            builder.Services.AddScoped<IAccountService, AccountService>();
            builder.Services.AddSingleton<IMailService, MailService>();
            builder.Services.AddScoped<IPublisherService, PublisherService>();
            builder.Services.AddScoped<ISupplierService, SupplierService>();
            builder.Services.AddScoped<IUserRoleService, UserRoleService>();
            builder.Services.AddScoped<IRoleService, RoleService>();
            #endregion

            // Đăng ký AutoMapper
            builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

            builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
            {
                //reset password valid 2 hours
                options.TokenLifespan = TimeSpan.FromHours(2);
            });

            #region Cấu hình Mail
            // Thêm các dịch vụ vào DI container
            builder.Services.AddControllersWithViews();
            //IUrlHelper
            builder.Services.AddScoped<IUrlHelper>(x =>
            {
                var actionContext = x.GetRequiredService<IActionContextAccessor>().ActionContext;
                var factory = x.GetRequiredService<IUrlHelperFactory>();
                return factory.GetUrlHelper(actionContext);
            });
            builder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            #endregion

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                });
            }
            app.UseCors("AllowAllOrigins"); // Apply CORS policy
            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DataContext>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

            try
            {
                await context.Database.MigrateAsync();
                await SeedData.Seed(context, roleManager);  
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "A problem occurred during migration");
            }

            app.Run();
/*        }
    }
}*/
