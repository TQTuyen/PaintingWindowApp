using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PaintingApp.Contracts;
using PaintingApp.Data.Entities;
using PaintingApp.Data.Repositories.Interfaces;

namespace PaintingApp.ViewModels;

public partial class MainScreenViewModel : BaseViewModel
{
    private readonly IProfileRepository _profileRepository;

    [ObservableProperty]
    private ObservableCollection<Profile> _profiles = new();

    [ObservableProperty]
    private Profile? _selectedProfile;

    public MainScreenViewModel(
        INavigationService navigationService,
        IDialogService dialogService,
        IProfileRepository profileRepository)
        : base(navigationService, dialogService)
    {
        Title = "Welcome";
        _profileRepository = profileRepository;
    }

    public override async Task InitializeAsync()
    {
        await LoadProfilesAsync();
    }

    [RelayCommand]
    private async Task LoadProfilesAsync()
    {
        await ExecuteAsync(async () =>
        {
            var profiles = await _profileRepository.GetAllAsync();
            _profiles.Clear();
            foreach (var profile in profiles)
            {
                _profiles.Add(profile);
            }
        });
    }

    [RelayCommand]
    private async Task SelectProfileAsync(Profile? profile)
    {
        if (profile == null) return;

        var confirmed = await DialogService.ShowConfirmationAsync(
            "Select Profile",
            $"Do you want to continue with profile '{profile.Name}'?");

        if (confirmed)
        {
            _selectedProfile = profile;
            NavigationService.NavigateTo("Management", profile);
        }
    }

    [RelayCommand]
    private async Task CreateProfileAsync()
    {
        await DialogService.ShowMessageAsync("Coming Soon", "Profile creation will be available in a future update.");
    }
}
