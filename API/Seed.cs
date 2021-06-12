

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace API
{
    public class Seed
    {
        public static async Task SeedData(DataContext context,
          UserManager<AppUser> userManager)
        {
            if (!context.Users.Any())
            {
                string[] userIds = {
                "be5cgf48-7f69-4a75-8de8-076e570bade5",
                "38f2h820-f93f-4948-92b6-bdc283a351f1",
                "5ae57946-8eab-42ab-8054-099b8f79c37a",
            };
                var users = new List<AppUser>
                {
                    new AppUser
                    {
                        Id = userIds[0],
                        UserName = "bob@test.com",
                        Email = "bob@test.com"
                    },
                    new AppUser
                    {
                    Id = userIds[1],
                        UserName = "jane@test.com",
                        Email = "jane@test.com"
                    },
                    new AppUser
                    {
                    Id = userIds[2],
                        UserName = "tom@test.com",
                        Email = "tom@test.com"
                    },
                };

                foreach (var user in users)
                {
                    await userManager.CreateAsync(user, "Pa$$w0rd");
                }
            }
        }
    }
}