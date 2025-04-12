using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Threading;
using Newtonsoft.Json;
using WPF_WinCompanion.Widgets.CurrencyExchange;

namespace WPF_WinCompanion.ViewModel;

public class MainViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    private string _dateTimeText;
    public string DateTimeText
    {
        get => _dateTimeText;
        set
        {
            _dateTimeText = value;
            OnPropertyChanged(nameof(DateTimeText));
        }
    }

    private string _usdRate;
    public string UsdRate
    {
        get => _usdRate;
        set
        {
            _usdRate = value;
            OnPropertyChanged(nameof(UsdRate));
        }
    }

    private string _eurRate;
    public string EurRate
    {
        get => _eurRate;
        set
        {
            _eurRate = value;
            OnPropertyChanged(nameof(EurRate));
        }
    }

    private string _gbpRate;
    public string GbpRate
    {
        get => _gbpRate;
        set
        {
            _gbpRate = value;
            OnPropertyChanged(nameof(GbpRate));
        }
    }

    private DispatcherTimer _timer;

    public MainViewModel()
    {
        StartClock();
        _ = UpdateCurrencyValues();
    }

    private void StartClock()
    {
        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(1)
        };
        _timer.Tick += (s, e) =>
        {
            DateTimeText = DateTime.Now.ToString("MMM dd  HH:mm:ss", CultureInfo.InvariantCulture);
        };
        _timer.Start();
    }

    private async Task UpdateCurrencyValues()
    {
        var rates = await GetExchangeRatesAsync();
        if (rates != null)
        {
            UsdRate = rates["USD"].ToString("F2");
            EurRate = rates["EUR"].ToString("F2");
            GbpRate = rates["GBP"].ToString("F2");
        }
    }

    private async Task<Dictionary<string, decimal>> GetExchangeRatesAsync()
    {
        string url = "https://bank.gov.ua/NBUStatService/v1/statdirectory/exchangenew?json";
        using HttpClient client = new HttpClient();
        try
        {
            string response = await client.GetStringAsync(url);
            var exchangeRates = JsonConvert.DeserializeObject<List<ExchangeRate>>(response);

            return exchangeRates
                .Where(x => x.Cc == "USD" || x.Cc == "EUR" || x.Cc == "GBP")
                .ToDictionary(x => x.Cc, x => x.Rate);
        }
        catch
        {
            return null;
        }
    }

    private void OnPropertyChanged(string propertyName)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}