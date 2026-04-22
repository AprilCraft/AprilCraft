namespace AprilCraft.Web.Data.Models;

public class Design
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }

    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    public string? ClientName { get; set; }
    public DateTime DesignDate { get; set; } = DateTime.UtcNow;
    public bool IsFeatured { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public List<DesignTag> DesignTags { get; set; } = new();
    public List<ClientFeedback> ClientFeedbacks { get; set; } = new();
    public List<DesignVariant> Variants { get; set; } = new();
    public List<ModificationHistory> ModificationHistories { get; set; } = new();
    public List<DesignInspiration> DesignInspirations { get; set; } = new();
    public List<Resource> Resources { get; set; } = new();

    // Computed helpers
    public DesignVariant? PrimaryVariant =>
        Variants.FirstOrDefault(v => v.IsClientSelected) ?? Variants.OrderBy(v => v.SortOrder).FirstOrDefault();
}
