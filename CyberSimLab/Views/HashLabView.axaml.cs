using Avalonia.Controls;
using Avalonia.Input.Platform;
using Avalonia;
using System.Threading.Tasks;

namespace CyberSimLab.Views
{
    public partial class HashLabView : UserControl
    {
        public HashLabView()
        {
            InitializeComponent();
        }

        private async void CopyButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (DataContext is CyberSimLab.ViewModels.HashLabViewModel vm && !string.IsNullOrEmpty(vm.HashResult))
            {
                var clipboard = TopLevel.GetTopLevel(this)?.Clipboard;
                if (clipboard != null)
                {
                    await clipboard.SetTextAsync(vm.HashResult);
                }
            }
        }
    }
}
