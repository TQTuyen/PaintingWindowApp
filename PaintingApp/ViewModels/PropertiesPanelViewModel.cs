using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PaintingApp.Models;
using PaintingApp.Services;
using System;
using System.Collections.ObjectModel;

namespace PaintingApp.ViewModels
{
    public partial class PropertiesPanelViewModel : ObservableObject
    {
        private readonly ShapePropertyProviderFactory _propertyProviderFactory;

        [ObservableProperty]
        private ShapeModel? _selectedShape;

        [ObservableProperty]
        private ObservableCollection<EditableProperty> _properties;

        [ObservableProperty]
        private bool _hasSelection;

        public PropertiesPanelViewModel(ShapePropertyProviderFactory propertyProviderFactory)
        {
            _propertyProviderFactory = propertyProviderFactory;
            Properties = new ObservableCollection<EditableProperty>();
        }

        partial void OnSelectedShapeChanged(ShapeModel? value)
        {
            HasSelection = value != null;
            LoadProperties();
        }

        private void LoadProperties()
        {
            Properties.Clear();

            if (SelectedShape == null) return;

            var provider = _propertyProviderFactory.GetProvider(SelectedShape.Type);
            var properties = provider.GetEditableProperties(SelectedShape);

            foreach (var property in properties)
            {
                property.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == nameof(EditableProperty.Value))
                    {
                        UpdateShapeProperty(property);
                    }
                };
                Properties.Add(property);
            }
        }

        private void UpdateShapeProperty(EditableProperty property)
        {
            if (SelectedShape == null) return;

            var provider = _propertyProviderFactory.GetProvider(SelectedShape.Type);
            provider.SetProperty(SelectedShape, property.Name, property.Value);

            ShapePropertyChanged?.Invoke(this, SelectedShape);
        }

        [RelayCommand]
        private void DeleteShape()
        {
            if (SelectedShape == null) return;
            ShapeDeleted?.Invoke(this, SelectedShape);
        }

        public event EventHandler<ShapeModel>? ShapeDeleted;
        public event EventHandler<ShapeModel>? ShapePropertyChanged;
    }
}
