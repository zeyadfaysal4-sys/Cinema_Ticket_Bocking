using Cinema_Ticket_Bocking.Data;
using Cinema_Ticket_Bocking.Models;
using Cinema_Ticket_Bocking.Repository;
using Cinema_Ticket_Bocking.Utiltes;
using Cinema_Ticket_Bocking.Utiltes.DbSeeder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Stripe;

namespace Cinema_Ticket_Bocking
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            var connectionString =
           builder.Configuration.GetConnectionString("DefaultConnection")
           ?? throw new InvalidOperationException("Connection string"
           + "'DefaultConnection' not found.");

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });


            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = true;
            }).AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

            builder.Services.AddScoped<IRepository<Category>, Repositories<Category>>();
            builder.Services.AddScoped<IRepository<Movie>, Repositories<Movie>>();
            builder.Services.AddScoped<IRepository<Cinema>, Repositories<Cinema>>();
            builder.Services.AddScoped<IRepository<Actors>, Repositories<Actors>>();
            builder.Services.AddScoped<IRepository<Book>, Repositories<Book>>();
            builder.Services.AddScoped<IRepository<Promotion>, Repositories<Promotion>>();
            builder.Services.AddScoped<IRepository<Cart>, Repositories<Cart>>();
            builder.Services.AddScoped<IRepository<ApplicationUserOtp>, Repositories<ApplicationUserOtp>>();
            builder.Services.AddScoped<IRepository<User>, Repositories<User>>();
            builder.Services.AddTransient<IEmailSender, EmailSender>();
            builder.Services.AddScoped<IDbInitializer, DbInitializer>();


            builder.Services.ConfigureApplicationCookie(option =>
            {

                option.LoginPath = "/Identity/Account/Login";
                option.AccessDeniedPath = "/Identity/Account/AccessDenied";
                option.SlidingExpiration = true;

            });

            StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];


            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var initializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
                await initializer.InitializeAsync();

            }
            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{area=Custmer}/{controller=Home}/{action=Custmers}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
