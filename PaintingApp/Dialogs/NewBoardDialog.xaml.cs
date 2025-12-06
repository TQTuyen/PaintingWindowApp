using System;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using PaintingApp.Data.Entities;
using Windows.UI;

namespace PaintingApp.Dialogs;

public sealed partial class NewBoardDialog : ContentDialog
{
    private readonly Profile _profile;
    private string _backgroundColor = "#FFFFFF";

    public DrawingBoard? Result { get; private set; }

    public NewBoardDialog(Profile profile)
    {
        InitializeComponent();
        _profile = profile;

        if (profile != null)
        {
            WidthNumberBox.Value = profile.DefaultCanvasWidth;
            HeightNumberBox.Value = profile.DefaultCanvasHeight;
        }

        NameTextBox.Text = $"Board {DateTime.Now:yyyy-MM-dd HH:mm}";
        NameTextBox.SelectAll();
    }

    private void NameTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        NameErrorText.Visibility = Visibility.Collapsed;
    }

    private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        if (!ValidateInput())
        {
            args.Cancel = true;
            return;
        }

        Result = new DrawingBoard
        {
            Name = NameTextBox.Text.Trim(),
            ProfileId = _profile.Id,
            Width = (int)WidthNumberBox.Value,
            Height = (int)HeightNumberBox.Value,
            BackgroundColor = _backgroundColor,
            CreatedDate = DateTime.UtcNow,
            LastModified = DateTime.UtcNow
        };
    }

    private bool ValidateInput()
    {
        var name = NameTextBox.Text?.Trim();

        if (string.IsNullOrWhiteSpace(name))
        {
            NameErrorText.Text = "Board name is required";
            NameErrorText.Visibility = Visibility.Visible;
            NameTextBox.Focus(FocusState.Programmatic);
            return false;
        }

        return true;
    }

    private void ChooseBackgroundColor_Click(object sender, RoutedEventArgs e)
    {
        var button = sender as Button;
        if (button == null) return;

        var colorPicker = new ColorPicker
        {
            Color = ParseColor(_backgroundColor),
            ColorSpectrumShape = ColorSpectrumShape.Ring,
            IsAlphaEnabled = false
        };

        var confirmButton = new Button
        {
            Content = "OK",
            HorizontalAlignment = HorizontalAlignment.Right,
            Margin = new Thickness(0, 10, 0, 0)
        };

        var panel = new StackPanel();
        panel.Children.Add(colorPicker);
        panel.Children.Add(confirmButton);

        var flyout = new Flyout
        {
            Content = panel
        };

        confirmButton.Click += (s, args) =>
        {
            _backgroundColor = ColorToHex(colorPicker.Color);
            UpdateBackgroundPreview();
            flyout.Hide();
        };

        flyout.ShowAt(button);
    }

    private void UpdateBackgroundPreview()
    {
        BackgroundColorPreview.Background = new SolidColorBrush(ParseColor(_backgroundColor));
    }

    private static Color ParseColor(string hex)
    {
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

    private static string ColorToHex(Color color)
    {
        return $"#{color.R:X2}{color.G:X2}{color.B:X2}";
    }
}
