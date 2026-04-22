using AprilCraft.Web.Data.Models;

namespace AprilCraft.Web.Services;

public class DesignDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int CategoryId { get; set; }
    public string? ClientName { get; set; }
    public DateTime DesignDate { get; set; } = DateTime.Today;
    public bool IsFeatured { get; set; }
    public List<string> Tags { get; set; } = new();
    public List<int> InspirationIds { get; set; } = new();
}

public class VariantDto
{
    public string Title { get; set; } = "Default";
    public string? Notes { get; set; }
    public string ImagePath { get; set; } = string.Empty;
    public string ThumbnailPath { get; set; } = string.Empty;
    public bool IsClientSelected { get; set; }
    public int SortOrder { get; set; }
}

public class ResourceDto
{
    public string Name { get; set; } = string.Empty;
    public ResourceType Type { get; set; } = ResourceType.Other;
    public string? Url { get; set; }
    public string? Notes { get; set; }
}

public class FeedbackDto
{
    public string ClientName { get; set; } = string.Empty;
    public string Feedback { get; set; } = string.Empty;
    public int Rating { get; set; } = 5;
    public bool IsPublic { get; set; } = true;
}
