﻿
using BookAPI.Data;
using BookAPI.Database;
using BookAPI.Helper;
using BookAPI.Models;
using BookAPI.Repositories;
using BookAPI.Repositories.Interfaces;
using BookAPI.Repositories.UnitOfWork;
using BookAPI.Seeding;
using BookAPI.Services;
using BookAPI.Services.Interfaces;
using Common;
using EcommerceWeb.Services;
using Microsoft.AspNetCore.Authentication.Google;
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
using StackExchange.Redis;
using System.Text;


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

builder.Services.AddRepository().AddService();
//CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder => builder.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader());
});

builder.Services.Configure<AppSetting>(builder.Configuration.GetSection("JWT"));

#region setup cache
var cacheDurationInHours = builder.Configuration.GetValue<int>("CacheSettings:CacheDurationInHours");
var cacheSlidingExpirationInMinutes = builder.Configuration.GetValue<int>("CacheSettings:SlidingExpirationInMinutes");
var cacheDuration = TimeSpan.FromHours(cacheDurationInHours);
var cacheSlidingExpiration = TimeSpan.FromMinutes(cacheSlidingExpirationInMinutes);
builder.Services.AddSingleton(new CacheSetting
{
    Duration = cacheDuration,
    SlidingExpiration = cacheSlidingExpiration
});
builder.Services.AddMemoryCache(options =>
{
    options.SizeLimit = 10240; // Giới hạn tổng kích thước bộ nhớ cache (10 MB)
});
#endregion

#region redis custom
var redisConfiguration = new RedisConfiguration();
builder.Configuration.GetSection("RedisConfiguration").Bind(redisConfiguration);
if (string.IsNullOrEmpty(redisConfiguration.ConnectionString) || !redisConfiguration.Enabled)
{
    return;
}
builder.Services.AddSingleton(redisConfiguration);
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    return ConnectionMultiplexer.Connect(redisConfiguration.ConnectionString);
});
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = redisConfiguration.ConnectionString;
});
builder.Services.AddSingleton<IResponseCacheService, ResponseCacheService>();
builder.Services.Configure<RedisConfiguration>(builder.Configuration.GetSection("RedisConfiguration"));

#endregion

#region Identity
//Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                            .AddEntityFrameworkStores<DataContext>().AddDefaultTokenProviders();

builder.Services.Configure<GoogleAuthSettings>(builder.Configuration.GetSection("Authentication:Google"));
builder.Services.Configure<FacebookAuthSettings>(builder.Configuration.GetSection("Authentication:Facebook"));
//authentication - Token 
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    

}).AddJwtBearer(options =>
{
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
}).AddGoogle(GoogleDefaults.AuthenticationScheme, googleOptions =>
{
    googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"];
    googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
    googleOptions.CallbackPath = new PathString(builder.Configuration["Authentication:Google:CallbackPath"]);// "/signin-google";

})
.AddFacebook(facebookOptions =>
{
    facebookOptions.AppId = builder.Configuration["Authentication:Facebook:ClientId"];
    facebookOptions.AppSecret = builder.Configuration["Authentication:Facebook:ClientSecret"];
    facebookOptions.CallbackPath = new PathString(builder.Configuration["Authentication:Facebook:CallbackPath"]);
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
app.UseCors("AllowAllOrigins"); 
app.UseHttpsRedirection(); 
app.UseMiddleware<TokenValidationMiddleware>(); 
app.UseMiddleware<ErrorHandleMiddleware>(); 
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

