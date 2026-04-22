using AprilCraft.Web.Data.Models;
using Microsoft.AspNetCore.Identity;

namespace AprilCraft.Web.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        var db = services.GetRequiredService<AppDbContext>();
        var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        db.Database.EnsureCreated();

        // Seed admin role & user
        if (!await roleManager.RoleExistsAsync("Admin"))
            await roleManager.CreateAsync(new IdentityRole("Admin"));

        const string adminEmail = "admin@aprilcraft.cm";
        if (await userManager.FindByEmailAsync(adminEmail) == null)
        {
            var admin = new IdentityUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };
            var result = await userManager.CreateAsync(admin, "Admin@2025!");
            if (result.Succeeded)
                await userManager.AddToRoleAsync(admin, "Admin");
        }

        // Seed default categories
        if (!db.Categories.Any())
        {
            db.Categories.AddRange(
                new Category { Name = "Flyer", Description = "Event flyers and promotional materials", Icon = "flyer" },
                new Category { Name = "Logo", Description = "Brand logos and identity design", Icon = "logo" },
                new Category { Name = "Banner", Description = "Banners and backdrops", Icon = "banner" },
                new Category { Name = "Business Card", Description = "Business cards and contact cards", Icon = "card" },
                new Category { Name = "Social Media", Description = "Social media graphics and posts", Icon = "social" },
                new Category { Name = "Badge", Description = "Badges and stickers", Icon = "badge" },
                new Category { Name = "Brochure", Description = "Brochures and booklets", Icon = "brochure" },
                new Category { Name = "Poster", Description = "Posters and large format prints", Icon = "poster" }
            );
            await db.SaveChangesAsync();
        }
    }
}
