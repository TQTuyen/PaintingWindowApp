using System.Threading.Tasks;
using Microsoft.UI.Xaml;

namespace PaintingApp.Contracts
{
    public interface IDialogService
    {
        XamlRoot? XamlRoot { get; set; }

        Task ShowMessageAsync(string title, string message);

        Task<bool> ShowConfirmationAsync(string title, string message);

        Task<bool> ShowConfirmationAsync(string title, string message, string primaryButtonText, string secondaryButtonText);

        Task ShowErrorAsync(string title, string errorMessage);

        Task ShowWarningAsync(string title, string warningMessage);

        Task<string?> ShowInputAsync(string title, string message, string defaultValue = "");

        Task<int> ShowThreeButtonDialogAsync(string title, string message, string primaryButtonText, string secondaryButtonText, string closeButtonText);
    }
}
