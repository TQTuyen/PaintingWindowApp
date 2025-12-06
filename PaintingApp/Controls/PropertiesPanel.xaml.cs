using System;
using System.ComponentModel;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using PaintingApp.Models;
using Windows.UI;

namespace PaintingApp.Controls;

public sealed partial class PropertiesPanel : UserControl, INotifyPropertyChanged
{
    public static readonly DependencyProperty SelectedShapeProperty =
        DependencyProperty.Register(
            nameof(SelectedShape),
            typeof(ShapeModel),
            typeof(PropertiesPanel),
            new PropertyMetadata(null, OnSelectedShapeChanged));

    public ShapeModel? SelectedShape
    {
        get => (ShapeModel?)GetValue(SelectedShapeProperty);
        set => SetValue(SelectedShapeProperty, value);
    }

    public bool HasSelection => SelectedShape != null;

    public event PropertyChangedEventHandler? PropertyChanged;
    public event EventHandler<ShapeModel>? ShapeDeleted;
    public event EventHandler<ShapeModel>? ShapePropertyChanged;

    private bool _isUpdatingControls;

    public PropertiesPanel()
    {
        InitializeComponent();
    }

    private static void OnSelectedShapeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is PropertiesPanel panel)
        {
            panel.OnSelectedShapeChanged(e.OldValue as ShapeModel, e.NewValue as ShapeModel);
        }
    }

    private void OnSelectedShapeChanged(ShapeModel? oldShape, ShapeModel? newShape)
    {
        if (oldShape != null)
        {
            oldShape.PropertyChanged -= Shape_PropertyChanged;
        }

        if (newShape != null)
        {
            newShape.PropertyChanged += Shape_PropertyChanged;
            UpdateControlValues();
        }

        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HasSelection)));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedShape)));
    }

    private void Shape_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (!_isUpdatingControls)
        {
            UpdateControlValues();
        }
    }

    private void UpdateControlValues()
    {
        if (SelectedShape == null) return;

        _isUpdatingControls = true;

        StrokeColorPicker.Color = SelectedShape.StrokeColor;
        StrokeThicknessSlider.Value = SelectedShape.StrokeThickness;
        StrokeThicknessNumberBox.Value = SelectedShape.StrokeThickness;
        FillColorPicker.Color = SelectedShape.FillColor;

        _isUpdatingControls = false;
    }

    private void StrokeColorPicker_ColorChanged(ColorPicker sender, ColorChangedEventArgs args)
    {
        if (_isUpdatingControls || SelectedShape == null) return;

        SelectedShape.StrokeColor = args.NewColor;
        ShapePropertyChanged?.Invoke(this, SelectedShape);
    }

    private void FillColorPicker_ColorChanged(ColorPicker sender, ColorChangedEventArgs args)
    {
        if (_isUpdatingControls || SelectedShape == null) return;

        SelectedShape.FillColor = args.NewColor;
        ShapePropertyChanged?.Invoke(this, SelectedShape);
    }

    private void StrokeThicknessSlider_ValueChanged(object sender, Microsoft.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
    {
        if (_isUpdatingControls || SelectedShape == null) return;

        _isUpdatingControls = true;
        SelectedShape.StrokeThickness = e.NewValue;
        StrokeThicknessNumberBox.Value = e.NewValue;
        _isUpdatingControls = false;

        ShapePropertyChanged?.Invoke(this, SelectedShape);
    }

    private void StrokeThicknessNumberBox_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
    {
        if (_isUpdatingControls || SelectedShape == null || double.IsNaN(args.NewValue)) return;

        _isUpdatingControls = true;
        SelectedShape.StrokeThickness = args.NewValue;
        StrokeThicknessSlider.Value = args.NewValue;
        _isUpdatingControls = false;

        ShapePropertyChanged?.Invoke(this, SelectedShape);
    }

    private void NoFill_Click(object sender, RoutedEventArgs e)
    {
        if (SelectedShape == null) return;

        SelectedShape.FillColor = Colors.Transparent;
        FillColorPicker.Color = Colors.Transparent;
        ShapePropertyChanged?.Invoke(this, SelectedShape);
    }

    private void DeleteShape_Click(object sender, RoutedEventArgs e)
    {
        if (SelectedShape != null)
        {
            ShapeDeleted?.Invoke(this, SelectedShape);
        }
    }
}
