using AprilCraft.Web.Data;
using AprilCraft.Web.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace AprilCraft.Web.Services;

public class DesignService(AppDbContext db) : IDesignService
{
    public async Task<List<Design>> GetAllAsync(int? categoryId = null, string? search = null, bool? featured = null)
    {
        var q = db.Designs
            .Include(d => d.Category)
            .Include(d => d.Variants)
            .Include(d => d.DesignTags).ThenInclude(dt => dt.Tag)
            .AsQueryable();

        if (categoryId.HasValue)
            q = q.Where(d => d.CategoryId == categoryId.Value);

        if (!string.IsNullOrWhiteSpace(search))
            q = q.Where(d => d.Title.Contains(search) || (d.ClientName != null && d.ClientName.Contains(search)));

        if (featured.HasValue)
            q = q.Where(d => d.IsFeatured == featured.Value);

        return await q.OrderByDescending(d => d.DesignDate).ToListAsync();
    }

    public async Task<Design?> GetByIdAsync(int id) =>
        await db.Designs
            .Include(d => d.Category)
            .Include(d => d.Variants.OrderBy(v => v.SortOrder))
            .Include(d => d.DesignTags).ThenInclude(dt => dt.Tag)
            .Include(d => d.ClientFeedbacks)
            .Include(d => d.ModificationHistories.OrderByDescending(m => m.CreatedAt))
            .Include(d => d.DesignInspirations).ThenInclude(di => di.Inspiration)
            .Include(d => d.Resources)
            .FirstOrDefaultAsync(d => d.Id == id);

    public async Task<Design> CreateAsync(DesignDto dto)
    {
        var design = new Design
        {
            Title = dto.Title,
            Description = dto.Description,
            CategoryId = dto.CategoryId,
            ClientName = dto.ClientName,
            DesignDate = dto.DesignDate,
            IsFeatured = dto.IsFeatured,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        db.Designs.Add(design);
        await db.SaveChangesAsync();

        await SyncTagsAsync(design.Id, dto.Tags);
        await SyncInspirationsAsync(design.Id, dto.InspirationIds);

        return design;
    }

    public async Task UpdateAsync(int id, DesignDto dto)
    {
        var design = await db.Designs.FindAsync(id) ?? throw new KeyNotFoundException();
        design.Title = dto.Title;
        design.Description = dto.Description;
        design.CategoryId = dto.CategoryId;
        design.ClientName = dto.ClientName;
        design.DesignDate = dto.DesignDate;
        design.IsFeatured = dto.IsFeatured;
        design.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();

        await SyncTagsAsync(id, dto.Tags);
        await SyncInspirationsAsync(id, dto.InspirationIds);
    }

    public async Task DeleteAsync(int id)
    {
        var design = await db.Designs.FindAsync(id);
        if (design != null) { db.Designs.Remove(design); await db.SaveChangesAsync(); }
    }

    public async Task AddVariantAsync(int designId, VariantDto dto)
    {
        db.DesignVariants.Add(new DesignVariant
        {
            DesignId = designId,
            Title = dto.Title,
            Notes = dto.Notes,
            ImagePath = dto.ImagePath,
            ThumbnailPath = dto.ThumbnailPath,
            IsClientSelected = dto.IsClientSelected,
            SortOrder = dto.SortOrder
        });
        await db.SaveChangesAsync();
    }

    public async Task UpdateVariantAsync(int variantId, VariantDto dto)
    {
        var v = await db.DesignVariants.FindAsync(variantId) ?? throw new KeyNotFoundException();
        v.Title = dto.Title;
        v.Notes = dto.Notes;
        v.IsClientSelected = dto.IsClientSelected;
        v.SortOrder = dto.SortOrder;
        await db.SaveChangesAsync();
    }

    public async Task DeleteVariantAsync(int variantId)
    {
        var v = await db.DesignVariants.FindAsync(variantId);
        if (v != null) { db.DesignVariants.Remove(v); await db.SaveChangesAsync(); }
    }

    public async Task SetSelectedVariantAsync(int designId, int variantId)
    {
        var variants = await db.DesignVariants.Where(v => v.DesignId == designId).ToListAsync();
        foreach (var v in variants) v.IsClientSelected = v.Id == variantId;
        await db.SaveChangesAsync();
    }

    public async Task AddFeedbackAsync(int designId, FeedbackDto dto)
    {
        db.ClientFeedbacks.Add(new ClientFeedback
        {
            DesignId = designId,
            ClientName = dto.ClientName,
            Feedback = dto.Feedback,
            Rating = dto.Rating,
            IsPublic = dto.IsPublic
        });
        await db.SaveChangesAsync();
    }

    public async Task DeleteFeedbackAsync(int feedbackId)
    {
        var f = await db.ClientFeedbacks.FindAsync(feedbackId);
        if (f != null) { db.ClientFeedbacks.Remove(f); await db.SaveChangesAsync(); }
    }

    public async Task AddResourceAsync(int designId, ResourceDto dto)
    {
        db.Resources.Add(new Resource
        {
            DesignId = designId,
            Name = dto.Name,
            Type = dto.Type,
            Url = dto.Url,
            Notes = dto.Notes
        });
        await db.SaveChangesAsync();
    }

    public async Task DeleteResourceAsync(int resourceId)
    {
        var r = await db.Resources.FindAsync(resourceId);
        if (r != null) { db.Resources.Remove(r); await db.SaveChangesAsync(); }
    }

    public async Task AddInspirationLinkAsync(int designId, int inspirationId)
    {
        if (!db.DesignInspirations.Any(di => di.DesignId == designId && di.InspirationId == inspirationId))
        {
            db.DesignInspirations.Add(new DesignInspiration { DesignId = designId, InspirationId = inspirationId });
            await db.SaveChangesAsync();
        }
    }

    public async Task RemoveInspirationLinkAsync(int designId, int inspirationId)
    {
        var di = await db.DesignInspirations.FindAsync(designId, inspirationId);
        if (di != null) { db.DesignInspirations.Remove(di); await db.SaveChangesAsync(); }
    }

    public async Task AddModificationHistoryAsync(int designId, string versionLabel, string? notes, string? imagePath)
    {
        db.ModificationHistories.Add(new ModificationHistory
        {
            DesignId = designId,
            VersionLabel = versionLabel,
            Notes = notes,
            ImagePath = imagePath
        });
        await db.SaveChangesAsync();
    }

    public Task<int> GetTotalCountAsync() => db.Designs.CountAsync();
    public Task<int> GetFeaturedCountAsync() => db.Designs.CountAsync(d => d.IsFeatured);

    public async Task<List<Design>> GetRecentAsync(int count = 5) =>
        await db.Designs
            .Include(d => d.Category)
            .Include(d => d.Variants)
            .OrderByDescending(d => d.CreatedAt)
            .Take(count).ToListAsync();

    private async Task SyncTagsAsync(int designId, List<string> tagNames)
    {
        var existing = await db.DesignTags.Where(dt => dt.DesignId == designId).ToListAsync();
        db.DesignTags.RemoveRange(existing);
        await db.SaveChangesAsync();

        foreach (var name in tagNames.Where(t => !string.IsNullOrWhiteSpace(t)).Select(t => t.Trim()).Distinct())
        {
            var tag = await db.Tags.FirstOrDefaultAsync(t => t.Name == name)
                      ?? db.Tags.Add(new Tag { Name = name }).Entity;
            await db.SaveChangesAsync();
            db.DesignTags.Add(new DesignTag { DesignId = designId, TagId = tag.Id });
        }
        await db.SaveChangesAsync();
    }

    private async Task SyncInspirationsAsync(int designId, List<int> inspirationIds)
    {
        var existing = await db.DesignInspirations.Where(di => di.DesignId == designId).ToListAsync();
        db.DesignInspirations.RemoveRange(existing);
        await db.SaveChangesAsync();

        foreach (var iId in inspirationIds.Distinct())
            db.DesignInspirations.Add(new DesignInspiration { DesignId = designId, InspirationId = iId });
        await db.SaveChangesAsync();
    }
}
