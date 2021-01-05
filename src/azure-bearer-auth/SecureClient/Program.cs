using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Identity.Client;
using System.Linq;
using System.Net.Http.Headers;

namespace SecureClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await RunAsync();
        }

        private static async Task RunAsync()
        {
            AuthConfig config = AuthConfig.ReadFileFromJson("appsettings.json");
            IConfidentialClientApplication app;
            app = ConfidentialClientApplicationBuilder.Create(config.ClientId)
                .WithClientSecret(config.ClientSecret)
                .WithAuthority(new Uri(config.Authority))
                .Build();

            string[] ResourceIds = new string[1] {config.ResourceId};
            AuthenticationResult result = null;

            try 
            {
                result = await app.AcquireTokenForClient(ResourceIds).ExecuteAsync();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Tocken  Aquired \n");
                Console.WriteLine(result.AccessToken);
                Console.ResetColor();
            }
            catch (MsalClientException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
            
                return;
            }

            var client = new HttpClient();
            var defaultRequestHeadres = client.DefaultRequestHeaders;

            if (defaultRequestHeadres.Accept == null || 
                !defaultRequestHeadres.Accept.Any(m => m.MediaType == "application/json"))
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            } 

            defaultRequestHeadres.Authorization = new AuthenticationHeaderValue("bearer", result.AccessToken);

            HttpResponseMessage response = await client.GetAsync(config.BaseAddress);

            if (response.IsSuccessStatusCode)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                var json = response.Content;
                Console.WriteLine(json);
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Failed to call API: {response.StatusCode}\r\nContent: {await response.Content.ReadAsStringAsync()}");
                Console.ResetColor();

            }
        }
    }
}
