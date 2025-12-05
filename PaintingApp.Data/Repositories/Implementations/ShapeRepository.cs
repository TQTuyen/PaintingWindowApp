using Microsoft.EntityFrameworkCore;
using PaintingApp.Data.Entities;
using PaintingApp.Data.Repositories.Interfaces;

namespace PaintingApp.Data.Repositories.Implementations;

public class ShapeRepository : IShapeRepository
{
    private readonly AppDbContext _context;

    public ShapeRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Shape?> GetByIdAsync(int id)
    {
        return await _context.Shapes.FindAsync(id);
    }

    public async Task<IEnumerable<Shape>> GetAllAsync()
    {
        return await _context.Shapes
            .AsNoTracking()
            .OrderBy(s => s.ZIndex)
            .ToListAsync();
    }

    public async Task<Shape> AddAsync(Shape entity)
    {
        if (entity.CreatedDate == default)
        {
            entity.CreatedDate = DateTime.UtcNow;
        }

        _context.Shapes.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task UpdateAsync(Shape entity)
    {
        _context.Shapes.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _context.Shapes.FindAsync(id);
        if (entity != null)
        {
            _context.Shapes.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Shapes.AnyAsync(s => s.Id == id);
    }

    public async Task<IEnumerable<Shape>> GetByDrawingBoardIdAsync(int boardId)
    {
        return await _context.Shapes
            .AsNoTracking()
            .Where(s => s.DrawingBoardId == boardId)
            .OrderBy(s => s.ZIndex)
            .ToListAsync();
    }

    public async Task<IEnumerable<Shape>> GetByTemplateGroupIdAsync(int templateGroupId)
    {
        return await _context.Shapes
            .AsNoTracking()
            .Where(s => s.TemplateGroupId == templateGroupId)
            .OrderBy(s => s.ZIndex)
            .ToListAsync();
    }

    public async Task<IEnumerable<Shape>> GetByTypeAsync(ShapeType type)
    {
        return await _context.Shapes
            .AsNoTracking()
            .Where(s => s.Type == type)
            .OrderBy(s => s.ZIndex)
            .ToListAsync();
    }

    // Groups shapes by type and returns count for each type (used for dashboard statistics)
    public async Task<Dictionary<ShapeType, int>> GetShapeTypeStatisticsAsync()
    {
        return await _context.Shapes
            .AsNoTracking()
            .GroupBy(s => s.Type)
            .Select(g => new { Type = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Type, x => x.Count);
    }

    public async Task DeleteByDrawingBoardIdAsync(int boardId)
    {
        var shapes = await _context.Shapes
            .Where(s => s.DrawingBoardId == boardId)
            .ToListAsync();

        if (shapes.Count > 0)
        {
            _context.Shapes.RemoveRange(shapes);
            await _context.SaveChangesAsync();
        }
    }

    public async Task DeleteByTemplateGroupIdAsync(int templateGroupId)
    {
        var shapes = await _context.Shapes
            .Where(s => s.TemplateGroupId == templateGroupId)
            .ToListAsync();

        if (shapes.Count > 0)
        {
            _context.Shapes.RemoveRange(shapes);
            await _context.SaveChangesAsync();
        }
    }
}
