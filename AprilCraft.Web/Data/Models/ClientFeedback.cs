namespace AprilCraft.Web.Data.Models;

public class ClientFeedback
{
    public int Id { get; set; }
    public int DesignId { get; set; }
    public Design Design { get; set; } = null!;

    public string ClientName { get; set; } = string.Empty;
    public string Feedback { get; set; } = string.Empty;
    public int Rating { get; set; } = 5; // 1–5
    public bool IsPublic { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
