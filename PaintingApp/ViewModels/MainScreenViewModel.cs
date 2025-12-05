using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using PaintingApp.Contracts;
using PaintingApp.Data.Entities;
using PaintingApp.Data.Repositories.Interfaces;
using PaintingApp.Dialogs;

namespace PaintingApp.ViewModels;

public partial class MainScreenViewModel : BaseViewModel
{
    private readonly IProfileRepository _profileRepository;
    private readonly IProfileStateService _profileStateService;
    private readonly IThemeService _themeService;

    [ObservableProperty]
    private ObservableCollection<Profile> _profiles = new();

    [ObservableProperty]
    private Profile? _selectedProfile;

    public MainScreenViewModel(
        INavigationService navigationService,
        IDialogService dialogService,
        IProfileRepository profileRepository,
        IProfileStateService profileStateService,
        IThemeService themeService)
        : base(navigationService, dialogService)
    {
        Title = "Welcome";
        _profileRepository = profileRepository;
        _profileStateService = profileStateService;
        _themeService = themeService;
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
            Profiles.Clear();
            foreach (var profile in profiles)
            {
                Profiles.Add(profile);
            }
        });
    }

    [RelayCommand]
    private async Task SelectProfileAsync(Profile? profile)
    {
        if (profile == null) return;

        var confirmed = await DialogService.ShowConfirmationAsync(
            "Select Profile",
            $"Use profile '{profile.Name}'?\n\nThis will apply the profile's theme and canvas settings.");

        if (confirmed)
        {
            _profileStateService.SetProfile(profile);
            _themeService.SetTheme(profile.Theme);
            SelectedProfile = profile;

            NavigationService.NavigateTo("Drawing");
        }
    }

    [RelayCommand]
    private async Task CreateProfileAsync()
    {
        if (App.MainWindow?.Content?.XamlRoot == null) return;

        var dialog = new ProfileDialog(_profileRepository)
        {
            XamlRoot = App.MainWindow.Content.XamlRoot
        };

        var result = await dialog.ShowAsync();

        if (result == ContentDialogResult.Primary && dialog.Result != null)
        {
            await ExecuteAsync(async () =>
            {
                var newProfile = await _profileRepository.AddAsync(dialog.Result);
                Profiles.Add(newProfile);
            });
        }
    }

    [RelayCommand]
    private async Task EditProfileAsync(Profile? profile)
    {
        if (profile == null || App.MainWindow?.Content?.XamlRoot == null) return;

        var dialog = new ProfileDialog(_profileRepository, profile)
        {
            XamlRoot = App.MainWindow.Content.XamlRoot
        };

        var result = await dialog.ShowAsync();

        if (result == ContentDialogResult.Primary && dialog.Result != null)
        {
            await ExecuteAsync(async () =>
            {
                await _profileRepository.UpdateAsync(dialog.Result);
                await LoadProfilesAsync();
            });
        }
    }
}
