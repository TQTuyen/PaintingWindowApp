using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using PaintingApp.Contracts;

namespace PaintingApp.Services
{
    public sealed class DialogService : IDialogService
    {
        public XamlRoot? XamlRoot { get; set; }

        public async Task ShowMessageAsync(string title, string message)
        {
            if (!EnsureXamlRoot())
            {
                return;
            }

            var dialog = new ContentDialog
            {
                Title = title,
                Content = message,
                CloseButtonText = "OK",
                DefaultButton = ContentDialogButton.Close,
                XamlRoot = XamlRoot
            };

            await dialog.ShowAsync();
            Debug.WriteLine($"DialogService: Message dialog shown - Title: '{title}'");
        }

        public async Task<bool> ShowConfirmationAsync(string title, string message)
        {
            return await ShowConfirmationAsync(title, message, "Yes", "No");
        }

        public async Task<bool> ShowConfirmationAsync(string title, string message, string primaryButtonText, string secondaryButtonText)
        {
            if (!EnsureXamlRoot())
            {
                return false;
            }

            var dialog = new ContentDialog
            {
                Title = title,
                Content = message,
                PrimaryButtonText = primaryButtonText,
                SecondaryButtonText = secondaryButtonText,
                DefaultButton = ContentDialogButton.Primary,
                XamlRoot = XamlRoot
            };

            var result = await dialog.ShowAsync();
            var confirmed = result == ContentDialogResult.Primary;
            
            Debug.WriteLine($"DialogService: Confirmation dialog shown - Title: '{title}', Result: {(confirmed ? "Confirmed" : "Declined")}");
            return confirmed;
        }

        public async Task ShowErrorAsync(string title, string errorMessage)
        {
            if (!EnsureXamlRoot())
            {
                return;
            }

            var dialog = new ContentDialog
            {
                Title = title,
                Content = errorMessage,
                CloseButtonText = "OK",
                DefaultButton = ContentDialogButton.Close,
                XamlRoot = XamlRoot
            };

            // Style the dialog to indicate an error (using a StackPanel with icon)
            var contentPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Spacing = 12
            };

            var icon = new FontIcon
            {
                Glyph = "\uE783", // Error icon
                FontSize = 24,
                Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Red)
            };

            var messageBlock = new TextBlock
            {
                Text = errorMessage,
                TextWrapping = TextWrapping.Wrap,
                VerticalAlignment = VerticalAlignment.Center
            };

            contentPanel.Children.Add(icon);
            contentPanel.Children.Add(messageBlock);
            dialog.Content = contentPanel;

            await dialog.ShowAsync();
            Debug.WriteLine($"DialogService: Error dialog shown - Title: '{title}'");
        }

        public async Task ShowWarningAsync(string title, string warningMessage)
        {
            if (!EnsureXamlRoot())
            {
                return;
            }

            var dialog = new ContentDialog
            {
                Title = title,
                CloseButtonText = "OK",
                DefaultButton = ContentDialogButton.Close,
                XamlRoot = XamlRoot
            };

            // Style the dialog to indicate a warning (using a StackPanel with icon)
            var contentPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Spacing = 12
            };

            var icon = new FontIcon
            {
                Glyph = "\uE7BA", // Warning icon
                FontSize = 24,
                Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Orange)
            };

            var messageBlock = new TextBlock
            {
                Text = warningMessage,
                TextWrapping = TextWrapping.Wrap,
                VerticalAlignment = VerticalAlignment.Center
            };

            contentPanel.Children.Add(icon);
            contentPanel.Children.Add(messageBlock);
            dialog.Content = contentPanel;

            await dialog.ShowAsync();
            Debug.WriteLine($"DialogService: Warning dialog shown - Title: '{title}'");
        }

        public async Task<string?> ShowInputAsync(string title, string message, string defaultValue = "")
        {
            if (!EnsureXamlRoot())
            {
                return null;
            }

            var inputTextBox = new TextBox
            {
                Text = defaultValue,
                PlaceholderText = message,
                Width = 300
            };

            var contentPanel = new StackPanel
            {
                Spacing = 8
            };

            if (!string.IsNullOrEmpty(message))
            {
                contentPanel.Children.Add(new TextBlock
                {
                    Text = message,
                    TextWrapping = TextWrapping.Wrap
                });
            }

            contentPanel.Children.Add(inputTextBox);

            var dialog = new ContentDialog
            {
                Title = title,
                Content = contentPanel,
                PrimaryButtonText = "OK",
                CloseButtonText = "Cancel",
                DefaultButton = ContentDialogButton.Primary,
                XamlRoot = XamlRoot
            };

            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                Debug.WriteLine($"DialogService: Input dialog confirmed - Title: '{title}', Value: '{inputTextBox.Text}'");
                return inputTextBox.Text;
            }

            Debug.WriteLine($"DialogService: Input dialog cancelled - Title: '{title}'");
            return null;
        }

        public async Task<int> ShowThreeButtonDialogAsync(string title, string message, string primaryButtonText, string secondaryButtonText, string closeButtonText)
        {
            if (!EnsureXamlRoot())
            {
                return 0;
            }

            var dialog = new ContentDialog
            {
                Title = title,
                Content = message,
                PrimaryButtonText = primaryButtonText,
                SecondaryButtonText = secondaryButtonText,
                CloseButtonText = closeButtonText,
                DefaultButton = ContentDialogButton.Primary,
                XamlRoot = XamlRoot
            };

            var result = await dialog.ShowAsync();

            var returnValue = result switch
            {
                ContentDialogResult.Primary => 1,
                ContentDialogResult.Secondary => 2,
                _ => 0
            };

            Debug.WriteLine($"DialogService: Three-button dialog shown - Title: '{title}', Result: {returnValue}");
            return returnValue;
        }

        private bool EnsureXamlRoot()
        {
            if (XamlRoot == null)
            {
                Debug.WriteLine("DialogService: XamlRoot is not set. Cannot show dialog. " +
                    "Ensure XamlRoot is set from MainWindow after the window is loaded.");
                return false;
            }

            return true;
        }
    }
}
