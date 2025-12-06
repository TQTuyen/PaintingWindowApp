using System;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using PaintingApp.Models;
using PaintingApp.ViewModels;

namespace PaintingApp.Controls
{
    public sealed partial class PropertiesPanel : UserControl
    {
        public PropertiesPanelViewModel ViewModel { get; private set; } = null!;

        public static readonly DependencyProperty SelectedShapeProperty =
            DependencyProperty.Register(nameof(SelectedShape), typeof(ShapeModel),
            typeof(PropertiesPanel), new PropertyMetadata(null, OnSelectedShapeChanged));

        public ShapeModel? SelectedShape
        {
            get => (ShapeModel?)GetValue(SelectedShapeProperty);
            set => SetValue(SelectedShapeProperty, value);
        }

        private bool _isUpdatingControls;

        public PropertiesPanel()
        {
            this.InitializeComponent();
            ViewModel = App.GetService<PropertiesPanelViewModel>();

            ViewModel.ShapeDeleted += (s, shape) => ShapeDeleted?.Invoke(this, shape);
            ViewModel.ShapePropertyChanged += (s, shape) => ShapePropertyChanged?.Invoke(this, shape);
        }

        private static void OnSelectedShapeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var panel = (PropertiesPanel)d;
            panel.ViewModel.SelectedShape = e.NewValue as ShapeModel;
            panel.UpdateControlValues();
        }

        private void UpdateControlValues()
        {
            if (ViewModel?.SelectedShape == null) return;

            _isUpdatingControls = true;

            StrokeColorPicker.Color = ViewModel.SelectedShape.StrokeColor;
            StrokeThicknessSlider.Value = ViewModel.SelectedShape.StrokeThickness;
            StrokeThicknessNumberBox.Value = ViewModel.SelectedShape.StrokeThickness;
            FillColorPicker.Color = ViewModel.SelectedShape.FillColor;

            _isUpdatingControls = false;
        }

        private void StrokeColorPicker_ColorChanged(ColorPicker sender, ColorChangedEventArgs args)
        {
            if (_isUpdatingControls || ViewModel?.SelectedShape == null) return;

            ViewModel.SelectedShape.StrokeColor = args.NewColor;
            ShapePropertyChanged?.Invoke(this, ViewModel.SelectedShape);
        }

        private void FillColorPicker_ColorChanged(ColorPicker sender, ColorChangedEventArgs args)
        {
            if (_isUpdatingControls || ViewModel?.SelectedShape == null) return;

            ViewModel.SelectedShape.FillColor = args.NewColor;
            ShapePropertyChanged?.Invoke(this, ViewModel.SelectedShape);
        }

        private void StrokeThicknessSlider_ValueChanged(object sender, Microsoft.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            if (_isUpdatingControls || ViewModel?.SelectedShape == null) return;

            _isUpdatingControls = true;
            ViewModel.SelectedShape.StrokeThickness = e.NewValue;
            StrokeThicknessNumberBox.Value = e.NewValue;
            _isUpdatingControls = false;

            ShapePropertyChanged?.Invoke(this, ViewModel.SelectedShape);
        }

        private void StrokeThicknessNumberBox_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            if (_isUpdatingControls || ViewModel?.SelectedShape == null || double.IsNaN(args.NewValue)) return;

            _isUpdatingControls = true;
            ViewModel.SelectedShape.StrokeThickness = args.NewValue;
            StrokeThicknessSlider.Value = args.NewValue;
            _isUpdatingControls = false;

            ShapePropertyChanged?.Invoke(this, ViewModel.SelectedShape);
        }

        private void NoFill_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel?.SelectedShape == null) return;

            ViewModel.SelectedShape.FillColor = Colors.Transparent;
            FillColorPicker.Color = Colors.Transparent;
            ShapePropertyChanged?.Invoke(this, ViewModel.SelectedShape);
        }

        public event EventHandler<ShapeModel>? ShapeDeleted;
        public event EventHandler<ShapeModel>? ShapePropertyChanged;
    }
}
