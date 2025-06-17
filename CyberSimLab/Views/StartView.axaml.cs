using Avalonia.Controls;
using System;

namespace CyberSimLab.Views
{
    public partial class StartView : UserControl
    {
        public event EventHandler? EnterLabRequested;
        public StartView()
        {
            InitializeComponent();
        }
        private void EnterLab_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            EnterLabRequested?.Invoke(this, EventArgs.Empty);
        }
    }
} 