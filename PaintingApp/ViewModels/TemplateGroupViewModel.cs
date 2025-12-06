using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Media.Imaging;
using System;

namespace PaintingApp.ViewModels;

public partial class TemplateGroupViewModel : ObservableObject
{
    [ObservableProperty]
    private int _id;

    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private string? _description;

    [ObservableProperty]
    private int _usageCount;

    [ObservableProperty]
    private int _shapeCount;

    [ObservableProperty]
    private DateTime _createdDate;

    [ObservableProperty]
    private BitmapImage? _thumbnail;

    public bool HasThumbnail => Thumbnail != null;
}
