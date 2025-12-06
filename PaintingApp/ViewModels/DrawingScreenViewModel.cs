using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI;
using PaintingApp.Contracts;
using PaintingApp.Data.Entities;
using PaintingApp.Data.Repositories.Interfaces;
using Windows.UI;

namespace PaintingApp.ViewModels;

public partial class DrawingScreenViewModel : BaseViewModel
{
    private readonly IProfileStateService _profileStateService;
    private readonly IDrawingBoardRepository _drawingBoardRepository;
    private readonly IShapeRepository _shapeRepository;

    [ObservableProperty]
    private Profile? _currentProfile;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveBoardCommand))]
    private DrawingBoard? _currentBoard;

    [ObservableProperty]
    private ObservableCollection<Shape> _shapes = new();

    [ObservableProperty]
    private double _canvasWidth = 800;

    [ObservableProperty]
    private double _canvasHeight = 600;

    [ObservableProperty]
    private string _selectedTool = "Select";

    [ObservableProperty]
    private Color _strokeColor = Colors.Black;

    [ObservableProperty]
    private double _strokeThickness = 2.0;

    [ObservableProperty]
    private Color _fillColor = Colors.Transparent;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveBoardCommand))]
    private bool _hasUnsavedChanges;

    [ObservableProperty]
    private string _boardName = "Untitled";

    public bool CanSave => CurrentBoard != null && CurrentProfile != null;

    public DrawingScreenViewModel(
        INavigationService navigationService,
        IDialogService dialogService,
        IProfileStateService profileStateService,
        IDrawingBoardRepository drawingBoardRepository,
        IShapeRepository shapeRepository)
        : base(navigationService, dialogService)
    {
        _profileStateService = profileStateService;
        _drawingBoardRepository = drawingBoardRepository;
        _shapeRepository = shapeRepository;

        Title = "Drawing";
    }

    public override Task InitializeAsync()
    {
        LoadProfileSettings();
        CreateInitialBoard();
        return Task.CompletedTask;
    }

    private void LoadProfileSettings()
    {
        CurrentProfile = _profileStateService.CurrentProfile;

        if (CurrentProfile != null)
        {
            CanvasWidth = CurrentProfile.DefaultCanvasWidth;
            CanvasHeight = CurrentProfile.DefaultCanvasHeight;
            StrokeThickness = CurrentProfile.DefaultStrokeThickness;
            StrokeColor = ParseColor(CurrentProfile.DefaultStrokeColor);
        }
    }

    private void CreateInitialBoard()
    {
        if (CurrentProfile == null) return;

        CurrentBoard = new DrawingBoard
        {
            Name = $"Board {DateTime.Now:yyyy-MM-dd HH:mm}",
            ProfileId = CurrentProfile.Id,
            Width = (int)CanvasWidth,
            Height = (int)CanvasHeight,
            CreatedDate = DateTime.UtcNow,
            LastModified = DateTime.UtcNow
        };

        BoardName = CurrentBoard.Name;
        HasUnsavedChanges = false;
    }

    [RelayCommand]
    private async Task NewBoardAsync()
    {
        if (HasUnsavedChanges)
        {
            var saveConfirm = await DialogService.ShowConfirmationAsync(
                "Unsaved Changes",
                "You have unsaved changes. Do you want to save the current board?"
            );

            if (saveConfirm)
            {
                await SaveBoardAsync();
            }
        }

        if (CurrentProfile == null) return;

        CurrentBoard = new DrawingBoard
        {
            Name = $"Board {DateTime.Now:yyyy-MM-dd HH:mm}",
            ProfileId = CurrentProfile.Id,
            Width = (int)CanvasWidth,
            Height = (int)CanvasHeight,
            CreatedDate = DateTime.UtcNow,
            LastModified = DateTime.UtcNow
        };

        Shapes.Clear();
        HasUnsavedChanges = false;
        BoardName = CurrentBoard.Name;
    }

    [RelayCommand(CanExecute = nameof(CanSave))]
    private async Task SaveBoardAsync()
    {
        if (CurrentBoard == null || CurrentProfile == null)
            return;

        await ExecuteAsync(async () =>
        {
            CurrentBoard.LastModified = DateTime.UtcNow;

            if (CurrentBoard.Id == 0)
            {
                CurrentBoard = await _drawingBoardRepository.AddAsync(CurrentBoard);
            }
            else
            {
                await _drawingBoardRepository.UpdateAsync(CurrentBoard);
            }

            foreach (var shape in Shapes.Where(s => s.Id == 0))
            {
                shape.DrawingBoardId = CurrentBoard.Id;
                await _shapeRepository.AddAsync(shape);
            }

            HasUnsavedChanges = false;

            await DialogService.ShowMessageAsync(
                "Saved",
                $"Board '{CurrentBoard.Name}' saved successfully!"
            );
        });
    }

    [RelayCommand]
    private async Task LoadBoardAsync(DrawingBoard? board)
    {
        if (board == null)
            return;

        if (HasUnsavedChanges)
        {
            var saveConfirm = await DialogService.ShowConfirmationAsync(
                "Unsaved Changes",
                "You have unsaved changes. Do you want to save the current board?"
            );

            if (saveConfirm)
            {
                await SaveBoardAsync();
            }
        }

        await ExecuteAsync(async () =>
        {
            var boardWithShapes = await _drawingBoardRepository.GetWithShapesAsync(board.Id);

            if (boardWithShapes != null)
            {
                CurrentBoard = boardWithShapes;
                BoardName = boardWithShapes.Name;
                CanvasWidth = boardWithShapes.Width;
                CanvasHeight = boardWithShapes.Height;

                Shapes.Clear();
                foreach (var shape in boardWithShapes.Shapes.OrderBy(s => s.ZIndex))
                {
                    Shapes.Add(shape);
                }

                HasUnsavedChanges = false;
            }
        });
    }

    [RelayCommand]
    private void SelectTool(string toolName)
    {
        SelectedTool = toolName;
    }

    public void AddShape(Shape shape)
    {
        shape.ZIndex = Shapes.Count;
        Shapes.Add(shape);
        HasUnsavedChanges = true;
    }

    public void RemoveShape(Shape shape)
    {
        Shapes.Remove(shape);
        HasUnsavedChanges = true;
    }

    private static Color ParseColor(string? hex)
    {
        if (string.IsNullOrEmpty(hex))
            return Colors.Black;

        hex = hex.TrimStart('#');

        if (hex.Length == 6)
            hex = "FF" + hex;

        if (hex.Length == 8)
        {
            return Color.FromArgb(
                Convert.ToByte(hex.Substring(0, 2), 16),
                Convert.ToByte(hex.Substring(2, 2), 16),
                Convert.ToByte(hex.Substring(4, 2), 16),
                Convert.ToByte(hex.Substring(6, 2), 16)
            );
        }

        return Colors.Black;
    }
}
