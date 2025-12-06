using PaintingApp.Data.Entities;

namespace PaintingApp.Data.Repositories.Interfaces;

public interface IProfileRepository
{
    Task<Profile?> GetByIdAsync(int id);
    Task<IEnumerable<Profile>> GetAllAsync();
    Task<Profile> AddAsync(Profile entity);
    Task UpdateAsync(Profile entity);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);

    Task<Profile?> GetByNameAsync(string name);
    Task<bool> IsNameUniqueAsync(string name, int? excludeId = null);
    Task<IEnumerable<Profile>> GetProfilesWithBoardsAsync();
    Task<IEnumerable<Profile>> GetProfilesWithTemplatesAsync();
    Task<int> GetBoardCountAsync(int profileId);
    Task<int> GetTemplateCountAsync(int profileId);
}
