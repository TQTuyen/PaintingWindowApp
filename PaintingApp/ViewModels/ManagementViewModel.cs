using System.Collections.ObjectModel;
using System.Linq;
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

    [ObservableProperty]
    private DrawingBoard? _selectedBoard;

    public bool HasBoards => Boards.Count > 0;

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
            OnPropertyChanged(nameof(HasBoards));
        });
    }

    [RelayCommand]
    private void OpenBoard(DrawingBoard? board)
    {
        if (board == null) return;
        NavigationService.NavigateTo("Drawing", board.Id);
    }

    [RelayCommand]
    private async Task DeleteBoardAsync(DrawingBoard? board)
    {
        if (board == null)
            return;

        var confirmed = await DialogService.ShowConfirmationAsync(
            "Delete Board?",
            $"This will permanently delete \"{board.Name}\" and all its shapes. This action cannot be undone.",
            "Delete",
            "Cancel"
        );

        if (!confirmed)
            return;

        var boardExists = await _drawingBoardRepository.ExistsAsync(board.Id);

        if (!boardExists)
        {
            Boards.Remove(board);
            OnPropertyChanged(nameof(HasBoards));
            await DialogService.ShowMessageAsync(
                "Board Not Found",
                "This board has already been deleted."
            );
            return;
        }

        await ExecuteAsync(async () =>
        {
            await _drawingBoardRepository.DeleteAsync(board.Id);
            Boards.Remove(board);
            OnPropertyChanged(nameof(HasBoards));
        },
        onError: async ex =>
        {
            await DialogService.ShowErrorAsync(
                "Delete Failed",
                "Unable to delete the board. Please try again."
            );
        });
    }
}
