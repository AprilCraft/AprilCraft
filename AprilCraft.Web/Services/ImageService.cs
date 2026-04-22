using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace AprilCraft.Web.Services;

public interface IImageService
{
    Task<(string imagePath, string thumbPath)> SaveVariantImageAsync(
        Stream stream, string originalFileName, int designId, string variantTitle);
    Task<string> SaveInspirationImageAsync(Stream stream, string originalFileName);
    void DeleteImage(string relativePath);
}

public class ImageService(IWebHostEnvironment env) : IImageService
{
    private readonly string _uploadsRoot = Path.Combine(env.WebRootPath, "uploads");

    public async Task<(string imagePath, string thumbPath)> SaveVariantImageAsync(
        Stream stream, string originalFileName, int designId, string variantTitle)
    {
        var designFolder = $"designs/{designId}";
        var thumbFolder = $"thumbnails/{designId}";

        Directory.CreateDirectory(Path.Combine(_uploadsRoot, designFolder));
        Directory.CreateDirectory(Path.Combine(_uploadsRoot, thumbFolder));

        var ext = Path.GetExtension(originalFileName).ToLowerInvariant();
        var safeName = $"{SanitizeName(variantTitle)}_{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}{ext}";

        var imgPath = Path.Combine(_uploadsRoot, designFolder, safeName);
        var thumbPath = Path.Combine(_uploadsRoot, thumbFolder, safeName);

        using var image = await Image.LoadAsync(stream);
        await image.SaveAsync(imgPath);

        var thumb = image.Clone(x => x.Resize(new ResizeOptions
        {
            Size = new SixLabors.ImageSharp.Size(500, 0),
            Mode = ResizeMode.Max
        }));
        await thumb.SaveAsync(thumbPath);

        return ($"uploads/{designFolder}/{safeName}", $"uploads/{thumbFolder}/{safeName}");
    }

    public async Task<string> SaveInspirationImageAsync(Stream stream, string originalFileName)
    {
        var folder = "uploads/inspirations";
        Directory.CreateDirectory(Path.Combine(_uploadsRoot, "inspirations"));

        var ext = Path.GetExtension(originalFileName).ToLowerInvariant();
        var name = $"insp_{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}{ext}";
        var path = Path.Combine(_uploadsRoot, "inspirations", name);

        using var image = await Image.LoadAsync(stream);
        var resized = image.Clone(x => x.Resize(new ResizeOptions { Size = new SixLabors.ImageSharp.Size(800, 0), Mode = ResizeMode.Max }));
        await resized.SaveAsync(path);

        return $"{folder}/{name}";
    }

    public void DeleteImage(string relativePath)
    {
        if (string.IsNullOrWhiteSpace(relativePath)) return;
        var full = Path.Combine(env.WebRootPath, relativePath.TrimStart('/'));
        if (File.Exists(full)) File.Delete(full);
    }

    private static string SanitizeName(string name) =>
        string.Concat(name.Where(c => char.IsLetterOrDigit(c) || c == '-' || c == '_'))
              .ToLowerInvariant()[..Math.Min(name.Length, 30)];
}
