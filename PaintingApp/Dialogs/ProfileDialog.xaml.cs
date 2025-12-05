using System;
using System.Threading.Tasks;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using PaintingApp.Data.Entities;
using PaintingApp.Data.Repositories.Interfaces;
using Windows.UI;

namespace PaintingApp.Dialogs;

public sealed partial class ProfileDialog : ContentDialog
{
    private readonly IProfileRepository _profileRepository;
    private readonly Profile? _profile;
    private readonly bool _isEditMode;
    private string _selectedStrokeColor = "#000000";

    public Profile? Result { get; private set; }

    public ProfileDialog(IProfileRepository profileRepository, Profile? profile = null)
    {
        InitializeComponent();
        _profileRepository = profileRepository;
        _profile = profile;
        _isEditMode = profile != null;

        if (_isEditMode)
        {
            Title = "Edit Profile";
            LoadProfileData();
        }
        else
        {
            Title = "Create New Profile";
        }
    }

    private void LoadProfileData()
    {
        if (_profile == null) return;

        NameTextBox.Text = _profile.Name;
        DescriptionTextBox.Text = _profile.Description ?? string.Empty;

        ThemeComboBox.SelectedIndex = _profile.Theme?.ToLowerInvariant() switch
        {
            "light" => 1,
            "dark" => 2,
            _ => 0
        };

        CanvasWidthNumberBox.Value = _profile.DefaultCanvasWidth;
        CanvasHeightNumberBox.Value = _profile.DefaultCanvasHeight;
        StrokeThicknessNumberBox.Value = _profile.DefaultStrokeThickness;

        _selectedStrokeColor = _profile.DefaultStrokeColor ?? "#000000";

        StrokeStyleComboBox.SelectedIndex = _profile.DefaultStrokeStyle?.ToLowerInvariant() switch
        {
            "dashed" => 1,
            "dotted" => 2,
            _ => 0
        };

        UpdateColorPreview();
    }

    private void NameTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        NameErrorText.Visibility = Visibility.Collapsed;
    }

    private async void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        var deferral = args.GetDeferral();

        try
        {
            if (!await ValidateInputAsync())
            {
                args.Cancel = true;
                return;
            }

            var theme = (ThemeComboBox.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? "System";
            var strokeStyle = (StrokeStyleComboBox.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? "Solid";

            if (_isEditMode && _profile != null)
            {
                _profile.Name = NameTextBox.Text.Trim();
                _profile.Description = string.IsNullOrWhiteSpace(DescriptionTextBox.Text) ? null : DescriptionTextBox.Text.Trim();
                _profile.Theme = theme;
                _profile.DefaultCanvasWidth = (int)CanvasWidthNumberBox.Value;
                _profile.DefaultCanvasHeight = (int)CanvasHeightNumberBox.Value;
                _profile.DefaultStrokeThickness = StrokeThicknessNumberBox.Value;
                _profile.DefaultStrokeColor = _selectedStrokeColor;
                _profile.DefaultStrokeStyle = strokeStyle;

                Result = _profile;
            }
            else
            {
                Result = new Profile
                {
                    Name = NameTextBox.Text.Trim(),
                    Description = string.IsNullOrWhiteSpace(DescriptionTextBox.Text) ? null : DescriptionTextBox.Text.Trim(),
                    Theme = theme,
                    DefaultCanvasWidth = (int)CanvasWidthNumberBox.Value,
                    DefaultCanvasHeight = (int)CanvasHeightNumberBox.Value,
                    DefaultStrokeThickness = StrokeThicknessNumberBox.Value,
                    DefaultStrokeColor = _selectedStrokeColor,
                    DefaultStrokeStyle = strokeStyle,
                    CreatedDate = DateTime.UtcNow
                };
            }
        }
        finally
        {
            deferral.Complete();
        }
    }

    private async Task<bool> ValidateInputAsync()
    {
        var name = NameTextBox.Text?.Trim();

        if (string.IsNullOrWhiteSpace(name))
        {
            NameErrorText.Text = "Profile name is required";
            NameErrorText.Visibility = Visibility.Visible;
            NameTextBox.Focus(FocusState.Programmatic);
            return false;
        }

        var excludeId = _isEditMode ? _profile?.Id : null;
        var isUnique = await _profileRepository.IsNameUniqueAsync(name, excludeId);

        if (!isUnique)
        {
            NameErrorText.Text = "A profile with this name already exists";
            NameErrorText.Visibility = Visibility.Visible;
            NameTextBox.Focus(FocusState.Programmatic);
            return false;
        }

        return true;
    }

    private async void ChooseStrokeColor_Click(object sender, RoutedEventArgs e)
    {
        var button = sender as Button;
        if (button == null) return;

        var colorPicker = new ColorPicker
        {
            Color = ParseColor(_selectedStrokeColor),
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
            _selectedStrokeColor = ColorToHex(colorPicker.Color);
            UpdateColorPreview();
            flyout.Hide();
        };

        flyout.ShowAt(button);
    }

    private void UpdateColorPreview()
    {
        StrokeColorPreview.Background = new SolidColorBrush(ParseColor(_selectedStrokeColor));
    }

    // Parses hex color string (#RGB, #RRGGBB, or #AARRGGBB) to Color
    private static Color ParseColor(string hex)
    {
        hex = hex.TrimStart('#');

        if (hex.Length == 6)
        {
            hex = "FF" + hex;
        }

        if (hex.Length == 8)
        {
            return Color.FromArgb(
                Convert.ToByte(hex[..2], 16),
                Convert.ToByte(hex[2..4], 16),
                Convert.ToByte(hex[4..6], 16),
                Convert.ToByte(hex[6..8], 16)
            );
        }

        return Colors.Black;
    }

    private static string ColorToHex(Color color)
    {
        return $"#{color.R:X2}{color.G:X2}{color.B:X2}";
    }
}
