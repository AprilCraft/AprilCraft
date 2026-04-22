using AprilCraft.Web.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AprilCraft.Web.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext(options)
{
    public DbSet<Design> Designs => Set<Design>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<DesignTag> DesignTags => Set<DesignTag>();
    public DbSet<ClientFeedback> ClientFeedbacks => Set<ClientFeedback>();
    public DbSet<DesignVariant> DesignVariants => Set<DesignVariant>();
    public DbSet<ModificationHistory> ModificationHistories => Set<ModificationHistory>();
    public DbSet<Inspiration> Inspirations => Set<Inspiration>();
    public DbSet<DesignInspiration> DesignInspirations => Set<DesignInspiration>();
    public DbSet<Resource> Resources => Set<Resource>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<DesignTag>()
            .HasKey(dt => new { dt.DesignId, dt.TagId });

        builder.Entity<DesignInspiration>()
            .HasKey(di => new { di.DesignId, di.InspirationId });

        builder.Entity<DesignTag>()
            .HasOne(dt => dt.Design).WithMany(d => d.DesignTags).HasForeignKey(dt => dt.DesignId);
        builder.Entity<DesignTag>()
            .HasOne(dt => dt.Tag).WithMany(t => t.DesignTags).HasForeignKey(dt => dt.TagId);

        builder.Entity<DesignInspiration>()
            .HasOne(di => di.Design).WithMany(d => d.DesignInspirations).HasForeignKey(di => di.DesignId);
        builder.Entity<DesignInspiration>()
            .HasOne(di => di.Inspiration).WithMany(i => i.DesignInspirations).HasForeignKey(di => di.InspirationId);
    }
}
