namespace PaintingApp.Contracts
{
    public interface INavigationService
    {
        bool CanGoBack { get; }

        bool NavigateTo<T>(object? parameter = null) where T : class;

        bool NavigateTo(string pageKey, object? parameter = null);

        bool GoBack();

        void ClearBackStack();
    }
}
