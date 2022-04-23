using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace auth_microsoft_identity
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            this._configuration = configuration;
        }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(opt => {
                opt.UseSqlite(_configuration.GetConnectionString("ConnectionString"));
            });
            
            services.AddScoped<auth_microsoft_identity.IJwtGenerator, JwtGenerator>();
            
            services.AddIdentity<AppUser, AppRole>(opt =>
            {
                    opt.Password.RequireDigit = false;
                    opt.Password.RequireLowercase = false;
                    opt.Password.RequireNonAlphanumeric = false;
                    opt.Password.RequireUppercase = false;
                    opt.Password.RequiredLength = 4;    
            })
            .AddEntityFrameworkStores<ApplicationDbContext>();

            services.ConfigureApplicationCookie(opt =>
            {
                opt.LoginPath = "/Admin/Login";
                opt.AccessDeniedPath = "/Admin/AccessDenied";
            });

            services.AddAuthorization(opt => {
                opt.AddPolicy("SuperUser", builder => {
                    builder.RequireAssertion(x => 
                        x.User.HasClaim(ClaimTypes.Role, "Administrator") && 
                        x.User.HasClaim(ClaimTypes.Role, "Manager"));
                });
            });

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Token"]));
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(
                    opt =>
                        {
                            opt.TokenValidationParameters = new TokenValidationParameters
                                                                {
                                                                    ValidateIssuerSigningKey = true,
                                                                    IssuerSigningKey = key,
                                                                    ValidateAudience = false,
                                                                    ValidateIssuer = false,
                                                                };
                        });	

            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => {
                endpoints.MapDefaultControllerRoute();
            });

        }
    }
}
