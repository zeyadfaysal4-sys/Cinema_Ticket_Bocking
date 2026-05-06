using Cinema_Ticket_Bocking.Data;
using Cinema_Ticket_Bocking.Models;
using Cinema_Ticket_Bocking.Repository;
using Cinema_Ticket_Bocking.Utiltes;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;

namespace Cinema_Ticket_Bocking
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            var connectionString =
           builder.Configuration.GetConnectionString("DefaultConnection")
           ?? throw new InvalidOperationException("Connection string"
           + "'DefaultConnection' not found.");

            builder.Services.AddDbContext<ApplicationDbContext>(options=>
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
            builder.Services.AddScoped<IRepository<ApplicationUserOtp>, Repositories<ApplicationUserOtp>>();
            builder.Services.AddScoped<IRepository<User>, Repositories<User>>();
            builder.Services.AddTransient<IEmailSender, EmailSender>();



            var app = builder.Build();

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
                pattern: "{area=Identity}/{controller=Account}/{action=Register}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
