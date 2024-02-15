using Microsoft.AspNetCore.Identity;

namespace JWTAuthAspNet7WebAPI.Core.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
