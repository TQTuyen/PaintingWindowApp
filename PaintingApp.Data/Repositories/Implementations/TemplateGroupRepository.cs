using Microsoft.EntityFrameworkCore;
using PaintingApp.Data.Entities;
using PaintingApp.Data.Repositories.Interfaces;

namespace PaintingApp.Data.Repositories.Implementations;

public class TemplateGroupRepository : ITemplateGroupRepository
{
    private readonly AppDbContext _context;

    public TemplateGroupRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<TemplateGroup?> GetByIdAsync(int id)
    {
        return await _context.TemplateGroups.FindAsync(id);
    }

    public async Task<IEnumerable<TemplateGroup>> GetAllAsync()
    {
        return await _context.TemplateGroups
            .AsNoTracking()
            .OrderBy(t => t.Name)
            .ToListAsync();
    }

    public async Task<TemplateGroup> AddAsync(TemplateGroup entity)
    {
        if (entity.CreatedDate == default)
        {
            entity.CreatedDate = DateTime.UtcNow;
        }

        _context.TemplateGroups.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task UpdateAsync(TemplateGroup entity)
    {
        _context.TemplateGroups.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _context.TemplateGroups.FindAsync(id);
        if (entity != null)
        {
            _context.TemplateGroups.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.TemplateGroups.AnyAsync(t => t.Id == id);
    }

    public async Task<IEnumerable<TemplateGroup>> GetByProfileIdAsync(int profileId)
    {
        return await _context.TemplateGroups
            .AsNoTracking()
            .Where(t => t.ProfileId == profileId)
            .OrderBy(t => t.Name)
            .ToListAsync();
    }

    public async Task<TemplateGroup?> GetWithShapesAsync(int id)
    {
        return await _context.TemplateGroups
            .AsNoTracking()
            .Include(t => t.Shapes.OrderBy(s => s.ZIndex))
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<IEnumerable<TemplateGroup>> GetTopUsedAsync(int count = 10)
    {
        return await _context.TemplateGroups
            .AsNoTracking()
            .OrderByDescending(t => t.UsageCount)
            .Take(count)
            .ToListAsync();
    }

    public async Task IncrementUsageCountAsync(int id)
    {
        await _context.TemplateGroups
            .Where(t => t.Id == id)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(t => t.UsageCount, t => t.UsageCount + 1));
    }

    public async Task<int> GetShapeCountAsync(int templateGroupId)
    {
        return await _context.Shapes.CountAsync(s => s.TemplateGroupId == templateGroupId);
    }
}
