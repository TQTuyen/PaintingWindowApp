using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI;
using Microsoft.UI.Xaml.Controls;
using PaintingApp.Contracts;
using PaintingApp.Data.Entities;
using PaintingApp.Data.Repositories.Interfaces;
using PaintingApp.Dialogs;
using PaintingApp.Models;
using PaintingApp.Services;
using Windows.UI;

namespace PaintingApp.ViewModels;

public partial class DrawingScreenViewModel : BaseViewModel
{
    private readonly IProfileStateService _profileStateService;
    private readonly IDrawingBoardRepository _drawingBoardRepository;
    private readonly IShapeRepository _shapeRepository;
    private readonly ShapeAdapterProvider _adapterProvider;

    [ObservableProperty]
    private Profile? _currentProfile;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveBoardCommand))]
    private DrawingBoard? _currentBoard;

    [ObservableProperty]
    private ObservableCollection<ShapeModel> _shapes = [];

    [ObservableProperty]
    private double _canvasWidth = 800;

    [ObservableProperty]
    private double _canvasHeight = 600;

    [ObservableProperty]
    private Color _canvasBackgroundColor = Colors.White;

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

    public event EventHandler? ShapesRendered;

    public DrawingScreenViewModel(
        INavigationService navigationService,
        IDialogService dialogService,
        IProfileStateService profileStateService,
        IDrawingBoardRepository drawingBoardRepository,
        IShapeRepository shapeRepository,
        ShapeAdapterProvider adapterProvider)
        : base(navigationService, dialogService)
    {
        _profileStateService = profileStateService;
        _drawingBoardRepository = drawingBoardRepository;
        _shapeRepository = shapeRepository;
        _adapterProvider = adapterProvider;

        Title = "Drawing";
    }

    public override Task InitializeAsync()
    {
        LoadProfileSettings();
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

    [RelayCommand]
    private async Task NewBoardAsync()
    {
        if (CurrentProfile == null)
        {
            await DialogService.ShowErrorAsync(
                "No Profile Selected",
                "Please select a profile first."
            );
            return;
        }

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

        if (App.MainWindow?.Content?.XamlRoot == null) return;

        var dialog = new NewBoardDialog(CurrentProfile)
        {
            XamlRoot = App.MainWindow.Content.XamlRoot
        };

        var result = await dialog.ShowAsync();

        if (result == ContentDialogResult.Primary && dialog.Result != null)
        {
            await ExecuteAsync(async () =>
            {
                var newBoard = await _drawingBoardRepository.AddAsync(dialog.Result);

                CurrentBoard = newBoard;
                BoardName = newBoard.Name;
                CanvasWidth = newBoard.Width;
                CanvasHeight = newBoard.Height;
                CanvasBackgroundColor = ParseColor(newBoard.BackgroundColor);
                Shapes.Clear();
                HasUnsavedChanges = false;
                
                RenderShapes();
            });
        }
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
                await _shapeRepository.DeleteByDrawingBoardIdAsync(CurrentBoard.Id);
            }

            foreach (var shapeModel in Shapes)
            {
                var adapter = _adapterProvider.GetAdapter(shapeModel.Type);
                var shapeEntity = adapter.ToEntity(shapeModel);
                shapeEntity.DrawingBoardId = CurrentBoard.Id;
                await _shapeRepository.AddAsync(shapeEntity);
            }

            HasUnsavedChanges = false;

            await DialogService.ShowMessageAsync(
                "Saved",
                $"Board '{CurrentBoard.Name}' saved with {Shapes.Count} shapes!"
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
                CanvasBackgroundColor = ParseColor(boardWithShapes.BackgroundColor);

                Shapes.Clear();
                foreach (var shapeEntity in boardWithShapes.Shapes.OrderBy(s => s.ZIndex))
                {
                    var adapter = _adapterProvider.GetAdapter(shapeEntity.Type);
                    var shapeModel = adapter.ToModel(shapeEntity);
                    Shapes.Add(shapeModel);
                }

                HasUnsavedChanges = false;
                RenderShapes();
            }
        });
    }

    [RelayCommand]
    private async Task LoadBoardByIdAsync(int boardId)
    {
        await ExecuteAsync(async () =>
        {
            var boardWithShapes = await _drawingBoardRepository.GetWithShapesAsync(boardId);

            if (boardWithShapes != null)
            {
                CurrentBoard = boardWithShapes;
                BoardName = boardWithShapes.Name;
                CanvasWidth = boardWithShapes.Width;
                CanvasHeight = boardWithShapes.Height;
                CanvasBackgroundColor = ParseColor(boardWithShapes.BackgroundColor);

                Shapes.Clear();
                foreach (var shapeEntity in boardWithShapes.Shapes.OrderBy(s => s.ZIndex))
                {
                    var adapter = _adapterProvider.GetAdapter(shapeEntity.Type);
                    var shapeModel = adapter.ToModel(shapeEntity);
                    Shapes.Add(shapeModel);
                }

                HasUnsavedChanges = false;
                RenderShapes();
            }
        });
    }

    private void RenderShapes()
    {
        ShapesRendered?.Invoke(this, EventArgs.Empty);
    }

    [RelayCommand]
    private void SelectTool(string toolName)
    {
        SelectedTool = toolName;
    }

    public void AddShape(ShapeModel shape)
    {
        shape.ZIndex = Shapes.Count;
        Shapes.Add(shape);
        HasUnsavedChanges = true;
    }

    public void RemoveShape(ShapeModel shape)
    {
        Shapes.Remove(shape);
        HasUnsavedChanges = true;
    }

    private static Color ParseColor(string? hex)
    {
        if (string.IsNullOrEmpty(hex))
            return Colors.White;

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

        return Colors.White;
    }
}
