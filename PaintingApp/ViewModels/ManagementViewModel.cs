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
    private readonly ITemplateGroupRepository _templateGroupRepository;
    private readonly IProfileStateService _profileStateService;

    [ObservableProperty]
    private ObservableCollection<string> _breadcrumbItems;

    [ObservableProperty]
    private int _selectedTabIndex;

    [ObservableProperty]
    private ObservableCollection<DrawingBoard> _boards = new();

    [ObservableProperty]
    private DrawingBoard? _selectedBoard;

    [ObservableProperty]
    private ObservableCollection<TemplateGroupViewModel> _templates = new();

    [ObservableProperty]
    private TemplateGroupViewModel? _selectedTemplate;

    public bool HasBoards => Boards.Count > 0;
    public bool HasTemplates => Templates.Count > 0;

    public ManagementViewModel(
        INavigationService navigationService,
        IDialogService dialogService,
        IDrawingBoardRepository drawingBoardRepository,
        ITemplateGroupRepository templateGroupRepository,
        IProfileStateService profileStateService)
        : base(navigationService, dialogService)
    {
        _drawingBoardRepository = drawingBoardRepository;
        _templateGroupRepository = templateGroupRepository;
        _profileStateService = profileStateService;
        Title = "Management";
        _breadcrumbItems = new ObservableCollection<string> { "Management", "Boards" };
    }

    public override async Task InitializeAsync()
    {
        await LoadBoardsAsync();
        await LoadTemplatesAsync();
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
    private async Task LoadTemplatesAsync()
    {
        if (_profileStateService.CurrentProfile == null)
            return;

        await ExecuteAsync(async () =>
        {
            var templateGroups = await _templateGroupRepository.GetByProfileIdAsync(
                _profileStateService.CurrentProfile.Id);

            Templates.Clear();
            foreach (var template in templateGroups)
            {
                var shapeCount = await _templateGroupRepository.GetShapeCountAsync(template.Id);
                
                Templates.Add(new TemplateGroupViewModel
                {
                    Id = template.Id,
                    Name = template.Name,
                    Description = template.Description,
                    UsageCount = template.UsageCount,
                    ShapeCount = shapeCount,
                    CreatedDate = template.CreatedDate,
                    Thumbnail = null // Placeholder: thumbnail generation can be added later
                });
            }
            OnPropertyChanged(nameof(HasTemplates));
        });
    }

    [RelayCommand]
    private async Task RefreshAllAsync()
    {
        await LoadBoardsAsync();
        await LoadTemplatesAsync();
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

    [RelayCommand]
    private async Task DeleteTemplateAsync(TemplateGroupViewModel? template)
    {
        if (template == null)
            return;

        var confirmed = await DialogService.ShowConfirmationAsync(
            "Delete Template?",
            $"This will permanently delete \"{template.Name}\" and all its shapes. This action cannot be undone.",
            "Delete",
            "Cancel"
        );

        if (!confirmed)
            return;

        var templateExists = await _templateGroupRepository.ExistsAsync(template.Id);

        if (!templateExists)
        {
            Templates.Remove(template);
            OnPropertyChanged(nameof(HasTemplates));
            await DialogService.ShowMessageAsync(
                "Template Not Found",
                "This template has already been deleted."
            );
            return;
        }

        await ExecuteAsync(async () =>
        {
            await _templateGroupRepository.DeleteAsync(template.Id);
            Templates.Remove(template);
            OnPropertyChanged(nameof(HasTemplates));
        },
        onError: async ex =>
        {
            await DialogService.ShowErrorAsync(
                "Delete Failed",
                "Unable to delete the template. Please try again."
            );
        });
    }

    public void AddTemplate(TemplateGroupViewModel template)
    {
        Templates.Insert(0, template);
        OnPropertyChanged(nameof(HasTemplates));
    }
}
