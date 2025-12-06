using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Navigation;
using PaintingApp.ViewModels;

namespace PaintingApp.Views;

public sealed partial class DrawingView : Page
{
    public DrawingScreenViewModel ViewModel { get; }

    public DrawingView()
    {
        InitializeComponent();
        ViewModel = App.GetService<DrawingScreenViewModel>();
        DataContext = ViewModel;
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        await ViewModel.InitializeAsync();
    }

    private void ToolRadioButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is RadioButton radioButton && radioButton.Tag is string toolName)
        {
            ViewModel.SelectToolCommand.Execute(toolName);
        }
    }

    private void DrawingCanvas_PointerPressed(object sender, PointerRoutedEventArgs e)
    {
        // TODO
    }

    private void DrawingCanvas_PointerMoved(object sender, PointerRoutedEventArgs e)
    {
        // TODO
    }

    private void DrawingCanvas_PointerReleased(object sender, PointerRoutedEventArgs e)
    {
        // TODO
    }
}
