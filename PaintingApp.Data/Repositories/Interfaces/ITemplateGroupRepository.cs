using PaintingApp.Data.Entities;

namespace PaintingApp.Data.Repositories.Interfaces;

public interface ITemplateGroupRepository
{
    Task<TemplateGroup?> GetByIdAsync(int id);
    Task<IEnumerable<TemplateGroup>> GetAllAsync();
    Task<TemplateGroup> AddAsync(TemplateGroup entity);
    Task UpdateAsync(TemplateGroup entity);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);

    Task<IEnumerable<TemplateGroup>> GetByProfileIdAsync(int profileId);
    Task<TemplateGroup?> GetWithShapesAsync(int id);
    Task<IEnumerable<TemplateGroup>> GetTopUsedAsync(int count = 10);
    Task IncrementUsageCountAsync(int id);
    Task<int> GetShapeCountAsync(int templateGroupId);
}
