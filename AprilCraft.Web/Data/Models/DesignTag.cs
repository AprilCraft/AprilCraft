namespace AprilCraft.Web.Data.Models;

public class DesignTag
{
    public int DesignId { get; set; }
    public Design Design { get; set; } = null!;

    public int TagId { get; set; }
    public Tag Tag { get; set; } = null!;
}
