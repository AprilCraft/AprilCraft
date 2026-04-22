namespace AprilCraft.Web.Data.Models;

public enum ResourceType
{
    Font,
    Icon,
    Asset,
    Image,
    Other
}

public class Resource
{
    public int Id { get; set; }
    public int DesignId { get; set; }
    public Design Design { get; set; } = null!;

    public string Name { get; set; } = string.Empty;
    public ResourceType Type { get; set; } = ResourceType.Other;
    public string? Url { get; set; }
    public string? FilePath { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
