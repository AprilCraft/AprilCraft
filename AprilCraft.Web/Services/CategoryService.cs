using AprilCraft.Web.Data;
using AprilCraft.Web.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace AprilCraft.Web.Services;

public class CategoryService(AppDbContext db) : ICategoryService
{
    public Task<List<Category>> GetAllAsync() =>
        db.Categories.OrderBy(c => c.Name).ToListAsync();

    public Task<Category?> GetByIdAsync(int id) =>
        db.Categories.FirstOrDefaultAsync(c => c.Id == id);

    public async Task<Category> CreateAsync(string name, string? description, string? icon)
    {
        var cat = new Category { Name = name, Description = description, Icon = icon };
        db.Categories.Add(cat);
        await db.SaveChangesAsync();
        return cat;
    }

    public async Task UpdateAsync(int id, string name, string? description, string? icon)
    {
        var cat = await db.Categories.FindAsync(id) ?? throw new KeyNotFoundException();
        cat.Name = name; cat.Description = description; cat.Icon = icon;
        await db.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var cat = await db.Categories.FindAsync(id);
        if (cat != null) { db.Categories.Remove(cat); await db.SaveChangesAsync(); }
    }
}
