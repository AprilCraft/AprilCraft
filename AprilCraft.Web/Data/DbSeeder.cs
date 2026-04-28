using AprilCraft.Web.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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
        if (!await db.Categories.AnyAsync())
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

        // Seed common tags used in gallery cards and filters.
        if (!await db.Tags.AnyAsync())
        {
            db.Tags.AddRange(
                new Tag { Name = "Modern" },
                new Tag { Name = "Minimal" },
                new Tag { Name = "Corporate" },
                new Tag { Name = "Event" },
                new Tag { Name = "Luxury" },
                new Tag { Name = "Social" },
                new Tag { Name = "Print" },
                new Tag { Name = "Branding" }
            );
            await db.SaveChangesAsync();
        }

        if (!await db.Inspirations.AnyAsync())
        {
            db.Inspirations.AddRange(
                new Inspiration
                {
                    Title = "Swiss Grid Poster Composition",
                    Description = "High-contrast type and strict grid alignment for event promotions.",
                    Url = "https://www.behance.net/",
                    ImagePath = "/assets/images/inspirations/swiss-grid.jpg"
                },
                new Inspiration
                {
                    Title = "Luxury Monogram Systems",
                    Description = "Elegant serif logotypes and restrained gold accents.",
                    Url = "https://dribbble.com/",
                    ImagePath = "/assets/images/inspirations/luxury-monogram.jpg"
                },
                new Inspiration
                {
                    Title = "Social Ad Storyboards",
                    Description = "Mobile-first layouts optimized for social storytelling.",
                    Url = "https://www.pinterest.com/",
                    ImagePath = "/assets/images/inspirations/social-ads.jpg"
                }
            );
            await db.SaveChangesAsync();
        }

        if (!await db.Designs.AnyAsync())
        {
            var categories = await db.Categories.ToDictionaryAsync(c => c.Name, c => c.Id);
            var tags = await db.Tags.ToDictionaryAsync(t => t.Name, t => t.Id);
            var inspirations = await db.Inspirations.ToDictionaryAsync(i => i.Title, i => i.Id);

            var designs = new List<Design>
            {
                new()
                {
                    Title = "Glow Night Concert Flyer",
                    Description = "Bold neon-styled flyer concept for a live music event.",
                    CategoryId = categories["Flyer"],
                    ClientName = "Pulse Arena",
                    DesignDate = new DateTime(2026, 1, 18),
                    IsFeatured = true,
                    Variants =
                    [
                        new DesignVariant
                        {
                            Title = "Primary",
                            ImagePath = "/assets/images/designs/glow-night-flyer-v1.jpg",
                            ThumbnailPath = "/assets/images/designs/thumbs/glow-night-flyer-v1.jpg",
                            IsClientSelected = true,
                            SortOrder = 1
                        },
                        new DesignVariant
                        {
                            Title = "Blue Accent",
                            ImagePath = "/assets/images/designs/glow-night-flyer-v2.jpg",
                            ThumbnailPath = "/assets/images/designs/thumbs/glow-night-flyer-v2.jpg",
                            IsClientSelected = false,
                            SortOrder = 2
                        }
                    ],
                    DesignTags =
                    [
                        new DesignTag { TagId = tags["Event"] },
                        new DesignTag { TagId = tags["Modern"] },
                        new DesignTag { TagId = tags["Print"] }
                    ],
                    ClientFeedbacks =
                    [
                        new ClientFeedback
                        {
                            ClientName = "Ayo, Event Director",
                            Feedback = "The headline hierarchy and color punch looked amazing in print.",
                            Rating = 5,
                            IsPublic = true
                        }
                    ],
                    Resources =
                    [
                        new Resource
                        {
                            Name = "Bebas Neue",
                            Type = ResourceType.Font,
                            Url = "https://fonts.google.com/specimen/Bebas+Neue",
                            Notes = "Main display headline font"
                        }
                    ],
                    ModificationHistories =
                    [
                        new ModificationHistory
                        {
                            VersionLabel = "v1.1",
                            Notes = "Adjusted CTA contrast and venue details.",
                            ImagePath = "/assets/images/designs/history/glow-night-flyer-v11.jpg"
                        }
                    ],
                    DesignInspirations =
                    [
                        new DesignInspiration { InspirationId = inspirations["Swiss Grid Poster Composition"] }
                    ]
                },
                new()
                {
                    Title = "Maison Fleur Monogram",
                    Description = "Refined monogram and wordmark for a boutique floral brand.",
                    CategoryId = categories["Logo"],
                    ClientName = "Maison Fleur",
                    DesignDate = new DateTime(2026, 2, 4),
                    IsFeatured = true,
                    Variants =
                    [
                        new DesignVariant
                        {
                            Title = "Gold Monogram",
                            ImagePath = "/assets/images/designs/maison-fleur-logo-v1.jpg",
                            ThumbnailPath = "/assets/images/designs/thumbs/maison-fleur-logo-v1.jpg",
                            IsClientSelected = true,
                            SortOrder = 1
                        }
                    ],
                    DesignTags =
                    [
                        new DesignTag { TagId = tags["Luxury"] },
                        new DesignTag { TagId = tags["Branding"] },
                        new DesignTag { TagId = tags["Minimal"] }
                    ],
                    Resources =
                    [
                        new Resource
                        {
                            Name = "Cormorant Garamond",
                            Type = ResourceType.Font,
                            Url = "https://fonts.google.com/specimen/Cormorant+Garamond"
                        },
                        new Resource
                        {
                            Name = "Gold Foil Mockup",
                            Type = ResourceType.Asset,
                            Url = "https://www.freepik.com/"
                        }
                    ],
                    DesignInspirations =
                    [
                        new DesignInspiration { InspirationId = inspirations["Luxury Monogram Systems"] }
                    ]
                },
                new()
                {
                    Title = "TechNova Launch Banner",
                    Description = "Web banner campaign visuals for a SaaS feature launch.",
                    CategoryId = categories["Banner"],
                    ClientName = "TechNova",
                    DesignDate = new DateTime(2026, 3, 1),
                    IsFeatured = false,
                    Variants =
                    [
                        new DesignVariant
                        {
                            Title = "Desktop 1920x600",
                            ImagePath = "/assets/images/designs/technova-banner-v1.jpg",
                            ThumbnailPath = "/assets/images/designs/thumbs/technova-banner-v1.jpg",
                            IsClientSelected = true,
                            SortOrder = 1
                        },
                        new DesignVariant
                        {
                            Title = "Tablet 1200x500",
                            ImagePath = "/assets/images/designs/technova-banner-v2.jpg",
                            ThumbnailPath = "/assets/images/designs/thumbs/technova-banner-v2.jpg",
                            IsClientSelected = false,
                            SortOrder = 2
                        }
                    ],
                    DesignTags =
                    [
                        new DesignTag { TagId = tags["Corporate"] },
                        new DesignTag { TagId = tags["Modern"] }
                    ],
                    ClientFeedbacks =
                    [
                        new ClientFeedback
                        {
                            ClientName = "Maya, Marketing Lead",
                            Feedback = "Great clarity across responsive sizes.",
                            Rating = 4,
                            IsPublic = true
                        }
                    ]
                },
                new()
                {
                    Title = "Cafe Aroma Social Carousel",
                    Description = "Instagram carousel templates for monthly offers and stories.",
                    CategoryId = categories["Social Media"],
                    ClientName = "Cafe Aroma",
                    DesignDate = new DateTime(2026, 3, 21),
                    IsFeatured = false,
                    Variants =
                    [
                        new DesignVariant
                        {
                            Title = "Warm Tones",
                            ImagePath = "/assets/images/designs/cafe-aroma-social-v1.jpg",
                            ThumbnailPath = "/assets/images/designs/thumbs/cafe-aroma-social-v1.jpg",
                            IsClientSelected = true,
                            SortOrder = 1
                        }
                    ],
                    DesignTags =
                    [
                        new DesignTag { TagId = tags["Social"] },
                        new DesignTag { TagId = tags["Minimal"] }
                    ],
                    DesignInspirations =
                    [
                        new DesignInspiration { InspirationId = inspirations["Social Ad Storyboards"] }
                    ]
                }
            };

            db.Designs.AddRange(designs);
            await db.SaveChangesAsync();
        }
    }
}
