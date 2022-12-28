using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IdentityProvider.Data;

public static class IdentityExtensions
{
    public static void AddIdentity(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(opt =>
            opt.UseSqlServer(
                configuration.GetConnectionString("IdentityDb")
            ));
        services.AddIdentity<IdentityUser, IdentityRole>(opt => {
                opt.SignIn.RequireConfirmedAccount = false;
                opt.Password.RequireDigit = true;
                opt.Password.RequireLowercase = true;
                opt.Password.RequireNonAlphanumeric = true;
                opt.Password.RequireUppercase = true;
                opt.Password.RequiredLength = 6;
                opt.Password.RequiredUniqueChars = 1;
            }).AddEntityFrameworkStores<ApplicationDbContext>();
    }

    public static void ApplyMigrations(this IApplicationBuilder app)
    {
        Console.WriteLine("--> Attempting to apply migrations...");

        using var scope = app.ApplicationServices.CreateScope();

        var context = scope.ServiceProvider.GetService<ApplicationDbContext>();
        try
        {
            context?.Database.Migrate();
        }
        catch(Exception ex)
        {
            Console.WriteLine($"--> Could not run migrations. Reason: {ex.Message}");
        }
    }
}