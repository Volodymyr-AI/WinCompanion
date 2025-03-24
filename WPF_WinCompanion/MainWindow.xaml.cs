using Currency_Exchange.Service.Domain;
using News_Widget.MainPage;
using News_Widget.MainPage.Controllers;
using Newtonsoft.Json;
using System.Globalization;
using System.Net.Http;
using System.Windows;
using System.Windows.Threading;
using WPF_WinCompanion.Apps_Windows.Chess_App;
using WPF_WinCompanion.Apps_Windows.Chess_App.Views;
using WPF_WinCompanion.Apps_Windows.FinanceTracker_App;
using WPF_WinCompanion.Apps_Windows.Notes_App;
using WPF_WinCompanion.Apps_Windows.Weather_App;
using WPF_WinCompanion.Controls;

namespace WPF_WinCompanion
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DispatcherTimer _timer;
        //private NewsApiClient _newsApiClient;
        
        
        public MainWindow()
        {
            InitializeComponent();
            StartDateTimeUpdater();
            UpdateCurrencyValues();
            //_newsApiClient = new NewsApiClient();
            //LoadNews();
            //SetWindowSize();
        }

        private void SetWindowSize()
        {
            var screenWidth = SystemParameters.PrimaryScreenWidth;
            var screenHeight = SystemParameters.PrimaryScreenHeight;

            this.Width = screenWidth * 0.6;
            this.Height = screenHeight * 0.4;
        }

        // Date and time widget
        private void StartDateTimeUpdater()
        {
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            DateTimeText.Text = DateTime.Now.ToString("MMM dd  HH:mm:ss", CultureInfo.InvariantCulture);
        }

        // Currency widget
        private async void UpdateCurrencyValues()
        {
            var rates = await GetExchangeRatesAsync();
            if (rates != null)
            {
                DollarValue.Text = rates["USD"].ToString("F2");
                EuroValue.Text = rates["EUR"].ToString("F2");
                PoundValue.Text = rates["GBP"].ToString("F2");
            }
            else
            {
                MessageBox.Show("Failed to fetch rates");
            }
        }

        private async Task<Dictionary<string, decimal>> GetExchangeRatesAsync()
        {
            string url = "https://bank.gov.ua/NBUStatService/v1/statdirectory/exchangenew?json";
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string response = await client.GetStringAsync(url);
                    var exchangeRates = JsonConvert.DeserializeObject<List<ExchangeRate>>(response);

                    Dictionary<string, decimal> rates = new Dictionary<string, decimal>();
                    foreach (var rate in exchangeRates)
                    {
                        if (rate.Cc == "USD" || rate.Cc == "EUR" || rate.Cc == "GBP")
                        {
                            rates[rate.Cc] = rate.Rate;
                        }
                    }
                    return rates;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error fetching exchange rates: " + ex.Message);
                    return null;
                }
            }
        }


        // Open apps' windows
        private void WeatherApp_Click(object sender, RoutedEventArgs e)
        {
            WeatherWindow weatherWindow = new WeatherWindow();
            weatherWindow.Show();
        }

        private void FinanceTracker_Click(object sender, RoutedEventArgs e)
        {
            FinanceTrackerWindow financeTrackerWindow = new FinanceTrackerWindow();
            financeTrackerWindow.Show();
        }

        private void NotesApp_Click(object sender, RoutedEventArgs e)
        {
            NotesAppWindow notesappWindow = new NotesAppWindow();
            notesappWindow.Show();
        }

        private void ChessApp_Click(object sender, RoutedEventArgs e)
        {
            ChessWindow chessAppWindow = new ChessWindow();
            chessAppWindow.Show();
        }

        //News widget
        //private async void LoadNews()
        //{
        //    var newsArticles = await _newsApiClient.GetNewsAsync();
        //    if (newsArticles != null)
        //    {
        //        UpdateMarquee(newsArticles);
        //    }
        //    else
        //    {
        //        MessageBox.Show("Failed to fetch news. Please check the logs for details.");
        //    }
        //}

        //private void UpdateMarquee(List<NewsArticle> newsArticles)
        //{
        //    if (newsArticles == null || newsArticles.Count == 0) return;

        //    string newsText = string.Join(" | ", newsArticles.ConvertAll(article => article.Title));
        //    MarqueeControl marqueeControl = (MarqueeControl)this.FindName("MarqueeControl");
        //    if (marqueeControl != null)
        //    {
        //        marqueeControl.UpdateNewsText(newsText);
        //    }
        //    else
        //    {
        //        Console.WriteLine("MarqueeControl not found");
        //    }
        //}
    }
}