using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using PaintingApp.Data.Entities;
using PaintingApp.ViewModels;

namespace PaintingApp.Views;

public sealed partial class MainScreenView : Page
{
    public MainScreenViewModel ViewModel { get; }

    public MainScreenView()
    {
        InitializeComponent();
        ViewModel = App.GetService<MainScreenViewModel>();
        DataContext = ViewModel;
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        await ViewModel.InitializeAsync();
    }

    private void ProfileGridView_ItemClick(object sender, ItemClickEventArgs e)
    {
        if (e.ClickedItem is Profile profile)
        {
            ViewModel.SelectProfileCommand.Execute(profile);
        }
    }

    private void SelectProfile_Click(object sender, RoutedEventArgs e)
    {
        if (sender is MenuFlyoutItem menuItem && menuItem.Tag is Profile profile)
        {
            ViewModel.SelectProfileCommand.Execute(profile);
        }
    }

    private void EditProfile_Click(object sender, RoutedEventArgs e)
    {
        if (sender is MenuFlyoutItem menuItem && menuItem.Tag is Profile profile)
        {
            ViewModel.EditProfileCommand.Execute(profile);
        }
    }

    private void DeleteProfile_Click(object sender, RoutedEventArgs e)
    {
        if (sender is MenuFlyoutItem menuItem && menuItem.Tag is Profile profile)
        {
            ViewModel.DeleteProfileCommand.Execute(profile);
        }
    }
}
