using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WeatherApp.Models;
using System.Text.Json;
namespace WeatherApp.Pages;

public class IndexModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _config;

    public IndexModel(IHttpClientFactory httpClientFactory, IConfiguration config)
    {
        _httpClientFactory = httpClientFactory;
        _config = config;
    }
    public WeatherInfo? Weather { get; set; }
    public string? City { get; set; }

    public async Task OnGetAsync(string? city)
    {

        if (string.IsNullOrEmpty(city))
        {
            return;
        }
        
        City = city;
        var client = _httpClientFactory.CreateClient();
        var apiKey = _config["OpenWeatherMap:ApiKey"];

        var url = $"https://api.openweathermap.org/data/2.5/weather?q={city}&appid={apiKey}&units=metric";

        var response = await client.GetAsync(url);
        if (!response.IsSuccessStatusCode) return;

        using var stream = await response.Content.ReadAsStreamAsync();
        using var doc = await JsonDocument.ParseAsync(stream);
        var root = doc.RootElement;

        Weather = new WeatherInfo
        {
            City = root.GetProperty("name").GetString() ?? "",
            Description = root.GetProperty("weather")[0].GetProperty("description").GetString() ?? "",
            Temperature = root.GetProperty("main").GetProperty("temp").GetDouble()
        };
    }
}
