using Microsoft.AspNetCore.Identity;

namespace API
{

    public class AppUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}