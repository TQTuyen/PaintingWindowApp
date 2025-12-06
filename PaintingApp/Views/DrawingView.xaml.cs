using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using PaintingApp.Contracts;
using PaintingApp.Models;
using PaintingApp.Services;
using PaintingApp.ViewModels;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.UI;

namespace PaintingApp.Views;

public sealed partial class DrawingView : Page
{
    public DrawingScreenViewModel ViewModel { get; }

    private readonly ShapeRendererFactory _rendererFactory;
    private readonly ShapeFactoryProvider _factoryProvider;
    private readonly ShapeTransformerProvider _transformerProvider;
    private readonly ShapeResizerProvider _resizerProvider;
    private readonly ResizeHandleManager _handleManager;
    private readonly IDialogService _dialogService;
    private readonly INavigationService _navigationService;

    private bool _isDrawing;
    private Point _startPoint;
    private ShapeModel? _currentShape;
    private IShapeFactory? _currentFactory;
    private Border? _selectionAdorner;

    private bool _isDraggingShape;
    private Point _dragOffset;
    private ShapeModel? _draggedShape;

    private bool _isResizingShape;
    private ResizeHandle _activeHandle;
    private Point _resizeStartPoint;

    private bool _isNavigatingAway;

    private const string TemplateIdDataFormat = "TemplateGroupId";

    public DrawingView()
    {
        InitializeComponent();
        ViewModel = App.GetService<DrawingScreenViewModel>();
        _rendererFactory = App.GetService<ShapeRendererFactory>();
        _factoryProvider = App.GetService<ShapeFactoryProvider>();
        _transformerProvider = App.GetService<ShapeTransformerProvider>();
        _resizerProvider = App.GetService<ShapeResizerProvider>();
        _handleManager = App.GetService<ResizeHandleManager>();
        _dialogService = App.GetService<IDialogService>();
        _navigationService = App.GetService<INavigationService>();
        DataContext = ViewModel;

        ViewModel.ShapesRendered += OnShapesRendered;
        ViewModel.TemplateInsertionModeChanged += OnTemplateInsertionModeChanged;
    }

    public static Visibility IsCollectionEmpty(int count)
    {
        return count == 0 ? Visibility.Visible : Visibility.Collapsed;
    }

    public static DoubleCollection? GetDashArrayPreview(StrokeDashStyle style)
    {
        return style switch
        {
            StrokeDashStyle.Solid => null,
            StrokeDashStyle.Dash => [4, 2],
            StrokeDashStyle.Dot => [1, 2],
            StrokeDashStyle.DashDot => [4, 2, 1, 2],
            StrokeDashStyle.DashDotDot => [4, 2, 1, 2, 1, 2],
            _ => null
        };
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        _isNavigatingAway = false;
        await ViewModel.InitializeAsync();

        if (e.Parameter is int boardId && boardId > 0)
        {
            await ViewModel.LoadBoardByIdCommand.ExecuteAsync(boardId);
        }

        ViewModel.PropertyChanged += ViewModel_PropertyChanged;
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        base.OnNavigatedFrom(e);
        ViewModel.PropertyChanged -= ViewModel_PropertyChanged;
        
        if (ViewModel.SelectedShape != null)
        {
            ViewModel.SelectedShape.PropertyChanged -= SelectedShape_PropertyChanged;
        }
    }

    protected override async void OnNavigatingFrom(NavigatingCancelEventArgs e)
    {
        if (_isNavigatingAway || !ViewModel.HasUnsavedChanges)
        {
            base.OnNavigatingFrom(e);
            return;
        }

        e.Cancel = true;

        var targetPageType = e.SourcePageType;
        var navigationParameter = e.Parameter;

        var result = await _dialogService.ShowThreeButtonDialogAsync(
            "Unsaved Changes",
            "You have unsaved changes. Do you want to save before leaving?",
            "Save",
            "Don't Save",
            "Cancel"
        );

        if (result == 1) // Save
        {
            await ViewModel.SaveSilentlyAsync();
            
            _isNavigatingAway = true;
            ViewModel.HasUnsavedChanges = false;
            
            DispatcherQueue.TryEnqueue(() =>
            {
                _navigationService.NavigateTo(targetPageType, navigationParameter);
            });
        }
        else if (result == 2) // Don't Save
        {
            _isNavigatingAway = true;
            ViewModel.HasUnsavedChanges = false;
            
            DispatcherQueue.TryEnqueue(() =>
            {
                _navigationService.NavigateTo(targetPageType, navigationParameter);
            });
        }
        // Cancel (result == 0) - do nothing, stay on page
    }

    private void OnTemplateInsertionModeChanged(object? sender, EventArgs e)
    {
    }

    private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ViewModel.SelectedShape))
        {
            if (ViewModel.SelectedShape != null)
            {
                ViewModel.SelectedShape.PropertyChanged += SelectedShape_PropertyChanged;
                ShowSelectionAdorner(ViewModel.SelectedShape);
            }
            else
            {
                HideSelectionAdorner();
            }
        }
    }

    private void SelectedShape_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (sender is ShapeModel shape)
        {
            RenderAllShapes();
            ShowSelectionAdorner(shape);
            ViewModel.MarkShapeModified(shape);
        }
    }

    private void OnShapesRendered(object? sender, EventArgs e)
    {
        RenderAllShapes();
    }

    private void RenderAllShapes()
    {
        DrawingCanvas.Children.Clear();

        foreach (var shapeModel in ViewModel.Shapes)
        {
            var renderer = _rendererFactory.GetRenderer(shapeModel.Type);
            renderer.Render(DrawingCanvas, shapeModel);
        }

        // Show selection adorners for all selected shapes
        foreach (var selectedShape in ViewModel.SelectedShapes)
        {
            ShowSelectionAdornerForShape(selectedShape, isPrimary: selectedShape == ViewModel.SelectedShape);
        }

        // If only single selection via SelectedShape (backward compatibility)
        if (ViewModel.SelectedShape != null && !ViewModel.SelectedShapes.Contains(ViewModel.SelectedShape))
        {
            ShowSelectionAdorner(ViewModel.SelectedShape);
        }
    }

    private void ShowSelectionAdornerForShape(ShapeModel shape, bool isPrimary)
    {
        var bounds = shape.GetBounds();
        if (bounds.Width <= 0 || bounds.Height <= 0) return;

        var adorner = new Border
        {
            BorderBrush = new SolidColorBrush(isPrimary ? Colors.DodgerBlue : Colors.CornflowerBlue),
            BorderThickness = new Thickness(isPrimary ? 2 : 1.5),
            Width = bounds.Width + 10,
            Height = bounds.Height + 10,
            CornerRadius = new CornerRadius(2),
            IsHitTestVisible = false
        };

        Canvas.SetLeft(adorner, bounds.X - 5);
        Canvas.SetTop(adorner, bounds.Y - 5);
        Canvas.SetZIndex(adorner, 10000);

        DrawingCanvas.Children.Add(adorner);

        // Show resize handles only for primary selection
        if (isPrimary)
        {
            _handleManager.ShowHandles(DrawingCanvas, bounds);
        }
    }

    private void ToolMenuItem_Click(object sender, RoutedEventArgs e)
    {
        if (sender is MenuFlyoutItem item && item.CommandParameter is string toolName)
        {
            CurrentToolIcon.Glyph = toolName switch
            {
                "Select" => "\uE7C9",
                "Line" => "\uE76D",
                "Rectangle" => "\uE739",
                "Circle" => "\uEA3A",
                "Oval" => "\uF138",
                "Triangle" => "\uE879",
                "Polygon" => "\uF408",
                _ => "\uE7C9"
            };
        }
    }

    private void DrawingCanvas_PointerPressed(object sender, PointerRoutedEventArgs e)
    {
        var point = e.GetCurrentPoint(DrawingCanvas).Position;
        var isCtrlPressed = Microsoft.UI.Input.InputKeyboardSource.GetKeyStateForCurrentThread(Windows.System.VirtualKey.Control)
            .HasFlag(Windows.UI.Core.CoreVirtualKeyStates.Down);

        // Handle template insertion mode (click-to-place)
        if (ViewModel.IsTemplateInsertionMode)
        {
            _ = ViewModel.InsertTemplateAtPositionCommand.ExecuteAsync(point);
            return;
        }

        if (ViewModel.SelectedTool == "Select")
        {
            var handleHit = _handleManager.HitTestHandle(point);

            if (handleHit != ResizeHandle.None && ViewModel.SelectedShape != null)
            {
                _isResizingShape = true;
                _activeHandle = handleHit;
                _resizeStartPoint = point;
                DrawingCanvas.CapturePointer(e.Pointer);
                return;
            }

            var clickedShape = HitTestShape(point);

            if (clickedShape != null)
            {
                if (isCtrlPressed)
                {
                    // Ctrl+Click: Toggle selection
                    ViewModel.ToggleShapeSelection(clickedShape);
                    RenderAllShapes();
                }
                else if (ViewModel.SelectedShapes.Contains(clickedShape))
                {
                    // Clicked on already selected shape - start dragging
                    _isDraggingShape = true;
                    _draggedShape = clickedShape;

                    var transformer = _transformerProvider.GetTransformer(clickedShape.Type);
                    var referencePoint = transformer.GetReferencePoint(clickedShape);

                    _dragOffset = new Point(
                        point.X - referencePoint.X,
                        point.Y - referencePoint.Y
                    );

                    DrawingCanvas.CapturePointer(e.Pointer);
                }
                else
                {
                    // Click on unselected shape - select it (clearing previous selection)
                    ViewModel.SelectShape(clickedShape, addToSelection: false);
                    RenderAllShapes();
                }
            }
            else
            {
                // Clicked on empty space - clear selection
                if (!isCtrlPressed)
                {
                    ViewModel.ClearSelection();
                    RenderAllShapes();
                }
            }

            return;
        }

        if (!_factoryProvider.HasFactory(ViewModel.SelectedTool)) return;

        _startPoint = point;
        _isDrawing = true;

        _currentFactory = _factoryProvider.GetFactory(ViewModel.SelectedTool);
        _currentShape = _currentFactory?.CreateShape(
            point,
            ViewModel.StrokeColor,
            ViewModel.StrokeThickness,
            ViewModel.FillColor
        );

        if (_currentShape != null)
        {
            _currentShape.StrokeDashStyle = ViewModel.SelectedStrokeDashStyle;
        }

        DrawingCanvas.CapturePointer(e.Pointer);
    }

    private void DrawingCanvas_PointerMoved(object sender, PointerRoutedEventArgs e)
    {
        var currentPoint = e.GetCurrentPoint(DrawingCanvas).Position;

        if (_isResizingShape && ViewModel.SelectedShape != null)
        {
            var resizer = _resizerProvider.GetResizer(ViewModel.SelectedShape.Type);
            resizer.Resize(ViewModel.SelectedShape, _activeHandle, _resizeStartPoint, currentPoint);

            _resizeStartPoint = currentPoint;

            RenderAllShapes();
            return;
        }

        if (_isDraggingShape && _draggedShape != null)
        {
            var transformer = _transformerProvider.GetTransformer(_draggedShape.Type);
            var oldReference = transformer.GetReferencePoint(_draggedShape);

            var newReferenceX = currentPoint.X - _dragOffset.X;
            var newReferenceY = currentPoint.Y - _dragOffset.Y;

            var deltaX = newReferenceX - oldReference.X;
            var deltaY = newReferenceY - oldReference.Y;

            transformer.Translate(_draggedShape, deltaX, deltaY);

            RenderAllShapes();
            return;
        }

        if (!_isDrawing || _currentShape == null || _currentFactory == null) return;

        _currentFactory.UpdateShape(_currentShape, _startPoint, currentPoint);

        _currentShape.StrokeColor = ViewModel.StrokeColor;
        _currentShape.StrokeThickness = ViewModel.StrokeThickness;
        _currentShape.StrokeDashStyle = ViewModel.SelectedStrokeDashStyle;
        _currentShape.FillColor = ViewModel.FillColor;

        DrawingCanvas.Children.Clear();
        RenderAllShapes();

        var renderer = _rendererFactory.GetRenderer(_currentShape.Type);
        renderer.Render(DrawingCanvas, _currentShape);
    }

    private void DrawingCanvas_PointerReleased(object sender, PointerRoutedEventArgs e)
    {
        if (_isResizingShape)
        {
            _isResizingShape = false;
            _activeHandle = ResizeHandle.None;

            if (ViewModel.SelectedShape != null)
            {
                ViewModel.MarkShapeModified(ViewModel.SelectedShape);
            }

            DrawingCanvas.ReleasePointerCapture(e.Pointer);
            return;
        }

        if (_isDraggingShape)
        {
            _isDraggingShape = false;

            if (_draggedShape != null)
            {
                ViewModel.MarkShapeModified(_draggedShape);
            }

            _draggedShape = null;
            DrawingCanvas.ReleasePointerCapture(e.Pointer);
            return;
        }

        if (!_isDrawing || _currentShape == null || _currentFactory == null)
        {
            _isDrawing = false;
            return;
        }

        var endPoint = e.GetCurrentPoint(DrawingCanvas).Position;
        _currentFactory.UpdateShape(_currentShape, _startPoint, endPoint);

        ViewModel.AddShape(_currentShape);

        DrawingCanvas.Children.Clear();
        RenderAllShapes();

        _isDrawing = false;
        _currentShape = null;
        _currentFactory = null;

        DrawingCanvas.ReleasePointerCapture(e.Pointer);
    }

    private ShapeModel? HitTestShape(Point point)
    {
        for (int i = ViewModel.Shapes.Count - 1; i >= 0; i--)
        {
            var shape = ViewModel.Shapes[i];
            if (IsPointInShape(point, shape))
            {
                return shape;
            }
        }
        return null;
    }

    private static bool IsPointInShape(Point point, ShapeModel shape)
    {
        var bounds = shape.GetBounds();
        var expandedBounds = new Rect(
            bounds.X - 5,
            bounds.Y - 5,
            bounds.Width + 10,
            bounds.Height + 10
        );
        return expandedBounds.Contains(point);
    }

    private void ShowSelectionAdorner(ShapeModel shape)
    {
        HideSelectionAdorner();

        var bounds = shape.GetBounds();
        if (bounds.Width <= 0 || bounds.Height <= 0) return;

        _selectionAdorner = new Border
        {
            BorderBrush = new SolidColorBrush(Colors.DodgerBlue),
            BorderThickness = new Thickness(2),
            Width = bounds.Width + 10,
            Height = bounds.Height + 10,
            CornerRadius = new CornerRadius(2),
            IsHitTestVisible = false
        };

        Canvas.SetLeft(_selectionAdorner, bounds.X - 5);
        Canvas.SetTop(_selectionAdorner, bounds.Y - 5);
        Canvas.SetZIndex(_selectionAdorner, 10000);

        DrawingCanvas.Children.Add(_selectionAdorner);

        _handleManager.ShowHandles(DrawingCanvas, bounds);
    }

    private void HideSelectionAdorner()
    {
        if (_selectionAdorner != null)
        {
            DrawingCanvas.Children.Remove(_selectionAdorner);
            _selectionAdorner = null;
        }
        _handleManager.HideHandles(DrawingCanvas);
    }

    private void StrokePresetColor_Click(object sender, ItemClickEventArgs e)
    {
        if (e.ClickedItem is Border border && border.Tag is string colorHex)
        {
            ViewModel.StrokeColor = ParseColor(colorHex);
        }
    }

    private void FillPresetColor_Click(object sender, ItemClickEventArgs e)
    {
        if (e.ClickedItem is Border border && border.Tag is string colorHex)
        {
            ViewModel.FillColor = ParseColor(colorHex);
        }
    }

    private void NoFill_Click(object sender, RoutedEventArgs e)
    {
        ViewModel.FillColor = Colors.Transparent;
    }

    private void PropertiesPanel_ShapeDeleted(object sender, ShapeModel shape)
    {
        ViewModel.RemoveShape(shape);
        ViewModel.SelectedShape = null;
        HideSelectionAdorner();
        RenderAllShapes();
    }

    private void PropertiesPanel_ShapePropertyChanged(object sender, ShapeModel shape)
    {
        RenderAllShapes();
        ViewModel.MarkShapeModified(shape);
    }

    private static Color ParseColor(string hex)
    {
        hex = hex.TrimStart('#');

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

    #region Template Drag-and-Drop Handling

    private void TemplatesListView_DragItemsStarting(object sender, DragItemsStartingEventArgs e)
    {
        if (e.Items.Count == 0) return;

        if (e.Items[0] is TemplateGroupViewModel template)
        {
            e.Data.SetData(TemplateIdDataFormat, template.Id);
            e.Data.RequestedOperation = DataPackageOperation.Copy;
        }
    }

    private void DrawingCanvas_DragOver(object sender, DragEventArgs e)
    {
        // Check if the drag data contains a template ID
        if (e.DataView.Contains(TemplateIdDataFormat))
        {
            e.AcceptedOperation = DataPackageOperation.Copy;
            e.DragUIOverride.Caption = "Insert template";
            e.DragUIOverride.IsCaptionVisible = true;
            e.DragUIOverride.IsGlyphVisible = true;
        }
        else
        {
            e.AcceptedOperation = DataPackageOperation.None;
        }
    }

    private async void DrawingCanvas_Drop(object sender, DragEventArgs e)
    {
        if (!e.DataView.Contains(TemplateIdDataFormat)) return;

        try
        {
            var templateId = (int)await e.DataView.GetDataAsync(TemplateIdDataFormat);
            var dropPosition = e.GetPosition(DrawingCanvas);

            await ViewModel.InsertTemplateAsync(templateId, dropPosition);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error dropping template: {ex.Message}");
        }
    }

    #endregion

    #region Template Click-to-Place Handling

    private void TemplateItem_PointerPressed(object sender, PointerRoutedEventArgs e)
    {
        if (sender is FrameworkElement element && element.DataContext is TemplateGroupViewModel template)
        {
            ViewModel.SelectTemplateForInsertionCommand.Execute(template.Id);
        }
    }

    #endregion
}
