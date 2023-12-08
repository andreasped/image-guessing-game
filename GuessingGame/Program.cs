using System;
using System.Globalization;
using System.IO;
using System.Linq;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using GuessingGame.SharedKernel;
using GuessingGame.Core.Domain.Game.Services;
using GuessingGame.Core.Domain.User;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using GuessingGame.Core.Domain.Game;
using Microsoft.AspNetCore.Identity;
using GuessingGame.Core.Domain.Result.Events;
using GuessingGame.Core.Domain.Result.Handlers;
using Microsoft.AspNetCore.Authentication.Cookies;



var builder = WebApplication.CreateBuilder();

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession();

builder.Services.AddSignalR();

builder.Services.AddDbContext<GameContext>(options =>
{
    options.UseSqlite($"Data Source={Path.Combine("Infrastructure", "Data", "game.db")}");
});

builder.Logging.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Warning); // stop database commands from logging to terminal


builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddScoped<IOracleService, RandomOracleService>();
builder.Services.AddScoped<IImageService, ImageService>();

builder.Services.AddIdentity<UserIdentity, IdentityRole>()
    .AddEntityFrameworkStores<GameContext>()
    .AddDefaultTokenProviders()
    .AddUserManager<UserManager<UserIdentity>>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.Name = "Cookie";
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    options.LoginPath = "/Index";
    options.Cookie.SameSite = SameSiteMode.None;
    options.ReturnUrlParameter = CookieAuthenticationDefaults.ReturnUrlParameter;
    options.SlidingExpiration = true;
});

builder.Services.AddMediatR(typeof(Program));

builder.Services.AddRazorPages();

builder.Logging.AddConsole();

builder.Services.AddHttpContextAccessor();

builder.Logging.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Warning); // stop database commands from logging to terminal


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    try
    {
        if (app.Environment.IsDevelopment())
        {
            var db = scope.ServiceProvider.GetRequiredService<GameContext>();
            if (!db.Images.Any()) // Add images to database
            {
                ImageData.Init();
                db.Images.AddRange(ImageData.Images);
                db.SaveChanges();
            }
        }
        else
        {
            // Handle the non-development scenario
            Console.WriteLine("The application is not in development mode. Skipping database initialization.");
        }
    }
    catch (InvalidOperationException ex)
    {
        // Handle exceptions related to service retrieval
        Console.WriteLine($"Service retrieval failed: {ex.Message}");
    }
    catch (IOException ex)
    {
        // Handle file-related exceptions
        Console.WriteLine($"File operation failed: {ex.Message}");
    }
    catch (DbUpdateException ex)
    {
        // Handle database update exceptions
        Console.WriteLine($"Database update failed: {ex.Message}");
    }
    catch (Exception ex)
    {
        // Handle any other exceptions that were not previously caught
        Console.WriteLine($"An unexpected error occurred: {ex.Message}");
    }
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.UseSession();

app.MapRazorPages();

app.MapHub<GuessingHub>("/guessingHub");

app.Run();

public partial class Program { }