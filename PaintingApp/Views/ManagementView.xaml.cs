using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using PaintingApp.ViewModels;

namespace PaintingApp.Views;

public sealed partial class ManagementView : Page
{
    public ManagementViewModel ViewModel { get; }

    public ManagementView()
    {
        ViewModel = App.GetService<ManagementViewModel>();
        InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        ViewModel.OnNavigatedTo(e.Parameter);
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        base.OnNavigatedFrom(e);
        ViewModel.OnNavigatedFrom();
    }
}
