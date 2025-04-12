using System.Windows;
using System.Windows.Markup;
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
        
        private List<Window> _childWindows = new List<Window>();
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
            this.Closed += MainWindow_Closed;
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            foreach (var window in _childWindows)
            {
                window.Close();
            }
        }

        private void AddChildWindow(Window window)
        {
            _childWindows.Add(window);
            window.Closed += (s, e) => _childWindows.Remove(window);
        }

        
        private void WeatherApp_Click(object sender, RoutedEventArgs e)
        {
            var window = new WeatherWindow();
            AddChildWindow(window);
            window.Show();
        }

        private void FinanceTracker_Click(object sender, RoutedEventArgs e)
        {
            var window = new FinanceTrackerWindow();
            AddChildWindow(window);
            window.Show();
        }

        private void NotesApp_Click(object sender, RoutedEventArgs e)
        {
            var window = new NotesAppWindow();
            AddChildWindow(window);
            window.Show();
        }

        private void ChessApp_Click(object sender, RoutedEventArgs e)
        {
            var window = new ChessWindow();
            AddChildWindow(window);
            window.Show();
        }
    }
}