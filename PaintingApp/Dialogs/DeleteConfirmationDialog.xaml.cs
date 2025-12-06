using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace PaintingApp.Dialogs
{
    public sealed partial class DeleteConfirmationDialog : ContentDialog
    {
        public string ProfileName { get; }
        public int BoardCount { get; }
        public int TemplateCount { get; }
        public Visibility HasBoards => BoardCount > 0 ? Visibility.Visible : Visibility.Collapsed;
        public Visibility HasTemplates => TemplateCount > 0 ? Visibility.Visible : Visibility.Collapsed;
        public Visibility HasRelatedData => (BoardCount > 0 || TemplateCount > 0) ? Visibility.Visible : Visibility.Collapsed;

        public DeleteConfirmationDialog(string profileName, int boardCount, int templateCount)
        {
            ProfileName = profileName;
            BoardCount = boardCount;
            TemplateCount = templateCount;
            InitializeComponent();
        }
    }
}
