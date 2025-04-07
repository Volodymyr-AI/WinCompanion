using System.Windows;
using WPF_WinCompanion.Apps_Windows.Chess_App.Views;
using WPF_WinCompanion.Apps_Windows.FinanceTracker_App;
using WPF_WinCompanion.Apps_Windows.Notes_App;
using WPF_WinCompanion.Apps_Windows.Weather_App;
using WPF_WinCompanion.ViewModel;

namespace WPF_WinCompanion
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }

        private void WeatherApp_Click(object sender, RoutedEventArgs e)
        {
            new WeatherWindow().Show();
        }

        private void FinanceTracker_Click(object sender, RoutedEventArgs e)
        {
            new FinanceTrackerWindow().Show();
        }

        private void NotesApp_Click(object sender, RoutedEventArgs e)
        {
            new NotesAppWindow().Show();
        }

        private void ChessApp_Click(object sender, RoutedEventArgs e)
        {
            new ChessWindow().Show();
        }
    }
}