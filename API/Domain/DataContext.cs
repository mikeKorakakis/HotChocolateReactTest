using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace API
{
    public class DataContext: IdentityDbContext<AppUser>
    {
         public DataContext(DbContextOptions<DataContext> options) : base(options) { }
    }
}