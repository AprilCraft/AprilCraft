namespace AprilCraft.Web.Data.Models;

public class DesignVariant
{
    public int Id { get; set; }
    public int DesignId { get; set; }
    public Design Design { get; set; } = null!;

    public string Title { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public string ImagePath { get; set; } = string.Empty;
    public string ThumbnailPath { get; set; } = string.Empty;
    public bool IsClientSelected { get; set; }
    public int SortOrder { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
