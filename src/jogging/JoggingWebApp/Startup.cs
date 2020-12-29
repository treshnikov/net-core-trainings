using System;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.Common;
using BusinessLogic.DAL;
using BusinessLogic.Managers;
using DAL;
using DAL.Repositories;
using Domain;
using JoggingWebApp.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace JoggingWebApp
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
            services.AddDbContext<JoggingDbContext>(
                item => item.UseSqlServer(Configuration.GetConnectionString("JoggingDb")));  
            
            services.AddTransient<IUserPrincipal>(provider =>
            {
                var user = provider.GetService<IHttpContextAccessor>()?.HttpContext?.User;
                var id = user?.FindFirstValue(ClaimTypes.Name);
                return new UserPrincipal(Convert.ToInt64(id));
            });
            
            var appSettingsSection = Configuration.GetSection("AppSettings");
            
            var appSettings = appSettingsSection.Get<AppSettings>();
            services.Configure<AppSettings>(appSettingsSection);
            
            var key = Encoding.ASCII.GetBytes(appSettings.JwtSecretKey);
            services.AddAuthentication(x =>
                {
                    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(x =>
                {
                    x.Events = new JwtBearerEvents
                    {
                        OnTokenValidated = context =>
                        {
                            var userService = context.HttpContext.RequestServices.GetRequiredService<IUserManager>();
                            var userId = long.Parse(context.Principal.Identity.Name);
                            var user = userService.Get(userId);
                            if (user == null)
                            {
                                context.Fail("Unauthorized");
                            }
                            return Task.CompletedTask;
                        }
                    };
                    x.RequireHttpsMetadata = false;
                    x.SaveToken = true;
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });
            
            services.AddAuthorization(options =>
            {
                foreach (Permissions permission in Enum.GetValues(typeof(Permissions)))
                {
                    options.AddPolicy(
                        permission.ToString(), 
                        policy => policy.RequireAssertion(
                            context => context.User.HasClaim(c => c.Type == nameof(Permissions) && 
                                                                  c.Value.Split(',').Contains(((int) permission).ToString()))));
                }
            });
            
            services.AddControllers();
            services.AddHttpContextAccessor();
            
            services.AddTransient(provider => provider.GetService<ILoggerFactory>().CreateLogger(string.Empty));
            
            services.AddScoped<IRepository<JoggingData>, JoggingDataRepository>();
            services.AddScoped<IRepository<User>, UserRepository>();
            services.AddScoped<IReadOnlyRepository<UserRole>, UserRoleRepository>();
            services.AddScoped<IReadOnlyRepository<ServerSettings>, ServerSettingsRepository>();
            services.AddScoped<IJoggingDataManager, JoggingDataManager>();
            services.AddTransient<IUserManager, UserManager>();
            services.AddScoped<IWeekReportProvider, WeekReportProvider>();
            services.AddScoped<IServerSettingsManager, ServerSettingsManager>();
            services.AddScoped<IWeatherProvider, WeatherProvider>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            loggerFactory.AddFile("Logs/{Date}.log");

            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            
            app.UseMiddleware<ExceptionMiddleware>();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}