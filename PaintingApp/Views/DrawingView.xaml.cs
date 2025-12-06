using System;
using System.Collections.Generic;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Navigation;
using PaintingApp.Contracts;
using PaintingApp.Models;
using PaintingApp.Services;
using PaintingApp.ViewModels;
using Windows.Foundation;

namespace PaintingApp.Views;

public sealed partial class DrawingView : Page
{
    public DrawingScreenViewModel ViewModel { get; }

    private readonly ShapeRendererFactory _rendererFactory;
    private readonly ShapeFactoryProvider _factoryProvider;

    private bool _isDrawing;
    private Point _startPoint;
    private ShapeModel? _currentShape;
    private IShapeFactory? _currentFactory;

    private readonly List<ShapeModel> _shapeModels = [];

    public DrawingView()
    {
        InitializeComponent();
        ViewModel = App.GetService<DrawingScreenViewModel>();
        _rendererFactory = App.GetService<ShapeRendererFactory>();
        _factoryProvider = App.GetService<ShapeFactoryProvider>();
        DataContext = ViewModel;

        ViewModel.ShapesRendered += OnShapesRendered;
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        await ViewModel.InitializeAsync();

        if (e.Parameter is int boardId && boardId > 0)
        {
            await ViewModel.LoadBoardByIdCommand.ExecuteAsync(boardId);
        }
    }

    private void OnShapesRendered(object? sender, EventArgs e)
    {
        RenderAllShapes();
    }

    private void RenderAllShapes()
    {
        DrawingCanvas.Children.Clear();

        foreach (var shapeModel in _shapeModels)
        {
            var renderer = _rendererFactory.GetRenderer(shapeModel.Type);
            renderer.Render(DrawingCanvas, shapeModel);
        }
    }

    private void ToolRadioButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is RadioButton radioButton && radioButton.Tag is string toolName)
        {
            ViewModel.SelectToolCommand.Execute(toolName);
        }
    }

    private void DrawingCanvas_PointerPressed(object sender, PointerRoutedEventArgs e)
    {
        if (!_factoryProvider.HasFactory(ViewModel.SelectedTool)) return;

        var point = e.GetCurrentPoint(DrawingCanvas).Position;
        _startPoint = point;
        _isDrawing = true;

        _currentFactory = _factoryProvider.GetFactory(ViewModel.SelectedTool);
        _currentShape = _currentFactory?.CreateShape(
            point,
            ViewModel.StrokeColor,
            ViewModel.StrokeThickness,
            ViewModel.FillColor
        );

        DrawingCanvas.CapturePointer(e.Pointer);
    }

    private void DrawingCanvas_PointerMoved(object sender, PointerRoutedEventArgs e)
    {
        if (!_isDrawing || _currentShape == null || _currentFactory == null) return;

        var currentPoint = e.GetCurrentPoint(DrawingCanvas).Position;
        _currentFactory.UpdateShape(_currentShape, _startPoint, currentPoint);

        DrawingCanvas.Children.Clear();
        RenderAllShapes();

        var renderer = _rendererFactory.GetRenderer(_currentShape.Type);
        renderer.Render(DrawingCanvas, _currentShape);
    }

    private void DrawingCanvas_PointerReleased(object sender, PointerRoutedEventArgs e)
    {
        if (!_isDrawing || _currentShape == null || _currentFactory == null)
        {
            _isDrawing = false;
            return;
        }

        var endPoint = e.GetCurrentPoint(DrawingCanvas).Position;
        _currentFactory.UpdateShape(_currentShape, _startPoint, endPoint);

        _currentShape.ZIndex = _shapeModels.Count;
        _shapeModels.Add(_currentShape);
        ViewModel.HasUnsavedChanges = true;

        DrawingCanvas.Children.Clear();
        RenderAllShapes();

        _isDrawing = false;
        _currentShape = null;
        _currentFactory = null;

        DrawingCanvas.ReleasePointerCapture(e.Pointer);
    }
}
