using BusinessLogic;
using BusinessLogic.BookManager;
using BusinessLogic.Common;
using BusinessLogic.Elasticsearch;
using DAL;
using DAL.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WebServer.Middleware;
using Microsoft.EntityFrameworkCore;

namespace WebServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            
            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);
            
            services.AddDbContext<BooksDbContext>(
                opts => opts.UseSqlServer(
                    Configuration.GetConnectionString("BooksDB")));
            
            services.AddTransient(provider => provider.GetService<ILoggerFactory>().CreateLogger(string.Empty));
            
            services.AddScoped<IBooksManager, BooksManager>();
            services.AddScoped<IBookRepository, BookRepository>();
            services.AddScoped<IBookReviewRepository, BookReviewRepository>();
            services.AddScoped<IElasticSearchService, ElasticSearchService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app, 
            IWebHostEnvironment env, 
            ILoggerFactory loggerFactory,
            IHostApplicationLifetime lifetime,
            IElasticSearchService elasticSearchService)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            loggerFactory.AddFile("Logs/{Date}.log");
            
            app.UseMiddleware<ExceptionMiddleware>();

            app.UseRouting();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

            //lifetime.ApplicationStarted.Register(() => elasticSearchService.CheckIndex());
        }
    }
}