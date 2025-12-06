using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace PaintingApp.Models
{
    public enum PropertyType
    {
        Number,
        Color,
        Point
    }

    public partial class EditableProperty : ObservableObject
    {
        [ObservableProperty]
        private string _name;

        [ObservableProperty]
        private string _displayName;

        [ObservableProperty]
        private object _value;

        [ObservableProperty]
        private PropertyType _type;

        [ObservableProperty]
        private double _minimumValue;

        [ObservableProperty]
        private double _maximumValue;

        [ObservableProperty]
        private string _category;

        public double DoubleValue
        {
            get => Value is double d ? d : 0.0;
            set
            {
                if (Value is double current && current == value) return;
                Value = value;
                OnPropertyChanged();
            }
        }

        partial void OnValueChanged(object value)
        {
            OnPropertyChanged(nameof(DoubleValue));
        }

        public EditableProperty(string name, string displayName, object value, PropertyType type, string category = "General")
        {
            Name = name;
            DisplayName = displayName;
            Value = value;
            Type = type;
            Category = category;
            MinimumValue = double.MinValue;
            MaximumValue = double.MaxValue;
        }
    }
}
