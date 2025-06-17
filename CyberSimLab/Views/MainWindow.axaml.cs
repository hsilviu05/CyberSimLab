using Avalonia.Controls;
using System;

namespace CyberSimLab.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var startView = new StartView();
            startView.EnterLabRequested += (_, __) => ShowMainTabs();
            MainContent.Content = startView;
        }

        private void ShowMainTabs()
        {
            var tabs = new TabControl
            {
                Items =
                {
                    new TabItem { Header = "Hashing Laboratory", Content = new HashLabView() },
                    new TabItem { Header = "Brute Force Simulator", Content = new BruteForceView() }
                }
            };
            MainContent.Content = tabs;
        }
    }
}
