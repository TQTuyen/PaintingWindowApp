using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using PaintingApp.Contracts;

namespace PaintingApp.ViewModels;

public partial class ManagementViewModel : BaseViewModel
{
    [ObservableProperty]
    private ObservableCollection<string> _breadcrumbItems;

    [ObservableProperty]
    private int _selectedTabIndex;

    public ManagementViewModel(INavigationService navigationService, IDialogService dialogService)
        : base(navigationService, dialogService)
    {
        Title = "Management";
        _breadcrumbItems = new ObservableCollection<string> { "Management", "Boards" };
    }

    partial void OnSelectedTabIndexChanged(int value)
    {
        UpdateBreadcrumb(value);
    }

    private void UpdateBreadcrumb(int tabIndex)
    {
        var tabName = tabIndex switch
        {
            0 => "Boards",
            1 => "Templates",
            2 => "Dashboard",
            _ => "Boards"
        };

        if (BreadcrumbItems.Count > 1)
        {
            BreadcrumbItems[1] = tabName;
        }
        else
        {
            BreadcrumbItems.Add(tabName);
        }
    }

    public override void OnNavigatedTo(object? parameter)
    {
        base.OnNavigatedTo(parameter);
        SelectedTabIndex = 0;
    }
}
