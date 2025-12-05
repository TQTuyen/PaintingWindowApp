using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using PaintingApp.Contracts;
using PaintingApp.Views;

namespace PaintingApp;

public sealed partial class MainWindow : Window
{
    private readonly INavigationService _navigationService;

    public MainWindow()
    {
        InitializeComponent();

        ExtendsContentIntoTitleBar = true;
        SetTitleBar(AppTitleBar);
        SetupMicaBackdrop();

        _navigationService = App.GetService<INavigationService>();
        _navigationService.Frame = ContentFrame;

        RegisterPages();

        ContentFrame.Navigated += OnFrameNavigated;
    }

    private void RegisterPages()
    {
        _navigationService.RegisterPage("Management", typeof(ManagementView));
    }

    private void SetupMicaBackdrop()
    {
        if (MicaController.IsSupported())
        {
            SystemBackdrop = new MicaBackdrop { Kind = MicaKind.Base };
        }
    }

    private void OnFrameNavigated(object sender, NavigationEventArgs e)
    {
        BackButton.IsEnabled = _navigationService.CanGoBack;
    }

    private void BackButton_Click(object sender, RoutedEventArgs e)
    {
        _navigationService.GoBack();
    }

    private void NavView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {
        if (args.IsSettingsSelected)
        {
            return;
        }

        if (args.SelectedItemContainer is NavigationViewItem selectedItem)
        {
            var tag = selectedItem.Tag?.ToString();
            if (!string.IsNullOrEmpty(tag))
            {
                _navigationService.NavigateTo(tag);
            }
        }
    }

    // Adjusts title bar margin when NavigationView switches to Minimal mode (hamburger button appears)
    private void NavView_DisplayModeChanged(NavigationView sender, NavigationViewDisplayModeChangedEventArgs args)
    {
        AppTitleBar.Margin = new Thickness
        {
            Left = args.DisplayMode == NavigationViewDisplayMode.Minimal ? 48 : 0,
            Top = 0,
            Right = 0,
            Bottom = 0
        };
    }
}
