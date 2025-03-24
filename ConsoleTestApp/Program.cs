using NewsAPI;
using NewsAPI.Constants;
using NewsAPI.Models;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ConsoleTestApp
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var apiKey = "3f3410f6a0224117815c9aef46c503cb";
            var newsApiClient = new NewsApiClient(apiKey);

            var articlesResponse = newsApiClient.GetEverything(new EverythingRequest
            {
                Q = "Apple",
                SortBy = SortBys.Popularity,
                Language = Languages.EN,
                From = new DateTime(2024, 1, 25)
            });

            if (articlesResponse.Status == Statuses.Ok)
            {
                // total results found
                Console.WriteLine(articlesResponse.TotalResults);

                // here's the first 20
                foreach (var article in articlesResponse.Articles)
                {
                    // title
                    Console.WriteLine(article.Title);
                    // author
                    Console.WriteLine(article.Author);
                    // description
                    Console.WriteLine(article.Description);
                    // url
                    Console.WriteLine(article.Url);
                    // image
                    Console.WriteLine(article.UrlToImage);
                    // published at
                    Console.WriteLine(article.PublishedAt);
                }
            }

            Console.ReadLine();
        }
    }
}
