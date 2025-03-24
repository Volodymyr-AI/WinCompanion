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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPF_WinCompanion.Controls
{
    /// <summary>
    /// Interaction logic for MarqueeControl.xaml
    /// </summary>
    public partial class MarqueeControl : UserControl
    {
        public MarqueeControl()
        {
            InitializeComponent();
            Loaded += MarqueeControl_Loaded;
        }

        private void MarqueeControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            StartMarqueeAnimation();
        }

        private void StartMarqueeAnimation()
        {
            DoubleAnimation marqueeAnimation = new DoubleAnimation
            {
                From = ActualWidth,
                To = -MarqueeTextBlock.ActualWidth,
                RepeatBehavior = RepeatBehavior.Forever,
                Duration = new Duration(TimeSpan.FromSeconds(20))
            };

            MarqueeTransform.BeginAnimation(TranslateTransform.XProperty, marqueeAnimation);
        }

        public void UpdateNewsText(string newsText)
        {
            MarqueeTextBlock.Text = newsText;
            StartMarqueeAnimation();
        }
    }
}
