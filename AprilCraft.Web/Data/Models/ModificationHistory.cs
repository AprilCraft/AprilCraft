namespace AprilCraft.Web.Data.Models;

public class ModificationHistory
{
    public int Id { get; set; }
    public int DesignId { get; set; }
    public Design Design { get; set; } = null!;

    public string VersionLabel { get; set; } = string.Empty; // e.g. "v1.0", "v2.1"
    public string? Notes { get; set; }
    public string? ImagePath { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
