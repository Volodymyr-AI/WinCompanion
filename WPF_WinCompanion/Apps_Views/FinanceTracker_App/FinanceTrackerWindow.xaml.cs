using System.Windows;

namespace WPF_WinCompanion.Apps_Views.FinanceTracker_App
{
    /// <summary>
    /// Interaction logic for FinanceTrackerWindow.xaml
    /// </summary>
    public partial class FinanceTrackerWindow : Window
    {
        private List<decimal> Incomes = new List<decimal>();
        private List<decimal> Outcomes = new List<decimal>();
        public FinanceTrackerWindow()
        {
            InitializeComponent();
        }
    }
}
