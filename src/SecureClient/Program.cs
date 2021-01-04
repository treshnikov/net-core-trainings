using System;
using System.Threading.Tasks;
using Microsoft.Identity.Client;

namespace SecureClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await RunAsync();
            AuthConfig config = AuthConfig.ReadFileFromJson("appsettings.json");
            Console.WriteLine($"Authority: {config.Authority}");
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
        }

        }
    }
}
