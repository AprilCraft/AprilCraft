using AprilCraft.Web.Data.Models;

namespace AprilCraft.Web.Services;

public interface ICategoryService
{
    Task<List<Category>> GetAllAsync();
    Task<Category?> GetByIdAsync(int id);
    Task<Category> CreateAsync(string name, string? description, string? icon);
    Task UpdateAsync(int id, string name, string? description, string? icon);
    Task DeleteAsync(int id);
}
