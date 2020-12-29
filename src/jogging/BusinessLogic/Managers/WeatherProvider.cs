using System;
using System.Net.Http;
using System.Threading.Tasks;
using BusinessLogic.Common;
using Microsoft.Extensions.Options;

namespace BusinessLogic.Managers
{
    public interface IWeatherProvider
    {
        Task<string> Get(double latitude, double longitude);
    }
    
    public class WeatherProvider : IWeatherProvider
    {
        private readonly AppSettings _appSettings;

        public WeatherProvider(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }
        
        public async Task<string> Get(double latitude, double longitude)
        {
            using var client = new HttpClient();
            try
            {
                client.BaseAddress = new Uri("http://api.openweathermap.org");
                var response = await client.GetAsync($"/data/2.5/weather?lat={latitude}&lon={longitude}&appid={_appSettings.OpenWeatherApiKey}&units=metric");
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException ex)
            {
                return null;
            }
        }
    }
}