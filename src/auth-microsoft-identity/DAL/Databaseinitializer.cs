using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace auth_microsoft_identity
{
    public static class Databaseinitializer
    {
        public static void Init(IServiceProvider scopeServiceProvider)
        {
            var userManager = scopeServiceProvider.GetService<UserManager<AppUser>>();

            var user = new AppUser
            {
                UserName = "admin",
            };

            var result = userManager.CreateAsync(user, "admin").GetAwaiter().GetResult();
            if (result.Succeeded)
            {
                userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, "Administrator")).GetAwaiter().GetResult();
                userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, "Manager")).GetAwaiter().GetResult();
            }
        }
    }
}