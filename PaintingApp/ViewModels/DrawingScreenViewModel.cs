using System;
using System.Collections.Generic;
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

    private readonly List<int> _deletedShapeIds = [];

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

    [ObservableProperty]
    private ShapeModel? _selectedShape;

    [ObservableProperty]
    private bool _hasSelection;

    partial void OnSelectedShapeChanged(ShapeModel? value)
    {
        HasSelection = value != null;
    }

    public bool CanSave => CurrentBoard != null && CurrentProfile != null;

    public event EventHandler? ShapesRendered;
    public event EventHandler? SelectionChanged;

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
                _deletedShapeIds.Clear();
                HasUnsavedChanges = false;
                
                RenderShapes();
            });
        }
    }

    [RelayCommand(CanExecute = nameof(CanSave))]
    private async Task SaveBoardAsync()
    {
        await SaveBoardInternalAsync(showSuccessDialog: true);
    }

    public async Task SaveSilentlyAsync()
    {
        await SaveBoardInternalAsync(showSuccessDialog: false);
    }

    private async Task SaveBoardInternalAsync(bool showSuccessDialog)
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

            // Handle deleted shapes
            if (_deletedShapeIds.Count > 0)
            {
                await _shapeRepository.BulkDeleteAsync(_deletedShapeIds);
                _deletedShapeIds.Clear();
            }

            // Separate new and modified shapes
            var newShapes = new List<Shape>();
            var modifiedShapes = new List<Shape>();
            var newShapeModels = new List<ShapeModel>();

            foreach (var shapeModel in Shapes)
            {
                var adapter = _adapterProvider.GetAdapter(shapeModel.Type);
                var shapeEntity = adapter.ToEntity(shapeModel);
                shapeEntity.DrawingBoardId = CurrentBoard.Id;

                if (shapeModel.IsNew)
                {
                    newShapes.Add(shapeEntity);
                    newShapeModels.Add(shapeModel);
                }
                else if (shapeModel.IsModified)
                {
                    shapeEntity.Id = shapeModel.Id;
                    modifiedShapes.Add(shapeEntity);
                }
            }

            // Bulk insert new shapes
            if (newShapes.Count > 0)
            {
                await _shapeRepository.BulkInsertAsync(newShapes);

                // Update shape models with generated IDs
                for (int i = 0; i < newShapes.Count; i++)
                {
                    newShapeModels[i].Id = newShapes[i].Id;
                    newShapeModels[i].IsNew = false;
                    newShapeModels[i].IsModified = false;
                }
            }

            // Bulk update modified shapes
            if (modifiedShapes.Count > 0)
            {
                await _shapeRepository.BulkUpdateAsync(modifiedShapes);

                foreach (var shape in Shapes.Where(s => !s.IsNew && s.IsModified))
                {
                    shape.IsModified = false;
                }
            }

            HasUnsavedChanges = false;

            if (showSuccessDialog)
            {
                await DialogService.ShowMessageAsync(
                    "Saved",
                    $"Board '{CurrentBoard.Name}' saved with {Shapes.Count} shapes!"
                );
            }
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
                _deletedShapeIds.Clear();

                foreach (var shapeEntity in boardWithShapes.Shapes.OrderBy(s => s.ZIndex))
                {
                    var adapter = _adapterProvider.GetAdapter(shapeEntity.Type);
                    var shapeModel = adapter.ToModel(shapeEntity);
                    shapeModel.Id = shapeEntity.Id;
                    shapeModel.IsNew = false;
                    shapeModel.IsModified = false;
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
                _deletedShapeIds.Clear();

                foreach (var shapeEntity in boardWithShapes.Shapes.OrderBy(s => s.ZIndex))
                {
                    var adapter = _adapterProvider.GetAdapter(shapeEntity.Type);
                    var shapeModel = adapter.ToModel(shapeEntity);
                    shapeModel.Id = shapeEntity.Id;
                    shapeModel.IsNew = false;
                    shapeModel.IsModified = false;
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
        shape.IsNew = true;
        shape.IsModified = false;
        Shapes.Add(shape);
        HasUnsavedChanges = true;
    }

    public void RemoveShape(ShapeModel shape)
    {
        Shapes.Remove(shape);

        if (!shape.IsNew && shape.Id > 0)
        {
            _deletedShapeIds.Add(shape.Id);
        }

        HasUnsavedChanges = true;
    }

    public void MarkShapeModified(ShapeModel shape)
    {
        if (!shape.IsNew)
        {
            shape.IsModified = true;
        }
        HasUnsavedChanges = true;
    }

    [RelayCommand]
    private void DeselectShape()
    {
        SelectedShape = null;
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
