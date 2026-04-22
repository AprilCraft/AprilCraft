using AprilCraft.Web.Data.Models;

namespace AprilCraft.Web.Services;

public interface IDesignService
{
    Task<List<Design>> GetAllAsync(int? categoryId = null, string? search = null, bool? featured = null);
    Task<Design?> GetByIdAsync(int id);
    Task<Design> CreateAsync(DesignDto dto);
    Task UpdateAsync(int id, DesignDto dto);
    Task DeleteAsync(int id);

    Task AddVariantAsync(int designId, VariantDto dto);
    Task UpdateVariantAsync(int variantId, VariantDto dto);
    Task DeleteVariantAsync(int variantId);
    Task SetSelectedVariantAsync(int designId, int variantId);

    Task AddFeedbackAsync(int designId, FeedbackDto dto);
    Task DeleteFeedbackAsync(int feedbackId);

    Task AddResourceAsync(int designId, ResourceDto dto);
    Task DeleteResourceAsync(int resourceId);

    Task AddInspirationLinkAsync(int designId, int inspirationId);
    Task RemoveInspirationLinkAsync(int designId, int inspirationId);

    Task AddModificationHistoryAsync(int designId, string versionLabel, string? notes, string? imagePath);

    Task<int> GetTotalCountAsync();
    Task<int> GetFeaturedCountAsync();
    Task<List<Design>> GetRecentAsync(int count = 5);
}
