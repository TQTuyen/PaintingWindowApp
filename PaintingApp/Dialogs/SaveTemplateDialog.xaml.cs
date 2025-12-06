using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace PaintingApp.Dialogs;

public sealed partial class SaveTemplateDialog : ContentDialog
{
    public string TemplateName { get; private set; } = string.Empty;
    public string? TemplateDescription { get; private set; }

    public SaveTemplateDialog(int shapeCount)
    {
        InitializeComponent();

        ShapeCountInfoBar.Message = $"{shapeCount} shape{(shapeCount == 1 ? "" : "s")} will be saved to this template";
        NameTextBox.Text = $"Template {DateTime.Now:yyyy-MM-dd HH:mm}";
        NameTextBox.SelectAll();
    }

    private void NameTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        NameErrorText.Visibility = Visibility.Collapsed;
    }

    private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        if (!ValidateInput())
        {
            args.Cancel = true;
            return;
        }

        TemplateName = NameTextBox.Text.Trim();
        TemplateDescription = string.IsNullOrWhiteSpace(DescriptionTextBox.Text)
            ? null
            : DescriptionTextBox.Text.Trim();
    }

    private bool ValidateInput()
    {
        var name = NameTextBox.Text?.Trim();

        if (string.IsNullOrWhiteSpace(name))
        {
            NameErrorText.Text = "Template name is required";
            NameErrorText.Visibility = Visibility.Visible;
            NameTextBox.Focus(FocusState.Programmatic);
            return false;
        }

        return true;
    }
}
