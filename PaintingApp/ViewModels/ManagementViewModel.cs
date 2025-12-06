using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using PaintingApp.Contracts;
using PaintingApp.Data.Entities;
using PaintingApp.Data.Repositories.Interfaces;
using PaintingApp.Models;
using SkiaSharp;

namespace PaintingApp.ViewModels;

public partial class ManagementViewModel : BaseViewModel
{
    private readonly IDrawingBoardRepository _drawingBoardRepository;
    private readonly ITemplateGroupRepository _templateGroupRepository;
    private readonly IShapeRepository _shapeRepository;
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

    [ObservableProperty]
    private ObservableCollection<ShapeTypeUsageItem> _shapeTypeUsage = [];

    [ObservableProperty]
    private ObservableCollection<TemplateUsageItem> _topTemplates = [];

    [ObservableProperty]
    private int _totalShapeCount;

    [ObservableProperty]
    private int _totalTemplateUsageCount;

    [ObservableProperty]
    private bool _isDashboardLoading;

    [ObservableProperty]
    private ISeries[] _shapeTypeSeries = [];

    [ObservableProperty]
    private ISeries[] _topTemplatesSeries = [];

    [ObservableProperty]
    private ICartesianAxis[] _topTemplatesXAxes = [];

    [ObservableProperty]
    private ICartesianAxis[] _topTemplatesYAxes = [];

    public bool HasBoards => Boards.Count > 0;
    public bool HasTemplates => Templates.Count > 0;
    public bool HasBoardsLoaded => _allBoards.Count > 0;
    public bool HasTemplatesLoaded => _allTemplates.Count > 0;
    public bool HasShapeTypeData => ShapeTypeUsage.Count > 0;
    public bool HasTopTemplatesData => TopTemplates.Count > 0;

    public ManagementViewModel(
        INavigationService navigationService,
        IDialogService dialogService,
        IDrawingBoardRepository drawingBoardRepository,
        ITemplateGroupRepository templateGroupRepository,
        IShapeRepository shapeRepository,
        IProfileStateService profileStateService)
        : base(navigationService, dialogService)
    {
        _drawingBoardRepository = drawingBoardRepository;
        _templateGroupRepository = templateGroupRepository;
        _shapeRepository = shapeRepository;
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
        
        if (value == 2)
        {
            _ = LoadDashboardDataAsync();
        }
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
    private async Task LoadDashboardDataAsync()
    {
        if (_profileStateService.CurrentProfile == null)
            return;

        IsDashboardLoading = true;

        try
        {
            await Task.WhenAll(
                LoadShapeTypeUsageAsync(),
                LoadTopTemplatesAsync()
            );
        }
        finally
        {
            IsDashboardLoading = false;
        }
    }

    private async Task LoadShapeTypeUsageAsync()
    {
        if (_profileStateService.CurrentProfile == null)
            return;

        var statistics = await _shapeRepository.GetShapeTypeStatisticsByProfileIdAsync(
            _profileStateService.CurrentProfile.Id);

        ShapeTypeUsage.Clear();
        TotalShapeCount = statistics.Values.Sum();

        var seriesList = new List<ISeries>();
        var index = 0;

        foreach (var stat in statistics.OrderByDescending(s => s.Value))
        {
            var percentage = TotalShapeCount > 0 
                ? (double)stat.Value / TotalShapeCount * 100 
                : 0;

            var color = ShapeTypeUsageItem.GetColorForIndex(index);
            
            ShapeTypeUsage.Add(new ShapeTypeUsageItem
            {
                ShapeType = stat.Key.ToString(),
                Count = stat.Value,
                Percentage = percentage,
                Color = color
            });

            seriesList.Add(new PieSeries<int>
            {
                Values = [stat.Value],
                Name = stat.Key.ToString(),
                Fill = new SolidColorPaint(new SKColor(color.R, color.G, color.B)),
                DataLabelsSize = 12,
                DataLabelsPosition = LiveChartsCore.Measure.PolarLabelsPosition.Outer,
                DataLabelsPaint = new SolidColorPaint(SKColors.Gray),
                DataLabelsFormatter = point => $"{stat.Key}: {stat.Value}"
            });

            index++;
        }

        ShapeTypeSeries = [.. seriesList];
        OnPropertyChanged(nameof(HasShapeTypeData));
    }

    private async Task LoadTopTemplatesAsync(int topN = 5)
    {
        if (_profileStateService.CurrentProfile == null)
            return;

        var templates = await _templateGroupRepository.GetTopUsedByProfileIdAsync(
            _profileStateService.CurrentProfile.Id, topN);

        TopTemplates.Clear();
        var templateList = templates.ToList();
        TotalTemplateUsageCount = templateList.Sum(t => t.UsageCount);

        var templateNames = new List<string>();
        var usageCounts = new List<int>();
        var index = 0;

        foreach (var template in templateList)
        {
            var percentage = TotalTemplateUsageCount > 0 
                ? (double)template.UsageCount / TotalTemplateUsageCount * 100 
                : 0;

            var color = TemplateUsageItem.GetColorForIndex(index);

            TopTemplates.Add(new TemplateUsageItem
            {
                Id = template.Id,
                TemplateName = template.Name,
                UsageCount = template.UsageCount,
                Percentage = percentage,
                Color = color
            });

            templateNames.Add(template.Name);
            usageCounts.Add(template.UsageCount);
            index++;
        }

        if (usageCounts.Count > 0)
        {
            TopTemplatesSeries =
            [
                new ColumnSeries<int>
                {
                    Values = usageCounts,
                    Name = "Usage Count",
                    Fill = new SolidColorPaint(new SKColor(0, 120, 215)),
                    MaxBarWidth = 40,
                    Padding = 8
                }
            ];

            TopTemplatesXAxes =
            [
                new Axis
                {
                    Labels = templateNames,
                    LabelsRotation = 0,
                    TextSize = 11,
                    LabelsPaint = new SolidColorPaint(SKColors.Gray)
                }
            ];

            TopTemplatesYAxes =
            [
                new Axis
                {
                    MinLimit = 0,
                    MinStep = 1,
                    TextSize = 11,
                    LabelsPaint = new SolidColorPaint(SKColors.Gray)
                }
            ];
        }
        else
        {
            TopTemplatesSeries = [];
            TopTemplatesXAxes = [];
            TopTemplatesYAxes = [];
        }

        OnPropertyChanged(nameof(HasTopTemplatesData));
    }

    [RelayCommand]
    private async Task RefreshDashboardAsync()
    {
        await LoadDashboardDataAsync();
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
        
        if (SelectedTabIndex == 2)
        {
            await LoadDashboardDataAsync();
        }
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
