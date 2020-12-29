using Microsoft.AspNetCore.Authorization;

namespace JoggingWebApp
{
    public class HasPermissionAttribute : AuthorizeAttribute
    {
        public HasPermissionAttribute(Permissions permission) : base(permission.ToString())
        {
            
        }
    }
}