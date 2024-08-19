using BookAPI.Helper;
using BookAPI.Models;
using BookAPI.Repositories;
using BookAPI.Repositories.Database;
using BookAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;

namespace BookAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
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

            var builder = WebApplication.CreateBuilder(args);

            #region Authentication
            //token
            builder.Services.Configure<AppSetting>(builder.Configuration.GetSection("AppSettings"));

            var secretKey = builder.Configuration["AppSettings:SecretKey"]; //Lấy giá trị của SecretKey từ cấu hình

            //kiểm tra xem secretKey 
            if (string.IsNullOrEmpty(secretKey))
            {
                throw new InvalidOperationException("SecretKey is not configured.");
            }

            //chuyển đổi một chuỗi secretKey thành một mảng byte
            var secretKeyBytes = Encoding.UTF8.GetBytes(secretKey);

            //AddAuthentication
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    //tự cấp token
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    //ký vào token
                    IssuerSigningKey = new SymmetricSecurityKey(secretKeyBytes),
                    ValidateIssuerSigningKey = true,

                    ClockSkew = TimeSpan.Zero,
                };
            });
            #endregion

            builder.Host.UseSerilog();
            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            //connect database
            builder.Services.AddDbContext<DataContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DbBook")));

            //
            builder.Services.AddScoped<ISachRepository, SachRepository>();
            builder.Services.AddScoped<ILoaiRepository, LoaiRepository>();
            builder.Services.AddScoped<IGioHangRepository, GioHangRepository>();
            builder.Services.AddScoped<IGioHangChiTietRepository, GioHangChiTietRepository>();
            builder.Services.AddScoped<IKhachHangRepository, KhachHangRepository>();
            builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

            // Đăng ký AutoMapper
            builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
