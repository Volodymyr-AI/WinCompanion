using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Net;

namespace News_Widget.MainPage.Controllers
{
    public class NewsApiClient
    {
        private readonly string _apiKey = "3f3410f6a0224117815c9aef46c503cb";
        private readonly HttpClient _httpClient;

        public NewsApiClient()
        {
            _httpClient = new HttpClient();
        }

        public async Task<List<NewsArticle>> GetNewsAsync()
        {
            try
            {
                var url = $"https://newsapi.org/v2/top-headlines?country=ua&apiKey={_apiKey}";
                Console.WriteLine($"Requesting URL: {url}");

                var response = await _httpClient.GetStringAsync(url);
                var newsResponse = JsonConvert.DeserializeObject<NewsApiResponse>(response);
                return newsResponse?.Articles;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Request error: {ex.Message}");
                return null;
            }
        }
    }
}
