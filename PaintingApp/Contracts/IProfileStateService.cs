using System;
using PaintingApp.Data.Entities;

namespace PaintingApp.Contracts;

public interface IProfileStateService
{
    Profile? CurrentProfile { get; }

    bool HasProfile { get; }

    event EventHandler<Profile?> ProfileChanged;

    void SetProfile(Profile profile);

    void ClearProfile();
}
