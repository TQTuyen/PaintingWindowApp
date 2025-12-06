using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI;
using PaintingApp.Contracts;
using PaintingApp.Data.Entities;
using Windows.UI;

namespace PaintingApp.ViewModels;

public partial class DrawingScreenViewModel : BaseViewModel
{
    private readonly IProfileStateService _profileStateService;

    [ObservableProperty]
    private Profile? _currentProfile;

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

    public DrawingScreenViewModel(
        INavigationService navigationService,
        IDialogService dialogService,
        IProfileStateService profileStateService)
        : base(navigationService, dialogService)
    {
        _profileStateService = profileStateService;
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
    private void SelectTool(string toolName)
    {
        SelectedTool = toolName;
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
                System.Convert.ToByte(hex.Substring(0, 2), 16),
                System.Convert.ToByte(hex.Substring(2, 2), 16),
                System.Convert.ToByte(hex.Substring(4, 2), 16),
                System.Convert.ToByte(hex.Substring(6, 2), 16)
            );
        }

        return Colors.Black;
    }
}
