using System.Linq;
using Microsoft.AspNetCore.Http;

namespace API
{
    public class UserAccessor : IUserAccessor
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UserAccessor(IHttpContextAccessor httpContextAccessor)
        {
            this._httpContextAccessor = httpContextAccessor;
        }

        public string GetCurrentUsername()
        {
            var user = _httpContextAccessor.HttpContext.User;
            var username = user?.Claims.FirstOrDefault(x => x.Type == "sub")?.Value;
            // var username = _httpContextAccessor.HttpContext.User?.Claims?
            // .FirstOrDefault(x => x.Type == ClaimTypes.Expiration)?.Value;

            return username;
        }
    }
}