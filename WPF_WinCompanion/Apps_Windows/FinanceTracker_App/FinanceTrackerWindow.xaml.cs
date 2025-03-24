using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WPF_WinCompanion.Apps_Windows.FinanceTracker_App
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
