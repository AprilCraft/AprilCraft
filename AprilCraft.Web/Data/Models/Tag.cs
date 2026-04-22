namespace AprilCraft.Web.Data.Models;

public class Tag
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public List<DesignTag> DesignTags { get; set; } = new();
}
