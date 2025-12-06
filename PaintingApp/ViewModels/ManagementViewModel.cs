using System;
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

    private ObservableCollection<BoardViewModel> _allBoards = [];
    private ObservableCollection<TemplateGroupViewModel> _allTemplates = [];

    [ObservableProperty]
    private ObservableCollection<string> _breadcrumbItems;

    [ObservableProperty]
    private int _selectedTabIndex;

    [ObservableProperty]
    private ObservableCollection<BoardViewModel> _boards = [];

    [ObservableProperty]
    private BoardViewModel? _selectedBoard;

    [ObservableProperty]
    private string _boardSearchText = string.Empty;

    [ObservableProperty]
    private ObservableCollection<TemplateGroupViewModel> _templates = [];

    [ObservableProperty]
    private TemplateGroupViewModel? _selectedTemplate;

    [ObservableProperty]
    private string _templateSearchText = string.Empty;

    public bool HasBoards => Boards.Count > 0;
    public bool HasTemplates => Templates.Count > 0;
    public bool HasBoardsLoaded => _allBoards.Count > 0;
    public bool HasTemplatesLoaded => _allTemplates.Count > 0;

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
        _breadcrumbItems = ["Management", "Boards"];
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

    partial void OnBoardSearchTextChanged(string value)
    {
        FilterBoards();
    }

    partial void OnTemplateSearchTextChanged(string value)
    {
        FilterTemplates();
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

            _allBoards.Clear();
            foreach (var board in boards.OrderByDescending(b => b.LastModified))
            {
                var shapeCount = await _drawingBoardRepository.GetShapeCountAsync(board.Id);
                _allBoards.Add(BoardViewModel.FromEntity(board, shapeCount));
            }

            FilterBoards();
            OnPropertyChanged(nameof(HasBoardsLoaded));
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

            _allTemplates.Clear();
            foreach (var template in templateGroups.OrderByDescending(t => t.UsageCount))
            {
                var shapeCount = await _templateGroupRepository.GetShapeCountAsync(template.Id);

                _allTemplates.Add(new TemplateGroupViewModel
                {
                    Id = template.Id,
                    Name = template.Name,
                    Description = template.Description,
                    UsageCount = template.UsageCount,
                    ShapeCount = shapeCount,
                    CreatedDate = template.CreatedDate
                });
            }

            FilterTemplates();
            OnPropertyChanged(nameof(HasTemplatesLoaded));
        });
    }

    private void FilterBoards()
    {
        Boards.Clear();

        var filtered = string.IsNullOrWhiteSpace(BoardSearchText)
            ? _allBoards
            : _allBoards.Where(b => 
                b.Name.Contains(BoardSearchText, StringComparison.OrdinalIgnoreCase) ||
                b.SizeDisplay.Contains(BoardSearchText, StringComparison.OrdinalIgnoreCase));

        foreach (var board in filtered)
        {
            Boards.Add(board);
        }

        OnPropertyChanged(nameof(HasBoards));
    }

    private void FilterTemplates()
    {
        Templates.Clear();

        var filtered = string.IsNullOrWhiteSpace(TemplateSearchText)
            ? _allTemplates
            : _allTemplates.Where(t =>
                t.Name.Contains(TemplateSearchText, StringComparison.OrdinalIgnoreCase) ||
                (t.Description?.Contains(TemplateSearchText, StringComparison.OrdinalIgnoreCase) ?? false));

        foreach (var template in filtered)
        {
            Templates.Add(template);
        }

        OnPropertyChanged(nameof(HasTemplates));
    }

    [RelayCommand]
    private void ClearBoardSearch()
    {
        BoardSearchText = string.Empty;
    }

    [RelayCommand]
    private void ClearTemplateSearch()
    {
        TemplateSearchText = string.Empty;
    }

    [RelayCommand]
    private async Task RefreshAllAsync()
    {
        await LoadBoardsAsync();
        await LoadTemplatesAsync();
    }

    [RelayCommand]
    private void OpenBoard(BoardViewModel? board)
    {
        if (board == null) return;
        NavigationService.NavigateTo("Drawing", board.Id);
    }

    [RelayCommand]
    private async Task DeleteBoardAsync(BoardViewModel? board)
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
            _allBoards.Remove(board);
            FilterBoards();
            await DialogService.ShowMessageAsync(
                "Board Not Found",
                "This board has already been deleted."
            );
            return;
        }

        await ExecuteAsync(async () =>
        {
            await _drawingBoardRepository.DeleteAsync(board.Id);
            _allBoards.Remove(board);
            FilterBoards();
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
            _allTemplates.Remove(template);
            FilterTemplates();
            await DialogService.ShowMessageAsync(
                "Template Not Found",
                "This template has already been deleted."
            );
            return;
        }

        await ExecuteAsync(async () =>
        {
            await _templateGroupRepository.DeleteAsync(template.Id);
            _allTemplates.Remove(template);
            FilterTemplates();
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
        _allTemplates.Insert(0, template);
        FilterTemplates();
    }

    public void UpdateTemplateUsageCount(int templateId)
    {
        var template = _allTemplates.FirstOrDefault(t => t.Id == templateId);
        if (template != null)
        {
            template.UsageCount++;
        }
    }
}
