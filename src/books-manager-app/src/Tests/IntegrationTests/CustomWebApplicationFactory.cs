using System;
using System.Linq;
using BusinessLogic.Elasticsearch;
using DAL;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Tests.IntegrationTests
{
    /// <summary>
    /// Implementation of WebApplicationFactory with InMemory database
    /// Read more: https://docs.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-3.1
    /// </summary>
    /// <typeparam name="TStartup"></typeparam>
    public class CustomWebApplicationFactory<TStartup>
        : WebApplicationFactory<TStartup> where TStartup: class
    {
        private readonly IMock<IElasticSearchService> _elasticMock;

        /// <summary>
        /// Tests can run in parallel, so we have to use unique database names
        /// </summary>
        private readonly Guid _databaseNameSuffix;

        public CustomWebApplicationFactory(IMock<IElasticSearchService> elasticMock)
        {
            _elasticMock = elasticMock;
            _databaseNameSuffix = Guid.NewGuid();
        }
        
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                ReplaceDatabase(services);
                ReplaceElastic(services);

                var sp = services.BuildServiceProvider();

                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<BooksDbContext>();
                    db.Database.EnsureCreated();
                }
            });
        }

        private void ReplaceElastic(IServiceCollection services)
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(IElasticSearchService));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            services.Add(new ServiceDescriptor(typeof(IElasticSearchService), _elasticMock.Object));
        }

        private void ReplaceDatabase(IServiceCollection services)
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<BooksDbContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // Add ApplicationDbContext using an in-memory database for testing.
            services.AddDbContext<BooksDbContext>(options =>
            {
                options.UseInMemoryDatabase("InMemoryDbForTesting" + _databaseNameSuffix.ToString("N"));
            });
        }
    }
}