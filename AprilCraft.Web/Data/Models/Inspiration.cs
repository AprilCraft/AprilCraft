namespace AprilCraft.Web.Data.Models;

public class Inspiration
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Url { get; set; }
    public string? ImagePath { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<DesignInspiration> DesignInspirations { get; set; } = new();
}
