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
using Windows.Foundation;
using Windows.UI;

namespace PaintingApp.ViewModels;

public partial class DrawingScreenViewModel : BaseViewModel
{
    private readonly IProfileStateService _profileStateService;
    private readonly IDrawingBoardRepository _drawingBoardRepository;
    private readonly IShapeRepository _shapeRepository;
    private readonly ITemplateGroupRepository _templateGroupRepository;
    private readonly ShapeAdapterProvider _adapterProvider;
    private readonly ShapeTransformerProvider _transformerProvider;

    private readonly List<int> _deletedShapeIds = [];

    [ObservableProperty]
    private Profile? _currentProfile;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveBoardCommand))]
    [NotifyCanExecuteChangedFor(nameof(DeleteBoardCommand))]
    private DrawingBoard? _currentBoard;

    [ObservableProperty]
    private ObservableCollection<ShapeModel> _shapes = [];

    [ObservableProperty]
    private ObservableCollection<ShapeModel> _selectedShapes = [];

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
    private StrokeDashStyle _selectedStrokeDashStyle = StrokeDashStyle.Solid;

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
    [NotifyCanExecuteChangedFor(nameof(SaveAsTemplateCommand))]
    private bool _hasSelection;

    public ObservableCollection<StrokeDashStyle> AvailableDashStyles { get; } =
    [
        StrokeDashStyle.Solid,
        StrokeDashStyle.Dash,
        StrokeDashStyle.Dot,
        StrokeDashStyle.DashDot,
        StrokeDashStyle.DashDotDot
    ];

    partial void OnSelectedShapeChanged(ShapeModel? value)
    {
        HasSelection = value != null || SelectedShapes.Count > 0;
    }

    public bool CanSave => CurrentBoard != null && CurrentProfile != null;
    public bool CanDeleteBoard => CurrentBoard != null && CurrentBoard.Id > 0;
    public bool CanSaveAsTemplate => HasSelection && CurrentProfile != null;

    public event EventHandler? ShapesRendered;
    public event EventHandler? SelectionChanged;
    public event EventHandler? TemplateCreated;

    public DrawingScreenViewModel(
        INavigationService navigationService,
        IDialogService dialogService,
        IProfileStateService profileStateService,
        IDrawingBoardRepository drawingBoardRepository,
        IShapeRepository shapeRepository,
        ITemplateGroupRepository templateGroupRepository,
        ShapeAdapterProvider adapterProvider,
        ShapeTransformerProvider transformerProvider)
        : base(navigationService, dialogService)
    {
        _profileStateService = profileStateService;
        _drawingBoardRepository = drawingBoardRepository;
        _shapeRepository = shapeRepository;
        _templateGroupRepository = templateGroupRepository;
        _adapterProvider = adapterProvider;
        _transformerProvider = transformerProvider;

        Title = "Drawing";
        
        SelectedShapes.CollectionChanged += (s, e) =>
        {
            HasSelection = SelectedShapes.Count > 0 || SelectedShape != null;
            SelectionChanged?.Invoke(this, EventArgs.Empty);
        };
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
            SelectedStrokeDashStyle = ParseStrokeDashStyle(CurrentProfile.DefaultStrokeStyle);
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
                ClearSelection();
                
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

    [RelayCommand(CanExecute = nameof(CanDeleteBoard))]
    private async Task DeleteBoardAsync()
    {
        if (CurrentBoard == null || CurrentBoard.Id <= 0)
            return;

        var confirmed = await DialogService.ShowConfirmationAsync(
            "Delete Board?",
            $"This will permanently delete \"{CurrentBoard.Name}\" and all its shapes. This action cannot be undone.",
            "Delete",
            "Cancel"
        );

        if (!confirmed)
            return;

        var boardId = CurrentBoard.Id;
        var boardExists = await _drawingBoardRepository.ExistsAsync(boardId);

        if (!boardExists)
        {
            await DialogService.ShowMessageAsync(
                "Board Not Found",
                "This board has already been deleted."
            );
            NavigateToManagement();
            return;
        }

        await ExecuteAsync(async () =>
        {
            await _drawingBoardRepository.DeleteAsync(boardId);
        },
        onError: async ex =>
        {
            await DialogService.ShowErrorAsync(
                "Delete Failed",
                "Unable to delete the board. Please try again."
            );
        });

        // Clear state and navigate back
        ClearBoardState();
        NavigateToManagement();
    }

    private void ClearBoardState()
    {
        CurrentBoard = null;
        BoardName = "Untitled";
        Shapes.Clear();
        _deletedShapeIds.Clear();
        HasUnsavedChanges = false;
        SelectedShape = null;
        ClearSelection();
    }

    private void NavigateToManagement()
    {
        NavigationService.NavigateTo("Management");
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
                ClearSelection();

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
                ClearSelection();

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
        SelectedShapes.Remove(shape);

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
        ClearSelection();
    }

    public void ClearSelection()
    {
        foreach (var shape in SelectedShapes)
        {
            shape.IsSelected = false;
        }
        SelectedShapes.Clear();
        SelectedShape = null;
        HasSelection = false;
    }

    public void SelectShape(ShapeModel shape, bool addToSelection = false)
    {
        if (!addToSelection)
        {
            ClearSelection();
        }

        if (!SelectedShapes.Contains(shape))
        {
            shape.IsSelected = true;
            SelectedShapes.Add(shape);
        }

        SelectedShape = shape;
        HasSelection = true;
    }

    public void ToggleShapeSelection(ShapeModel shape)
    {
        if (SelectedShapes.Contains(shape))
        {
            shape.IsSelected = false;
            SelectedShapes.Remove(shape);
            
            if (SelectedShape == shape)
            {
                SelectedShape = SelectedShapes.LastOrDefault();
            }
        }
        else
        {
            shape.IsSelected = true;
            SelectedShapes.Add(shape);
            SelectedShape = shape;
        }

        HasSelection = SelectedShapes.Count > 0;
    }

    [RelayCommand(CanExecute = nameof(CanSaveAsTemplate))]
    private async Task SaveAsTemplateAsync()
    {
        if (CurrentProfile == null)
        {
            await DialogService.ShowErrorAsync(
                "No Profile Selected",
                "Please select a profile first."
            );
            return;
        }

        var shapesToSave = SelectedShapes.Count > 0 
            ? SelectedShapes.ToList() 
            : (SelectedShape != null ? [SelectedShape] : []);

        if (shapesToSave.Count == 0)
        {
            await DialogService.ShowWarningAsync(
                "No Shapes Selected",
                "Please select at least one shape to save as a template."
            );
            return;
        }

        if (App.MainWindow?.Content?.XamlRoot == null) return;

        var dialog = new SaveTemplateDialog(shapesToSave.Count)
        {
            XamlRoot = App.MainWindow.Content.XamlRoot
        };

        var result = await dialog.ShowAsync();

        if (result != ContentDialogResult.Primary) return;

        await ExecuteAsync(async () =>
        {
            var boundingBox = ComputeBoundingBox(shapesToSave);

            var templateGroup = new TemplateGroup
            {
                Name = dialog.TemplateName,
                Description = dialog.TemplateDescription,
                ProfileId = CurrentProfile.Id,
                UsageCount = 0,
                CreatedDate = DateTime.UtcNow
            };

            templateGroup = await _templateGroupRepository.AddAsync(templateGroup);

            var templateShapes = new List<Shape>();

            for (int i = 0; i < shapesToSave.Count; i++)
            {
                var shapeModel = shapesToSave[i];
                var adapter = _adapterProvider.GetAdapter(shapeModel.Type);
                
                var clonedModel = CloneAndNormalizeShape(shapeModel, boundingBox);
                var shapeEntity = adapter.ToEntity(clonedModel);
                
                shapeEntity.TemplateGroupId = templateGroup.Id;
                shapeEntity.DrawingBoardId = null;
                shapeEntity.ZIndex = i;
                shapeEntity.CreatedDate = DateTime.UtcNow;

                templateShapes.Add(shapeEntity);
            }

            await _shapeRepository.BulkInsertAsync(templateShapes);

            TemplateCreated?.Invoke(this, EventArgs.Empty);

            await DialogService.ShowMessageAsync(
                "Template Saved",
                $"Template '{templateGroup.Name}' created with {shapesToSave.Count} shape{(shapesToSave.Count == 1 ? "" : "s")}."
            );
        });
    }

    private static Rect ComputeBoundingBox(IList<ShapeModel> shapes)
    {
        if (shapes.Count == 0)
            return Rect.Empty;

        double minX = double.MaxValue, minY = double.MaxValue;
        double maxX = double.MinValue, maxY = double.MinValue;

        foreach (var shape in shapes)
        {
            var bounds = shape.GetBounds();
            
            if (bounds.Left < minX) minX = bounds.Left;
            if (bounds.Top < minY) minY = bounds.Top;
            if (bounds.Right > maxX) maxX = bounds.Right;
            if (bounds.Bottom > maxY) maxY = bounds.Bottom;
        }

        return new Rect(minX, minY, maxX - minX, maxY - minY);
    }

    private ShapeModel CloneAndNormalizeShape(ShapeModel original, Rect boundingBox)
    {
        var transformer = _transformerProvider.GetTransformer(original.Type);
        var adapter = _adapterProvider.GetAdapter(original.Type);
        
        // Create entity and convert back to get a clean clone
        var entity = adapter.ToEntity(original);
        var cloned = adapter.ToModel(entity);
        
        // Normalize position by translating relative to bounding box origin
        var refPoint = transformer.GetReferencePoint(cloned);
        var offsetX = -boundingBox.Left;
        var offsetY = -boundingBox.Top;
        
        transformer.Translate(cloned, offsetX, offsetY);
        
        cloned.IsNew = true;
        cloned.IsModified = false;
        
        return cloned;
    }

    private static StrokeDashStyle ParseStrokeDashStyle(string? strokeStyle)
    {
        if (string.IsNullOrEmpty(strokeStyle))
            return StrokeDashStyle.Solid;

        return Enum.TryParse<StrokeDashStyle>(strokeStyle, true, out var result)
            ? result
            : StrokeDashStyle.Solid;
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
