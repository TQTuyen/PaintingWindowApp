using Microsoft.EntityFrameworkCore;
using PaintingApp.Data.Entities;
using PaintingApp.Data.Repositories.Interfaces;

namespace PaintingApp.Data.Repositories.Implementations;

public class DrawingBoardRepository : IDrawingBoardRepository
{
    private readonly AppDbContext _context;

    public DrawingBoardRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<DrawingBoard?> GetByIdAsync(int id)
    {
        return await _context.DrawingBoards.FindAsync(id);
    }

    public async Task<IEnumerable<DrawingBoard>> GetAllAsync()
    {
        return await _context.DrawingBoards
            .AsNoTracking()
            .OrderByDescending(b => b.LastModified)
            .ToListAsync();
    }

    public async Task<DrawingBoard> AddAsync(DrawingBoard entity)
    {
        var now = DateTime.UtcNow;
        if (entity.CreatedDate == default)
        {
            entity.CreatedDate = now;
        }
        entity.LastModified = now;

        _context.DrawingBoards.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task UpdateAsync(DrawingBoard entity)
    {
        entity.LastModified = DateTime.UtcNow;
        _context.DrawingBoards.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _context.DrawingBoards.FindAsync(id);
        if (entity != null)
        {
            _context.DrawingBoards.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.DrawingBoards.AnyAsync(b => b.Id == id);
    }

    public async Task<IEnumerable<DrawingBoard>> GetByProfileIdAsync(int profileId)
    {
        return await _context.DrawingBoards
            .AsNoTracking()
            .Where(b => b.ProfileId == profileId)
            .OrderByDescending(b => b.LastModified)
            .ToListAsync();
    }

    public async Task<DrawingBoard?> GetWithShapesAsync(int id)
    {
        return await _context.DrawingBoards
            .AsNoTracking()
            .Include(b => b.Shapes.OrderBy(s => s.ZIndex))
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<IEnumerable<DrawingBoard>> GetRecentByProfileAsync(int profileId, int count = 10)
    {
        return await _context.DrawingBoards
            .AsNoTracking()
            .Where(b => b.ProfileId == profileId)
            .OrderByDescending(b => b.LastModified)
            .Take(count)
            .ToListAsync();
    }

    public async Task<int> GetShapeCountAsync(int boardId)
    {
        return await _context.Shapes.CountAsync(s => s.DrawingBoardId == boardId);
    }
}
