using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PaintingApp.Contracts;
using PaintingApp.Data.Entities;
using PaintingApp.Data.Repositories.Interfaces;

namespace PaintingApp.ViewModels;

public partial class ManagementViewModel : BaseViewModel
{
    private readonly IDrawingBoardRepository _drawingBoardRepository;
    private readonly IProfileStateService _profileStateService;

    [ObservableProperty]
    private ObservableCollection<string> _breadcrumbItems;

    [ObservableProperty]
    private int _selectedTabIndex;

    [ObservableProperty]
    private ObservableCollection<DrawingBoard> _boards = new();

    public ManagementViewModel(
        INavigationService navigationService,
        IDialogService dialogService,
        IDrawingBoardRepository drawingBoardRepository,
        IProfileStateService profileStateService)
        : base(navigationService, dialogService)
    {
        _drawingBoardRepository = drawingBoardRepository;
        _profileStateService = profileStateService;
        Title = "Management";
        _breadcrumbItems = new ObservableCollection<string> { "Management", "Boards" };
    }

    public override async Task InitializeAsync()
    {
        await LoadBoardsAsync();
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

    [RelayCommand]
    private async Task LoadBoardsAsync()
    {
        if (_profileStateService.CurrentProfile == null)
            return;

        await ExecuteAsync(async () =>
        {
            var boards = await _drawingBoardRepository.GetByProfileIdAsync(
                _profileStateService.CurrentProfile.Id);

            Boards.Clear();
            foreach (var board in boards)
            {
                Boards.Add(board);
            }
        });
    }

    [RelayCommand]
    private void OpenBoard(DrawingBoard? board)
    {
        if (board == null) return;
        NavigationService.NavigateTo("Drawing", board.Id);
    }
}
