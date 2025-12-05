using Microsoft.EntityFrameworkCore;
using PaintingApp.Data.Entities;
using PaintingApp.Data.Repositories.Interfaces;

namespace PaintingApp.Data.Repositories.Implementations;

public class ProfileRepository : IProfileRepository
{
    private readonly AppDbContext _context;

    public ProfileRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Profile?> GetByIdAsync(int id)
    {
        return await _context.Profiles.FindAsync(id);
    }

    public async Task<IEnumerable<Profile>> GetAllAsync()
    {
        return await _context.Profiles
            .AsNoTracking()
            .OrderBy(p => p.Name)
            .ToListAsync();
    }

    public async Task<Profile> AddAsync(Profile entity)
    {
        if (entity.CreatedDate == default)
        {
            entity.CreatedDate = DateTime.UtcNow;
        }

        _context.Profiles.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task UpdateAsync(Profile entity)
    {
        _context.Profiles.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _context.Profiles.FindAsync(id);
        if (entity != null)
        {
            _context.Profiles.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Profiles.AnyAsync(p => p.Id == id);
    }

    public async Task<Profile?> GetByNameAsync(string name)
    {
        return await _context.Profiles
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Name == name);
    }

    // excludeId allows checking uniqueness during updates (excludes current profile from check)
    public async Task<bool> IsNameUniqueAsync(string name, int? excludeId = null)
    {
        return !await _context.Profiles
            .AnyAsync(p => p.Name == name && (!excludeId.HasValue || p.Id != excludeId.Value));
    }

    public async Task<IEnumerable<Profile>> GetProfilesWithBoardsAsync()
    {
        return await _context.Profiles
            .AsNoTracking()
            .Include(p => p.DrawingBoards)
            .OrderBy(p => p.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<Profile>> GetProfilesWithTemplatesAsync()
    {
        return await _context.Profiles
            .AsNoTracking()
            .Include(p => p.TemplateGroups)
            .OrderBy(p => p.Name)
            .ToListAsync();
    }
}
