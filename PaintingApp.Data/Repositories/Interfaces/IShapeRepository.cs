using PaintingApp.Data.Entities;

namespace PaintingApp.Data.Repositories.Interfaces;

public interface IShapeRepository
{
    Task<Shape?> GetByIdAsync(int id);
    Task<IEnumerable<Shape>> GetAllAsync();
    Task<Shape> AddAsync(Shape entity);
    Task UpdateAsync(Shape entity);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);

    Task<IEnumerable<Shape>> GetByDrawingBoardIdAsync(int boardId);
    Task<IEnumerable<Shape>> GetByTemplateGroupIdAsync(int templateGroupId);
    Task<IEnumerable<Shape>> GetByTypeAsync(ShapeType type);
    Task<Dictionary<ShapeType, int>> GetShapeTypeStatisticsAsync();
    Task DeleteByDrawingBoardIdAsync(int boardId);
    Task DeleteByTemplateGroupIdAsync(int templateGroupId);
}
