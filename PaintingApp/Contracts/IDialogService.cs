using System.Threading.Tasks;

namespace PaintingApp.Contracts
{
    public interface IDialogService
    {
        Task ShowMessageAsync(string title, string message);

        Task<bool> ShowConfirmationAsync(string title, string message);

        Task ShowErrorAsync(string title, string errorMessage);

        Task ShowWarningAsync(string title, string warningMessage);

        Task<string?> ShowInputAsync(string title, string message, string defaultValue = "");
    }
}
