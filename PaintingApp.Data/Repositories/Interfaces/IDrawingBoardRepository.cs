using PaintingApp.Data.Entities;

namespace PaintingApp.Data.Repositories.Interfaces;

public interface IDrawingBoardRepository
{
    Task<DrawingBoard?> GetByIdAsync(int id);
    Task<IEnumerable<DrawingBoard>> GetAllAsync();
    Task<DrawingBoard> AddAsync(DrawingBoard entity);
    Task UpdateAsync(DrawingBoard entity);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);

    Task<IEnumerable<DrawingBoard>> GetByProfileIdAsync(int profileId);
    Task<DrawingBoard?> GetWithShapesAsync(int id);
    Task<IEnumerable<DrawingBoard>> GetRecentByProfileAsync(int profileId, int count = 10);
    Task<int> GetShapeCountAsync(int boardId);
}
