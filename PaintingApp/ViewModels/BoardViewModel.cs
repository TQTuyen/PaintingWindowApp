using CommunityToolkit.Mvvm.ComponentModel;
using PaintingApp.Data.Entities;
using System;

namespace PaintingApp.ViewModels;

public partial class BoardViewModel : ObservableObject
{
    [ObservableProperty]
    private int _id;

    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private int _width;

    [ObservableProperty]
    private int _height;

    [ObservableProperty]
    private string _backgroundColor = "#FFFFFF";

    [ObservableProperty]
    private int _shapeCount;

    [ObservableProperty]
    private DateTime _createdDate;

    [ObservableProperty]
    private DateTime _lastModified;

    public string SizeDisplay => $"{Width} × {Height}";

    public static BoardViewModel FromEntity(DrawingBoard board, int shapeCount)
    {
        return new BoardViewModel
        {
            Id = board.Id,
            Name = board.Name,
            Width = board.Width,
            Height = board.Height,
            BackgroundColor = board.BackgroundColor,
            ShapeCount = shapeCount,
            CreatedDate = board.CreatedDate,
            LastModified = board.LastModified
        };
    }
}
