using System;
using PaintingApp.Contracts;
using PaintingApp.Data.Entities;

namespace PaintingApp.Services;

public class ProfileStateService : IProfileStateService
{
    private Profile? _currentProfile;

    public Profile? CurrentProfile => _currentProfile;

    public bool HasProfile => _currentProfile != null;

    public event EventHandler<Profile?>? ProfileChanged;

    public void SetProfile(Profile profile)
    {
        _currentProfile = profile;
        ProfileChanged?.Invoke(this, profile);
    }

    public void ClearProfile()
    {
        _currentProfile = null;
        ProfileChanged?.Invoke(this, null);
    }
}
