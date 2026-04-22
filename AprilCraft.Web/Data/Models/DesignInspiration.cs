namespace AprilCraft.Web.Data.Models;

public class DesignInspiration
{
    public int DesignId { get; set; }
    public Design Design { get; set; } = null!;

    public int InspirationId { get; set; }
    public Inspiration Inspiration { get; set; } = null!;
}
