using AprilCraft.Web.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

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

        await SeedPortfolioDesignsAsync(db);
    }

    private static async Task SeedPortfolioDesignsAsync(AppDbContext db)
    {
        var contentRoot = Directory.GetCurrentDirectory();
        var designsDirectory = Path.Combine(contentRoot, "wwwroot", "assets", "images", "designs");
        var thumbnailsDirectory = Path.Combine(contentRoot, "wwwroot", "assets", "images", "thumbnails");

        if (!Directory.Exists(designsDirectory))
            return;

        var validExts = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { ".jpg", ".jpeg", ".png", ".webp" };
        var categoryByName = await db.Categories.ToDictionaryAsync(c => c.Name, StringComparer.OrdinalIgnoreCase);
        var existingPaths = new HashSet<string>(
            await db.DesignVariants.Select(v => v.ImagePath).ToListAsync(),
            StringComparer.OrdinalIgnoreCase);

        var files = Directory
            .EnumerateFiles(designsDirectory)
            .Where(path => validExts.Contains(Path.GetExtension(path)))
            .OrderBy(Path.GetFileName, StringComparer.OrdinalIgnoreCase)
            .ToList();

        var groupedDesigns = files
            .GroupBy(file => BuildDesignKey(Path.GetFileNameWithoutExtension(file)))
            .OrderBy(group => group.Key, StringComparer.OrdinalIgnoreCase)
            .ToList();

        var now = DateTime.UtcNow;
        var added = 0;

        foreach (var group in groupedDesigns)
        {
            var designFiles = group
                .OrderBy(file => Path.GetFileName(file), StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (designFiles.Count == 0)
                continue;

            var representativeFile = designFiles[0];
            var representativeName = Path.GetFileName(representativeFile);
            var title = BuildTitleFromFilename(representativeName);
            var categoryName = InferCategoryName(representativeName, title);

            if (!categoryByName.TryGetValue(categoryName, out var category))
                category = categoryByName["Social Media"];

            var design = new Design
            {
                Title = title,
                Description = $"Portfolio archive import ({category.Name})",
                CategoryId = category.Id,
                ClientName = "AprilCraft",
                DesignDate = now,
                IsFeatured = designFiles.Any(file => ShouldFeature(Path.GetFileName(file), BuildTitleFromFilename(Path.GetFileName(file)))),
                CreatedAt = now,
                UpdatedAt = now
            };

            var variantOrder = 0;
            foreach (var file in designFiles)
            {
                var fileName = Path.GetFileName(file);
                if (string.IsNullOrWhiteSpace(fileName))
                    continue;

                var imagePath = $"/assets/images/designs/{fileName}";
                if (existingPaths.Contains(imagePath))
                    continue;

                var thumbnailPath = imagePath;
                var thumbSource = Path.Combine(thumbnailsDirectory, fileName);
                if (File.Exists(thumbSource))
                    thumbnailPath = $"/assets/images/thumbnails/{fileName}";

                design.Variants.Add(new DesignVariant
                {
                    Title = BuildVariantTitle(fileName),
                    Notes = "Seeded from project portfolio archive.",
                    ImagePath = imagePath,
                    ThumbnailPath = thumbnailPath,
                    IsClientSelected = variantOrder == 0,
                    SortOrder = variantOrder,
                    CreatedAt = now
                });

                existingPaths.Add(imagePath);
                variantOrder++;
            }

            if (design.Variants.Count == 0)
                continue;

            db.Designs.Add(design);
            added++;
        }

        if (added > 0)
            await db.SaveChangesAsync();
    }

    private static string BuildTitleFromFilename(string fileName)
    {
        var raw = Path.GetFileNameWithoutExtension(fileName);
        if (string.IsNullOrWhiteSpace(raw))
            return "Untitled Design";

        var cleaned = raw
            .Replace("_", " ")
            .Replace("-", " ")
            .Replace("(compressed)", "", StringComparison.OrdinalIgnoreCase)
            .Replace("(FILEminimizer)", "", StringComparison.OrdinalIgnoreCase)
            .Replace("preview", "", StringComparison.OrdinalIgnoreCase)
            .Replace("main flyer", "", StringComparison.OrdinalIgnoreCase)
            .Replace("poster", "", StringComparison.OrdinalIgnoreCase)
            .Replace("social", "", StringComparison.OrdinalIgnoreCase)
            .Replace("share", "", StringComparison.OrdinalIgnoreCase)
            .Replace("print", "", StringComparison.OrdinalIgnoreCase)
            .Replace("mockup", "", StringComparison.OrdinalIgnoreCase);

        cleaned = Regex.Replace(cleaned, "\\s+", " ").Trim();
        if (string.IsNullOrWhiteSpace(cleaned))
            return "Untitled Design";

        return cleaned;
    }

    private static string BuildVariantTitle(string fileName)
    {
        var variant = BuildTitleFromFilename(fileName);
        if (string.IsNullOrWhiteSpace(variant) || variant.Equals("Untitled Design", StringComparison.OrdinalIgnoreCase))
            return "Primary Version";

        return char.ToUpperInvariant(variant[0]) + variant[1..];
    }

    private static string BuildDesignKey(string rawName)
    {
        if (string.IsNullOrWhiteSpace(rawName))
            return "untitled";

        var value = rawName
            .Replace("(", " ", StringComparison.OrdinalIgnoreCase)
            .Replace(")", " ", StringComparison.OrdinalIgnoreCase)
            .Replace("_", " ")
            .Replace("-", " ");

        value = Regex.Replace(value, "(?i)\\b(preview|mockup|main flyer|poster|social|share|print|version|variant|v\\d+)\\b", " ");
        value = Regex.Replace(value, "[^a-zA-Z0-9 ]+", " ");
        value = Regex.Replace(value, "\\s+", " ").Trim();

        return value.ToLowerInvariant();
    }

    private static string InferCategoryName(string fileName, string title)
    {
        var value = $"{fileName} {title}".ToLowerInvariant();

        if (value.Contains("logo") || value.Contains("badge")) return "Logo";
        if (value.Contains("business card") || value.Contains("card")) return "Business Card";
        if (value.Contains("roll up") || value.Contains("banner")) return "Banner";
        if (value.Contains("book cover") || value.Contains("brochure") || value.Contains("menu card")) return "Brochure";
        if (value.Contains("certificate") || value.Contains("poster")) return "Poster";
        if (value.Contains("sticker")) return "Badge";

        return "Flyer";
    }

    private static bool ShouldFeature(string fileName, string title)
    {
        var value = $"{fileName} {title}".ToLowerInvariant();
        return value.Contains("wedding")
            || value.Contains("conference")
            || value.Contains("grand")
            || value.Contains("seminar")
            || value.Contains("logo")
            || value.Contains("book cover");
    }
}
